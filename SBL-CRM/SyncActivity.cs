using System;
using System.IO;
using System.Net;
using System.Globalization;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SBLCRM.Lib;
using SBLCRM.Lib.Entities;

using RestSharp;
using RestSharp.Authenticators;

namespace SBLCRM
{
	[Activity (Label = "SyncActivity")]			
	public class SyncActivity : Activity
	{
		private DateTime selectedDate = DateTime.Now.Date;
		private List<DateTime> dates = null;
		private List<SyncQueue> queue = null;

		private Spinner spnDates = null;
		private LinearLayout llSyncItems = null;
		private ImageView ivSync = null;
		private ProgressDialog progressDialog = null;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your application here
			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.AddFlags (WindowManagerFlags.KeepScreenOn);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Sync);

			//
			dates = SyncQueueManager.GetAvailableDatesDesc ();

			spnDates = FindViewById<Spinner> (Resource.Id.sfSelectedDateSpinner);
			//			spnDates.Adapter = new ArrayAdapter (Activity, Android.Resource.Layout.SimpleSpinnerItem, SyncQueueManager.DatesToString(dates));
			ArrayAdapter adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleSpinnerItem, SyncQueueManager.DatesToString(dates));
			adapter.SetDropDownViewResource (Resource.Layout.Spinner);
			spnDates.Adapter = adapter;
			spnDates.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
				//				TextView tv = (TextView) e.View;
				selectedDate = dates[e.Position];
				Toast.MakeText(this, selectedDate.ToString(@"d"), ToastLength.Short).Show();
				queue = (List<SyncQueue>) SyncQueueManager.GetSyncQueue(dates[e.Position]);

				RefreshContent();
			};

			llSyncItems = FindViewById<LinearLayout> (Resource.Id.sfList);
			ivSync = FindViewById<ImageView> (Resource.Id.sfSyncImage);

			ivSync.Click += (object sender, EventArgs e) => {
				//progressDialog = ProgressDialog.Show(this, "", "Загрузка информации на сервер", true);
				//progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
				//int i = 0;
//				foreach (var item in queue) {
//					if (!item.isSync) {
//						File.Delete(item.fileLoacation);
//						SyncQueueManager.DeleteSyncQueue(item);
//						progressDialog.SetMessage(String.Format(@"Удалено id:{0}", item.id));
//						i++;
//					}
//				}
				//SyncQueueManager.AddToQueue( new Attendance{
//				progressDialog.SetMessage(String.Format(@"Удалено всего:{0}", i));
//				Thread.Sleep(3000);
//				progressDialog.Dismiss();
				progressDialog = ProgressDialog.Show(this, "", "Загрузка информации на сервер", true);
				progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
				new Thread(new ThreadStart(delegate
					{
						//LOAD METHOD TO GET ACCOUNT INFO
						RunOnUiThread(() => progressDialog.SetMessage(@"Начало загрузки информации о посещениях"));

						UpLoadAttendances();
						UpLoadAttendanceResults();
						UpLoadAttendanceGPSPoints();
						UpLoadAttendancePhotos();
						SyncQueueManager.SaveSyncQueueToDisk();

						//HIDE PROGRESS DIALOG
						RunOnUiThread(() => { progressDialog.SetMessage(@"Обновление данных"); RefreshContent(); progressDialog.Dismiss(); }); //progressBar.Visibility = ViewStates.Gone);
					})).Start();
			};
		}

		void UpLoadAttendances()
		{
			string cookieName = string.Empty;
			string cookieValue = string.Empty;
			var user = Common.GetCurrentUser ();

			var login = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

			//login.Authenticator = new SimpleAuthenticator("identifier", "lyubin.p@gmail.com", "password", "q1234567");
			login.Authenticator = new SimpleAuthenticator(@"identifier", user.username, @"password", user.password);
			login.CookieContainer = new CookieContainer();

			var request = new RestRequest(@"auth/local", Method.POST);
			var response = login.Execute<User>(request);
			User userRes = response.Data;

			if (userRes == null)
			{
				RunOnUiThread(() => progressDialog.SetMessage(@"Не удалось пройти аутентификацию!"));
				return;
			}

			cookieName = response.Cookies[0].Name;
			cookieValue = response.Cookies[0].Value;

			var queueToUpload = (List<SyncQueue>) SyncQueueManager.GetSyncQueue(selectedDate);
			foreach (var q in queueToUpload) {
				try {
					if (( q.type == SyncQueueType.sqtAttendance) && (!q.isSync)) {

						var client = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

						//Debug.WriteLine(@"Получение информации о себе.", @"Info");
						Attendance oldAttendance = SyncQueueManager.GetAttendace(q.fileLocation);
						RunOnUiThread(() => progressDialog.SetMessage(string.Format(@"Загрузка посещения с id:{0}", oldAttendance.id)));
						request = new RestRequest(@"Attendance/", Method.POST);
						request.AddCookie(cookieName, cookieValue);
						request.RequestFormat = DataFormat.Json;
						request.JsonSerializer.ContentType = @"application/json; charset=utf-8";
						//request.AddBody(oldAttendance);
						request.AddParameter(@"application/json; charset=utf-8", oldAttendance.ToJSON(), ParameterType.RequestBody);
						var respAttendance = client.Execute<Attendance>(request);

						Attendance newAttendance = respAttendance.Data;
						//					Thread.Sleep (500);

						switch (respAttendance.StatusCode) {
						case HttpStatusCode.OK:
						case HttpStatusCode.Created:
							if (AttendanceResultManager.CorrectAttendanceForSync (oldAttendance.id, newAttendance.id)
								&& AttendancePhotoManager.CorrectAttendanceForSync (oldAttendance.id, newAttendance.id)
								&& AttendanceGPSPointManager.CorrectAttendanceForSync (oldAttendance.id, newAttendance.id)
								&& AttendanceManager.CorrectAfterSync (oldAttendance, newAttendance)) {
								q.isSync = true;
								SyncQueueManager.SaveSyncQueue (q, false);
								RunOnUiThread (() => {
									progressDialog.SetMessage (string.Format (@"Посещение с id:{0} ЗАГРУЖЕНО!", oldAttendance.id));
									//								RefreshContent ();
								});
							} else {
								RunOnUiThread (() => {
									progressDialog.SetMessage (string.Format (@"Не удалось скорректировать данные для посещения с id:{0} ОШИБКА!", oldAttendance.id));
									//								RefreshContent ();
								});
							}
							continue;
						default:
							//						Thread.Sleep (500);
							RunOnUiThread(() => progressDialog.SetMessage(@"Не удалось загрузить посещение!"));
							//						Thread.Sleep (1500);
							break;
						}
					}
				} catch (Exception ex) {
					RunOnUiThread(() => progressDialog.SetMessage(@"Error : " + ex.Message));
					//						Thread.Sleep (1500);
					break;				
				}
			}
		}

		void UpLoadAttendanceResults()
		{
			string cookieName = string.Empty;
			string cookieValue = string.Empty;
			var user = Common.GetCurrentUser ();

			var login = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

			//login.Authenticator = new SimpleAuthenticator("identifier", "lyubin.p@gmail.com", "password", "q1234567");
			login.Authenticator = new SimpleAuthenticator(@"identifier", user.username, @"password", user.password);
			login.CookieContainer = new CookieContainer();

			var request = new RestRequest(@"auth/local", Method.POST);
			var response = login.Execute<User>(request);
			User userRes = response.Data;

			if (userRes == null)
			{
				RunOnUiThread(() => progressDialog.SetMessage(@"Не удалось пройти аутентификацию!"));
			}

			cookieName = response.Cookies[0].Name;
			cookieValue = response.Cookies[0].Value;

			var queueToUpload = (List<SyncQueue>) SyncQueueManager.GetSyncQueue(selectedDate);
			foreach (var q in queueToUpload) {
				try {
					if (( q.type == SyncQueueType.sqtAttendanceResult) && (!q.isSync)) {
						var client = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

						//Debug.WriteLine(@"Получение информации о себе.", @"Info");
						AttendanceResult attendanceResult = SyncQueueManager.GetAttendaceResult(q.fileLocation);
						Attendance attendance = AttendanceManager.GetAttendance (attendanceResult.attendance);
						RunOnUiThread(() => progressDialog.SetMessage(string.Format(@"Загрузка значения с id {0} по посещению с id:{1}", attendanceResult.id, attendance.id)));
						request = new RestRequest(@"AttendanceResult/", Method.POST);
						request.AddCookie(cookieName, cookieValue);
						request.RequestFormat = DataFormat.Json;
						request.JsonSerializer.ContentType = @"application/json; charset=utf-8";
						request.AddParameter(@"application/json; charset=utf-8", attendanceResult.ToJSON(), ParameterType.RequestBody);
						//					attendanceResult.id = 0;
						//					request.AddBody(attendanceResult);

						var respAttendanceResult = client.Execute(request);

						switch (respAttendanceResult.StatusCode) {
						case HttpStatusCode.OK:
						case HttpStatusCode.Created:
							q.isSync = true;
							SyncQueueManager.SaveSyncQueue (q, false);
							//						Thread.Sleep (500);
							RunOnUiThread(() => {
								progressDialog.SetMessage(string.Format(@"Значение с id {0} по посещению с id:{1} ЗАГРУЖЕНО!", attendanceResult.id, attendance.id));
								//							RefreshContent();
							});
							continue;
						default:
							//						Thread.Sleep (500);
							RunOnUiThread(() => progressDialog.SetMessage(@"Не удалось загрузить значение по посещению!"));
							//						Thread.Sleep (1500);
							break;
						}
					}
				} catch (Exception ex) {
					RunOnUiThread(() => progressDialog.SetMessage(@"Error : " + ex.Message));
					//						Thread.Sleep (1500);
					break;				
				}
			}
		}

		void UpLoadAttendanceGPSPoints()
		{
			string cookieName = string.Empty;
			string cookieValue = string.Empty;
			var user = Common.GetCurrentUser ();

			var login = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

			//login.Authenticator = new SimpleAuthenticator("identifier", "lyubin.p@gmail.com", "password", "q1234567");
			login.Authenticator = new SimpleAuthenticator(@"identifier", user.username, @"password", user.password);
			login.CookieContainer = new CookieContainer();

			var request = new RestRequest(@"auth/local", Method.POST);
			var response = login.Execute<User>(request);
			User userRes = response.Data;

			if (userRes == null)
			{
				RunOnUiThread(() => progressDialog.SetMessage(@"Не удалось пройти аутентификацию!"));
			}

			cookieName = response.Cookies[0].Name;
			cookieValue = response.Cookies[0].Value;

			var queueToUpload = (List<SyncQueue>) SyncQueueManager.GetSyncQueue(selectedDate);
			foreach (var q in queueToUpload) {
				try {
					if (( q.type == SyncQueueType.sqtAttendanceGPSPoint) && (!q.isSync)) {
						var client = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

						//Debug.WriteLine(@"Получение информации о себе.", @"Info");
						AttendanceGPSPoint attendanceGPSPoint = SyncQueueManager.GetAttendanceGPSPoint(q.fileLocation);
						Attendance attendance = AttendanceManager.GetAttendance (attendanceGPSPoint.attendance);
						RunOnUiThread(() => progressDialog.SetMessage(string.Format(@"Загрузка GPS значений с id {0} по посещению с id:{1}", attendanceGPSPoint.id, attendance.id)));
						request = new RestRequest(@"AttendanceGPSPoint/", Method.POST);
						request.AddCookie(cookieName, cookieValue);
						request.RequestFormat = DataFormat.Json;
						request.JsonSerializer.ContentType = @"application/json; charset=utf-8";
						request.AddParameter(@"application/json; charset=utf-8", attendanceGPSPoint.ToJSON(), ParameterType.RequestBody);
						//					attendanceResult.id = 0;
						//					request.AddBody(attendanceResult);

						var respAttendanceGPSPoint = client.Execute(request);

						switch (respAttendanceGPSPoint.StatusCode) {
						case HttpStatusCode.OK:
						case HttpStatusCode.Created:
							q.isSync = true;
							SyncQueueManager.SaveSyncQueue (q, false);
							//						Thread.Sleep (500);
							RunOnUiThread(() => {
								progressDialog.SetMessage(string.Format(@"GPS значение с id {0} по посещению с id:{1} ЗАГРУЖЕНО!", attendanceGPSPoint.id, attendance.id));
								//							RefreshContent();
							});
							continue;
						default:
							//						Thread.Sleep (500);
							RunOnUiThread(() => progressDialog.SetMessage(@"Не удалось загрузить GPS значение по посещению!"));
							//						Thread.Sleep (1500);
							break;
						}
					}
				} catch (Exception ex) {
					RunOnUiThread(() => progressDialog.SetMessage(@"Error : " + ex.Message));
					//						Thread.Sleep (1500);
					break;				
				}
			}
		}

		void UpLoadAttendancePhotos()
		{
			string cookieName = string.Empty;
			string cookieValue = string.Empty;
			var user = Common.GetCurrentUser ();

			var login = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

			//login.Authenticator = new SimpleAuthenticator("identifier", "lyubin.p@gmail.com", "password", "q1234567");
			login.Authenticator = new SimpleAuthenticator(@"identifier", user.username, @"password", user.password);
			login.CookieContainer = new CookieContainer();

			var loginReq = new RestRequest(@"auth/local", Method.POST);
			var loginRes = login.Execute<User>(loginReq);
			User userRes = loginRes.Data;

			if (userRes == null)
			{
				RunOnUiThread(() => progressDialog.SetMessage(@"Не удалось пройти аутентификацию!"));
			}

			cookieName = loginRes.Cookies[0].Name;
			cookieValue = loginRes.Cookies[0].Value;

			var queueToUpload = (List<SyncQueue>) SyncQueueManager.GetSyncQueue(selectedDate);
			foreach (var q in queueToUpload) {
				try {
					if (( q.type == SyncQueueType.sqtAttendancePhoto) && (!q.isSync)) {
						//Debug.WriteLine(@"Получение информации о себе.", @"Info");
						AttendancePhoto attendancePhoto = SyncQueueManager.GetAttendancePhoto(q.fileLocation);
						Attendance attendance = AttendanceManager.GetAttendance (attendancePhoto.attendance);
						RunOnUiThread(() => progressDialog.SetMessage(string.Format(@"Загрузка фото с id {0} по посещению с id:{1}", attendancePhoto.id, attendance.id)));

						var client = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

						//					var request = new RestRequest (@"AttendancePhoto/create?attendance={attendance}&longitude={longitude}&latitude={latitude}&stamp={stamp}", Method.POST);
						var request = new RestRequest (@"AttendancePhoto/create?attendance={attendance}&longitude={longitude}&latitude={latitude}", Method.POST);
						request.AddCookie(cookieName, cookieValue);
						request.AddUrlSegment(@"attendance", attendancePhoto.attendance.ToString());
						request.AddUrlSegment(@"longitude", attendancePhoto.longitude.ToString(CultureInfo.CreateSpecificCulture("en-GB")));
						request.AddUrlSegment(@"latitude", attendancePhoto.latitude.ToString(CultureInfo.CreateSpecificCulture("en-GB")));
						//					request.AddUrlSegment(@"stamp", attendancePhoto.stamp.ToString());
						request.AddFile (@"photo", File.ReadAllBytes (attendancePhoto.photoPath), Path.GetFileName (attendancePhoto.photoPath), string.Empty);

						var response = client.Execute(request);

						switch (response.StatusCode) {
						case HttpStatusCode.OK:
						case HttpStatusCode.Created:
							//					case HttpStatusCode.U
							q.isSync = true;
							SyncQueueManager.SaveSyncQueue (q, false);
							//						Thread.Sleep (500);
							RunOnUiThread(() => {
								progressDialog.SetMessage(string.Format(@"Фото с id {0} по посещению с id:{1}"" ЗАГРУЖЕНО!", attendancePhoto.id, attendance.id));
								//							RefreshContent();
							});
							continue;
						default:
							//						Thread.Sleep (500);
							RunOnUiThread(() => progressDialog.SetMessage(@"Не удалось загрузить фото по посещению!"));
							//						Thread.Sleep (1500);
							break;
						}
					}
				} catch (Exception ex) {
					RunOnUiThread(() => progressDialog.SetMessage(@"Error : " + ex.Message));
					//						Thread.Sleep (1500);
					break;				
				}
			}
		}

		void RefreshContent()
		{
			llSyncItems.RemoveAllViews ();
			queue = (List<SyncQueue>)SyncQueueManager.GetSyncQueue (selectedDate);
			foreach (var q in queue) {
				View view = LayoutInflater.Inflate(Resource.Layout.SyncFragmentItem, null);

				RelativeLayout rl = view.FindViewById<RelativeLayout> (Resource.Id.sfiRelativeLayout);
				ImageView iv = view.FindViewById<ImageView> (Resource.Id.sfiStatusImage);

				TextView type = view.FindViewById<TextView> (Resource.Id.sfiTypeInfoText);
				TextView loc = view.FindViewById<TextView> (Resource.Id.sfiLocationText);

				try {
					if (q.isSync) {
						rl.SetBackgroundColor (Android.Graphics.Color.LightGreen);
						iv.SetImageResource (Resource.Drawable.ic_check_circle_white_36dp);
						type.SetTextAppearance (this, Resource.Style.text_success);
						loc.SetTextAppearance (this, Resource.Style.text_success_small);
					} else {
						rl.SetBackgroundColor (Android.Graphics.Color.LightPink);
						iv.SetImageResource (Resource.Drawable.ic_highlight_off_white_36dp);
						type.SetTextAppearance (this, Resource.Style.text_danger);
						loc.SetTextAppearance (this, Resource.Style.text_danger_small);
					}

					Attendance att = null;
					Pharmacy pharm = null;
					switch (q.type) {
					case SyncQueueType.sqtAttendance:
						att = SyncQueueManager.GetAttendace (q.fileLocation);
						pharm = PharmacyManager.GetPharmacy (att.pharmacy);
						type.Text = string.Format(@"Тип: Посещение аптеки {0} за дату {1}", pharm.fullName, att.date.ToString(@"d"));
						loc.Text = string.Format(@"Размещение: {0}", q.fileLocation);
						break;
					case SyncQueueType.sqtAttendanceResult:
						AttendanceResult attRes = SyncQueueManager.GetAttendaceResult (q.fileLocation);
						att = AttendanceManager.GetAttendance(attRes.attendance);
						pharm = PharmacyManager.GetPharmacy (att.pharmacy);
						type.Text = string.Format(@"Тип: Значение по препарату в посещение аптеки {0} за дату {1}", pharm.fullName, att.date.ToString(@"d"));
						loc.Text = string.Format(@"Размещение: {0}", q.fileLocation);
						break;
					case SyncQueueType.sqtAttendanceGPSPoint:
						AttendanceGPSPoint attGPS = SyncQueueManager.GetAttendanceGPSPoint (q.fileLocation);
						att = AttendanceManager.GetAttendance(attGPS.attendance);
						pharm = PharmacyManager.GetPharmacy (att.pharmacy);
						type.Text = string.Format(@"Тип: GPS значение в посещение аптеки {0} за дату {1} - lat:{2}, lon:{3}", pharm.fullName, att.date.ToString(@"d"), attGPS.latitude, attGPS.longitude);
						loc.Text = string.Format(@"Размещение: {0}", q.fileLocation);
						break;
					case SyncQueueType.sqtAttendancePhoto:
						AttendancePhoto attPho = SyncQueueManager.GetAttendancePhoto (q.fileLocation);
						type.Text = string.Format(@"Фото: {0}", attPho.photoPath);
						loc.Text = q.fileLocation;
						break;
					default:
						type.Text = @"Неизвестный тип файла";
						type.SetTextColor (Android.Graphics.Color.DarkRed);
						break;
					}
				} catch (Exception ex) {
					type.Text = @"Error:";
					loc.Text = ex.Message;
				}

				llSyncItems.AddView (view);
			}
		}
	}
}


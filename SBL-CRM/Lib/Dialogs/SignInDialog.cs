using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

using RestSharp;
using RestSharp.Serializers;
using RestSharp.Authenticators;

using SBLCRM.Lib.Entities;

namespace SBLCRM.Lib.Dialogs
{
	public class SigninDialog : DialogFragment
	{
		Button bSignUp = null;

		Animation mAnimation= null;
		Context context = null;
		Activity activity = null;
		LinearLayout llInfo = null;
		LinearLayout llSuccess = null;
		LinearLayout llWarning = null;
		LinearLayout llDanger = null;
		TextView tvProgressInfo = null;
		TextView tvProgressSuccess = null;
		TextView tvProgressWarning = null;
		TextView tvProgressDanger = null;

		TextView tvUsername = null;
		TextView tvPassword = null;

		User user = new User();

		string cookieName = "";
		string cookieValue = "";

		public event EventHandler SuccessSignedIn;

		public SigninDialog (Activity parent)
		{
			activity = parent;
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			this.RetainInstance = true;
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			Dialog.SetCanceledOnTouchOutside (true);

			var view = inflater.Inflate (Resource.Layout.SignInDialog, container, false);

			bSignUp = view.FindViewById<Button> (Resource.Id.btnDialogEmail);

			llInfo = view.FindViewById<LinearLayout> (Resource.Id.llInfo);
			llSuccess = view.FindViewById<LinearLayout> (Resource.Id.llSuccess);
			llWarning = view.FindViewById<LinearLayout> (Resource.Id.llWarning);
			llDanger = view.FindViewById<LinearLayout> (Resource.Id.llDanger);
			tvProgressInfo = view.FindViewById<TextView> (Resource.Id.sidProgressInfo);
			tvProgressSuccess = view.FindViewById<TextView> (Resource.Id.sidProgressSuccess);
			tvProgressWarning = view.FindViewById<TextView> (Resource.Id.sidProgressWarning);
			tvProgressDanger = view.FindViewById<TextView> (Resource.Id.sidProgressDanger);

			tvUsername = view.FindViewById<TextView> (Resource.Id.txtUsername);
			tvPassword = view.FindViewById<TextView> (Resource.Id.txtPassword);

			context = inflater.Context;

			bSignUp.Click += (object sender, EventArgs e) => {
				Toast.MakeText(inflater.Context, @"Click", ToastLength.Short).Show();
				mAnimation = AnimationUtils.LoadAnimation(inflater.Context,Resource.Animation.slide_right);
//				anim.FillAfter = true;
//				anim.AnimationEnd += Anim_AnimationEnd;
//				mAnimation = new TranslateAnimation(
//					Dimension.RelativeToSelf, 0f,
//					Dimension.RelativeToParent, 1.0f,
//					Dimension.Absolute, 0f,
//					Dimension.Absolute, 0f);
//				//mAnimation.FillAfter = true;
//				mAnimation.Duration = 500;
				//mAnimation.RepeatCount = -1;
				//mAnimation.RepeatMode = RepeatMode.Reverse;
				//mAnimation.Interpolator = new LinearInterpolator();
				mAnimation.AnimationEnd += Anim_AnimationEnd;
				bSignUp.StartAnimation(mAnimation);

//				ThreadPool.QueueUserWorkItem (o => SlowMethod ());

				//bSignUp.Animate().TranslationY(bSignUp.Height).SetListener(new ;
//				if (mAnimation.HasEnded) {
//					Toast.MakeText(inflater.Context, @"Animation End", ToastLength.Short).Show();
//				} else {
//					Toast.MakeText(inflater.Context, @"Animation NOT End", ToastLength.Short).Show();
//				}
			};

			return view;
		}

		public override void OnDestroyView()
		{
			if (this.Dialog != null && this.RetainInstance)
				this.Dialog.SetDismissMessage(null);
			base.OnDestroyView();
		}

		protected virtual void OnSuccessSignedIn(EventArgs e)
		{
			if (SuccessSignedIn != null) {
				SuccessSignedIn (this, e);
			}
		}

		private void SlowMethod ()
		{
//			Thread.Sleep (5000);
//
//							if (mAnimation.HasEnded) {
//				activity.RunOnUiThread(() => Toast.MakeText(context, @"Animation End", ToastLength.Short).Show());
//							} else {
//				activity.RunOnUiThread(() =>Toast.MakeText(context, @"Animation NOT End", ToastLength.Short).Show());
//							}

			//user.username = @"Zvezdova957";
			//user.password = @"624590701";

			user.username = tvUsername.Text;
			user.password = tvPassword.Text;

			bool isAuth = false;

			if (true) {
				isAuth = onlineAuth(user.username, user.password);
			} else {
				isAuth = offlineAuth(user.username, user.password);
			}

			if (isAuth)
			{

				//Toast.MakeText(context, @"Authentificated", ToastLength.Short).Show())

				WriteInfo (@"Обновление внутренних данных", 2000);
				try {
					Common.SetCurrentUser (user);
					PharmacyManager.Refresh ();
					AttendanceManager.Refresh ();
					AttendancePhotoManager.Refresh ();
					AttendanceResultManager.Refresh ();
					SyncQueueManager.Refresh ();
				} catch (Exception ex) {
					Log.Error (@"Login", ex.Message);
					WriteDanger (@"ОШИБКА! ВХОД НЕ ВЫПОЛНЕН", 3000);
					return;
				}
				WriteSuccess(@"ВХОД ВЫПОЛНЕН УСПЕШНО", 3000);
				Dismiss ();
				OnSuccessSignedIn (EventArgs.Empty);
//				MessageBox.Show();
			}
			else
			{
				activity.RunOnUiThread(() => Toast.MakeText(context, @"NOT Authentificated", ToastLength.Short).Show());
				WriteDanger (@"ОШИБКА! ВХОД НЕ ВЫПОЛНЕН", 3000);
//				activity.RunOnUiThread (() => bSignUp.Visibility = ViewStates.Visible);
//				MessageBox.Show(@"NOT Authentificated");
			}

			//RunOnUiThread (() => textview.Text = "Method Complete");
		}

		private bool offlineAuth(string username, string password)
		{
			//Debug.WriteLine(@"Начало проверки данных", @"Info");
			User user = Common.GetUser(username);

			if (user == null)
			{
				//Debug.WriteLine(String.Format(@"Не найдена информация о пользователе %s. Попробуйте подключить интернет и повторить попытку.", username), @"Info");
				return false;
			}
			else
			{
				if (!user.password.Equals(password))
				{
					//Debug.WriteLine(@"Не удалось пройти аутентификацию! Проверьте, пожалуйста, логин и пароль.", @"Info");
					return false;
				} else
				{
					//Debug.WriteLine("Проверка наличия информации себе", @"Info");

					if (Common.GetMerchant(username) == null)
					{
						//Debug.WriteLine(@"Не удалось найти информацию о себе! Попробуйте подключить интернет и повторить попытку.", @"Info");
						return false;
					}
					else
					{
						//Debug.WriteLine("Проверка наличия информации о менеджере", @"Info");

						if (Common.GetManager(username) == null)
						{
							//Debug.WriteLine(@"Не удалось найти информацию о менеджере! Попробуйте подключить интернет и повторить попытку.", @"Info");
							return false;
						}
						else
						{
							//Debug.WriteLine("Проверка наличия информации о проекте", @"Info");

							if (Common.GetProject(username) == null)
							{
								//Debug.WriteLine(@"Не удалось найти информацию о проекте! Попробуйте подключить интернет и повторить попытку.", @"Info");
								return false;
							}
							else
							{
								//Debug.WriteLine("Проверка наличия информации о препаратах", @"Info");

								if (Common.GetDrugs(username) == null)
								{
									//Debug.WriteLine(@"Не удалось найти информацию о препаратах! Попробуйте подключить интернет и повторить попытку.", @"Info");
									return false;
								}
								else
								{
									//Debug.WriteLine("Проверка наличия информации о собираемых данных", @"Info");

									if (@"InformationType" == null)
									{
										//Debug.WriteLine(@"Не удалось найти информацию о собираемых данных! Попробуйте подключить интернет и повторить попытку.", @"Info");
										return false;
									}
									else
									{
										//Debug.WriteLine("Проверка наличия информации о района деятельности", @"Info");

										if (Common.GetTerritory(username) == null)
										{
											//Debug.WriteLine(@"Не удалось найти информацию о района деятельности! Попробуйте подключить интернет и повторить попытку.", @"Info");
											return false;
										}
										else
										{
											//Debug.WriteLine("Проверка наличия информации об аптеках района деятельности", @"Info");

											if (Common.GetPharmacies(username) == null)
											{
												//Debug.WriteLine(@"Не удалось найти информацию об аптеках района деятельности! Попробуйте подключить интернет и повторить попытку.", @"Info");
												return false;
											}
										}
									}
								}
							}
						}

					}

					return true;
				}

			}
		}

		private bool onlineAuth(string username, string password)
		{
			WriteInfo (@"Подключение к серверу");
			var login = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

			//login.Authenticator = new SimpleAuthenticator("identifier", "lyubin.p@gmail.com", "password", "q1234567");
//			login.Authenticator = new SimpleAuthenticator(@"identifier", username, @"password", password);
			login.CookieContainer = new CookieContainer();

//			login.Add

			var request = new RestRequest(@"auth/local", Method.POST);
			request.AddParameter (@"identifier", username);
			request.AddParameter (@"password", password);
			var response = login.Execute<User>(request);
			User user = response.Data;

			if (user == null)
			{
				//Debug.WriteLine(@"Не удалось пройти аутентификацию! Проверьте, пожалуйста, логин и пароль.", @"Info");
				WriteWarning (@"Не удалось пройти аутентификацию! Проверьте, пожалуйста, логин и пароль.", 2000);
				return false;
			}

			user.password = password;
			Common.SetUser(username, user);

			cookieName = response.Cookies[0].Name;
			cookieValue = response.Cookies[0].Value;

			var client = new RestClient(@"http://sbl-logisapp.rhcloud.com/");

			//Debug.WriteLine(@"Получение информации о себе.", @"Info");
			WriteInfo (@"Получение информации о себе", 2000);
			request = new RestRequest(@"merchant/me", Method.GET);
			request.AddCookie(cookieName, cookieValue);
			Merchant merchant = client.Execute<Merchant>(request).Data;

			if (!Common.SetMerchant(username, merchant))
			{
				//Debug.WriteLine(@"Не удалось сохранить информации о себе", @"Error");
				WriteWarning (@"Не удалось сохранить информации о себе", 2000);
				return false;
			}

			//Debug.WriteLine(@"Получение информации о менеджере", @"Info");
			WriteInfo (@"Получение информации о менеджере", 2000);
			request = new RestRequest(@"manager/{id}?populate=false", Method.GET);
			request.AddCookie(cookieName, cookieValue);
			request.AddUrlSegment(@"id", merchant.manager.ToString());
			Manager manager = client.Execute<Manager>(request).Data;

			if (!Common.SetManager(username, manager))
			{
				//Debug.WriteLine(@"Не удалось сохранить информации о менеджере", @"Error");
				WriteWarning (@"Не удалось сохранить информации о менеджере", 2000);
				return false;
			}

			//Debug.WriteLine(@"Получение информации о проекте", @"Info");
			WriteInfo (@"Получение информации о проекте", 2000);
			request = new RestRequest(@"project/{id}?populate=false", Method.GET);
			request.AddCookie(cookieName, cookieValue);
			request.AddUrlSegment(@"id", merchant.project.ToString());
			Project project = client.Execute<Project>(request).Data;

			if (!Common.SetProject(username, project))
			{
				//Debug.WriteLine(@"Не удалось сохранить информации о проекте", @"Error");
				WriteWarning (@"Не удалось сохранить информации о проекте", 2000);
				return false;
			}

			//Debug.WriteLine(@"Получение информации о препаратах", @"Info");
			WriteInfo (@"Получение информации о препаратах", 2000);
			request = new RestRequest(@"project/{id}/drugs", Method.GET);
			request.AddCookie(cookieName, cookieValue);
			request.AddUrlSegment(@"id", merchant.project.ToString());
			List<Drug> drugs = client.Execute<List<Drug>>(request).Data;

			if (!Common.SetDrugs(username, drugs))
			{
				//Debug.WriteLine(@"Не удалось сохранить информации о препаратах", @"Error");
				WriteWarning (@"Не удалось сохранить информации о препаратах", 2000);
				return false;
			}

			WriteInfo (@"Получение информации о собираемых данных", 2000);
			request = new RestRequest(@"project/{id}/infos", Method.GET);
			request.AddCookie(cookieName, cookieValue);
			request.AddUrlSegment(@"id", merchant.project.ToString());
			List<Info> infos = client.Execute<List<Info>>(request).Data;

			if (!Common.SetInfos(username, infos))
			{
				//Debug.WriteLine(@"Не удалось сохранить информации о препаратах", @"Error");
				WriteWarning (@"Не удалось сохранить информацию о собираемых данных", 2000);
				return false;
			}

			//Debug.WriteLine(@"Получение информации о районе деятельности", @"Info");
			WriteInfo (@"Получение информации о районе деятельности", 2000);
			request = new RestRequest(@"territory/{id}?populate=false", Method.GET);
			request.AddCookie(cookieName, cookieValue);
			request.AddUrlSegment(@"id", merchant.territory.ToString());
			Territory territory = client.Execute<Territory>(request).Data;

			if (!Common.SetTerritory(username, territory))
			{
				//Debug.WriteLine(@"Не удалось сохранить информации о районе деятельности", @"Error");
				WriteWarning (@"Не удалось сохранить информации о районе деятельности", 2000);
				return false;
			}

			//Debug.WriteLine(@"Получение информации об аптеках района деятельности", @"Info");
			WriteInfo (@"Получение информации об аптеках района деятельности", 2000);
			request = new RestRequest(@"territory/{id}/pharmacies?limit=1000", Method.GET);
			request.AddCookie(cookieName, cookieValue);
			request.AddUrlSegment(@"id", merchant.territory.ToString());
			List<Pharmacy> pharmacies = client.Execute<List<Pharmacy>>(request).Data;

			if (!Common.SetPharmacies(username, pharmacies))
			{
				//Debug.WriteLine(@"Не удалось сохранить информации о районе деятельности", @"Error");
				WriteWarning (@"Не удалось сохранить информации о районе деятельности", 2000);
				return false;
			}


//			request = new RestRequest(@"territory/{id}/pharmacies?limit=1000", Method.GET);
//			request.AddCookie(cookieName, cookieValue);
//			request.AddUrlSegment(@"id", merchant.territory.ToString());
//			List<Pharmacy> pharmacies = client.Execute<List<Pharmacy>>(request).Data;
//
//			if (!Common.SetPharmacies(username, pharmacies))
//			{
//				//Debug.WriteLine(@"Не удалось сохранить информации о районе деятельности", @"Error");
//				WriteWarning (@"Не удалось сохранить информации о районе деятельности", 2000);
//				return false;
//			}

			return true;
		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			Dialog.Window.RequestFeature (WindowFeatures.NoTitle);
			base.OnActivityCreated (savedInstanceState);
			Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation;
		}

		void Anim_AnimationEnd (object sender, Animation.AnimationEndEventArgs e)
		{
			bSignUp.Visibility = ViewStates.Gone;
			llInfo.Visibility = ViewStates.Visible;
			ThreadPool.QueueUserWorkItem (o => SlowMethod ());
		}

		void WriteInfo(string info, int sleepInMilliseconds = 1)
		{
			activity.RunOnUiThread(() => {
				tvProgressInfo.Text = info;
				Toast.MakeText(context, info, ToastLength.Short).Show();
				llInfo.Visibility = ViewStates.Visible;
			});
			Thread.Sleep (sleepInMilliseconds);
		}

		void WriteSuccess(string info, int sleepInMilliseconds = 1)
		{
			activity.RunOnUiThread(() => {
				tvProgressSuccess.Text = info;
				Toast.MakeText(context, info, ToastLength.Short).Show();
				llInfo.Visibility = ViewStates.Invisible;
				llSuccess.Visibility = ViewStates.Visible;
			});
			Thread.Sleep (sleepInMilliseconds);
		}

		void WriteWarning(string info, int sleepInMilliseconds = 1)
		{
			activity.RunOnUiThread(() => {
				tvProgressWarning.Text = info;
				Toast.MakeText(context, info, ToastLength.Short).Show();
				llInfo.Visibility = ViewStates.Invisible;
				llWarning.Visibility = ViewStates.Visible;
			});
			Thread.Sleep (sleepInMilliseconds);
		}

		void WriteDanger(string info, int sleepInMilliseconds = 1)
		{
			Activity.RunOnUiThread(() => {
				tvProgressDanger.Text = info;
				Toast.MakeText(context, info, ToastLength.Short).Show();
				llInfo.Visibility = ViewStates.Invisible;
				llDanger.Visibility = ViewStates.Visible;
			});

			Thread.Sleep (sleepInMilliseconds);

			activity.RunOnUiThread(() => {
				llWarning.Visibility = ViewStates.Gone;
				llDanger.Visibility = ViewStates.Gone;
				bSignUp.Visibility = ViewStates.Visible;
			});
		}
	}
}

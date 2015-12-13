using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using Android.App;
using Android.Widget;
using Android.Views;
using Android.OS;
using Android.Util;

using SBLCRM.Lib;
using SBLCRM.Lib.Entities;
using SBLCRM.Lib.Fragments;
using SBLCRM.Lib.Dialogs;

namespace SBLCRM
{
	enum ColumnPosition {cpFirst, cpLast, cpMiddle}

	[Activity (Label = "SBL-CRM", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		RelativeLayout upPanel = null;
		RelativeLayout botPanel = null;
		RelativeLayout beforeSignIn = null;
		FrameLayout content = null;
		TableLayout pharamcyTable = null;
		Button bSignIn = null;
		ImageView next = null;
		ImageView prev = null;
		TextView pageNum = null;

		ImageView upNextBlock = null;
		ImageView upPrevBlock = null;
		TextView upInfo = null;
		ImageView upLogout = null;
		ImageView upSync = null;
		Button upStartAttendance = null;
		Button upEndAttendance = null;
		Button upClose = null;

		User user = null;
		int page = 1;
		int itemsNum = 12;
		int fragmentNum = 1;
		int selectedPharmacyID = 0;
		bool isVisitStart = false;

		Fragment fragment = null;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey, this);
			base.OnCreate (savedInstanceState);

			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.AddFlags (WindowManagerFlags.KeepScreenOn);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			upPanel = FindViewById<RelativeLayout> (Resource.Id.maUpPanelRL);
			botPanel = FindViewById<RelativeLayout> (Resource.Id.maDownPanelRL);
			beforeSignIn = FindViewById<RelativeLayout> (Resource.Id.sifBeforeSignInRL);
			content = FindViewById<FrameLayout> (Resource.Id.maContentFL);
			pharamcyTable = FindViewById<TableLayout> (Resource.Id.maPharmacyTable);
			bSignIn = FindViewById<Button> (Resource.Id.maSignInButton);
			next = FindViewById<ImageView> (Resource.Id.maNextImage);
			prev = FindViewById<ImageView> (Resource.Id.maPrevImage);
			pageNum = FindViewById<TextView> (Resource.Id.maPageText);

			// Up Panel
			upNextBlock = FindViewById<ImageView> (Resource.Id.maNextBockIV);
			upNextBlock.Click += (object sender, EventArgs e) => {
				if (fragmentNum < 3) {
					fragmentNum ++;
					RefreshContent();
				}
			};
			upPrevBlock = FindViewById<ImageView> (Resource.Id.maPrevBlockIV);
			upPrevBlock.Click += (object sender, EventArgs e) => {
				if (fragmentNum > 1) {
					fragmentNum --;
					RefreshContent();
				}
			};
			upInfo = FindViewById<TextView> (Resource.Id.maInfoText);
			upLogout = FindViewById<ImageView> (Resource.Id.maLogOut);
			upLogout.Click += (object sender, EventArgs e) => {
				Common.SetCurrentUser(null);
				RefreshMainView();
			};
			upSync = FindViewById<ImageView> (Resource.Id.maSync);
			upSync.Click += (object sender, EventArgs e) => {
				StartActivity(typeof(SyncActivity)); 
			};
			upStartAttendance = FindViewById<Button> (Resource.Id.maStartAttendance);
			upStartAttendance.Click += UpStartAttendance_Click;
			upEndAttendance = FindViewById<Button> (Resource.Id.maEndAttendance);
			upEndAttendance.Click += UpEndAttendance_Click;
			upClose = FindViewById<Button> (Resource.Id.maClose);
			upClose.Click += UpRightB_Click;

			next.Click += (object sender, EventArgs e) => {
				page++;
				if (page == 1) {
					prev.Enabled = false;
				} else {
					prev.Enabled = true;
				}
				RefreshPharmacyTable ();
			};

			prev.Click += (object sender, EventArgs e) => {
				page--;
				if (page == 1) {
					prev.Enabled = false;
				} else {
					prev.Enabled = true;
				}
				RefreshPharmacyTable();
			};

			bSignIn.Click += (object sender, EventArgs e) => {
				//rlBeforeSignIn.Visibility = ViewStates.Gone;
				FragmentTransaction trans = FragmentManager.BeginTransaction ();
				SigninDialog signinDialog = new SigninDialog (this);
				signinDialog.Show (trans, "dialog fragment");
				signinDialog.SuccessSignedIn += SigninDialog_SuccessSignedIn;

				Log.Info ("maSignInButton", "Click");
			};

//			Common.SetCurrentUser (null);
			RefreshMainView ();
		}

		void UpEndAttendance_Click (object sender, EventArgs e)
		{
			isVisitStart = false;
			Common.SetIsAttendanceRun (user.username, isVisitStart);

			// SAVE
			upPrevBlock.Visibility = ViewStates.Gone;
			upNextBlock.Visibility = ViewStates.Gone;
			FragmentManager.BeginTransaction ().Remove (fragment).Commit ();
			fragmentNum = 1;

			Attendance newAttendance = AttendanceManager.GetCurrentAttendance ();
			List<AttendanceResult> newAttendanceResults = AttendanceResultManager.GetCurrentAttendanceResults ();
			List<AttendancePhoto> newAttendancePhotos = AttendancePhotoManager.GetCurrentAttendancePhotos ();
			int attID = AttendanceManager.SaveAttendance (newAttendance);
			AttendanceResultManager.SaveNewAttendanceResults (attID, newAttendanceResults);
			AttendancePhotoManager.SaveNewAttendancePhotos (attID, newAttendancePhotos);

			//Correct Pharmacy
			Pharmacy pharmacy = PharmacyManager.GetPharmacy (selectedPharmacyID);
			pharmacy.prev = DateTime.Now;
			pharmacy.next = DateTimeFormatInfo.CurrentInfo.Calendar.AddWeeks(pharmacy.prev, 2);
			PharmacyManager.SavePharmacy (pharmacy);

			//Clear
			AttendanceManager.SetCurrentAttendance (null);
			AttendanceResultManager.SetCurrentAttendanceResults (null);
			AttendancePhotoManager.SetCurrentAttendancePhotos (null);

			RefreshMainView ();
		}

		void RefreshContent()
		{
			Bundle args = new Bundle ();
			args.PutInt (Common.PHARMACY_ID, selectedPharmacyID);
			switch (fragmentNum)
			{
			case 1:
				upEndAttendance.Visibility = ViewStates.Gone;
				fragment = new Block1Fragment();
				break;
			case 2:
				upEndAttendance.Visibility = ViewStates.Gone;
				fragment = new DrugInfoFragment();
				break;
			case 3:
				upEndAttendance.Visibility = ViewStates.Visible;
				fragment = new PhotoAddFragment();
				break;
			}
			fragment.Arguments = args;
			FragmentManager.BeginTransaction ().Replace (Resource.Id.maContentFL, fragment).Commit();
		}

		void SigninDialog_SuccessSignedIn (object sender, EventArgs e)
		{
			RunOnUiThread(() => RefreshMainView ()); 
		}

		void RefreshMainView()
		{
			user = Common.GetCurrentUser ();

			// Testing
			List<Promo> promos = new List<Promo> ();

			promos.Add (new Promo { id = 1, name = @"Promo1", key = @"Key1" });
			promos.Add (new Promo { id = 2, name = @"Promo2", key = @"Key2" });
			promos.Add (new Promo { id = 3, name = @"Promo3", key = @"Key3" });
			promos.Add (new Promo { id = 4, name = @"Promo4", key = @"Key4" });

			Common.SetPromos (user.username, promos);

			List<NetCategory> netCategories = new List<NetCategory> ();

			netCategories.Add (new NetCategory { id = 1, name = @"NetCatName1", key = @"NetCat1" });
			netCategories.Add (new NetCategory { id = 2, name = @"NetCatName2", key = @"NetCat2" });
			netCategories.Add (new NetCategory { id = 3, name = @"NetCatName3", key = @"NetCat3" });
			netCategories.Add (new NetCategory { id = 4, name = @"NetCatName4", key = @"NetCat4" });

			Common.SetNetCategories (user.username, netCategories);

			AttendanceManager.SetCurrentAttendance (null);
			// Testing

			if (user == null) {
				content.Visibility = ViewStates.Gone;
				upPanel.Visibility = ViewStates.Gone;
				botPanel.Visibility = ViewStates.Gone;
				pharamcyTable.Visibility = ViewStates.Gone;
				beforeSignIn.Visibility = ViewStates.Visible;
			} else {
				Common.SetIsAttendanceRun (user.username, isVisitStart);
				content.Visibility = ViewStates.Gone;
				beforeSignIn.Visibility = ViewStates.Gone;

				// Set Up Panel
				upPanel.Visibility = ViewStates.Visible;
				botPanel.Visibility = ViewStates.Visible;
				upStartAttendance.Visibility = ViewStates.Gone;
				upEndAttendance.Visibility = ViewStates.Gone;
				upClose.Visibility = ViewStates.Gone;
				upNextBlock.Visibility = ViewStates.Gone;
				upPrevBlock.Visibility = ViewStates.Gone;
				Project project = Common.GetProject (user.username);
				Territory territory = Common.GetTerritory (user.username);
				upInfo.Visibility = ViewStates.Visible;
				upLogout.Visibility = ViewStates.Visible;
				upSync.Visibility = ViewStates.Visible;
				upInfo.Text = string.Format (@"ПРОЕКТ : {0}; ГОРОД : {1}", project.fullName, territory.baseCity);

				RefreshPharmacyTable ();
			}			
		}

		void RefreshPharmacyTable()
		{
			pharamcyTable.Visibility = ViewStates.Visible;

			//Add Header Row
			TableRow hRow = new TableRow (this);
			hRow.SetBackgroundResource(Resource.Drawable.bottomline);

			TextView hID = GetHeadItem (ColumnPosition.cpFirst);
			hID.Gravity = GravityFlags.Center;
			hID.Text = @"ID";
			hRow.AddView (hID);

			TextView hShortName = GetHeadItem (ColumnPosition.cpMiddle);
			hShortName.Gravity = GravityFlags.CenterVertical;
			hShortName.Text = @"Аптека";
			hRow.AddView (hShortName);

			TextView hTradeNet = GetHeadItem (ColumnPosition.cpMiddle);
			hTradeNet.Gravity = GravityFlags.CenterVertical;
			hTradeNet.Text = @"Сеть";
			hRow.AddView (hTradeNet);

			TextView hAddress = GetHeadItem (ColumnPosition.cpMiddle);
			hAddress.Gravity = GravityFlags.CenterVertical;
			hAddress.Text = @"Адрес";
			hRow.AddView (hAddress);

			TextView hWeekM2 = GetHeadItem (ColumnPosition.cpMiddle);
			hWeekM2.Gravity = GravityFlags.CenterVertical;
			hWeekM2.Text = @"Неделя -2";
			hRow.AddView (hWeekM2);

			TextView hWeekM1 = GetHeadItem (ColumnPosition.cpMiddle);
			hWeekM1.Gravity = GravityFlags.CenterVertical;
			hWeekM1.Text = @"Неделя -1";
			hRow.AddView (hWeekM1);

			TextView hWeek = GetHeadItem (ColumnPosition.cpMiddle);
			hWeek.Gravity = GravityFlags.CenterVertical;
			hWeek.Text = @"Текущ. неделя";
			hRow.AddView (hWeek);

			TextView hWeekP1 = GetHeadItem (ColumnPosition.cpMiddle);
			hWeekP1.Gravity = GravityFlags.CenterVertical;
			hWeekP1.Text = @"Неделя +1";
			hRow.AddView (hWeekP1);

			TextView hWeekP2 = GetHeadItem (ColumnPosition.cpLast);
			hWeekP2.Gravity = GravityFlags.CenterVertical;
			hWeekP2.Text = @"Неделя +2";
			hRow.AddView (hWeekP2);

			pharamcyTable.AddView(hRow);

			// Content
			if (pharamcyTable != null) {
				int childCount = pharamcyTable.ChildCount;

				// Remove all rows except the first one
				if (childCount > 1) {
					pharamcyTable.RemoveViews(1, childCount - 1);
				}
					
				pageNum.Text = string.Format(@"СТРАНИЦА : {0}", page);
				var pharmacies = (from pharm in PharmacyManager.GetPharmacies(string.Empty)
					           orderby pharm.next, pharm.id
								select pharm).Skip((page - 1) * itemsNum)
											 .Take(itemsNum);

				foreach (var pharmacy in pharmacies) {
					TableRow cRow = new TableRow (this);
					if (pharmacy.prev.Date == DateTime.Now.Date) {
						cRow.SetBackgroundResource (Resource.Drawable.bottomline_green);
					} else {
						cRow.SetBackgroundResource (Resource.Drawable.bottomline);
					}

					TextView id = GetItem(ColumnPosition.cpFirst);
					id.Gravity = GravityFlags.Center;
					id.Text = pharmacy.id.ToString ();
					cRow.AddView (id);

					TextView shortName = GetItem(ColumnPosition.cpMiddle);
					shortName.Gravity = GravityFlags.CenterVertical;
					shortName.Text = pharmacy.shortName;
					cRow.AddView (shortName);

					TextView tradeNet = GetItem(ColumnPosition.cpMiddle);
					tradeNet.Gravity = GravityFlags.CenterVertical;
					tradeNet.Text = pharmacy.tradenet.ToString();
					cRow.AddView (tradeNet);

					TextView address = GetItem(ColumnPosition.cpMiddle);
					address.Gravity = GravityFlags.CenterVertical;
					address.Text = pharmacy.address;
					cRow.AddView (address);

					DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
					Calendar cal = dfi.Calendar;
					int dWeekM = (cal.GetWeekOfYear (pharmacy.prev, dfi.CalendarWeekRule, dfi.FirstDayOfWeek) - cal.GetWeekOfYear (DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
					int dWeekP = (cal.GetWeekOfYear (pharmacy.next, dfi.CalendarWeekRule, dfi.FirstDayOfWeek) - cal.GetWeekOfYear (DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
				
					TextView weekM2 = GetItem(ColumnPosition.cpMiddle);
					weekM2.Gravity = GravityFlags.CenterVertical;
					weekM2.Text = (dWeekM == -2) ? pharmacy.prev.ToString(@"d") : string.Empty;
					cRow.AddView (weekM2);

					TextView weekM1 = GetItem(ColumnPosition.cpMiddle);
					weekM1.Gravity = GravityFlags.CenterVertical;
					weekM1.Text = (dWeekM == -1) ? pharmacy.prev.ToString(@"d") : string.Empty;
					cRow.AddView (weekM1);

//					ImageView action = GetImageItem (ColumnPosition.cpMiddle);
//					action.SetImageResource (Resource.Drawable.ic_adjust_black_24dp);
//					action.SetTag (Resource.String.pharmacyID, pharmacy.id);
//					action.Click += Action_Click;
//					cRow.AddView (action);

					Button action = GetButtonItem (ColumnPosition.cpMiddle);
					action.Text = DateTime.Now.ToString (@"d");
					action.SetTag (Resource.String.pharmacyID, pharmacy.id);
					action.Click += Action_Click;
					cRow.AddView (action);

					TextView weekP1 = GetItem(ColumnPosition.cpMiddle);
					weekP1.Gravity = GravityFlags.CenterVertical;
					weekP1.Text = (dWeekP == 1) ? pharmacy.next.ToString(@"d") : string.Empty;
					cRow.AddView (weekP1);

					TextView weekP2 = GetItem(ColumnPosition.cpLast);
					weekP2.Gravity = GravityFlags.CenterVertical;
					weekP2.Text = (dWeekP == 2) ? pharmacy.next.ToString(@"d") : string.Empty;
					cRow.AddView (weekP2);

					pharamcyTable.AddView (cRow);
				}
			}
		}

		TextView GetHeadItem (ColumnPosition columnPosition)
		{
			TextView textView = new TextView (this);
			textView.SetTextAppearance (this, Resource.Style.headerTextForPharmacy);
			textView.SetHeight (56);

			switch (columnPosition) {
			case ColumnPosition.cpFirst:
				textView.LayoutParameters = new TableRow.LayoutParams () { RightMargin = 24, LeftMargin = 24 };
				break;
			case ColumnPosition.cpMiddle:
				textView.LayoutParameters = new TableRow.LayoutParams () { RightMargin = 56 };
				break;		
			case ColumnPosition.cpLast:
				textView.LayoutParameters = new TableRow.LayoutParams () { LeftMargin = 24 };
				break;						
			default:
				break;
			}
			return textView;
		}

		void Action_Click (object sender, EventArgs e)
		{
//			ImageView img = (ImageView)sender;
//			selectedPharmacyID = (int) img.GetTag(Resource.String.pharmacyID);
			Button btn = (Button)sender;
			selectedPharmacyID = (int) btn.GetTag(Resource.String.pharmacyID);
			Bundle args = new Bundle ();
			args.PutInt (Common.PHARMACY_ID, selectedPharmacyID);
			fragment = new Block1Fragment ();
			fragment.Arguments = args;
			FragmentManager.BeginTransaction ().Replace (Resource.Id.maContentFL, fragment).Commit();
			pharamcyTable.Visibility = ViewStates.Gone;
			content.Visibility = ViewStates.Visible;

			// Setup Up Panel
			upInfo.Visibility = ViewStates.Gone;
			upLogout.Visibility = ViewStates.Gone;
			upSync.Visibility = ViewStates.Gone;
			upStartAttendance.Visibility = ViewStates.Visible;
			//Toast.MakeText(this, string.Format(@"ID : {0}", pharmacyID), ToastLength.Short).Show();

			upClose.Visibility = ViewStates.Visible;
		}

		void UpStartAttendance_Click (object sender, EventArgs e)
		{
			isVisitStart = true;
			Common.SetIsAttendanceRun (user.username, isVisitStart);

			upNextBlock.Visibility = ViewStates.Visible;
			upPrevBlock.Visibility = ViewStates.Visible;
			upStartAttendance.Visibility = ViewStates.Gone;
			upClose.Visibility = ViewStates.Gone;
		}

		void UpRightB_Click (object sender, EventArgs e)
		{
			FragmentManager.BeginTransaction ().Remove (fragment).Commit ();
			AttendanceManager.SetCurrentAttendance (null);
			AttendanceResultManager.SetCurrentAttendanceResults (null);
			AttendancePhotoManager.SetCurrentAttendancePhotos (null);
			RefreshMainView ();
		}

		TextView GetItem (ColumnPosition columnPosition)
		{
			TextView textView = new TextView (this);
			textView.SetTextAppearance (this, Resource.Style.rowTextForPharmacy);
			textView.SetHeight (48);

			switch (columnPosition) {
			case ColumnPosition.cpFirst:
				textView.LayoutParameters = new TableRow.LayoutParams () { RightMargin = 24, LeftMargin = 24 };
				break;
			case ColumnPosition.cpMiddle:
				textView.LayoutParameters = new TableRow.LayoutParams () { RightMargin = 56 };
				break;		
			case ColumnPosition.cpLast:
				textView.LayoutParameters = new TableRow.LayoutParams () { LeftMargin = 24 };
				break;						
			default:
				break;
			}
			return textView;
		}

		ImageView GetImageItem (ColumnPosition columnPosition)
		{
			ImageView imageView = new ImageView (this);

			switch (columnPosition) {
			case ColumnPosition.cpFirst:
				imageView.LayoutParameters = new TableRow.LayoutParams () { RightMargin = 24, LeftMargin = 24 };
//				new TableRow.LayoutParams () {Gravity = GravityFlags.CenterVertical, RightMargin = 24, LeftMargin = 24 };
				break;
			case ColumnPosition.cpMiddle:
				imageView.LayoutParameters = new TableRow.LayoutParams () { RightMargin = 56, Gravity = GravityFlags.CenterVertical };
				break;		
			case ColumnPosition.cpLast:
				imageView.LayoutParameters = new TableRow.LayoutParams () { LeftMargin = 24 };
				break;						
			default:
				break;
			}
			return imageView;
		}

		Button GetButtonItem (ColumnPosition columnPosition)
		{
			Button button = new Button (this);

			switch (columnPosition) {
			case ColumnPosition.cpFirst:
				button.LayoutParameters = new TableRow.LayoutParams () { RightMargin = 24, LeftMargin = 24 };
				break;
			case ColumnPosition.cpMiddle:
				button.LayoutParameters = new TableRow.LayoutParams () { RightMargin = 56, Gravity = GravityFlags.CenterVertical };
				break;		
			case ColumnPosition.cpLast:
				button.LayoutParameters = new TableRow.LayoutParams () { LeftMargin = 24 };
				break;						
			default:
				break;
			}
			return button;
		}

		string UppercaseFirst(string s)
		{
			if (string.IsNullOrEmpty (s)) {
				return string.Empty;
			}

			return char.ToUpper (s [0]) + s.Substring (1);
		}
	}
}

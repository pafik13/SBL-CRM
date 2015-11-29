using System;

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
		FrameLayout content = null;
		TableLayout pharamcyTable = null;
		Button bSignIn = null;
		ImageView next = null;
		ImageView prev = null;
		TextView pageNum = null;

		ImageView upNextBlock = null;
		ImageView upPrevBlock = null;
		TextView upInfo = null;
		Button upVisitControl = null;

		User user = null;
		int page = 1;
		int itemsNum = 12;
		bool isVisitStart = false;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			Xamarin.Insights.Initialize (XamarinInsights.ApiKey, this);
			base.OnCreate (savedInstanceState);

			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.AddFlags (WindowManagerFlags.KeepScreenOn);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			upPanel = FindViewById<RelativeLayout> (Resource.Id.maUpPanelRL);
			content = FindViewById<FrameLayout> (Resource.Id.maContentFL);
			pharamcyTable = FindViewById<TableLayout> (Resource.Id.maPharmacyTable);
			bSignIn = FindViewById<Button> (Resource.Id.maSignInButton);
			next = FindViewById<ImageView> (Resource.Id.maNextImage);
			prev = FindViewById<ImageView> (Resource.Id.maPrevImage);
			pageNum = FindViewById<TextView> (Resource.Id.maPageText);

			// Up Panel
			upNextBlock = FindViewById<ImageView> (Resource.Id.maNextBockIV);
			upPrevBlock = FindViewById<ImageView> (Resource.Id.maPrevBlockIV);
			upInfo = FindViewById<TextView> (Resource.Id.maInfoText);
			upVisitControl = FindViewById<Button> (Resource.Id.maVisitControlB);

			next.Click += (object sender, EventArgs e) => {
				page++;
				if (page == 1) {
					prev.Enabled = false;
				} else {
					prev.Enabled = true;
				}
				RefreshPharmacyTable();
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

		void SigninDialog_SuccessSignedIn (object sender, EventArgs e)
		{
			RunOnUiThread(() => RefreshMainView ()); 
		}

		void RefreshMainView()
		{
			user = Common.GetCurrentUser ();

			if (user == null) {
				content.Visibility = ViewStates.Gone;
				upPanel.Visibility = ViewStates.Gone;
				pharamcyTable.Visibility = ViewStates.Gone;
			} else {
				content.Visibility = ViewStates.Gone;

				// Set Up Panel
				upPanel.Visibility = ViewStates.Visible;
				upVisitControl.Visibility = ViewStates.Gone;
				upNextBlock.Visibility = ViewStates.Gone;
				upPrevBlock.Visibility = ViewStates.Gone;
				Project project = Common.GetProject (user.username);
				Territory territory = Common.GetTerritory (user.username);
				upInfo.Text = string.Format (@"ПРОЕКТ : {0}; ГОРОД : {1}", project.fullName, territory.baseCity);

				pharamcyTable.Visibility = ViewStates.Visible;

				RefreshPharmacyTable ();
			}			
		}

		void RefreshPharmacyTable()
		{
			//Add Header Row
			TableRow hRow = new TableRow (this);
			hRow.SetBackgroundResource(Resource.Drawable.bottomline);

			TextView hID = GetHeadItem (ColumnPosition.cpFirst);
			hID.Gravity = GravityFlags.Center;
			hID.Text = @"ID";
			hRow.AddView (hID);

			TextView hFullName = GetHeadItem (ColumnPosition.cpMiddle);
			hFullName.Gravity = GravityFlags.CenterVertical;
			hFullName.Text = @"Full Name";
			hRow.AddView (hFullName);

			TextView hShortName = GetHeadItem (ColumnPosition.cpMiddle);
			hShortName.Gravity = GravityFlags.CenterVertical;
			hShortName.Text = @"Short Name";
			hRow.AddView (hShortName);

//			TextView hOfficialName = GetHeadItem (ColumnPosition.cpMiddle);
//			hOfficialName.Gravity = GravityFlags.CenterVertical;
//			hOfficialName.Text = @"Official Name";
//			hRow.AddView (hOfficialName);

			TextView hAction = GetHeadItem (ColumnPosition.cpMiddle);
			hAction.Gravity = GravityFlags.CenterVertical;
			hAction.Text = @"Action";
			hRow.AddView (hAction);

			TextView hAddress = GetHeadItem (ColumnPosition.cpMiddle);
			hAddress.Gravity = GravityFlags.CenterVertical;
			hAddress.Text = @"Address";
			hRow.AddView (hAddress);

			TextView hSubway = GetHeadItem (ColumnPosition.cpMiddle);
			hSubway.Gravity = GravityFlags.CenterVertical;
			hSubway.Text = @"Subway";
			hRow.AddView (hSubway);

			TextView hPhone = GetHeadItem (ColumnPosition.cpMiddle);
			hPhone.Gravity = GravityFlags.CenterVertical;
			hPhone.Text = @"Phone";
			hRow.AddView (hPhone);

			TextView hEmail = GetHeadItem (ColumnPosition.cpMiddle);
			hEmail.Gravity = GravityFlags.CenterVertical;
			hEmail.Text = @"E-mail";
			hRow.AddView (hEmail);

			pharamcyTable.AddView(hRow);

			// Content
			if (pharamcyTable != null) {
				int childCount = pharamcyTable.ChildCount;

				// Remove all rows except the first one
				if (childCount > 1) {
					pharamcyTable.RemoveViews(1, childCount - 1);
				}
					
				pageNum.Text = string.Format(@"СТРАНИЦА : {0}", page);
				var pharmacies = PharmacyManager.GetPharmacies ((page - 1), itemsNum);

				foreach (var pharmacy in pharmacies) {
					TableRow cRow = new TableRow (this);
					cRow.SetBackgroundResource(Resource.Drawable.bottomline);

					TextView id = GetItem(ColumnPosition.cpFirst);
					id.Gravity = GravityFlags.Center;
					id.Text = pharmacy.id.ToString ();
					cRow.AddView (id);

					TextView fullName = GetItem(ColumnPosition.cpMiddle);
					fullName.Gravity = GravityFlags.CenterVertical;
					fullName.Text = pharmacy.fullName;
					cRow.AddView (fullName);

					TextView shortName = GetItem(ColumnPosition.cpMiddle);
					shortName.Gravity = GravityFlags.CenterVertical;
					shortName.Text = pharmacy.shortName;
					cRow.AddView (shortName);

//					TextView officialName = GetItem(ColumnPosition.cpMiddle);
//					officialName.Gravity = GravityFlags.CenterVertical;
//					officialName.Text = pharmacy.officialName;
//					cRow.AddView (officialName);

					ImageView action = GetImageItem (ColumnPosition.cpMiddle);
					action.SetImageResource (Resource.Drawable.ic_adjust_black_24dp);
					action.SetTag (Resource.String.pharmacyID, pharmacy.id);
					action.Click += Action_Click;
					cRow.AddView (action);

					TextView address = GetItem(ColumnPosition.cpMiddle);
					address.Gravity = GravityFlags.CenterVertical;
					address.Text = pharmacy.address;
					cRow.AddView (address);

					TextView subway = GetItem(ColumnPosition.cpMiddle);
					subway.Gravity = GravityFlags.CenterVertical;
					subway.Text = pharmacy.subway;
					cRow.AddView (subway);

					TextView phone = GetItem(ColumnPosition.cpMiddle);
					phone.Gravity = GravityFlags.CenterVertical;
					phone.Text = pharmacy.phone;
					cRow.AddView (phone);

					TextView email = GetItem(ColumnPosition.cpLast);
					email.Gravity = GravityFlags.CenterVertical;
					email.Text = pharmacy.email;
					cRow.AddView (email);

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
			ImageView img = (ImageView)sender;
			int pharmacyID = (int) img.GetTag(Resource.String.pharmacyID);
			Bundle args = new Bundle ();
			args.PutInt (Block1Fragment.PHARMACY_ID, pharmacyID);
			Fragment fragment = new Block1Fragment ();
			fragment.Arguments = args;
			FragmentManager.BeginTransaction ().Replace (Resource.Id.maContentFL, fragment).Commit();
			pharamcyTable.Visibility = ViewStates.Gone;
			content.Visibility = ViewStates.Visible;

			// Setup Up Panel
			upInfo.Visibility = ViewStates.Gone;
			upVisitControl.Visibility = ViewStates.Visible;
			upVisitControl.Text = @"НАЧАТЬ ВИЗИТ";
			upVisitControl.Click += UpVisitControl_Click;
			//Toast.MakeText(this, string.Format(@"ID : {0}", pharmacyID), ToastLength.Short).Show();
		}

		void UpVisitControl_Click (object sender, EventArgs e)
		{
			if (isVisitStart) {
				upNextBlock.Visibility = ViewStates.Gone;
				upPrevBlock.Visibility = ViewStates.Gone;
				upVisitControl.Visibility = ViewStates.Gone;
				isVisitStart = false;
			} else {
				upNextBlock.Visibility = ViewStates.Visible;
				upPrevBlock.Visibility = ViewStates.Visible;
				upVisitControl.Visibility = ViewStates.Gone;
				upVisitControl.Text = @"ЗАКОНЧИТЬ ВИЗИТ";
				isVisitStart = true;
			}
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

		string UppercaseFirst(string s)
		{
			if (string.IsNullOrEmpty (s)) {
				return string.Empty;
			}

			return char.ToUpper (s [0]) + s.Substring (1);
		}
	}
}

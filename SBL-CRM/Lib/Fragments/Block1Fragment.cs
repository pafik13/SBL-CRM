using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using SBLCRM.Lib.Entities;

namespace SBLCRM.Lib.Fragments
{
	public class Block1Fragment : Fragment
	{
		private EditText categoryInNetEdit = null;
		private EditText telephoneEdit = null;
		private EditText commentEdit = null;
		private int pharmacyID = -1;

		User user = null;
		Pharmacy pharmacy = null;
		Attendance attendance = null;
		Merchant merchant = null;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			base.OnCreateView (inflater, container, savedInstanceState);

			View rootView = inflater.Inflate (Resource.Layout.Block1Fragment, container, false);

			pharmacyID = Arguments.GetInt (Common.PHARMACY_ID);
			user = Common.GetCurrentUser ();
			merchant = Common.GetMerchant (user.username);
			pharmacy = PharmacyManager.GetPharmacy (pharmacyID);
			attendance = AttendanceManager.GetCurrentAttendance ();

			if (attendance == null) {
				attendance = AttendanceManager.GetLastAttendance (pharmacyID);

				if (attendance == null) {
					
					attendance = new Attendance () {
						pharmacy = pharmacyID,
						date = DateTime.Now,
						merchant = merchant.id
					};
				}
			}

			rootView.FindViewById<TextView> (Resource.Id.b1fTradenetText).Text = @"Аптечная Сеть";
			rootView.FindViewById<TextView> (Resource.Id.b1fCityText).Text = @"Город";
			rootView.FindViewById<TextView> (Resource.Id.b1fPharmacyNameText).Text = pharmacy.shortName;
			rootView.FindViewById<TextView> (Resource.Id.b1fPharmacyAddressText).Text = pharmacy.address;
			categoryInNetEdit = rootView.FindViewById<EditText> (Resource.Id.b1fCategoryInNetEdit);
			categoryInNetEdit.Text = attendance.category_net;
			rootView.FindViewById<TextView> (Resource.Id.b1fCategoryInOTCText).Text = pharmacy.category_otc;
			telephoneEdit = rootView.FindViewById<EditText> (Resource.Id.b1fTelephoneEdit);
			telephoneEdit.Text = attendance.telephone;
			commentEdit = rootView.FindViewById<EditText> (Resource.Id.b1fCommentEdit);
			commentEdit.Text = attendance.comment;

			return rootView;
		}

		public override void OnPause ()
		{
			base.OnPause ();
			if (Common.GetIsAttendanceRun (user.username)) {
				attendance.category_net = categoryInNetEdit.Text;
				attendance.telephone = telephoneEdit.Text;
				attendance.comment = commentEdit.Text;
				AttendanceManager.SetCurrentAttendance (attendance);
			}
		}
	}
}


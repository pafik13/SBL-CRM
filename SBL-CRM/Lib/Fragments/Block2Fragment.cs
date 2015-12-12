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

using SBLCRM.Lib;
using SBLCRM.Lib.Entities;

namespace SBLCRM
{
	public class Block2Fragment : Fragment
	{
		User user = null;

		private EditText purchaserFIOEdit = null;
		private TextView lastAttendanceText = null;
		private EditText nextAttendanceEdit = null;
		private TextView allAttendanciesText = null;
		private EditText promosEdit = null;
		private EditText pharmacistCountEdit = null;
		private Attendance currentAttendance = null;

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

			user = Common.GetCurrentUser ();

			View rootView = inflater.Inflate (Resource.Layout.Block2Fragment, container, false);

			purchaserFIOEdit = rootView.FindViewById<EditText> (Resource.Id.b2fPurchaserFIOEdit);
			lastAttendanceText = rootView.FindViewById<TextView> (Resource.Id.b2fLastAttendanceText);
			nextAttendanceEdit = rootView.FindViewById<EditText> (Resource.Id.b2fNextAttendanceEdit);
			allAttendanciesText = rootView.FindViewById<TextView> (Resource.Id.b2fAllAttendanciesText);
			promosEdit = rootView.FindViewById<EditText> (Resource.Id.b2fPromosEdit);
			pharmacistCountEdit = rootView.FindViewById<EditText> (Resource.Id.b2fPharmacistCountEdit);

			currentAttendance = AttendanceManager.GetCurrentAttendance ();

			purchaserFIOEdit.Text = currentAttendance.purchaserFIO;
			promosEdit.Text = currentAttendance.promos;
			pharmacistCountEdit.Text = currentAttendance.pharmacistCount.ToString();
			return rootView;
		}

		public override void OnPause ()
		{
			base.OnPause ();
			if (Common.GetIsAttendanceRun (user.username)) {
				currentAttendance.purchaserFIO = purchaserFIOEdit.Text;
				currentAttendance.promos = promosEdit.Text;
				currentAttendance.pharmacistCount = int.Parse (pharmacistCountEdit.Text);
				AttendanceManager.SetCurrentAttendance (currentAttendance);
			}
		}
	}
}


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
		public const string PHARMACY_ID = @"pharmacyID";

		LinearLayout ll = null;

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
			ll = rootView.FindViewById<LinearLayout> (Resource.Id.b1LinearLayout);
			int pharmacyID = Arguments.GetInt (Block1Fragment.PHARMACY_ID);
			Pharmacy pharmacy = PharmacyManager.GetPharmacy (pharmacyID);

			ll.AddView (GetItem (string.Format(@"ID : {0}", pharmacy.id)));
			ll.AddView (GetItem (string.Format(@"FullName : {0}", pharmacy.fullName)))																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																;
			ll.AddView (GetItem (string.Format(@"ShortName : {0}", pharmacy.shortName)));
			ll.AddView (GetItem (string.Format(@"Subway : {0}", pharmacy.subway)));

			return rootView;
		}

		TextView GetItem (string text)
		{
			TextView textView = new TextView (Activity);
			textView.SetTextAppearance (Activity, Resource.Style.rowTextForPharmacy);
			textView.SetHeight (48);
			textView.Text = text;

			return textView;
		}
	}
}


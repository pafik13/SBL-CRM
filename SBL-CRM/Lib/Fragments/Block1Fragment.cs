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
			int pharmacyID = Arguments.GetInt (Common.PHARMACY_ID);
			Pharmacy pharmacy = PharmacyManager.GetPharmacy (pharmacyID);

			rootView.FindViewById<TextView> (Resource.Id.b1fTradenetText).Text = @"Аптечная Сеть";
			rootView.FindViewById<TextView> (Resource.Id.b1fCityText).Text = @"Город";
			rootView.FindViewById<TextView> (Resource.Id.b1fPharmacyNameText).Text = pharmacy.shortName;
			rootView.FindViewById<TextView> (Resource.Id.b1fPharmacyAddressText).Text = pharmacy.address;
			categoryInNetEdit = rootView.FindViewById<EditText> (Resource.Id.b1fCategoryInNetEdit);
			categoryInNetEdit.Text = @"Категория";
			rootView.FindViewById<TextView> (Resource.Id.b1fCategoryInOTCText).Text = pharmacy.category_otc;
			telephoneEdit = rootView.FindViewById<EditText> (Resource.Id.b1fTelephoneEdit);
			telephoneEdit.Text = @"Телефон";
			commentEdit = rootView.FindViewById<EditText> (Resource.Id.b1fCommentEdit);
			commentEdit.Text = @"Комментарий";

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

		public override void OnPause ()
		{
			base.OnPause ();
			AttendanceManager.SetCurrentAttendance (new Attendance () {
				pharmacy = Arguments.GetInt (Common.PHARMACY_ID),
				date = DateTime.Now,
				category_net = categoryInNetEdit.Text,
				telephone = telephoneEdit.Text,
				comment = commentEdit.Text
			});
		}
	}
}


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
		List<Promo> promos = null;
		List<int> tempPromos = null;

		private EditText purchaserFIOEdit = null;
		private TextView lastAttendanceText = null;
		private EditText nextAttendanceEdit = null;
		private TextView allAttendanciesText = null;
		private EditText promosEdit = null;
		private Button promosButton = null;
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
			promos = Common.GetPromos (user.username);

			View rootView = inflater.Inflate (Resource.Layout.Block2Fragment, container, false);

			purchaserFIOEdit = rootView.FindViewById<EditText> (Resource.Id.b2fPurchaserFIOEdit);
			lastAttendanceText = rootView.FindViewById<TextView> (Resource.Id.b2fLastAttendanceText);
			nextAttendanceEdit = rootView.FindViewById<EditText> (Resource.Id.b2fNextAttendanceEdit);
			allAttendanciesText = rootView.FindViewById<TextView> (Resource.Id.b2fAllAttendanciesText);
			promosEdit = rootView.FindViewById<EditText> (Resource.Id.b2fPromosEdit);
			promosButton = rootView.FindViewById<Button> (Resource.Id.b2fPromosButton);
			promosButton.Click += (object sender, EventArgs e) => {
				bool[] checkedItems = new bool[promos.Count];
				if (currentAttendance.promos != null) {
					for (int i = 0; i < promos.Count; i++) {
						if(currentAttendance.promos.Contains(promos[i].id)){
							checkedItems[i] = true;
							tempPromos.Add(promos[i].id);
						}
					}
				}
				string[] items = (from promo in promos
							   orderby promo.id
								select promo.name).ToArray<string>();
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(Activity);
				builder.SetTitle("Выбор ПРОМО-матералов");
				builder.SetCancelable(false);
				builder.SetMultiChoiceItems(items, checkedItems, MultiListClicked);
				builder.SetPositiveButton(@"Сохранить", 
					delegate {
						currentAttendance.promos = tempPromos.ToArray<int>(); 
						builder.Dispose();
						RefreshPromos();
					}
				);
				builder.SetNegativeButton(@"Отмена", delegate { builder.Dispose(); });
				builder.Show();
			};
			pharmacistCountEdit = rootView.FindViewById<EditText> (Resource.Id.b2fPharmacistCountEdit);


			currentAttendance = AttendanceManager.GetCurrentAttendance ();
		
			purchaserFIOEdit.Text = currentAttendance.purchaserFIO;

			RefreshPromos ();
			pharmacistCountEdit.Text = currentAttendance.pharmacistCount.ToString();
			return rootView;
		}

		private void MultiListClicked (object sender, DialogMultiChoiceClickEventArgs e)
		{
			Toast.MakeText (Activity, string.Format ("You selected: {0}", (int)e.Which), ToastLength.Short).Show ();
			if (e.IsChecked) {
				tempPromos.Add (promos [e.Which].id);
			} else {
				tempPromos.Remove (promos [e.Which].id);
			}
		}

		private void RefreshPromos()
		{
			if (currentAttendance.promos != null) {
				promosEdit.Text = string.Join (@", ", (from promo in promos
				                                     where currentAttendance.promos.Contains (promo.id)
				                                     orderby promo.id
				                                     select promo.key));
			}
			tempPromos = new List<int> ();
		}

		public override void OnPause ()
		{
			base.OnPause ();
			if (Common.GetIsAttendanceRun (user.username)) {
				currentAttendance.purchaserFIO = purchaserFIOEdit.Text;
//				currentAttendance.promos = promosEdit.Text;
				currentAttendance.pharmacistCount = int.Parse (pharmacistCountEdit.Text);
				AttendanceManager.SetCurrentAttendance (currentAttendance);
			}
		}
	}
}


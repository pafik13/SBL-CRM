using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Views.InputMethods;
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
		private Spinner categoryNetSpinner = null;
		private EditText telephoneEdit = null;
		private EditText purchaserFIOEdit = null;
		private EditText promosEdit = null;
		private Button promosButton = null;
		private EditText pharmacistCountEdit = null;
		private EditText commentEdit = null;

		private int pharmacyID = -1;

		User user = null;
		List<NetCategory> netCategories = null;
		List<Promo> promos = null;
		List<int> tempPromos = null;

		Pharmacy pharmacy = null;
		Attendance attendance = null;
		Merchant merchant = null;
		Territory territory = null;

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
			netCategories = Common.GetNetCategories (user.username);
			promos = Common.GetPromos (user.username);
			merchant = Common.GetMerchant (user.username);
			territory = Common.GetTerritory (user.username);
			pharmacy = PharmacyManager.GetPharmacy (pharmacyID);

			var tradenets = Common.GetTradeNets (user.username);
			Dictionary <int, string> tnDict = new Dictionary<int, string> ();
			foreach (var item in tradenets) {
				tnDict.Add (item.id, item.shortName);
			};

			attendance = AttendanceManager.GetCurrentAttendance ();
			if (attendance == null) {
				attendance = AttendanceManager.GetLastAttendance (pharmacyID);

				if (attendance == null) {
					attendance = new Attendance () {
						pharmacy = pharmacyID,
						date = DateTime.Now,
						merchant = merchant.id
					};
				} else {
					attendance.id = -1;
				}
			}

			rootView.FindViewById<TextView> (Resource.Id.b1fTradenetText).Text = tnDict [pharmacy.tradenet];//@"Аптечная Сеть";
			rootView.FindViewById<TextView> (Resource.Id.b1fCityText).Text = territory.baseCity;
			rootView.FindViewById<TextView> (Resource.Id.b1fPharmacyNameText).Text = pharmacy.shortName;
			rootView.FindViewById<TextView> (Resource.Id.b1fPharmacyAddressText).Text = pharmacy.address;
			rootView.FindViewById<TextView> (Resource.Id.b1fCategoryInOTCText).Text = pharmacy.category_otc;
			rootView.FindViewById<TextView> (Resource.Id.b1fLastAttendanceText).Text = pharmacy.prev == DateTime.MinValue ? String.Empty : pharmacy.prev.ToString (@"d");
			rootView.FindViewById<TextView> (Resource.Id.b1fNextAttendanceText).Text = pharmacy.next == DateTime.MinValue ? String.Empty : pharmacy.next.ToString (@"d");
			rootView.FindViewById<TextView> (Resource.Id.b1fAllAttendanciesText).Text = AttendanceManager.GetStatistics(pharmacy.id);

			categoryNetSpinner = rootView.FindViewById<Spinner> (Resource.Id.b1fCategoryNetSpinner);
			ArrayAdapter adapter = new ArrayAdapter (Activity, Android.Resource.Layout.SimpleSpinnerItem, (from item in netCategories select item.key).ToArray<string>());
			adapter.SetDropDownViewResource(Resource.Layout.SpinnerItem);
			categoryNetSpinner.Adapter = adapter;
			categoryNetSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
				attendance.category_net = netCategories[e.Position].id;
			};
			// SetValue
			for (int i = 0; i < netCategories.Count; i++) {
				if (netCategories [i].id == attendance.category_net) {
					categoryNetSpinner.SetSelection (i);
				}
			}

			telephoneEdit = rootView.FindViewById<EditText> (Resource.Id.b1fTelephoneEdit);
			telephoneEdit.Text = attendance.telephone;

			purchaserFIOEdit = rootView.FindViewById<EditText> (Resource.Id.b1fPurchaserFIOEdit);
			purchaserFIOEdit.Text = attendance.purchaserFIO;

			promosEdit = rootView.FindViewById<EditText> (Resource.Id.b1fPromosEdit);
			promosButton = rootView.FindViewById<Button> (Resource.Id.b1fPromosButton);
			promosButton.Click += (object sender, EventArgs e) => {
				bool[] checkedItems = new bool[promos.Count];
				if (attendance.promos != null) {
					for (int i = 0; i < promos.Count; i++) {
						if(attendance.promos.Contains(promos[i].id)){
							checkedItems[i] = true;
							tempPromos.Add(promos[i].id);
						}
					}
				}
				string[] items = (from promo
								 	in promos
							    orderby promo.id
								 select promo.name).ToArray<string>();
				AlertDialog.Builder builder;
				builder = new AlertDialog.Builder(Activity);
				builder.SetTitle("Выбор ПРОМО-матералов");
				builder.SetCancelable(false);
				builder.SetMultiChoiceItems(items, checkedItems, MultiListClicked);
				builder.SetPositiveButton(@"Сохранить", 
					delegate {
						attendance.promos = tempPromos.ToArray<int>(); 
						builder.Dispose();
						RefreshPromos();
					}
				);
				builder.SetNegativeButton(@"Отмена", delegate { builder.Dispose(); });
				builder.Show();
			};
			RefreshPromos();

			pharmacistCountEdit = rootView.FindViewById<EditText> (Resource.Id.b1fPharmacistCountEdit);
			pharmacistCountEdit.Text = attendance.pharmacistCount.ToString ();

			commentEdit = rootView.FindViewById<EditText> (Resource.Id.b1fCommentEdit);
			commentEdit.Text = attendance.comment;

			RefreshControlsState ();
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
			if (attendance.promos != null) {
				promosEdit.Text = string.Join (@", ", (from promo in promos
					where attendance.promos.Contains (promo.id)
					orderby promo.id
					select promo.key));
			}
			tempPromos = new List<int> ();
		}

		public void RefreshControlsState()
		{
			if (Common.GetIsAttendanceRun (user.username)) {
				if (categoryNetSpinner.SelectedView == null) {
					categoryNetSpinner.Enabled = true;
				} else {
					categoryNetSpinner.SelectedView.Enabled = true;
					categoryNetSpinner.Enabled = true;
				}
				telephoneEdit.Enabled = true;
				purchaserFIOEdit.Enabled = true;
				promosButton.Enabled = true;
				pharmacistCountEdit.Enabled = true;
				commentEdit.Enabled = true;
			} else {
				if (categoryNetSpinner.SelectedView == null) {
					categoryNetSpinner.Enabled = false;
				} else {
					categoryNetSpinner.SelectedView.Enabled = false;
					categoryNetSpinner.Enabled = false;
				}
				telephoneEdit.Enabled = false;
				purchaserFIOEdit.Enabled = false;
				promosButton.Enabled = false;
				pharmacistCountEdit.Enabled = false;
				commentEdit.Enabled = false;
			}
		}

		public override void OnPause ()
		{
			base.OnPause ();
			if (Common.GetIsAttendanceRun (user.username)) {
				attendance.purchaserFIO = purchaserFIOEdit.Text;
				attendance.pharmacistCount = int.Parse (pharmacistCountEdit.Text);
				attendance.telephone = telephoneEdit.Text;
				attendance.comment = commentEdit.Text;
				AttendanceManager.SetCurrentAttendance (attendance);
			} else {
				AttendanceManager.SetCurrentAttendance (null);
			}
//			Activity.Window.SetSoftInputMode (SoftInput.StateAlwaysHidden);
//			if (Activity.CurrentFocus != null) {
//				InputMethodManager imm = (InputMethodManager)Activity.GetSystemService(Context.InputMethodService);
//				imm.HideSoftInputFromWindow(Activity.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
//			}
		}
	}
}


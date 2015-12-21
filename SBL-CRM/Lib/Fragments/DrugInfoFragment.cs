using System;
using System.Collections.Generic;
using System.Globalization;
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
	public class DrugInfoFragment : Fragment
	{
		private User user = null;
		private Project project = null;

		private TableLayout table = null;

		private List<Info> infos = null;
		private List<Drug> drugs = null;

		private List<AttendanceResult> newAttendanceResults = null;

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

			View rootView = inflater.Inflate (Resource.Layout.DrugInfoFragment, container, false);

			table = rootView.FindViewById<TableLayout> (Resource.Id.difTableLayout);

			user = Common.GetCurrentUser ();

			project = Common.GetProject (user.username);
				
			infos = Common.GetInfos (user.username);

			List<Drug> allDrugs = Common.GetDrugs (user.username);
			DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
			int currWeek = dfi.Calendar.GetWeekOfYear (DateTime.Now, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
			int skip = (currWeek - project.startWeek) % (allDrugs.Count / project.drugsInWeek);
				
			drugs = (from drug in allDrugs
				  orderby drug.id
				   select drug
			        ).Skip (skip * project.drugsInWeek)
				     .Take (project.drugsInWeek)
				     .ToList<Drug>();

//			AttendanceResultManager.SetCurrentAttendanceResults (null);

			newAttendanceResults = AttendanceResultManager.GetCurrentAttendanceResults ();

			if ((newAttendanceResults == null) || (newAttendanceResults.Count == 0)) {
				newAttendanceResults = AttendanceResultManager.GenerateResults (infos, drugs, @"N");
			}

			RefreshTable ();

			return rootView;
		}

		public override void OnPause ()
		{
			base.OnPause ();
			if (Common.GetIsAttendanceRun (user.username)) {
				AttendanceResultManager.SetCurrentAttendanceResults (newAttendanceResults);
			}
		}

		void RefreshTable()
		{
			table.RemoveAllViews ();

			TableRow.LayoutParams lpRow = new TableRow.LayoutParams ();
			lpRow.Height = TableLayout.LayoutParams.WrapContent;
			lpRow.Width = TableLayout.LayoutParams.WrapContent;;
			lpRow.Gravity = GravityFlags.Center;

			//header
			TableRow trHeader = new TableRow (Activity);
			TextView tvHDrug = new TextView (Activity);
			tvHDrug.Gravity = GravityFlags.Center;
			tvHDrug.SetMinimumHeight (64);
			tvHDrug.SetMinimumWidth (224);
			tvHDrug.SetMaxWidth (224);
			tvHDrug.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
			tvHDrug.SetMaxLines (2);
			tvHDrug.SetTextAppearance (Activity, Resource.Style.text_header_large);
			tvHDrug.Text = "Препараты";
			tvHDrug.LayoutParameters = lpRow;
			((TableRow.LayoutParams)tvHDrug.LayoutParameters).SetMargins (0, 0, 1, 0);
			tvHDrug.SetBackgroundColor (Android.Graphics.Color.White);
			trHeader.AddView (tvHDrug);

			int i = 0;

			TableRow.LayoutParams lpValue = new TableRow.LayoutParams ();
			lpValue.Height = TableLayout.LayoutParams.WrapContent;
			lpValue.Width = TableLayout.LayoutParams.WrapContent;
			lpValue.Gravity = GravityFlags.Center;
			lpValue.SetMargins (1, 1, 1, 1);

			foreach (var drug in drugs) {

				i++;

				TableRow trRow = new TableRow (Activity);

				TextView tvDrug = new TextView (Activity);
				tvDrug.Gravity = GravityFlags.CenterVertical;
				tvDrug.SetMinimumHeight (64);
				tvDrug.SetMinimumWidth (224);
				tvDrug.SetMaxWidth (224);
				tvDrug.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
				tvDrug.SetMaxLines (2);
				tvDrug.SetTextAppearance (Activity, Resource.Style.text_row_large);
				tvDrug.Text = string.Format(@"{0}: {1}", i, drug.fullName);
				tvDrug.LayoutParameters = lpRow;
				//					((TableRow.LayoutParams)tvDrug.LayoutParameters).Gravity = GravityFlags.CenterVertical
				tvDrug.SetBackgroundColor (Android.Graphics.Color.White);
				trRow.AddView (tvDrug);

				foreach (var info in infos) {

					if (trHeader.Parent == null) {
						TextView tvHInfo = new TextView (Activity);
						tvHInfo.Gravity = GravityFlags.Center;
						tvHInfo.SetMinimumHeight (64);
						tvHInfo.SetMinimumWidth (112);
						tvHInfo.SetMaxWidth (112);
						tvHInfo.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
						tvHInfo.SetMaxLines (2);
						tvHInfo.SetTextAppearance (Activity, Resource.Style.text_header_large);
						tvHInfo.Text = info.name;
						//			tvHInfo.LayoutParameters = lpRow;
						tvHInfo.SetBackgroundColor (Android.Graphics.Color.White);
						trHeader.AddView (tvHInfo);
					}
						
					RelativeLayout rlValue = new RelativeLayout(Activity);
					rlValue.SetGravity (GravityFlags.Center);
					rlValue.SetMinimumHeight (64);
					rlValue.SetMinimumWidth (64);
					rlValue.LayoutParameters = lpValue;

					rlValue.SetTag (Resource.String.IDinfo, info.id);
					rlValue.SetTag (Resource.String.IDdrug, drug.id);
//					rlValue.SetTag (Resource.String.IDattendance, attendace.id);

					string value = string.Empty;
					value = AttendanceResultManager.GetResultValue (newAttendanceResults, info.id, drug.id);

					if (info.valueType == @"number") {
						RelativeLayout.LayoutParams nlpValue = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.MatchParent);
						nlpValue.AddRule (LayoutRules.CenterInParent);
						nlpValue.SetMargins (1, 1, 1, 1);

						EditText evValue = new EditText (Activity) { LayoutParameters = nlpValue };
						evValue.SetMinimumWidth (64);
						evValue.SetMaxWidth (64);
						evValue.InputType = Android.Text.InputTypes.ClassNumber;
						value = value.Equals (@"N") ? string.Empty : value;
						evValue.Text = value;
						rlValue.AddView (evValue);
						evValue.AfterTextChanged += NumberValue_AfterTextChanged;
					}

					if (info.valueType == @"decimal") {
						RelativeLayout.LayoutParams dlpValue = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.MatchParent);
						dlpValue.AddRule (LayoutRules.CenterInParent);
						dlpValue.SetMargins (1, 1, 1, 1);

						EditText evValue = new EditText (Activity) { LayoutParameters = dlpValue };
						evValue.SetMinimumWidth (64);
						evValue.SetMaxWidth (64);
						evValue.InputType = Android.Text.InputTypes.NumberFlagDecimal;
						value = value.Equals (@"N") ? string.Empty : value;
						evValue.Text = value;
						rlValue.AddView (evValue);
						evValue.AfterTextChanged += DecimalValue_AfterTextChanged;
					}

					if (info.valueType == @"boolean") {
						rlValue.Click += Rl_Click;

						TextView tvValue = new TextView (Activity);
						tvValue.Gravity = GravityFlags.Center;
						if (string.IsNullOrEmpty (value) || value.Equals (@"N")) {
							tvValue.SetTextAppearance (Activity, Resource.Style.text_danger);
							rlValue.SetBackgroundColor (Android.Graphics.Color.LightPink);
						} else {
							tvValue.SetTextAppearance (Activity, Resource.Style.text_success);
							rlValue.SetBackgroundColor (Android.Graphics.Color.LightGreen);
						}
						tvValue.Text = AttendanceResult.StringBoolToRussian (value);
						rlValue.AddView (tvValue);	
					}

					trRow.AddView (rlValue);
				}

				if (trHeader.Parent == null) {
					table.AddView (trHeader);
					table.AddView (GetDelim(Android.Graphics.Color.Black));
				}
				table.AddView (trRow);
				table.AddView (GetDelim(Android.Graphics.Color.Brown));
			}
		}

		TableRow GetDelim(Android.Graphics.Color color)
		{
			TableRow.LayoutParams lpDelim = new TableRow.LayoutParams ();
			lpDelim.Height = TableLayout.LayoutParams.WrapContent;
			lpDelim.Width = TableLayout.LayoutParams.WrapContent;
			lpDelim.SetMargins (2, 1, 2, 1);
//			lpDelim.Span = currentAttendances.Count + 2;
			lpDelim.Span = newAttendanceResults.Count + 2;

			TableRow rDelim = new TableRow (Activity);
			View vDelim = new View (Activity);
			vDelim.SetMinimumHeight (3);
			vDelim.SetBackgroundColor (color);
			vDelim.LayoutParameters = lpDelim;
			rDelim.AddView (vDelim);

			return rDelim;
		}

		void NumberValue_AfterTextChanged (object sender, Android.Text.AfterTextChangedEventArgs e)
		{
			EditText evValue = (EditText) sender;
			RelativeLayout rlValue = (RelativeLayout) evValue.Parent;
			int IDdrug = (int) rlValue.GetTag(Resource.String.IDdrug);
			int IDinfo = (int) rlValue.GetTag(Resource.String.IDinfo);
			AttendanceResultManager.SetResultValue(newAttendanceResults, IDinfo, IDdrug, e.Editable.ToString ());
		}

		void DecimalValue_AfterTextChanged (object sender, Android.Text.AfterTextChangedEventArgs e)
		{
			EditText evValue = (EditText) sender;
			RelativeLayout rlValue = (RelativeLayout) evValue.Parent;
			int IDdrug = (int) rlValue.GetTag(Resource.String.IDdrug);
			int IDinfo = (int) rlValue.GetTag(Resource.String.IDinfo);
			AttendanceResultManager.SetResultValue(newAttendanceResults, IDinfo, IDdrug, e.Editable.ToString ());
		}

		void Rl_Click (object sender, EventArgs e)
		{
			RelativeLayout rlValue = (RelativeLayout) sender;
			TextView tvValue = (TextView) rlValue.GetChildAt (0);

			int IDdrug = (int) rlValue.GetTag(Resource.String.IDdrug);
			int IDinfo = (int) rlValue.GetTag(Resource.String.IDinfo);

//			string message = string.Format(@"Click to IDdrug:{0}, IDinfo:{0}", IDdrug, IDinfo);

//			Toast.MakeText(Activity,  message, ToastLength.Short).Show();

			string value = AttendanceResultManager.GetResultValue(newAttendanceResults, IDinfo, IDdrug);

			value = AttendanceResult.InvertStringBool(value);

			AttendanceResultManager.SetResultValue(newAttendanceResults, IDinfo, IDdrug, value);

			if (string.IsNullOrEmpty (value) || value.Equals(@"N")) {
				tvValue.SetTextAppearance (Activity, Resource.Style.text_danger);
				rlValue.SetBackgroundColor (Android.Graphics.Color.LightPink);
			} else {
				tvValue.SetTextAppearance (Activity, Resource.Style.text_success);
				rlValue.SetBackgroundColor (Android.Graphics.Color.LightGreen);
			}
			tvValue.Text = AttendanceResult.StringBoolToRussian(value);
			AttendanceResultManager.SetCurrentAttendanceResults (newAttendanceResults);
		}
	}
}


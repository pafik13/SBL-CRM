using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Provider;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Media;

using SBLCRM.Lib;
using SBLCRM.Lib.Entities;

namespace SBLCRM
{
	public class PhotoAddFragment : Fragment
	{
		private User user = null;
		private List<PhotoType> photoTypes = null;
		private Spinner spnPhotoTypes = null;
		private List<PhotoSubType> photoSubTypes = null;
		private List<PhotoSubType> currentPhotoSubTypes = null;
		private Spinner spnPhotoSubTypes = null;
		private Button btnAddPhoto = null;
		private LinearLayout llPhotoList = null;
		private static Java.IO.File file = null;
		private List<AttendancePhoto> newAttendancePhotos = null;
		private Dictionary<int, PhotoType> photoTypesDict = null;
		private Dictionary<int, PhotoSubType> photoSubTypesDict = null;

		LayoutInflater layoutInflater = null;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			layoutInflater = inflater;

			base.OnCreateView (inflater, container, savedInstanceState);

			View rootView = inflater.Inflate (Resource.Layout.PhotoAddFragment, container, false);

			user = Common.GetCurrentUser ();

			photoTypes = Common.GetPhotoTypes (user.username);
			photoTypesDict = new Dictionary<int, PhotoType> ();
			foreach (var item in photoTypes) {
				photoTypesDict.Add (item.id, item);
			}
			photoSubTypes = Common.GetPhotoSubTypes (user.username);
			photoSubTypesDict = new Dictionary<int, PhotoSubType> ();
			foreach (var item in photoSubTypes) {
				photoSubTypesDict.Add (item.id, item);
			}

			newAttendancePhotos = AttendancePhotoManager.GetCurrentAttendancePhotos ();
			if ((newAttendancePhotos == null) || (newAttendancePhotos.Count == 0)) {
				newAttendancePhotos = new List<AttendancePhoto> ();
			} 

			llPhotoList = rootView.FindViewById<LinearLayout> (Resource.Id.pafAddedPhotoLL);
			RefreshPhotoList ();

			spnPhotoTypes = rootView.FindViewById<Spinner> (Resource.Id.pafPhotoTypeSpinner);
			ArrayAdapter adapter = new ArrayAdapter (Activity, Android.Resource.Layout.SimpleSpinnerItem, (string []) (from item in photoTypes select item.name).ToArray());
			adapter.SetDropDownViewResource (Resource.Layout.SpinnerItem);
			spnPhotoTypes.Adapter = adapter;

			spnPhotoTypes.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
				int photoTypesID = photoTypes [e.Position].id;

				currentPhotoSubTypes = (List<PhotoSubType>)(from item in photoSubTypes where item.type == photoTypesID select item).ToList();
				ArrayAdapter adapterPST = new ArrayAdapter (Activity, Android.Resource.Layout.SimpleSpinnerItem, (string []) (from item in currentPhotoSubTypes select item.name).ToArray());
				adapterPST.SetDropDownViewResource(Resource.Layout.SpinnerItem);
				spnPhotoSubTypes.Adapter = adapterPST;

			};

			spnPhotoSubTypes = rootView.FindViewById<Spinner> (Resource.Id.pafPhotoSubTypeSpinner);

			btnAddPhoto = rootView.FindViewById<Button> (Resource.Id.pafAddPhotoButton);
			btnAddPhoto.Click += (object sender, EventArgs e) => {
				if (Common.CreateDirForPhotos (user)) {
					string type = photoTypes[spnPhotoTypes.SelectedItemPosition].name;
					type = Transliteration.Front(type, TransliterationType.Gost).Substring(0, Math.Min(5, type.Length)).ToUpper();
					string subtype = currentPhotoSubTypes[spnPhotoSubTypes.SelectedItemPosition].name;
					subtype = Transliteration.Front(subtype, TransliterationType.Gost).Substring(0, Math.Min(5, subtype.Length)).ToUpper();
					string stamp = DateTime.Now.ToString(@"yyyyMMddHHmmsszz");
					file = new Java.IO.File (Common.GetDirForPhotos(user), String.Format("PHOTO_{0}_{1}_{2}.jpg", type, subtype, stamp));
					Intent intent = new Intent (MediaStore.ActionImageCapture);
					intent.PutExtra (MediaStore.ExtraOutput, Android.Net.Uri.FromFile (file));
					StartActivityForResult (intent, 0);
				}
			};

			return rootView;
		}

		void RefreshPhotoList()
		{
			//llPhotoList.RemoveViews (1, llPhotoList.ChildCount - 1);
			llPhotoList.RemoveAllViews();

			int i = 0;
			foreach (var item in newAttendancePhotos) {
				i++;

				View vPhoto = layoutInflater.Inflate (Resource.Layout.PhotoAddListItem, null);
				vPhoto.SetTag (Resource.String.PhotoPath, item.photoPath);
				vPhoto.FindViewById<TextView> (Resource.Id.paliNumTV).Text = i.ToString ();
				vPhoto.FindViewById<TextView> (Resource.Id.paliTypeTV).Text = photoTypesDict[photoSubTypesDict[item.subType].type].name;
				vPhoto.FindViewById<TextView> (Resource.Id.paliSubTypeTV).Text = photoSubTypesDict[item.subType].name;
				vPhoto.FindViewById<TextView> (Resource.Id.paliDateTimeTV).Text = item.stamp.ToString("G");
				vPhoto.Click+= (object sender, EventArgs e) => {
					View view = (View) sender;
					//Toast.MakeText(Activity, view.GetTag(Resource.String.PhotoPath).ToString(), ToastLength.Short).Show();
					Intent intent = new Intent(Intent.ActionView);
					intent.SetDataAndType(Android.Net.Uri.Parse("file://" + view.GetTag(Resource.String.PhotoPath).ToString()), "image/*");
					StartActivity(intent);
				};
				llPhotoList.AddView (vPhoto);
			}
		}

		private float convertToDegree(String stringDMS){
			float result = 0.0f;
			if (string.IsNullOrEmpty(stringDMS)) {
				return result;
			} else {
				char[] spl1 = new char[1] { ',' };
				string[] DMS = stringDMS.Split(spl1, 3);

				char[] spl2 = new char[1] { '/' };
				string[] stringD = DMS[0].Split(spl2, 2);
				double D0 = double.Parse((stringD[0]));
				double D1 = double.Parse(stringD[1]);
				double FloatD = D0/D1;

				string[] stringM = DMS[1].Split(spl2, 2);
				double M0 = double.Parse(stringM[0]);
				double M1 = double.Parse(stringM[1]);
				double FloatM = M0/M1;

				string[] stringS = DMS[2].Split(spl2, 2);
				double S0 = double.Parse(stringS[0]);
				double S1 = double.Parse(stringS[1]);
				double FloatS = S0/S1;

				return (float)(FloatD + (FloatM/60) + (FloatS/3600));
			}
		}

		public override void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			if (resultCode == Result.Ok) {
				// Make it available in the gallery
				Intent mediaScanIntent = new Intent (Intent.ActionMediaScannerScanFile);
				Android.Net.Uri contentUri = Android.Net.Uri.FromFile (file);
				mediaScanIntent.SetData (contentUri);

				Activity.SendBroadcast (mediaScanIntent);

				ExifInterface exif = new ExifInterface (file.ToString ());

				AttendancePhoto attPhoto = new AttendancePhoto () { photoPath = file.ToString ()};
				DateTime dtStamp;
				if (DateTime.TryParse (exif.GetAttribute (ExifInterface.TagDatetime), out dtStamp)){
					attPhoto.stamp = dtStamp;
				};

				attPhoto.subType = currentPhotoSubTypes[spnPhotoSubTypes.SelectedItemPosition].id;
				newAttendancePhotos.Add (attPhoto);
				AttendancePhotoManager.SetCurrentAttendancePhotos (newAttendancePhotos);

				RefreshPhotoList ();
			}

			// Dispose of the Java side bitmap.
			GC.Collect();
		}

		public override void OnPause ()
		{
			base.OnPause ();
			if (Common.GetIsAttendanceRun (user.username)) {
				AttendancePhotoManager.SetCurrentAttendancePhotos (newAttendancePhotos);
			}
		}
	}
}


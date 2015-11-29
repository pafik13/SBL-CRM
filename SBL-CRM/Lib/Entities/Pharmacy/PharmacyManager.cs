using System;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using RestSharp;
using RestSharp.Extensions;

namespace SBLCRM.Lib.Entities
{
	public static class PharmacyManager
	{
		static PharmacyManager ()
		{
		}

		public static void Refresh()
		{
			PharmacyRepository.Refresh();
		}

		public static Pharmacy GetPharmacy(int id)
		{
			return PharmacyRepository.GetPharmacy(id);
		}

		public static IList<Pharmacy> GetPharmacies (string search = @"", int num = -1)
		{
			if (string.IsNullOrEmpty (search)) {
				if (num == -1) {
					return new List<Pharmacy> (PharmacyRepository.GetPharmacies ());
				} else {
					return new List<Pharmacy> (PharmacyRepository.GetPharmacies (num));
				}
			} else {
				return new List<Pharmacy> (PharmacyRepository.GetPharmacies (search));
			}
		}

		public static IList<Pharmacy> GetPharmacies (int page = 0, int num = 10)
		{
			return new List<Pharmacy> (PharmacyRepository.GetPharmacies (page * num, num));
		}

		public static IList<Pharmacy> GetPharmacies (int[] ids)
		{
			return new List<Pharmacy> (PharmacyRepository.GetPharmacies (ids));
		}

		public static int SavePharmacy (Pharmacy item)
		{
//			JsonSerializerSettings jsonSerSet = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
//			string json = JsonConvert.SerializeObject( item, Formatting.Indented,jsonSerSet);
//			var bytes = Encoding.Default.GetBytes (json);
//
//			Pharmacy afterUpload = null;
//
//			using (WebClient wb = new WebClient ()) {
//				wb.Headers.Add(HttpRequestHeader.ContentType, @"application/json");
//				var response = wb.UploadData ("http://sbl-logisapp.rhcloud.com/pharmacy", @"POST", bytes);
//
//				string pharmacy = Encoding.Default.GetString (response);
//				afterUpload = JsonConvert.DeserializeObject<Pharmacy> (pharmacy);
//			}

			return PharmacyRepository.SavePharmacy(item);
		}

		public static string [] ToArray(List<Pharmacy> pharmacies)
		{
			string [] result = new string[pharmacies.Count];
			for (int i = 0; i < pharmacies.Count; i++) {
				result [i] = pharmacies [i].fullName;
			}
			return result;
		}

		public static int DeletePharmacy(int id)
		{
			RestClient _restClient = new RestClient ();
			var request = new RestRequest (@"http://sbl-logisapp.rhcloud.com/pharmacy/{id}", Method.DELETE);
			request.AddUrlSegment ("id", "" + id);

//			Console.WriteLine ("Executing '{0}' request to '{1}'...", request.Method, _restClient.BuildUri (request));
			var response = _restClient.Execute (request);
			if (response.StatusCode == HttpStatusCode.NotFound) {
				throw new Exception ("Build does not exist for ID: " + id);
			}
//			CheckForError(response);
//			CheckForExpectedStatusCode(response, HttpStatusCode.NoContent);
			return PharmacyRepository.DeletePharmacy(id);
		}

//		public static int DoctorCompare (Doctor doc1, Doctor doc2)
//		{
//			return String.CompareOrdinal (doc1.SecondName, doc2.SecondName);
//		}
	}
}

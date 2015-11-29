using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace SBLCRM.Lib.Entities
{
	public class PharmacyRepository
	{
		static string fUserName;
		static List<Pharmacy> pharmacies;

		static PharmacyRepository ()
		{
			// set the db location
			pharmacies = new List<Pharmacy> ();

			Refresh ();
		}

		public static void Refresh()
		{
			User user = Common.GetCurrentUser ();
			if (user != null) {
				fUserName = user.username;
			} else {
				fUserName = @"";
				pharmacies = new List<Pharmacy> ();
			}

			ReadXml ();
		}

		static void ReadXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"pharmacies.xml");
			var serializer = new XmlSerializer(typeof(List<Pharmacy>));

			if (File.Exists(storeLocation)) {
				using (var stream = new FileStream(storeLocation, FileMode.Open))
				{
					pharmacies = (List<Pharmacy>)serializer.Deserialize(stream);
				}
			}
		}

		static bool WriteXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"pharmacies.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<Pharmacy>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, pharmacies);
			}

			return true;
		}

		public static Pharmacy GetPharmacy(int id)
		{
			for (var t = 0; t < pharmacies.Count; t++) {
				if (pharmacies[t].id == id)
					return pharmacies[t];
			}
			return null;
		}

		public static IEnumerable<Pharmacy> GetPharmacies ()
		{
			return pharmacies;
		}

		public static IEnumerable<Pharmacy> GetPharmacies (int num)
		{
			return (from pharm in pharmacies
			         orderby pharm.id
			         select pharm).Take (num);
		}

		public static IEnumerable<Pharmacy> GetPharmacies (int skip, int take)
		{
			return (from pharm in pharmacies
			        orderby pharm.id
			        select pharm).Skip (skip).Take (take);
		}

		public static IEnumerable<Pharmacy> GetPharmacies (string search)
		{
			return (
				from pharm in pharmacies
				where pharm.fullName.ToLower().Contains (search) || pharm.address.ToLower().Contains(search)
				orderby pharm.id
				select pharm);
		}

		public static IEnumerable<Pharmacy> GetPharmacies (int[] ids)
		{
			return (pharmacies.Where(pharm => ids.Contains(pharm.id)));
		}

		/// <summary>
		/// Insert or update a Doctor
		/// </summary>
		public static int SavePharmacy (Pharmacy item)
		{
			var max = 0;
			if (pharmacies.Count > 0)
				max = pharmacies.Max(x => x.id);

			if (item.id == 0) {
				item.id = ++max;
				pharmacies.Add (item);
			} else {
				var i = pharmacies.Find (x => x.id == item.id);
				if (i != null) {
					i = item; // replaces item in collection with updated value
				} else {
					pharmacies.Add (item);
				}
			}

			WriteXml ();
			return item.id;
		}

		public static int DeletePharmacy (int id)
		{
			for (var t = 0; t< pharmacies.Count; t++) {
				if (pharmacies[t].id == id){
					pharmacies.RemoveAt (t);
					WriteXml ();
					return 1;
				}
			}
			return -1;
		}
	}
}

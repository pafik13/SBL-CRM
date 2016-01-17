//using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using Android.OS;

using SBLCRM.Lib.Entities;

namespace SBLCRM.Lib
{
    public class Common
    {
		public const string PHARMACY_ID = @"pharmacyID";

        static Common()
		{
		}

		public static bool CreateDirForPhotos(User user)
		{
			string storeLocation = Path.Combine(GetDirForPhotos(user),  @"photos.xml");
			new FileInfo(storeLocation).Directory.Create();
			return true;
		}

		public static string GetDirForPhotos(User user)
		{
			return Path.Combine(DatabaseFileDir, user.username, @"Photos");
		}

		/******  CURRENT USER  *****/
		public static User GetCurrentUser()
		{
			string storeLocation = Path.Combine(DatabaseFileDir,  @"currentUser.xml");
			if (!File.Exists(storeLocation)) {
				return null;
			}

			var serializer = new XmlSerializer(typeof(User));

			using (var stream = new FileStream(storeLocation, FileMode.Open))
			{
				return (User)serializer.Deserialize(stream);
			}
		}

		public static bool SetCurrentUser(User user)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, @"currentUser.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(User));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, user);
			}

			return true;
		}

        public static User GetUser(string username)
        {
			if (string.IsNullOrEmpty (username)) {
				return null;
			}

            string storeLocation = Path.Combine(DatabaseFileDir, username, @"user.xml");
            var serializer = new XmlSerializer(typeof(User));

            using (var stream = new FileStream(storeLocation, FileMode.Open))
            {
                return (User)serializer.Deserialize(stream);
            }
        }

        public static bool SetUser(string username, User user)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"user.xml");
            new FileInfo(storeLocation).Directory.Create();
            var serializer = new XmlSerializer(typeof(User));
            using (var writer = new StreamWriter(storeLocation))
            {
                serializer.Serialize(writer, user);
            }

            return true;
        }

		/******  IS_ATTENDANCE_RUN  *****/
		public static bool GetIsAttendanceRun(string username)
		{
			if (string.IsNullOrEmpty (username)) {
				return false;
			}

			string storeLocation = Path.Combine(DatabaseFileDir, username, @"isAttendanceRun.xml");
			var serializer = new XmlSerializer(typeof(bool));

			using (var stream = new FileStream(storeLocation, FileMode.Open))
			{
				return (bool)serializer.Deserialize(stream);
			}
		}

		public static bool SetIsAttendanceRun(string username, bool isAttendanceRun)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"isAttendanceRun.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(bool));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, isAttendanceRun);
			}

			return true;
		}

		/******  MERCHANT  *****/
		public static Merchant GetMerchant (string username)
		{
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"merchant.xml");
		    var serializer = new XmlSerializer (typeof(Merchant));

			using (var stream = new FileStream (storeLocation, FileMode.Open)) {
                return (Merchant)serializer.Deserialize(stream);
			}
		}

        public static bool SetMerchant(string username, Merchant merchant)
		{
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"merchant.xml");
            new FileInfo(storeLocation).Directory.Create();
            var serializer = new XmlSerializer(typeof(Merchant));
			using (var writer = new StreamWriter (storeLocation)) {
                serializer.Serialize(writer, merchant);
			}

            return true;
		}

		/******  MANAGER  *****/
        public static Manager GetManager(string username)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"manager.xml");

			var serializer = new XmlSerializer(typeof(Manager));
            using (var stream = new FileStream(storeLocation, FileMode.Open))
            {
                return (Manager)serializer.Deserialize(stream);
            }
        }

        public static bool SetManager(string username, Manager manager)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"manager.xml");
            new FileInfo(storeLocation).Directory.Create();
            var serializer = new XmlSerializer(typeof(Manager));
            using (var writer = new StreamWriter(storeLocation))
            {
                serializer.Serialize(writer, manager);
            }

            return true;
        }

		/******  PROJECT  *****/
        public static Project GetProject(string username)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"project.xml");
            var serializer = new XmlSerializer(typeof(Project));

            using (var stream = new FileStream(storeLocation, FileMode.Open))
            {
                return (Project)serializer.Deserialize(stream);
            }
        }

        public static bool SetProject(string username, Project project)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"project.xml");
            new FileInfo(storeLocation).Directory.Create();
            var serializer = new XmlSerializer(typeof(Project));
            using (var writer = new StreamWriter(storeLocation))
            {
                serializer.Serialize(writer, project);
            }

            return true;
        }

		/******  DRUG  *****/
        public static List<Drug> GetDrugs(string username)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"drugs.xml");
            var serializer = new XmlSerializer(typeof(List<Drug>));

            using (var stream = new FileStream(storeLocation, FileMode.Open))
            {
                return (List<Drug>)serializer.Deserialize(stream);
            }
        }

        public static bool SetDrugs(string username, List<Drug> drugs)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"drugs.xml");
            new FileInfo(storeLocation).Directory.Create();
            var serializer = new XmlSerializer(typeof(List<Drug>));
            using (var writer = new StreamWriter(storeLocation))
            {
                serializer.Serialize(writer, drugs);
            }

            return true;
        }

		/******  INFO  *****/
		public static List<Info> GetInfos(string username)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"infos.xml");
			var serializer = new XmlSerializer(typeof(List<Info>));

			if (File.Exists (storeLocation)) {
				using (var stream = new FileStream (storeLocation, FileMode.Open)) {
					return (List<Info>)serializer.Deserialize (stream);
				}
			}

			return new List<Info> ();
		}

		public static bool SetInfos(string username, List<Info> infos)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"infos.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<Info>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, infos);
			}

			return true;
		}

		/******  PROMO  *****/
		public static List<Promo> GetPromos(string username)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"promos.xml");
			var serializer = new XmlSerializer(typeof(List<Promo>));

			if (File.Exists (storeLocation)) {
				using (var stream = new FileStream (storeLocation, FileMode.Open)) {
					return (List<Promo>)serializer.Deserialize (stream);
				}
			}

			return new List<Promo> ();
		}

		public static bool SetPromos(string username, List<Promo> promos)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"promos.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<Promo>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, promos);
			}

			return true;
		}

		/******  NETCATEGORY  *****/
		public static List<NetCategory> GetNetCategories(string username)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"netCategories.xml");
			var serializer = new XmlSerializer(typeof(List<NetCategory>));

			if (File.Exists (storeLocation)) {
				using (var stream = new FileStream (storeLocation, FileMode.Open)) {
					return (List<NetCategory>)serializer.Deserialize (stream);
				}
			}

			return new List<NetCategory> ();
		}

		public static bool SetNetCategories(string username, List<NetCategory> netCategories)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"netCategories.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<NetCategory>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, netCategories);
			}

			return true;
		}

		/******  PHOTOTYPE  *****/
		public static List<PhotoType> GetPhotoTypes(string username)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"photoTypes.xml");
			var serializer = new XmlSerializer(typeof(List<PhotoType>));

			if (File.Exists (storeLocation)) {
				using (var stream = new FileStream (storeLocation, FileMode.Open)) {
					return (List<PhotoType>)serializer.Deserialize (stream);
				}
			}

			return new List<PhotoType> ();
		}

		public static bool SetPhotoTypes(string username, List<PhotoType> photoTypes)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"photoTypes.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<PhotoType>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, photoTypes);
			}

			return true;
		}

		/******  PHOTOSUBTYPE  *****/
		public static List<PhotoSubType> GetPhotoSubTypes(string username)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"photoSubTypes.xml");
			var serializer = new XmlSerializer(typeof(List<PhotoSubType>));

			if (File.Exists (storeLocation)) {
				using (var stream = new FileStream (storeLocation, FileMode.Open)) {
					return (List<PhotoSubType>)serializer.Deserialize (stream);
				}
			}

			return new List<PhotoSubType> ();
		}

		public static bool SetPhotoSubTypes(string username, List<PhotoSubType> photoSubTypes)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"photoSubTypes.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<PhotoSubType>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, photoSubTypes);
			}

			return true;
		}

		/******  TERRITORY  *****/
        public static Territory GetTerritory(string username)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"territory.xml");
            var serializer = new XmlSerializer(typeof(Territory));

            using (var stream = new FileStream(storeLocation, FileMode.Open))
            {
                return (Territory)serializer.Deserialize(stream);
            }
        }

        public static bool SetTerritory(string username, Territory territory)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"territory.xml");
            new FileInfo(storeLocation).Directory.Create();
            var serializer = new XmlSerializer(typeof(Territory));
            using (var writer = new StreamWriter(storeLocation))
            {
                serializer.Serialize(writer, territory);
            }

            return true;
        }

		/******  TRADENET  *****/
		public static List<TradeNet> GetTradeNets(string username)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"tradenets.xml");
			var serializer = new XmlSerializer(typeof(List<TradeNet>));

			using (var stream = new FileStream(storeLocation, FileMode.Open))
			{
				return (List<TradeNet>)serializer.Deserialize(stream);
			}
		}

		public static bool SetTradeNets(string username, List<TradeNet> tradenets)
		{
			string storeLocation = Path.Combine(DatabaseFileDir, username, @"tradenets.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<TradeNet>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, tradenets);
			}

			return true;
		}

		/******  PHARMACY  *****/
        public static List<Pharmacy> GetPharmacies(string username)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"pharmacies.xml");
            var serializer = new XmlSerializer(typeof(List<Pharmacy>));

            using (var stream = new FileStream(storeLocation, FileMode.Open))
            {
                return (List<Pharmacy>)serializer.Deserialize(stream);
            }
        }

        public static bool SetPharmacies(string username, List<Pharmacy> pharmacies)
        {
            string storeLocation = Path.Combine(DatabaseFileDir, username, @"pharmacies.xml");
            new FileInfo(storeLocation).Directory.Create();
            var serializer = new XmlSerializer(typeof(List<Pharmacy>));
            using (var writer = new StreamWriter(storeLocation))
            {
                serializer.Serialize(writer, pharmacies);
            }

            return true;
        }

		public static string DatabaseFileDir {
            get {
				return Path.Combine(Environment.ExternalStorageDirectory.AbsolutePath, @"MyTempDir", @"SBLCRM");
            }
		}

    }
}

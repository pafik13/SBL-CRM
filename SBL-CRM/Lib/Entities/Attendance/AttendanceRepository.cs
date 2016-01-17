using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace SBLCRM.Lib.Entities
{
	public class AttendanceRepository
	{
		static string fUserName;
		static List<Attendance> attendances;

		static AttendanceRepository ()
		{
			// set the db location
			attendances = new List<Attendance> ();

			Refresh ();
		}

		public static void Refresh()
		{
			User user = Common.GetCurrentUser ();
			if (user != null) {
				fUserName = user.username;
			} else {
				fUserName = @"";
				attendances = new List<Attendance> ();
			}

			ReadXml ();
		}

		static void ReadXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"attendances.xml");
			var serializer = new XmlSerializer(typeof(List<Attendance>));

			if (File.Exists(storeLocation)) {
				using (var stream = new FileStream(storeLocation, FileMode.Open))
				{
					attendances = (List<Attendance>)serializer.Deserialize(stream);
				}
			}
		}

		static bool WriteXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"attendances.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<Attendance>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, attendances);
			}

			return true;
		}

		public static Attendance GetAttendance(int id)
		{
			for (var t = 0; t < attendances.Count; t++) {
				if (attendances[t].id == id)
					return attendances[t];
			}
			return null;
		}

		public static Attendance GetLastAttendance(int pharmacyID)
		{
			return (from att in attendances
					where att.pharmacy == pharmacyID
			         orderby att.date descending
			         select att).FirstOrDefault ();
		}

		public static IEnumerable<Attendance> GetAttendances ()
		{
			return attendances;
		}

		public static IEnumerable<Attendance> GetAttendances (int pharmacyID)
		{
			return (
			  from att in attendances
		    where att.pharmacy == pharmacyID
			  orderby att.date
			  select att
			);
		}

		/// <summary>
		/// Добавление или обновление доктора
		/// </summary>
		public static int SaveAttendance (Attendance item)
		{
			var max = 0;
			if (attendances.Count > 0)
				max = attendances.Max(x => x.id);

			if (item.id <= 0) {
				item.id = ++max;

				SyncQueueManager.AddToQueue (item);

				attendances.Add (item);
			} else {
				var i = attendances.Find (x => x.id == item.id);
				if (i != null) {
					i = item; // replaces item in collection with updated value
				} else {
					attendances.Add (item);
				}
			}

			WriteXml ();
			return item.id;
		}

		public static bool CorrectAfterSync(Attendance oldItem, Attendance newItem)
		{
			for (int i = 0; i < attendances.Count; i++) {
				if (attendances [i].id == oldItem.id) {
					attendances [i] = newItem;
					WriteXml ();
					return true;
				}
			}
			return false;
		}

		public static int DeleteAttendance (int id)
		{
			for (var t = 0; t< attendances.Count; t++) {
				if (attendances[t].id == id){
					attendances.RemoveAt (t);
					WriteXml ();
					return 1;
				}
			}
			return -1;
		}

		public static Attendance GetCurrentAttendance()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"Current", @"attendance.xml");
			if (!File.Exists(storeLocation)) {
				return null;
			}

			var serializer = new XmlSerializer(typeof(Attendance));

			using (var stream = new FileStream(storeLocation, FileMode.Open))
			{
				return (Attendance)serializer.Deserialize(stream);
			}
		}

		public static bool SetCurrentAttendance(Attendance attendance)
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"Current", @"attendance.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(Attendance));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, attendance);
			}

			return true;
		}

		public static string GetStatistics(int pharmacyID)
		{
			return @"Статистика посещений";
		}
	}
}

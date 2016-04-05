using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace SBLCRM.Lib.Entities
{
	public class AttendanceGPSPointRepository
	{
		static string fUserName;
		static List<AttendanceGPSPoint> attendanceGPSPoints;

		static AttendanceGPSPointRepository ()
		{
			// set the db location
			attendanceGPSPoints = new List<AttendanceGPSPoint> ();

			Refresh ();
		}

		public static void Refresh()
		{
			User user = Common.GetCurrentUser ();
			if (user != null) {
				fUserName = user.username;
			} else {
				fUserName = @"";
				attendanceGPSPoints = new List<AttendanceGPSPoint> ();
			}

			ReadXml ();
		}

		static void ReadXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"attendanceGPSPoints.xml");
			var serializer = new XmlSerializer(typeof(List<AttendanceGPSPoint>));

			if (File.Exists(storeLocation)) {
				using (var stream = new FileStream(storeLocation, FileMode.Open))
				{
					attendanceGPSPoints = (List<AttendanceGPSPoint>)serializer.Deserialize(stream);
				}
			}
		}

		static bool WriteXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"attendanceGPSPoints.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<AttendanceGPSPoint>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, attendanceGPSPoints);
			}

			return true;
		}

		public static AttendanceGPSPoint GetAttendanceGPSPoint(int id)
		{
			for (int i = 0; i  < attendanceGPSPoints.Count; i++) {
				if (attendanceGPSPoints[i].id == id)
					return attendanceGPSPoints[i];
			}
			return null;
		}

		public static IEnumerable<AttendanceGPSPoint> GetAttendanceGPSPoints ()
		{
			return attendanceGPSPoints;
		}

		public static IEnumerable<AttendanceGPSPoint> GetAttendanceGPSPoints (int attendanceID)
		{
			return (
				from attGPS in attendanceGPSPoints
				where attGPS.attendance == attendanceID
				orderby attGPS.stamp
				select attGPS
			);
		}

		/// <summary>
		/// Insert or update a AttendanceGPSPoint
		/// </summary>
		public static int SaveAttendanceGPSPoint (AttendanceGPSPoint item)
		{
			var max = 0;
			if (attendanceGPSPoints.Count > 0)
				max = attendanceGPSPoints.Max(x => x.id);

			if (item.id == 0) {
				item.id = ++max;

				attendanceGPSPoints.Add (item);
			} else {
				var i = attendanceGPSPoints.Find (x => x.id == item.id);
				if (i != null) {
					i = item; // replaces item in collection with updated value
				} else {
					attendanceGPSPoints.Add (item);
				}
			}

			WriteXml ();
			return item.id;
		}

		public static bool SaveNewAttendanceGPSPoints (int attendanceID, List<AttendanceGPSPoint> points)
		{
			foreach (var item in points) {
				item.attendance = attendanceID;
				var max = 0;
				if (attendanceGPSPoints.Count > 0)
					max = attendanceGPSPoints.Max(x => x.id);
				item.id = ++max;

				attendanceGPSPoints.Add (item);
			}
			WriteXml ();
			return true;
		}

		public static bool CreateItemsForSync(Attendance oldAttendance, Attendance newAttendance)
		{
			for (int i = 0; i < attendanceGPSPoints.Count; i++) {
				if (attendanceGPSPoints[i].attendance == oldAttendance.id) {
					attendanceGPSPoints[i].attendance = newAttendance.id;
					SyncQueueManager.AddToQueue (attendanceGPSPoints[i], oldAttendance);
					attendanceGPSPoints[i].attendance = oldAttendance.id;
				}				
			}

			return true;
		}

		public static int DeleteAttendanceGPSPoint (int id)
		{
			for (var t = 0; t< attendanceGPSPoints.Count; t++) {
				if (attendanceGPSPoints[t].id == id){
					attendanceGPSPoints.RemoveAt (t);
					WriteXml ();
					return t;
				}
			}
			return -1;
		}


		public static List<AttendanceGPSPoint> GetCurrentAttendanceGPSPoints()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"Current", @"attendanceGPSPoints.xml");
			if (!File.Exists(storeLocation)) {
				return null;
			}

			var serializer = new XmlSerializer(typeof(List<AttendanceGPSPoint>));

			using (var stream = new FileStream(storeLocation, FileMode.Open))
			{
				return (List<AttendanceGPSPoint>)serializer.Deserialize(stream);
			}
		}

		public static bool SetCurrentAttendanceGPSPoints(List<AttendanceGPSPoint> points)
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"Current", @"attendanceGPSPoints.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<AttendanceGPSPoint>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, points);
			}

			return true;
		}
	}
}

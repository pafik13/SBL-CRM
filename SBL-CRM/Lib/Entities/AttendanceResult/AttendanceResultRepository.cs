using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

namespace SBLCRM.Lib.Entities
{
	public class AttendanceResultRepository
	{
		static string fUserName;
		static List<AttendanceResult> attendanceResults;

		static AttendanceResultRepository ()
		{
			// set the db location
			attendanceResults = new List<AttendanceResult> ();

			Refresh ();
		}

		public static void Refresh()
		{
			User user = Common.GetCurrentUser ();
			if (user != null) {
				fUserName = user.username;
			} else {
				fUserName = @"";
				attendanceResults = new List<AttendanceResult> ();
			}

			ReadXml ();
		}

		static void ReadXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"attendanceResults.xml");
			var serializer = new XmlSerializer(typeof(List<AttendanceResult>));

			if (File.Exists(storeLocation)) {
				using (var stream = new FileStream(storeLocation, FileMode.Open))
				{
					attendanceResults = (List<AttendanceResult>)serializer.Deserialize(stream);
				}
			}
		}

		static bool WriteXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"attendanceResults.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<AttendanceResult>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, attendanceResults);
			}

			return true;
		}

		public static AttendanceResult GetAttendanceResult(int id)
		{
			for (var t = 0; t < attendanceResults.Count; t++) {
				if (attendanceResults[t].id == id)
					return attendanceResults[t];
			}
			return null;
		}

		public static IEnumerable<AttendanceResult> GetAttendanceResults ()
		{
			return attendanceResults;
		}

		public static IEnumerable<AttendanceResult> GetAttendanceResults (int attendanceID)
		{
			return (
			  from attRes in attendanceResults
		    where attRes.attendance == attendanceID
			  orderby attRes.info, attRes.drug
			  select attRes
			);
		}

		public static string GetAttendanceResultValue (
		  int attendanceID, int infoID, int drugID)
		{
			var searchResult = from attRes in attendanceResults
										     where (attRes.attendance == attendanceID)
												    && (attRes.info == infoID)
														&& (attRes.drug == drugID)
											   select attRes;

			if (searchResult.Count() > 1)
 			{
 				return string.Empty;
 			}

 			foreach (var item in searchResult) {
 				return item.value;
 			}

 			return string.Empty;
		}

		public static bool SetAttendanceResultValue (
		  int attendanceID, int infoID, int drugID, string value)
		{
			foreach (var result in attendanceResults) {
				if (result.attendance == attendanceID) {
					if (result.info == infoID) {
						if (result.drug == drugID) {
							result.value = value;
							WriteXml();
							return true;
						}
					}
				}
			}

				return false;
		}

		/// <summary>
		/// Insert or update a Doctor
		/// </summary>
		public static int SaveAttendanceResult (AttendanceResult item)
		{
			var max = 0;
			if (attendanceResults.Count > 0)
				max = attendanceResults.Max(x => x.id);

			if (item.id == 0) {
				item.id = ++max;

//				SyncQueueManager.AddToQueue (item);

				attendanceResults.Add (item);
			} else {
				var i = attendanceResults.Find (x => x.id == item.id);
				if (i != null) {
					i = item; // replaces item in collection with updated value
				} else {
					attendanceResults.Add (item);
				}
			}

			WriteXml ();
			return item.id;
		}

		public static bool SaveNewAttendanceResults (int attendanceID, List<AttendanceResult> results)
		{
			foreach (var item in results) {
				item.attendance = attendanceID;
				var max = 0;
				if (attendanceResults.Count > 0)
					max = attendanceResults.Max(x => x.id);
				item.id = ++max;

//				SyncQueueManager.AddToQueue (item);

				attendanceResults.Add (item);
			}
			WriteXml ();
			return true;
		}

		public static bool CorrectAttendanceForSync(int oldAttendance, int newAttendance)
		{
			for (int i = 0; i < attendanceResults.Count; i++) {
				if (attendanceResults[i].attendance == oldAttendance) {
					attendanceResults[i].attendance = newAttendance;
					SyncQueueManager.AddToQueue (attendanceResults[i]);
				}				
			}
				
			WriteXml ();
			return true;
		}

		public static int DeleteAttendanceResult (int id)
		{
			for (var t = 0; t< attendanceResults.Count; t++) {
				if (attendanceResults[t].id == id){
					attendanceResults.RemoveAt (t);
					WriteXml ();
					return 1;
				}
			}
			return -1;
		}


		public static List<AttendanceResult> GetCurrentAttendanceResults()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"Current", @"attendanceResults.xml");
			if (!File.Exists(storeLocation)) {
				return null;
			}

			var serializer = new XmlSerializer(typeof(List<AttendanceResult>));

			using (var stream = new FileStream(storeLocation, FileMode.Open))
			{
				return (List<AttendanceResult>)serializer.Deserialize(stream);
			}
		}

		public static bool SetCurrentAttendanceResults(List<AttendanceResult> attendanceResults)
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"Current", @"attendanceResults.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<AttendanceResult>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, attendanceResults);
			}

			return true;
		}
	}
}

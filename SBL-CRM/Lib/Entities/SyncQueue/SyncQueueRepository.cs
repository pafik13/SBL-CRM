using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;

namespace SBLCRM.Lib.Entities
{
	public class SyncQueueRepository
	{
		static string fUserName;
		static List<SyncQueue> queue;

		static SyncQueueRepository ()
		{
			// set the db location
			queue = new List<SyncQueue> ();

			Refresh ();
		}

		public static void Refresh()
		{
			User user = Common.GetCurrentUser ();
			if (user != null) {
				fUserName = user.username;
			} else {
				fUserName = @"";
				queue = new List<SyncQueue> ();
			}

			ReadXml ();
		}

		static void ReadXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"SyncQueue",  @"queue.xml");
			var serializer = new XmlSerializer(typeof(List<SyncQueue>));

			if (File.Exists(storeLocation)) {
				using (var stream = new FileStream(storeLocation, FileMode.Open))
				{
					queue = (List<SyncQueue>)serializer.Deserialize(stream);
				}
			}
		}

		static bool WriteXml ()
		{
			string storeLocation = Path.Combine(Common.DatabaseFileDir, fUserName, @"SyncQueue", @"queue.xml");
			new FileInfo(storeLocation).Directory.Create();
			var serializer = new XmlSerializer(typeof(List<SyncQueue>));
			using (var writer = new StreamWriter(storeLocation))
			{
				serializer.Serialize(writer, queue);
			}

			return true;
		}

		public static SyncQueue GetSyncQueue(int id)
		{
			for (var t = 0; t < queue.Count; t++) {
				if (queue[t].id == id)
					return queue[t];
			}
			return null;
		}

		public static IEnumerable<SyncQueue> GetSyncQueue ()
		{
			return queue;
		}

		public static IEnumerable<DateTime> GetAvailableDatesDesc ()
		{
			IEnumerable<DateTime> result = (
				from q in queue
				orderby q.stamp.Date descending
				select q.stamp.Date
			).Distinct();

			return result;
		}

		public static IEnumerable<DateTime> GetAvailableDatesAsc ()
		{
			IEnumerable<DateTime> result = (
				from q in queue
				orderby q.stamp.Date
				select q.stamp.Date
			).Distinct();

			return result;
		}

//		public static IEnumerable<DateTime> GetSyncedAttendacesCountByPharmacy ()
//		{
//			IEnumerable<DateTime> result = (
//				from q in queue
//				where q.type = 
//				select q.stamp.Date
//			).Distinct();
//
//			return result;
//		}

		public static IEnumerable<SyncQueue> GetSyncQueue (DateTime date)
		{
			return (from q in queue
			        where q.stamp.Date == date.Date
							orderby q.stamp
			        select q);
		}

		public static int AddToQueue (Attendance attendance)
		{
			SyncQueue queueItem = new SyncQueue() {type = SyncQueueType.sqtAttendance};

			queueItem.fileLoacation = Path.Combine(Common.DatabaseFileDir, fUserName, @"SyncQueue", String.Format("attendance_{0}.xml", Guid.NewGuid()));

			new FileInfo(queueItem.fileLoacation).Directory.Create();
			var serializer = new XmlSerializer(typeof(Attendance));
			using (var writer = new StreamWriter(queueItem.fileLoacation))
			{
				serializer.Serialize(writer, attendance);
			}

			return SaveSyncQueue(queueItem);
		}

		public static Attendance GetAttendace(string location)
		{
			var serializer = new XmlSerializer(typeof(Attendance));

			if (File.Exists(location)) {
				using (var stream = new FileStream(location, FileMode.Open))
				{
					return (Attendance)serializer.Deserialize(stream);
				}
			}

			return null;
		}

		public static int AddToQueue (AttendanceResult attendanceResult)
		{
			SyncQueue queueItem = new SyncQueue() {type = SyncQueueType.sqtAttendanceResult};

			queueItem.fileLoacation = Path.Combine(Common.DatabaseFileDir, fUserName, @"SyncQueue", String.Format("attendanceResult_{0}.xml", Guid.NewGuid()));

			new FileInfo(queueItem.fileLoacation).Directory.Create();
			var serializer = new XmlSerializer(typeof(AttendanceResult));
			using (var writer = new StreamWriter(queueItem.fileLoacation))
			{
				serializer.Serialize(writer, attendanceResult);
			}

			return SaveSyncQueue(queueItem);
		}

		public static AttendanceResult GetAttendaceResult(string location)
		{
			var serializer = new XmlSerializer(typeof(AttendanceResult));

			if (File.Exists(location)) {
				using (var stream = new FileStream(location, FileMode.Open))
				{
					return (AttendanceResult)serializer.Deserialize(stream);
				}
			}

			return null;
		}

		public static int AddToQueue(AttendancePhoto attendancePhoto)
		{
			SyncQueue queueItem = new SyncQueue() {type = SyncQueueType.sqtAttendancePhoto};

			queueItem.fileLoacation = Path.Combine(Common.DatabaseFileDir, fUserName, @"SyncQueue", String.Format("attendancePhoto_{0}.xml", Guid.NewGuid()));

			new FileInfo(queueItem.fileLoacation).Directory.Create();
			var serializer = new XmlSerializer(typeof(AttendancePhoto));
			using (var writer = new StreamWriter(queueItem.fileLoacation))
			{
				serializer.Serialize(writer, attendancePhoto);
			}

			return SaveSyncQueue(queueItem);
		}

		public static AttendancePhoto GetAttendancePhoto(string location)
		{
			var serializer = new XmlSerializer(typeof(AttendancePhoto));

			if (File.Exists(location)) {
				using (var stream = new FileStream(location, FileMode.Open))
				{
					return (AttendancePhoto)serializer.Deserialize(stream);
				}
			}

			return null;
		}

		/// <summary>
		/// Insert or update a Doctor
		/// </summary>
		public static int SaveSyncQueue (SyncQueue item)
		{
			var max = 0;
			if (queue.Count > 0)
				max = queue.Max(x => x.id);

			if (item.id == 0) {
				item.id = ++max;
				queue.Add (item);
			} else {
				var i = queue.Find (x => x.id == item.id);
				if (i != null) {
					i = item; // replaces item in collection with updated value
				} else {
					queue.Add (item);
				}
			}

			WriteXml ();
			return item.id;
		}

		public static int DeleteSyncQueue (int id)
		{
			for (var t = 0; t< queue.Count; t++) {
				if (queue[t].id == id){
					queue.RemoveAt (t);
					WriteXml ();
					return 1;
				}
			}
			return -1;
		}
	}
}

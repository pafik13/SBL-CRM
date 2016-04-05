using System;

namespace SBLCRM.Lib.Entities
{
	public enum SyncQueueType
	{
		sqtAttendance,
		sqtAttendanceResult,
		sqtAttendancePhoto,
		sqtAttendanceGPSPoint
	}

	public class SyncQueue: IEntity
	{
		public SyncQueue ()
		{
			stamp = DateTime.Now;
			isSync = false;
		}

		public int id { get; set; }
		public DateTime stamp { get; set; }
		public DateTime attendanceDate { get; set; }
		public SyncQueueType type { get; set; }
		public int itemID { get; set; }
		public string fileLocation { get; set; }
		public bool isSync { get; set; }
	}
}


using System;

namespace SBLCRM.Lib.Entities
{
	public enum SyncQueueType
	{
		sqtAttendance,
		sqtAttendanceResult,
		sqtAttendancePhoto
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
		public SyncQueueType type { get; set; }
		public string fileLoacation { get; set; }
		public bool isSync { get; set; }
	}
}


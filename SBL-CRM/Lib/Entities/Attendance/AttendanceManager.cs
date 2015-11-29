using System.Collections.Generic;

namespace SBLCRM.Lib.Entities
{
	public static class AttendanceManager
	{
		static AttendanceManager ()
		{
		}

		public static void Refresh()
		{
			AttendanceRepository.Refresh ();
		}

		public static Attendance GetAttendance(int id)
		{
			return AttendanceRepository.GetAttendance(id);
		}

		public static IList<Attendance> GetAttendances (int pharmacyID = -1)
		{
			if (pharmacyID == -1) {
				return new List<Attendance> (AttendanceRepository.GetAttendances ());
			} else {
				return new List<Attendance> (AttendanceRepository.GetAttendances (pharmacyID));
			}
		}

		public static int SaveAttendance (Attendance item)
		{
			return AttendanceRepository.SaveAttendance(item);;
		}

		public static bool CorrectAfterSync(Attendance oldItem, Attendance newItem)
		{
			return AttendanceRepository.CorrectAfterSync (oldItem, newItem);
		}

		public static int DeleteAttendance(int id)
		{
			return AttendanceRepository.DeleteAttendance(id);
		}
			
	}
}

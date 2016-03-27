using System.Linq;
using System.Collections.Generic;

namespace SBLCRM.Lib.Entities
{
	public class AttendanceGPSPointManager
	{
		public AttendanceGPSPointManager ()
		{
		}

		public static void Refresh()
		{
			AttendanceGPSPointRepository.Refresh ();
		}

		public static AttendanceGPSPoint GetAttendanceGPSPoint(int id)
		{
			return AttendanceGPSPointRepository.GetAttendanceGPSPoint(id);
		}

		public static IList<AttendanceGPSPoint> GetAttendanceGPSPoints (int attendanceID = -1)
		{
			if (attendanceID == -1) {
				return new List<AttendanceGPSPoint> (AttendanceGPSPointRepository.GetAttendanceGPSPoints ());
			} else {
				return new List<AttendanceGPSPoint> (AttendanceGPSPointRepository.GetAttendanceGPSPoints (attendanceID));
			}
		}

		public static int SaveAttendanceGPSPoint (AttendanceGPSPoint item)
		{
			return AttendanceGPSPointRepository.SaveAttendanceGPSPoint(item);
		}

		public static bool SaveNewAttendanceGPSPoints (int attendanceID, List<AttendanceGPSPoint> points)
		{
			return AttendanceGPSPointRepository.SaveNewAttendanceGPSPoints(attendanceID, points);
		}

		public static bool CorrectAttendanceForSync(int oldAttendance, int newAttendance)
		{
			return AttendanceGPSPointRepository.CorrectAttendanceForSync (oldAttendance, newAttendance);
		}

		public static int DeleteAttendanceGPSPoint(int id)
		{
			return AttendanceGPSPointRepository.DeleteAttendanceGPSPoint(id);
		}

		public static List<AttendanceGPSPoint> GetCurrentAttendanceGPSPoints()
		{
			return AttendanceGPSPointRepository.GetCurrentAttendanceGPSPoints ();
		}

		public static bool SetCurrentAttendanceGPSPoints(List<AttendanceGPSPoint> attendanceGPSPoints)
		{
			return AttendanceGPSPointRepository.SetCurrentAttendanceGPSPoints(attendanceGPSPoints);
		}
	}
}

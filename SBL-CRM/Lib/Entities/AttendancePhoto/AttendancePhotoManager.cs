using System.Collections.Generic;

namespace SBLCRM.Lib.Entities
{
	public static class AttendancePhotoManager
	{
		static AttendancePhotoManager ()
		{
		}

		public static void Refresh()
		{
			AttendancePhotoRepository.Refresh ();
		}

		public static AttendancePhoto GetAttendancePhoto(int id)
		{
			return AttendancePhotoRepository.GetAttendancePhoto(id);
		}

		public static IList<AttendancePhoto> GetAttendancePhotos (int attendanceID = -1)
		{
			if (attendanceID == -1) {
				return new List<AttendancePhoto> (AttendancePhotoRepository.GetAttendancePhotos ());
			} else {
				return new List<AttendancePhoto> (AttendancePhotoRepository.GetAttendancePhotos (attendanceID));
			}
		}

		public static int SaveAttendancePhoto (AttendancePhoto item)
		{
			return AttendancePhotoRepository.SaveAttendancePhoto(item);;
		}

		public static bool SaveNewAttendancePhotos (int attendanceID, List<AttendancePhoto> photos)
		{
			return AttendancePhotoRepository.SaveNewAttendancePhotos(attendanceID, photos);
		}

		public static bool CorrectAttendanceForSync(int oldAttendance, int newAttendance)
		{
			return AttendancePhotoRepository.CorrectAttendanceForSync (oldAttendance, newAttendance);
		}

		public static int DeleteAttendancePhoto(int id)
		{
			return AttendancePhotoRepository.DeleteAttendancePhoto(id);
		}

		public static List<AttendancePhoto> GetCurrentAttendancePhotos()
		{
			return AttendancePhotoRepository.GetCurrentAttendancePhotos ();
		}

		public static bool SetCurrentAttendancePhotos(List<AttendancePhoto> attendancePhotos)
		{
			return AttendancePhotoRepository.SetCurrentAttendancePhotos(attendancePhotos);
		}
	}
}

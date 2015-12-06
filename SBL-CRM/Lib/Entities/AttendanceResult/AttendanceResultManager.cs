using System.Linq;
using System.Collections.Generic;

namespace SBLCRM.Lib.Entities
{
	public static class AttendanceResultManager
	{
		static AttendanceResultManager ()
		{
		}

		public static void Refresh()
		{
			AttendanceResultRepository.Refresh ();
		}

		public static AttendanceResult GetAttendanceResult(int id)
		{
			return AttendanceResultRepository.GetAttendanceResult(id);
		}

		public static IList<AttendanceResult> GetAttendanceResults (int attendanceID = -1)
		{
			if (attendanceID == -1) {
				return new List<AttendanceResult> (AttendanceResultRepository.GetAttendanceResults ());
			} else {
				return new List<AttendanceResult> (AttendanceResultRepository.GetAttendanceResults (attendanceID));
			}
		}

		public static int SaveAttendanceResult (AttendanceResult item)
		{
			return AttendanceResultRepository.SaveAttendanceResult(item);
		}

		public static bool SaveNewAttendanceResults (int attendanceID, List<AttendanceResult> results)
		{
			return AttendanceResultRepository.SaveNewAttendanceResults(attendanceID, results);
		}

		public static string GetAttendanceResultValue (
		  int attendanceID, int infoID, int drugID)
		{
			return AttendanceResultRepository.GetAttendanceResultValue (
			  attendanceID, infoID, drugID
			);
		}

		public static bool SetAttendanceResultValue (
		  int attendanceID, int infoID, int drugID, string value)
		{
			return AttendanceResultRepository.SetAttendanceResultValue (
				attendanceID, infoID, drugID, value
			);
		}

		public static bool CorrectAttendanceForSync(int oldAttendance, int newAttendance)
		{
			return AttendanceResultRepository.CorrectAttendanceForSync (oldAttendance, newAttendance);
		}

		public static int DeleteAttendanceResult(int id)
		{
			return AttendanceResultRepository.DeleteAttendanceResult(id);
		}

		public static List<AttendanceResult> GetCurrentAttendanceResults()
		{
			return AttendanceResultRepository.GetCurrentAttendanceResults ();
		}

		public static bool SetCurrentAttendanceResults(List<AttendanceResult> attendanceResults)
		{
			return AttendanceResultRepository.SetCurrentAttendanceResults(attendanceResults);
		}

		public static List<AttendanceResult> GenerateResults(List<Info> infos, List<Drug> drugs, string defaultValue = null)
		{
			if (string.IsNullOrEmpty (defaultValue)) {
				defaultValue = string.Empty;
			}

			var results = new List<AttendanceResult> ();
			foreach (var info in infos) {
				foreach (var drug in drugs) {
					results.Add (new AttendanceResult () {
						info = info.id,
						drug = drug.id,
						value = defaultValue
					});
				}
			}

			return results;
		}

		public static string GetResultValue (List<AttendanceResult> results, int infoID, int drugID)
		{
			var searchResult = from attRes in results
					where (attRes.info == infoID) && (attRes.drug == drugID)
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

		public static bool SetResultValue (List<AttendanceResult> results, int infoID, int drugID, string value)
		{
			foreach (var result in results) {
				if (result.info == infoID) {
					if (result.drug == drugID) {
						result.value = value;
						return true;
					}
				}
			}

			return false;
		}
	}
}

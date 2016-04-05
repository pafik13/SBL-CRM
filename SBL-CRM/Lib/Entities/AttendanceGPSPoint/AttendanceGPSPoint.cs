using System;
using System.Globalization;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class AttendanceGPSPoint : IEntity
	{
		public AttendanceGPSPoint ()
		{
			// empty
		}

		public int id { get; set; }
		public int attendance { get; set; }
		public double longitude { get; set; }
		public double latitude { get; set; }
		public string provider { get; set; }
		public DateTime stamp { get; set; }

		public string ToJSON()
		{
			const char quote = '"';
			return
				@" { "
					+ quote + @"localID" + quote + @" : " + id + @","	
					+ quote + @"attendance" + quote + @" : " + attendance + @","	
					+ quote + @"longitude" + quote + @" : " + longitude.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + @","
					+ quote + @"latitude" + quote + @" : " + latitude.ToString(CultureInfo.CreateSpecificCulture("en-GB")) + @","
					+ quote + @"provider" + quote + @" : " + quote + provider + quote + @","
					+ quote + @"stamp" + quote + @" : " + quote + stamp.ToString(@"O") + quote
					+
				@" } ";
		}
	}
}
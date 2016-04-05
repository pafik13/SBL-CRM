using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class AttendancePhoto: IEntity
	{
		public AttendancePhoto ()
		{
			stamp = DateTime.Now;
		}

		public int id { get; set; }
		public int attendance { get; set; }
		public int drug { get; set; }
		public int subType { get; set; }
		public double longitude { get; set; }
		public double latitude { get; set; }
		public string photoPath { get; set; }
		public DateTime stamp { get; set; }

		public string ToJSON()
		{
			const char quote = '"';
			return
				@" { " 
					+ quote + @"localID" + quote + @" : " + id + @","	
					+ quote + @"attendance" + quote + @" : " + attendance + @","	
					+ quote + @"drug" + quote + @" : " + drug + @","
					+ quote + @"subType" + quote + @" : " + subType + @","
					+ quote + @"longitude" + quote + @" : " + longitude + @","
					+ quote + @"latitude" + quote + @" : " + latitude + @","
					+ quote + @"stamp" + quote + @" : " + quote + stamp.ToString(@"O") + quote
					+
				@" } ";
		}
	}
}

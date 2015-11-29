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
		public float longitude { get; set; }
		public float latitude { get; set; }
		public string photoPath { get; set; }
		public DateTime stamp { get; set; }

		public string ToJSON()
		{
			const char quote = '"';
			return
				@" { " 
					+ quote + @"attendance" + quote + @" : " + attendance + @","	
					+ quote + @"drug" + quote + @" : " + drug + @","
					+ quote + @"longitude" + quote + @" : " + longitude + @","
					+ quote + @"latitude" + quote + @" : " + latitude + @","
					+ quote + @"stamp" + quote + @" : " + quote + stamp.ToString(@"O") + quote
					+
				@" } ";
		}
	}
}

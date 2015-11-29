using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class Attendance: IEntity
	{
		public Attendance ()
		{
		}

		public int id { get; set; }
		public int merchant { get; set; }
		public int pharmacy { get; set; }
		public DateTime date { get; set; }

		public string ToJSON()
		{
			const char quote = '"';
			return
				@" { " 
					+ quote + "merchant" + quote + @" : " + merchant + ","	
					+ quote + "pharmacy" + quote + @" : " + pharmacy + ","
					+ quote + "date" + quote + @" : " + quote + date.ToString(@"O") + quote
					+
				@" } ";
		}
	}
}

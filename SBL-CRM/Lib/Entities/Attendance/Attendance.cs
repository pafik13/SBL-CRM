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
		public string category_net { get; set; }
		public string telephone { get; set; }
		public string purchaserFIO { get; set; }
		public string promos { get; set; }
		public int pharmacistCount { get; set; }
		public DateTime date { get; set; }

		public string ToJSON()
		{
			const char quote = '"';
			return
				@" { " 
					+ quote + @"merchant" + quote + @" : " + merchant + ","	
					+ quote + @"pharmacy" + quote + @" : " + pharmacy + ","
					+ quote + @"category_net" + quote + @" : " + quote + category_net + quote + ","
					+ quote + @"telephone" + quote + @" : " + quote + telephone + quote + ","
					+ quote + @"purchaserFIO" + quote + @" : " + quote + purchaserFIO + quote + ","
					+ quote + @"promos" + quote + @" : " + quote + promos + quote + ","
					+ quote + @"pharmacistCount" + quote + @" : " + pharmacistCount + ","
					+ quote + @"date" + quote + @" : " + quote + date.ToString(@"O") + quote
					+
				@" } ";
		}
	}
}

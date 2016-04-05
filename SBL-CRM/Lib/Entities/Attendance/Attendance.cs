using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class Attendance: IEntity
	{
		public Attendance ()
		{
			promos = new int[0];
		}

		public int id { get; set; }
		public int merchant { get; set; }
		public int pharmacy { get; set; }
		public int category_net { get; set; }
		public string telephone { get; set; }
		public string comment { get; set; }
		public string purchaserFIO { get; set; }
		public int[] promos { get; set; }
		public int pharmacistCount { get; set; }
		public DateTime date { get; set; }

		public string ToJSON()
		{
			const char quote = '"';
//			if (promos == null) {
//				promos = new int[0];
//			}
			return
				@" { " 
					+ quote + @"localID" + quote + @" : " + id + @","	
					+ quote + @"merchant" + quote + @" : " + merchant + @","	
					+ quote + @"pharmacy" + quote + @" : " + pharmacy + @","
					+ quote + @"category_net" + quote + @" : " + category_net + @","
					+ quote + @"telephone" + quote + @" : " + quote + telephone + quote + @","
					+ quote + @"comment" + quote + @" : " + quote + comment + quote + @","
					+ quote + @"purchaserFIO" + quote + @" : " + quote + purchaserFIO + quote + @","
					+ quote + @"promos" + quote + @" : " + @"[" + string.Join(@",", promos) + @"]" + @","
					+ quote + @"pharmacistCount" + quote + @" : " + pharmacistCount + @","
					+ quote + @"date" + quote + @" : " + quote + date.ToString(@"O") + quote
					+
				@" } ";
		}
	}
}

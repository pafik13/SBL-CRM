using System;

namespace SBLCRM.Lib.Entities
{
	public class Pharmacy: IEntity
	{
		public Pharmacy ()
		{
			// empty
		}

		public int id { get; set; }
		public string fullName { get; set; }
		public string shortName { get; set; }
		public string officialName { get; set; }
		public string address { get; set; }
		public string subway { get; set; }
		public string phone { get; set; }
		public string email { get; set; }
		public string category_otc { get; set; }
		public string category_sbl { get; set; }
		public string code_sbl { get; set; }
		public int tradenet { get; set; }
		public DateTime prev { get; set; }
		public DateTime next { get; set; }
	}
}


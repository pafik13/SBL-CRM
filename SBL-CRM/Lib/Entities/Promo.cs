using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class Promo : IEntity
	{
		public Promo ()
		{
		}

		public int id { get; set; }
		public string name { get; set; }
		public string key { get; set; }
	}

}


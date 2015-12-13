using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class NetCategory: IEntity
	{
		public NetCategory ()
		{
		}

		public int id { get; set; }
		public string name { get; set; }
		public string key { get; set; }
	}
}


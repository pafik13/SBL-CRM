using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class PhotoSubType : IEntity
	{
		public PhotoSubType ()
		{
		}

		public int id { get; set; }
		public string name { get; set; }
		public int type { get; set; }
	}

}

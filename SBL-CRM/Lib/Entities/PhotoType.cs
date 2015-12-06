using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class PhotoType : IEntity
	{
		public PhotoType ()
		{
		}

		public int id { get; set; }
		public string name { get; set; }
	}

}

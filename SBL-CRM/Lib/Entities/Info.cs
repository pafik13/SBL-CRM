using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class Info : IEntity
	{
		public Info ()
		{
		}

		public int id { get; set; }
		public string name { get; set; }
		public string valueType { get; set; }
	}

}


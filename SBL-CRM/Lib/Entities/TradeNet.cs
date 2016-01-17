using System;

namespace SBLCRM.Lib.Entities
{
	[Serializable]
	public class TradeNet : IEntity
	{

		public TradeNet()
		{
			// empty
		}

		public int id { get; set; }
		public string fullName { get; set; }
		public string shortName { get; set; }
		public string description { get; set; }
	}
}

using System;

namespace SBLCRM.Lib.Entities
{
    [Serializable]
    public class Territory : IEntity
    {

        public Territory()
        {
            // empty
        }

        public int id { get; set; }
        public string name { get; set; }
        public string info { get; set; }
        public string baseCity { get; set; }
    }
}

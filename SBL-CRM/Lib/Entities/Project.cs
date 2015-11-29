using System;

namespace SBLCRM.Lib.Entities
{
    [Serializable]
    public class Project : IEntity
    {

        public Project()
        {
            // empty
        }

        public int id { get; set; }
        public string fullName { get; set; }
        public string description { get; set; }
        public int[] drugs { get; set; }
    }

}
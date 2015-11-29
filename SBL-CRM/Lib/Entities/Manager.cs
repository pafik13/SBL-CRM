using System;

namespace SBLCRM.Lib.Entities
{
    [Serializable]
    public class Manager : IEntity
    {

        public Manager()
        {
            // empty
        }

        public int id { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public int sex { get; set; }
        public string phone { get; set; }
        public string job_role { get; set; }
        public int head { get; set; }
        public int user { get; set; }

    }
}

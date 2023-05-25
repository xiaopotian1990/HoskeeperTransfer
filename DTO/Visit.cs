using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Visit
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public long? UserID { get; set; }

        public DateTime CreateTime { get; set; }

        public long CreateUserID { get; set; }

        public int VisitType { get; set; }

        public int DealType { get; set; }

        public long? ExploitUserID { get; set; }
        public long? ManagerUserID { get; set; }
        public long HospitalID { get; set; }
        public int IsConsume { get; set; }
        public long TodaySymptomID { get; set; }
    }
}

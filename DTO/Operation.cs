using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Operation
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public long CreateUserID { get; set; }
        public DateTime? CreateTime { get; set; }
        public long? DeptID { get; set; }
        public long ChargeID { get; set; }
        public int Num { get; set; }
        public long? DoctorID { get; set; }


        public DateTime? EvaluationTime { get; set; }
        public int? EvaluationLevel { get; set; }
        public string EvaluationContent { get; set; }

        public long OrderDetailID { get; set; }

        public string Remark { get; set; }
    }
}

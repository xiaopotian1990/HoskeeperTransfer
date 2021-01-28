using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Consult
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public long CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
        public long Tool { get; set; }
        public string Content { get; set; }

        public DateTime? EvaluationTime { get; set; }
        public int? EvaluationLevel { get; set; }
        public string EvaluationContent { get; set; }
    }

    class ConsultDetail
    {
        public long ConsultID { get; set; }
        public long SymptomID { get; set; }
    }
}

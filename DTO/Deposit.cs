using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Deposit
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public long? CreateUserID { get; set; }
        public decimal Amount { get; set; }
        public long? ExploitUserID { get; set; }
        public long? ManagerUserID { get; set; }
        public long ChargeID { get; set; }
    }
}

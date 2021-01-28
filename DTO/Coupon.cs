using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Coupon
    {
        public long ID { get; set; }
        public long CustomerID { get; set; }
        public long? CreateUserID { get; set; }
        public decimal Amount { get; set; }
        public string ExpirationDate { get; set; }

        public long Time { get; set; }

        public long CategoryID { get; set; }
    }
}

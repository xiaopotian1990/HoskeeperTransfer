using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer
{
    class OrderDetail
    {
        public long ID { get; set;    }
        public long OrderDetailID { get; set; }
        
        public long OperationID { get; set; }

        public int Num { get; set; }

        public int RestNum { get; set; }
        public long CustomerID { get; set; }
        public long ChargeID { get; set; }
        public DateTime CreateTime { get; set; }

        public long OrderID { get; set; }

        public decimal Price { get; set; }
        public decimal FinalPrice { get; set; }
        public long? SetID { get; set; }

        public decimal DepositAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal DebtAmount { get; set; }

        public long? ExploitUserID { get; set; }
        public long? ManagerUserID { get; set; }

        public long CreateUserID { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    public class DataTransferOrder
    {
        public long ChargeID { get; set; }
        public decimal Price { get; set; }
        public int Num { get; set; }
        public decimal FinalPrice { get; set; }
        public int RestNum { get; set; }
        public long? SetID { get; set; }
        public int? SetNum { get; set; }
        public decimal? SetPrice { get; set; }
        public decimal? SetFinalPrice { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public DateTime CreateTime { get; set; }
        public long CustomerID { get; set; }
        public long CreateUserID { get; set; }
        public string Remark { get; set; }
        public VisitType VisitType { get; set; }
        public long? ExploitUserID { get; set; }
        public long? ManagerUserID { get; set; }
        public DealType DealType { get; set; }

        public string Custom10 { get; set; }

        public string Phone { get; set; }


        public decimal CashAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal DebtAmount { get; set; }
    }
}

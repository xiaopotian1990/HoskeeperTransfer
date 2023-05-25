using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class CashierCharge
    {
        public long CashierID { get; set; }
        public long ReferID { get; set; }
        public decimal CashCardAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal DebtAmount { get; set; }
        public decimal Amount { get; set; }
        public long HospitalID { get; set; }
        public decimal CommissionAmount { get; set; }
        public DateTime CreateTime { get; set; }
        public int OrderType { get; set; }
        public long CustomerID { get; set; }
        public long ChargeID { get; set; }
        public int Num { get; set; }
        public decimal OriginAmount { get; set; }
        public int VisitType { get; set; }
        public long OrderID { get; set; }
        public int SourceType { get; set; }
        public long OrderUserID { get; set; }
        public long BuyOrderUserID { get; set; }
        public int BuyVisitType { get; set; }
        public int DealType { get; set; }
    }
}

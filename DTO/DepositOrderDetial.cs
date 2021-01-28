using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class DepositOrderDetial
    {
        public long DepositID { get; set; }
        public long? DepositChargeID { get; set; }
        /// <summary>
        /// 预收款项目ID
        /// </summary>
        public long ChargeID { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Total { get; set; }
        public long OrderID { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public long ID { get; set; }
        public long CreateUserID { get; set; }
        /// <summary>
        /// 顾客ID
        /// </summary>
        public long CustomerID { get; set; }
        /// <summary>
        /// 下单医院ID
        /// </summary>
        public long HospitalID { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// 总金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public int PaidStatus { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PaidTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        public VisitType VisitType { get; set; }
        public int SourceType { get; set; }
        public long ExploitUserID { get; set; }
        public long ManagerUserID { get; set; }

        public long CashierID { get; set; }
        public long DetailID { get; set; }

        public decimal DepositAmount { get; set; }
        public long BuyOrderID { get; set; }

    }
}

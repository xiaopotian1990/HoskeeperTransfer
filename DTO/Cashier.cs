using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Cashier
    {
        public long ID { get; set; }
        public DateTime CreateTime { get; set; }
        public long OrderID { get; set; }
        /// <summary>
        /// 顾客ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 订单类型
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 所属医院ID
        /// </summary>
        public long HospitalID { get; set; }
        /// <summary>
        /// 收银用户
        /// </summary>
        public long CreateUserID { get; set; }
        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 现金
        /// </summary>
        public decimal Cash { get; set; }
        /// <summary>
        /// 刷卡
        /// </summary>
        public decimal Card { get; set; }
        /// <summary>
        /// 预收款
        /// </summary>
        public decimal Deposit { get; set; }
        /// <summary>
        /// 代金券
        /// </summary>
        public decimal Coupon { get; set; }
        /// <summary>
        /// 欠款
        /// </summary>
        public decimal Debt { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 结算状态
        /// </summary>
        public int Status { get; set; }



        public long TagID { get; set; }

        public int Type { get; set; }
        public decimal ConsumeAmount { get; set; }
    }
}

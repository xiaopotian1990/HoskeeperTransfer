using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class DepositChargeInfo
    {
        /// <summary>
        /// 预收款id
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 预收款类型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 预收款状态 状态0：停用1：使用
        /// </summary>
        public CommonStatus Status { get; set; }
        /// <summary>
        /// 是否只允许商城售卖
        /// </summary>
        public CommonStatus IsShopOnly { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 使用范围限制1：无限制2：按照项目分类进行限制3：按照指定项目进行限制
        /// </summary>
        public int ScopeLimit { get; set; }
        /// <summary>
        /// 使用范围限制值
        /// </summary>
        public string ScopeLimitValue { get; set; }
        /// <summary>
        /// 是否赠送代金券0：否1：是
        /// </summary>
        public int HasCoupon { get; set; }
        /// <summary>
        /// 卷类型id
        /// </summary>
        public long? CouponCategoryID { get; set; }
        /// <summary>
        /// 卷类型名称
        /// </summary>
        public string CouponCategoryName { get; set; }

        /// <summary>
        /// 卷金额
        /// </summary>
        public decimal? CouponAmount { get; set; }

        public long? ChargeID { get; set; }
        public long? ChargeCategoryID { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HoskeeperTransfer
{
    /// <summary>
    /// 积分消费方式
    /// </summary>
    public enum PointType
    {
        /// <summary>
        /// 手工赠送
        /// </summary>
        [Description("手工赠送")]
        ManualGive = 11,
        /// <summary>
        /// 消费赠送
        /// </summary>
        [Description("消费赠送")]
        ConsumeGive = 12,
        /// <summary>
        /// 手工扣减
        /// </summary>
        [Description("手工扣减")]
        ManualRebate = 21,
        /// <summary>
        /// 兑换券
        /// </summary>
        [Description("兑换券")]
        CouponExchange = 22,
        /// <summary>
        /// 兑换产品
        /// </summary>
        [Description("兑换产品")]
        ChargeExchange = 23,
        /// <summary>
        /// 退项目扣减
        /// </summary>
        [Description("退项目扣减")]
        BackRebate = 24,
        /// <summary>
        /// 退款扣减
        /// </summary>
        [Description("退款扣减")]
        RebateRebate = 25,
        /// <summary>
        /// 数据迁移
        /// </summary>
        [Description("数据迁移")]
        DataImport = 26,
        /// <summary>
        /// 激励赠送
        /// </summary>
        [Description("激励赠送")]
        Reward = 27,
    }
}

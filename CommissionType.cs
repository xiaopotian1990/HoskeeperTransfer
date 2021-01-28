using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HoskeeperTransfer
{
    /// <summary>
    /// 佣金使用类型
    /// </summary>
    public enum CommissionType
    {
        /// <summary>
        /// 消费
        /// </summary>
        [Description("订单支付")]
        Consume = 1,
        /// <summary>
        /// 换领待审核
        /// </summary>
        [Description("换领待审核")]
        OutAuditing = 2,
        /// <summary>
        /// 转让
        /// </summary>
        [Description("转让")]
        Send = 3,
        /// <summary>
        /// 数据迁移
        /// </summary>
        [Description("数据迁移")]
        DataImport = 5,
        /// <summary>
        /// 退款补偿
        /// </summary>
        [Description("退款补偿")]
        ConsumeBack = 6,
        /// <summary>
        /// 佣金提成
        /// </summary>
        [Description("层级提成")]
        Commission = 7,
        /// <summary>
        /// 激励赠送
        /// </summary>
        [Description("激励赠送")]
        Reward = 8,
        /// <summary>
        /// 分享家升级赠送
        /// </summary>
        [Description("分享家升级赠送")]
        ShareCategory = 9,
        /// <summary>
        /// 人工赠送
        /// </summary>
        [Description("人工赠送")]
        SystemSend = 10,
        /// <summary>
        /// 待换领
        /// </summary>
        [Description("待换领")]
        Outing = 11,
        /// <summary>
        /// 已换领
        /// </summary>
        [Description("已换领")]
        Outed = 12,
        /// <summary>
        /// 过期扣减
        /// </summary>
        [Description("过期扣减")]
        ExpirationOver = 13,
    }
}

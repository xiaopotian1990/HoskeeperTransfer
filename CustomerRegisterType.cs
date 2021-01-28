using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HoskeeperTransfer
{
    /// <summary>
    /// 顾客登记方式 1:网电登记 2:前台接待 3:市场登记 4:微信注册 5:手动推荐 6:分享注册
    /// </summary>
    public enum CustomerRegisterType
    {
        /// <summary>
        /// 网电登记
        /// </summary>
        [Description("网电登记")]
        Exploit = 1,
        /// <summary>
        /// 前台接待
        /// </summary>
        [Description("前台接待")]
        ForeGround = 2,
        /// <summary>
        /// 市场登记
        /// </summary>
        [Description("市场登记")]
        Market = 3,
        /// <summary>
        /// 微信注册
        /// </summary>
        [Description("微信注册")]
        WechatRegedit = 4,
        /// <summary>
        /// 手动推荐
        /// </summary>
        [Description("手动推荐")]
        ManualPromote = 5,
        /// <summary>
        /// 分享注册
        /// </summary>
        [Description("分享注册")]
        Share = 6,
        /// <summary>
        /// 数据迁移
        /// </summary>
        [Description("数据迁移")]
        DataTransfer = 7
    }
}

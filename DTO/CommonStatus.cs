using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    /// <summary>
    /// 通用状态类 0：停用；1：使用
    /// </summary>
    public enum CommonStatus
    {
        /// <summary>
        /// 停用
        /// </summary>
        [Description("停用")]
        Stop = 0,
        /// <summary>
        /// 使用
        /// </summary>
        [Description("使用")]
        Use = 1
    }
}

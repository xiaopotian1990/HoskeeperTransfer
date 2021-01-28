using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    public enum GenderEnum
    {
        /// <summary>
        /// 男
        /// </summary>
        [Description("男")]
        Boy =1,
        /// <summary>
        /// 女
        /// </summary>
        [Description("女")]
        Girl =2,
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All =999,
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace HoskeeperTransfer
{
    /// <summary>
    /// 顾客归属关系类型
    /// </summary>
    public enum OwnerShipType
    {
        /// <summary>
        /// 网电咨询师
        /// </summary>
        [Description("网电咨询师")]
        Exploit = 1,
        /// <summary>
        /// 现场咨询师
        /// </summary>
        [Description("现场咨询师")]
        Manager = 2,
        /// <summary>
        /// 科室客服
        /// </summary>
        [Description("科室客服")]
        DeptManager = 3,
        /// <summary>
        /// 科室医生
        /// </summary>
        [Description("科室医生")]
        DeptDoctor = 4,
    }
}

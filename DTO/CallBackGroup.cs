using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    public class CallBackGroup
    {
        /// <summary>
        /// ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 状态 1：使用；0：停用；2：删除
        /// </summary>
        public CommonStatus Status { get; set; }

        /// <summary>
        /// 回访组详细DTO
        /// </summary>
        public List<SmartCallbackGroupDetail> CallbackSetDetailGet { get; set; }


        public string OldID { get; set; }
    }

    /// <summary>
    /// 回访组详细DTO
    /// </summary>
    public class SmartCallbackGroupDetail
    {
        public long ID { get; set; }
        /// <summary>
        /// 回访组ID
        /// </summary>
        public long SetID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 天数
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// 回访类型名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 类型名称id
        /// </summary>
        public long CategoryID { get; set; }

        public string OldSetID { get; set; }

    }
}

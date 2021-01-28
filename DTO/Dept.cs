using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Dept
    {
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
        /// 开放状态 1 是 0 否
        /// </summary>
        public CommonStatus OpenStatus { get; set; }

        /// <summary>
        /// 是否参与分针
        /// </summary>
        public CommonStatus IsTriage { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 所属医院ID
        /// </summary>
        public long HospitalID { get; set; }
    }
}

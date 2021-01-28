using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class ChargeCategory
    {
        /// <summary>
        /// 项目分类id
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 项目分类名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父分类id
        /// </summary>
        public long? ParentID { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}

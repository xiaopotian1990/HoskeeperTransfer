using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class ProductCategory
    {
        /// <summary>
        /// 药物品Id
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 药物品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 父分类id
        /// </summary>
        public long? PID { get; set; }

        /// <summary>
        /// 药物品Id
        /// </summary>
        public string OldID { get; set; }

        /// <summary>
        /// 父分类id
        /// </summary>
        public string OldPID { get; set; }
    }

    class ProductCategoryTemp
    {
        /// <summary>
        /// 药物品Id
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 药物品名称
        /// </summary>
        public string Name { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 父分类id
        /// </summary>
        public string PID { get; set; }
    }

}

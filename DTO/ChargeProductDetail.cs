using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    public class ChargeProductDetail
    {
        /// <summary>
        /// 药物品id
        /// </summary>
        public long ProductID { get; set; }

        public long ID { get; set; }
        public long ChargeID { get; set; }
        /// <summary>
        /// 最小数量
        /// </summary>
        public int MinNum { get; set; }
        /// <summary>
        /// 最大数量
        /// </summary>
        public int MaxNum { get; set; }
    }
}

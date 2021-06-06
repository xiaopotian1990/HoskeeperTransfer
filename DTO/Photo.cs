using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Photo
    {
        public long ID { get; set; }
        /// <summary>
        /// 顾客ID
        /// </summary>
        public long CustomerID { get; set; }
        /// <summary>
        /// 创建回访提醒的用户
        /// </summary>
        public long CreateUserID { get; set; }
        /// <summary>
        /// 回访提醒创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        public long? ChargeID { get; set; }
        public string Remark { get; set; }
        public string ImageUrl { get; set; }
        public long? SymptomID { get; set; }
        public int Type { get; set; }
        public string ReducedImage { get; set; }
    }
}

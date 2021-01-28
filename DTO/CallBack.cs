using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class CallBack
    {
        /// <summary>
        /// 回访记录ID
        /// </summary>
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
        /// <summary>
        /// 沟通方式
        /// </summary>
        public long Tool { get; set; }
        /// <summary>
        /// 回访内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 回访类型
        /// </summary>
        public long CategoryID { get; set; }
        /// <summary>
        /// 回访计划
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 回访提醒人
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 回访提醒日期，具体到天
        /// </summary>
        public DateTime? TaskTime { get; set; }
        /// <summary>
        /// 回访提醒完成时间
        /// </summary>
        public DateTime? TaskCreateTime { get; set; }
        /// <summary>
        /// 回访提醒完成人
        /// </summary>
        public long TaskCreateUserID { get; set; }
        /// <summary>
        /// 回访状态
        /// </summary>
        public int Status { get; set; }
    }
}

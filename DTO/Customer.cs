using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Customer
    {
        public long ID { get; set; }
        public int? Age { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 男女
        /// </summary>
        public int Gender { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 备用手机号
        /// </summary>
        public string MobileBackup { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        public long? ChannelID { get; set; }
        /// <summary>
        /// 开发人员ID
        /// </summary>
        public long? CurrentExploitUserID { get; set; }
        /// <summary>
        /// 开发人员ID
        /// </summary>
        public long? CurrentManagerUserID { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 医院ID
        /// </summary>
        public long CreateUserHospitalID { get; set; }

        public long MemberCategoryID { get; set; }
        /// <summary>
        /// 操作人ID
        /// </summary>
        public long CreateUserID { get; set; }
        public decimal Point { get; set; }
        public decimal Commission { get; set; }
        /// <summary>
        /// 顾客登记方式
        /// </summary>
        public int Source { get; set; }
        public DateTime? CreateTime { get; set; }

        public long? PromoterID { get; set; }

        public string Remark { get; set; }
        public string Custom9 { get; set; }
        public string Custom10 { get; set; }

        public string CityName { get; set; }
        public string ProName { get; set; }

        public long? CurrentConsultSymptomID { get; set; }


        public string WeChat { get; set; }
        public string QQ { get; set; }
    }
}

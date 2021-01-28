using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class ChargeSet
    {
        /// <summary>
        /// id
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 套餐名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 状态0：停用1：使用
        /// </summary>
        public CommonStatus Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 拼音码
        /// </summary>
        public string PinYin { get; set; }
        /// <summary>
        /// 是否有时间限制0：没有1：有
        /// </summary>
        public int TimeLimit { get; set; }
        /// <summary>
        /// 0：从购买时算1：从消费时算
        /// </summary>
        public int TimeStart { get; set; }
        /// <summary>
        /// 天数
        /// </summary>
        public int? Days { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public long CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 医院ID
        /// </summary>
        public long HospitalID { get; set; }

        public string OldID { get; set; }
    }


    /// <summary>
    /// 套餐收费项目映射表
    /// </summary>
    public class SmartChargeSetDetail
    {
        /// <summary>
        /// 套餐收费项目映射id
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 套餐id
        /// </summary>
        public long SetID { get; set; }
        public string OldSetID { get; set; }
        /// <summary>
        /// 收费项目id
        /// </summary>
        public long ChargeID { get; set; }
        /// <summary>
        /// 收费项目名称
        /// </summary>
        public string ChargeName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// 总价格
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }


        public string Remark { get; set; }
        /// <summary>
        /// 拼音码
        /// </summary>
        public string PinYin { get; set; }
        /// <summary>
        /// 状态0：停用1：使用
        /// </summary>
        public CommonStatus Status { get; set; }
    }
}

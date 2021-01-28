using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    /// <summary>
    /// 代金券查询dto
    /// </summary>
    public class CouponCategoryInfo
    {
        /// <summary>
        /// 卷类型主键
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 卷类型名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态 状态0：停用1：使用
        /// </summary>
        public CommonStatus Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 使用范围限制1：无限制2：按照项目分类进行限制3：按照指定项目进行限制
        /// </summary>
        public int ScopeLimit { get; set; }

        /// <summary>
        /// 使用时间限制1：无限制2：指定日期之前3：生效之后N天
        /// </summary>
        public int TimeLimit { get; set; }
        /// <summary>
        /// /当TimeLimit为2时，代表指定日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 当TimeLimit为3时，代表生效之后多少天
        /// </summary>
        public int? Days { get; set; }

        public long? ChargeID { get; set; }
        public long? ChargeCategoryID { get; set; }

    }
}

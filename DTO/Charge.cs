using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Charge
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long? ID { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 项目分类id
        /// </summary>
        public long? CategoryID { get; set; }
        /// <summary>
        /// 拼音吗
        /// </summary>
        public string PinYin { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        ///  状态 0：停用1：使用
        /// </summary>
        public CommonStatus Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public long? UnitID { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 是否允许耗材  0 允许， 1不允许
        /// </summary>
        public int ProductAdd { get; set; }
        public CommonStatus IsEvaluate { get; set; }

        public long? ProductID { get; set; }
        public ChargeType Type { get; set; }

        public long? ItemID { get; set; }
    }

    public enum ChargeType
    {
        /// <summary>
        /// 项目
        /// </summary>
        Charge = 1,
        /// <summary>
        /// 药物品
        /// </summary>
        Product = 2
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class Product
    {
        /// <summary>
        /// 药物品id
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 药物品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 拼音码
        /// </summary>
        public string PinYin { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public long? CategoryID { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public string CategoryID1 { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public string CategoryID2 { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public string CategoryID3 { get; set; }
        /// <summary>
        /// 分类id
        /// </summary>
        public string CategoryID4 { get; set; }
        /// <summary>
        /// 规格
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public CommonStatus Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 库存单位s
        /// </summary>
        public long? UnitID { get; set; }
        public string UnitName { get; set; }
        /// <summary>
        /// 使用单位
        /// </summary>
        public long? MiniUnitID { get; set; }
        public string MinUnitName { get; set; }

        /// <summary>
        /// 进制
        /// </summary>
        public int? Scale { get; set; }

        /// <summary>
        /// 是否可售卖
        /// </summary>
        public CommonStatus IsSale { get; set; }
        /// <summary>
        /// 售价
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 出库仓库
        /// </summary>
        public long? WarehouseID { get; set; }

        public string WarehouseName { get; set; }
        /// <summary>
        /// 是否允许评价
        /// </summary>
        public CommonStatus IsEvaluate { get; set; }
        /// <summary>
        /// 转化为项目后的分类
        /// </summary>
        public long? ChargeCategoryID { get; set; }

        public CommonStatus IsSendPoint { get; set; }


        public DateTime? yxsj { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    public class Tag
    {
        /// <summary>
        /// ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 银行卡名称
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 操作人ID
        /// </summary>
        public long CreateUserID { get; set; }

        /// <summary>
        /// 禁止批量设置回访提醒
        /// </summary>
        public CommonStatus NotCallBack { get; set; }
        /// <summary>
        /// 进制批量赠送优惠券、佣金、积分
        /// </summary>
        public CommonStatus NotSend { get; set; }
        /// <summary>
        /// 进制批量发送短信
        /// </summary>
        public CommonStatus NotSSM { get; set; }

        public long? TagGroupID { get; set; }
        public string TagGroupName { get; set; }
        public CommonStatus Status { get; set; }
    }
}

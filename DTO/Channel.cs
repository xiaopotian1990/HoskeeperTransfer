using HoskeeperTransfer.DTO;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer
{
    class Channel
    {
        /// <summary>
        /// ID
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 状态 0：停用；1：使用
        /// </summary>
        public CommonStatus Status { get; set; }
        /// <summary>
        /// 渠道组
        /// </summary>
        public string GroupName { get; set; }
    }

    public class DataTransferChannel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public CommonStatus Status { get; set; }
        public int SortNo { get; set; }
        public string Remark { get; set; }
        public long HospitalID { get; set; }
        public string PinYin { get; set; }
        public string LinkMan { get; set; }
        public string Contact { get; set; }
        public long? ChannelGroupID { get; set; }

        public long? GroupID { get; set; }

        public long? ItemID { get; set; }


        public long CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class DataTransferChannelGroup
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int SortNo { get; set; }
        public string Remark { get; set; }
    }

    public class DataTransferChannelGroupDetail
    {
        public long ID { get; set; }
        public long ChannelID { get; set; }
        public long GroupID { get; set; }
    }
}

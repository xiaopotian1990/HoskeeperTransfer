using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    public class Rebate
    {
        public long? ID { get; set; }
        public int Type { get; set; }
        public long? ChannelID { get; set; }
        public string ChannelName { get; set; }
        public long? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public long? ChargeID { get; set; }
        public long? ChargeSetID { get; set; }
        public string ChargeName { get; set; }
        public string ChargeSetName { get; set; }
        public decimal Level1 { get; set; }
        public decimal Level2 { get; set; }
        public decimal Level3 { get; set; }
        public decimal Level4 { get; set; }
        public decimal Level5 { get; set; }
        public decimal Discount { get; set; }
        public CommonStatus Status { get; set; }
        public long? PID { get; set; }
        public int? IsOld { get; set; }

        public Rebate PRebate { get; set; }

        public int TimeLimit { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public string Name { get; set; }
    }
}

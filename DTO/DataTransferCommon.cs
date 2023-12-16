using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    public class DataTransferCommon
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string PinYin { get; set; }

        public int SortNo { get; set; }
        public long GroupID { get; set; }

        public long CategoryID { get; set; }
    }
}

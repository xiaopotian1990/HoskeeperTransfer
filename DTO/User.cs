using System;
using System.Collections.Generic;
using System.Text;

namespace HoskeeperTransfer.DTO
{
    class User
    {
        public long ID { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public GenderEnum Gender { get; set; }
        public long DeptID { get; set; }
        public CommonStatus Status { get; set; }
        public string Remark { get; set; }
        public string Phone { get; set; }
        public long HospitalID { get; set; }
        public decimal Discount { get; set; }
        public DateTime CreateTime { get; set; }
        public long CreateUserID { get; set; }

        public string DeptName { get; set; }

        public string Mobile { get; set; }
        public int Sort { get; set; }
    }
}

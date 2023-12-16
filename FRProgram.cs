using Dapper;
using HoskeeperTransfer.DTO;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace HoskeeperTransfer
{
    class FRProgram
    {
        private static long _hospitalID = 1;
        private static long _channelID = 429;
        private static SqlConnection _connection;
        private static SqlConnection _sourceSqlConnection;
        private static SqlTransaction _transaction;
        private static long _tool = 1;//电话工具
        private static long _symptomID = 1;//无症状
        private static long _callBackCategoryOfSH = 3;
        private static long _callBackCategoryOfWD = 1;
        private static long _callBackCategoryOfXC = 2;

        private static long _couponCategoryID = 14692223833048064;
        private static long _depositCategoryID = 14692224210437120;
        private static int _callbackNum = 50000;
        static void FRMain(string[] args)
        {
            try
            {


                //南京环亚
                _connection = new SqlConnection("Data Source=47.94.250.49a;Initial Catalog=Hoskeeper;Persist Security Info=True;User ID=sa;Password=jqOrao$uxXUE5qZt; MultipleActiveResultSets = true;connect timeout=90000000");


                _sourceSqlConnection = new SqlConnection(@"Data Source=47.94.174.168a;Initial Catalog=FRSoft_BJ_JTMH;Persist Security Info=True;User ID=nzzn;Password=nzzn2022@;MultipleActiveResultSets = true;connect timeout=90000");
                _connection.Open();
                _transaction = _connection.BeginTransaction();

                Item();
                //Channel();   //excel数据迁移  字典管理-CRM-了解途径
                //Dept();
                //Supplier();
                //Factory();
                //Tool();  手动录入
                //CallBackCategory();
                //CallBackGroup();
                //Unit();
                //Symptom();
                //User();
                //ChargeCategory();
                //Charge(); 
                //ProductCategory();
                //ProductToChargeCategory();
                //Product();  excel 药品与耗材管理  药品+耗材两个excel
                //NumChargeSet();



                //Customer();
                //CallBackTask();
                //CallBack();
                //ConsultExploit();
                //ConsultManager();
                //Visit();
                //Deposit();
                //Order();
                //Operation();
                CaculateOrderRestNum();
                //MobileInfo();
                //CustomerTag2();
                //Point();

                //ChargeUpdate();
                //CustomerKF();
                //FactoryNew();
                //MobileUpdate();

                //DeptCouument();
                //CustomerID();
                _transaction.Commit();
            }
            catch (Exception e)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                }

                Console.WriteLine("数据迁移失败：" + e.ToString());
            }
            finally
            {
                _connection.Close();
                _sourceSqlConnection.Close();
            }
        }

        /// <summary>
        /// 电话
        /// </summary>
        /// <returns></returns>
        public static void CustomerID()
        {
            Console.WriteLine("CustomerID导入开始！");
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\潍坊壹美\\需要删除的客户WF.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable callbackList = new DataTable("SmartDeleteCustomerTemp");
                callbackList.Columns.Add("ID", typeof(long));

                for (int row = 1; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }

                    DataRow dr = callbackList.NewRow();
                    dr["ID"] = long.Parse(worksheet.Cells[row, 1].Value.ToString());

                    callbackList.Rows.Add(dr);
                }
                if (callbackList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartDeleteCustomerTemp", callbackList);
                }
            }

            Console.WriteLine("CustomerID导入结束！");
        }

        /// <summary>
        /// 项目更新
        /// </summary>
        public static void MobileUpdate()
        {
            Console.WriteLine("电话开始更新");

            var list = _connection.Query<Customer>(@"select ID,MobileBackup from SmartCustomer where ISNUMERIC(MobileBackup)=0 and MobileBackup is not null and MobileBackup <> ''", null, _transaction);
            var result = new List<Customer>();

            foreach (var u in list)
            {
                //if(Regex.Matches(u.MobileBackup, "[a-zA-Z]").Count()>0)
                //{
                //    result.Add(new DTO.Customer()
                //    {
                //        ID = u.ID,
                //        MobileBackup = Regex.Replace(u.MobileBackup, "[a-zA-Z]", "",RegexOptions.IgnoreCase)
                //    });
                //}
                if (u.MobileBackup.Contains(" "))
                {
                    var temp = u.MobileBackup.Split(' ');
                    u.MobileBackup = temp[0];
                    u.MobileBackup2 = temp[1];
                    if (temp.Count() == 3)
                    {
                        u.MobileBackup3 = temp[2];
                    }
                    if (temp.Count() == 4)
                    {
                        u.MobileBackup4 = temp[3];
                    }
                    result.Add(u);
                }
            }

            if (result.Count() > 0)
            {
                _connection.Execute(@"update SmartCustomer set MobileBackup=@MobileBackup,MobileBackup2=@MobileBackup2,MobileBackup3=@MobileBackup3,MobileBackup4=@MobileBackup4 where ID=@ID", result, _transaction);
            }

            Console.WriteLine("电话结束更新");
        }


        /// <summary>
        /// 项目更新
        /// </summary>
        public static void ChargeUpdate()
        {
            Console.WriteLine("项目开始更新");

            List<DataTransferCommon> list = new List<DataTransferCommon>();
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\潍坊壹美\\项目更新模板.xlsx")))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;



                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }


                    list.Add(new DataTransferCommon()
                    {
                        ID = long.Parse(worksheet.Cells[row, 1].Value.ToString()),
                        Name = worksheet.Cells[row, 2].Value.ToString(),
                        Price = decimal.Parse(worksheet.Cells[row, 3].Value.ToString()),
                        PinYin = worksheet.Cells[row, 2].Value.ToString().PinYin()
                    });

                }

                _connection.Execute(@"update SmartCharge set Name=@Name,Price=@Price,PinYin=@PinYin where ID=@ID", list, _transaction);

            }

            Console.WriteLine("项目结束更新");
        }

        /// <summary>
        /// 电话
        /// </summary>
        /// <returns></returns>
        public static void MobileInfo()
        {
            Console.WriteLine("电话导入开始！");
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\安装\\Mobile.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable callbackList = new DataTable("SmartMobileInfo");
                callbackList.Columns.Add("Pre", typeof(string));
                callbackList.Columns.Add("Mobile", typeof(string));
                callbackList.Columns.Add("Province", typeof(string));
                callbackList.Columns.Add("City", typeof(string));
                callbackList.Columns.Add("Operators", typeof(string));
                callbackList.Columns.Add("AreaCode", typeof(string));
                callbackList.Columns.Add("PostCode", typeof(string));
                callbackList.Columns.Add("ZoneCode", typeof(string));

                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }

                    DataRow dr = callbackList.NewRow();
                    dr["Pre"] = worksheet.Cells[row, 1].Value.ToString();
                    dr["Mobile"] = worksheet.Cells[row, 2].Value.ToString();
                    dr["Province"] = worksheet.Cells[row, 3].Value.ToString();
                    dr["City"] = worksheet.Cells[row, 4].Value.ToString();
                    dr["Operators"] = worksheet.Cells[row, 5].Value.ToString();
                    dr["AreaCode"] = worksheet.Cells[row, 6].Value.ToString();
                    dr["PostCode"] = worksheet.Cells[row, 7].Value.ToString();
                    dr["ZoneCode"] = worksheet.Cells[row, 8].Value.ToString();

                    callbackList.Rows.Add(dr);
                }
                if (callbackList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartMobileInfo", callbackList);
                }
            }

            Console.WriteLine("电话导入结束！");
        }

        /// <summary>
        /// 科室档案
        /// </summary>
        /// <returns></returns>
        public static void DeptCouument()
        {
            Console.WriteLine("科室档案导入开始！");
            using (var package = new ExcelPackage(new System.IO.FileInfo(@"D:\哪吒智能\各医院\昆明丽都\无创注射病例.xlsx")))
            {
                //许可证，必须添加许可证，否则会报错
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable callbackList = new DataTable("SmartDocumentTest");
                callbackList.Columns.Add("Mobile", typeof(string));
                callbackList.Columns.Add("Birthday", typeof(DateTime));
                callbackList.Columns.Add("Age", typeof(GenderEnum));
                callbackList.Columns.Add("No", typeof(string));

                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }

                    DataRow dr = callbackList.NewRow();
                    dr["Mobile"] = worksheet.Cells[row, 1].Value.ToString();
                    dr["Birthday"] = DateTime.Parse(worksheet.Cells[row, 2].Value.ToString());
                    dr["Age"] = worksheet.Cells[row, 3].Value.ToString() == "女" ? GenderEnum.Girl : GenderEnum.Boy;
                    dr["No"] = worksheet.Cells[row, 4].Value.ToString();

                    callbackList.Rows.Add(dr);
                }
                if (callbackList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartDocumentTest", callbackList);
                }
            }

            Console.WriteLine("科室档案导入结束！");
        }

        /// <summary>
        /// 渠道
        /// </summary>
        public static void Channel()
        {
            Console.WriteLine("渠道导入开始！");
            Dictionary<string, List<DataTransferChannel>> dic = new Dictionary<string, List<DataTransferChannel>>();
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\淄博壹美\\渠道资料表.xlsx")))
            {

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                string channelName = "";
                string groupName = "";
                CommonStatus status = CommonStatus.Use;
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    channelName = "";
                    groupName = "";
                    status = CommonStatus.Use;
                    if (worksheet.Cells[row, 2].Value == null)
                    {
                        throw new Exception("第" + row + "行渠道不能为空！");
                    }
                    channelName = worksheet.Cells[row, 2].Value.ToString().Trim();
                    if (channelName.IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行渠道不能为空！");
                    }
                    if (worksheet.Cells[row, 4].Value != null)
                    {
                        groupName = worksheet.Cells[row, 4].Value.ToString().Trim();
                    }
                    if (worksheet.Cells[row, 14].Value == null)
                    {
                        throw new Exception("第" + row + "行状态不能为空！");
                    }
                    if (worksheet.Cells[row, 14].Value.ToString() != "使用")
                    {
                        status = CommonStatus.Stop;
                    }


                    var tempList = new List<DataTransferChannel>();
                    if (dic.ContainsKey(groupName))
                    {
                        tempList = dic[groupName];
                    }
                    else
                    {
                        dic.Add(groupName, tempList);
                    }
                    tempList.Add(new DataTransferChannel()
                    {
                        Name = channelName,
                        Remark = "批量导入",
                        SortNo = row,
                        Status = status
                    });
                }


                List<DataTransferChannel> channelAddList = new List<DataTransferChannel>();
                List<DataTransferChannelGroup> groupAddList = new List<DataTransferChannelGroup>();
                //List<DataTransferChannelGroupDetail> detailAddlist = new List<DataTransferChannelGroupDetail>();
                //数据加工
                foreach (var key in dic.Keys)
                {
                    var groupID = SingleIdWork.Instance(1).nextId();
                    if (!key.IsNullOrEmpty())
                    {
                        groupAddList.Add(new DataTransferChannelGroup()
                        {
                            ID = groupID,
                            Name = key,
                            Remark = "批量导入",
                            SortNo = 1
                        });
                    }

                    var temp = dic[key];
                    foreach (var u in temp)
                    {
                        var channelID = SingleIdWork.Instance(1).nextId();
                        long? channelGroupID;
                        if (key.IsNullOrEmpty())
                        {
                            channelGroupID = null;
                        }
                        else
                        {
                            channelGroupID = groupID;
                        }
                        channelAddList.Add(new DataTransferChannel()
                        {
                            ID = channelID,
                            SortNo = u.SortNo,
                            Remark = u.Remark,
                            Name = u.Name,
                            Status = u.Status,
                            ChannelGroupID = channelGroupID
                        });

                        //if (!key.IsNullOrEmpty())
                        //{
                        //    detailAddlist.Add(new DataTransferChannelGroupDetail()
                        //    {
                        //        ChannelID = channelID,
                        //        GroupID = groupID,
                        //        ID = SingleIdWork.Instance(1).nextId()
                        //    });
                        //}
                    }
                }

                var tempResult = channelAddList.GroupBy(u => u.Name).Where(u => u.Count() > 1).FirstOrDefault();
                if (tempResult != null)
                {
                    throw new Exception("渠道" + tempResult.Key + "重复");
                }

                ///导入数据库
                if (channelAddList.Count > 0)
                {
                    _connection.Execute("insert into [SmartChannel](ID,Name,[Status],SortNo,Remark,ChannelGroupID) values (@ID,@Name,@Status,@SortNo,@Remark,@ChannelGroupID)",
                       channelAddList, _transaction);
                }
                if (groupAddList.Count > 0)
                {
                    _connection.Execute("insert into SmartChannelGroup(ID,Name,SortNo,Remark) values(@ID, @Name,@SortNo, @Remark)",
                 groupAddList, _transaction);
                }
            }



            Console.WriteLine("渠道导入结束！");
        }



        public static void Item()
        {
            Console.WriteLine("渠道导入开始！");
            List<DataTransferChannel> groupList = new List<DataTransferChannel>();
            List<DataTransferChannel> categoryList = new List<DataTransferChannel>();
            List<DataTransferChannel> itemList = new List<DataTransferChannel>();
            List<DataTransferChannel> chargeList = new List<DataTransferChannel>();


            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\项目管理.xlsx")))
            {
                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                for (int row = 2; row <= rowCount; row++)
                {
                    var item = itemList.Where(x => x.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();
                    if (item == null)
                    {
                        item = new DataTransferChannel()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 5].Value.ToString().Trim(),
                            SortNo = 0,
                            Remark="批量导入"
                        };

                        var category = categoryList.Where(x => x.Name == worksheet.Cells[row, 4].Value.ToString().Trim()).FirstOrDefault();
                        if (category == null)
                        {
                            category = new DataTransferChannel()
                            {
                                ID = SingleIdWork.Instance(1).nextId(),
                                Name = worksheet.Cells[row, 4].Value.ToString().Trim(),
                                SortNo = 0,
                                Remark = "批量导入"
                            };

                            var group = groupList.Where(x => x.Name == worksheet.Cells[row, 3].Value.ToString().Trim()).FirstOrDefault();
                            if (group == null)
                            {
                                group = new DataTransferChannel()
                                {
                                    ID = SingleIdWork.Instance(1).nextId(),
                                    Name = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                    SortNo = 0,
                                    Remark = "批量导入"
                                };
                                groupList.Add(group);
                            }
                            category.GroupID = group.ID;
                            categoryList.Add(category);
                        }

                        item.GroupID = category.ID;
                        itemList.Add(item);

                    }

                    chargeList.Add(new DataTransferChannel()
                    {
                        ID = long.Parse(worksheet.Cells[row, 1].Value.ToString().Trim()),
                        ItemID = item.ID
                    });

                }


                _connection.Execute(@"insert into SmartItemGroup(ID,Name,SortNo,Remark) values(@ID,@Name,@SortNo,@Remark)", groupList, _transaction);
                _connection.Execute(@"insert into SmartItemChargeCategory(ID,Name,SortNo,Remark,GroupID) values(@ID,@Name,@SortNo,@Remark,@GroupID)", categoryList, _transaction);
                _connection.Execute(@"insert into SmartItem(ID,Name,SortNo,Remark,GroupID) values(@ID,@Name,@SortNo,@Remark,@GroupID)", itemList, _transaction);
                _connection.Execute(@"update SmartCharge set ItemID=@ItemID where ID=@ID", chargeList, _transaction);

            }



            Console.WriteLine("渠道导入结束！");
        }


        /// <summary>
        /// 渠道
        /// </summary>
        public static void Channel2()
        {
            Console.WriteLine("渠道导入开始！");


            var list = _sourceSqlConnection.Query<Channel>(@"SELECT [chn_name] AS Name,Case [chn_type] when 'A' then '美容院' when 'B' then '咨询中心' when 'C' then '网站' when 'D' then '其他' when 'Y' then '网络咨询' end as GroupName,1 as Status

  FROM [zsb_ctmchannels]");

            Dictionary<string, List<DataTransferChannel>> dic = new Dictionary<string, List<DataTransferChannel>>();

            foreach (var u in list)
            {
                var tempList = new List<DataTransferChannel>();
                if (dic.ContainsKey(u.GroupName))
                {
                    tempList = dic[u.GroupName];
                }
                else
                {
                    dic.Add(u.GroupName, tempList);
                }
                tempList.Add(new DataTransferChannel()
                {
                    Name = u.Name,
                    Remark = "批量导入",
                    SortNo = 0,
                    Status = CommonStatus.Use
                });
            }



            List<DataTransferChannel> channelAddList = new List<DataTransferChannel>();
            List<DataTransferChannelGroup> groupAddList = new List<DataTransferChannelGroup>();
            List<DataTransferChannelGroupDetail> detailAddlist = new List<DataTransferChannelGroupDetail>();
            //数据加工
            foreach (var key in dic.Keys)
            {
                var groupID = SingleIdWork.Instance(1).nextId();
                if (!key.IsNullOrEmpty())
                {
                    groupAddList.Add(new DataTransferChannelGroup()
                    {
                        ID = groupID,
                        Name = key,
                        Remark = "批量导入",
                        SortNo = 1
                    });
                }

                var temp = dic[key];
                foreach (var u in temp)
                {
                    var channelID = SingleIdWork.Instance(1).nextId();
                    channelAddList.Add(new DataTransferChannel()
                    {
                        ID = channelID,
                        SortNo = u.SortNo,
                        Remark = u.Remark,
                        Name = u.Name,
                        Status = u.Status,
                        ChannelGroupID = groupID
                    });


                }
            }

            var tempResult = channelAddList.GroupBy(u => u.Name).Where(u => u.Count() > 1).FirstOrDefault();
            if (tempResult != null)
            {
                throw new Exception("渠道" + tempResult.Key + "重复");
            }

            ///导入数据库
            if (channelAddList.Count > 0)
            {
                _connection.Execute("insert into [SmartChannel](ID,Name,[Status],SortNo,Remark,ChannelGroupID) values (@ID,@Name,@Status,@SortNo,@Remark,@ChannelGroupID)",
                   channelAddList, _transaction);
            }
            if (groupAddList.Count > 0)
            {
                _connection.Execute("insert into SmartChannelGroup(ID,Name,SortNo,Remark) values(@ID, @Name,@SortNo, @Remark)",
             groupAddList, _transaction);
            }



            Console.WriteLine("渠道导入结束！");
        }



        /// <summary>
        /// 部门
        /// </summary>
        public static void Dept()
        {
            Console.WriteLine("部门开始导入");
            var list = _sourceSqlConnection.Query<DataTransferChannel>(@"select mc as Name,sfky as Status,xh as SortNo from KS");

            int i = 10000;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";
                u.HospitalID = _hospitalID;
                i++;
            }

            _connection.Execute(@"insert into SmartDept(ID,Name,Remark,OpenStatus,SortNo,HospitalID,IsTriage) 
values (@ID,@Name,@Remark,@Status,@SortNo,@HospitalID,0)", list, _transaction);

            Console.WriteLine("部门结束导入");
        }


        /// <summary>
        /// 供应商
        /// </summary>
        public static void Supplier()
        {
            Console.WriteLine("供应商开始导入");
            var list = _sourceSqlConnection.Query<DataTransferChannel>(@"select mc as Name,pym as PinYin,lxr as LinkMan,
lxdh as Contact,
sfky as Status from GYS");

            int i = 10000;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";
                u.HospitalID = _hospitalID;
                if (u.LinkMan == null)
                {
                    u.LinkMan = "";
                }
                if (u.Contact == null)
                {
                    u.Contact = "";
                }
                i++;
            }

            _connection.Execute(@"insert into SmartSupplier(ID,Name,LinkMan,Contact,Remark,PinYin,HospitalID) 
values (@ID,@Name,@LinkMan,@Contact,@Remark,@PinYin,@HospitalID)", list, _transaction);

            Console.WriteLine("供应商结束导入");
        }


        /// <summary>
        /// 工具
        /// </summary>
        public static void Tool()
        {
            Console.WriteLine("工具开始导入");
            var list = _sourceSqlConnection.Query<DataTransferChannel>(@"SELECT [ctf_tools] as Name,[ctf_remark] as Remark FROM [zsb_ctftools]");

            int i = 10000;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";
                u.HospitalID = _hospitalID;
                u.Status = CommonStatus.Use;
                i++;
            }

            _connection.Execute(@"insert into [SmartTool] ([ID],[Name],[Remark],[Status]) 
values (@ID,@Name,@Remark,@Status)", list, _transaction);

            Console.WriteLine("工具结束导入");
        }


        /// <summary>
        /// 未成交
        /// </summary>
        public static void Fail()
        {
            Console.WriteLine("未成交开始导入");
            var list = _sourceSqlConnection.Query<DataTransferChannel>(@"SELECT [fat_info] as Name ,[fat_remark] as Remark,
Case when fat_status='STP' then 0 else 1 end as Status FROM [zsb_faltype]");

            int i = 10000;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";
                u.HospitalID = _hospitalID;
                i++;
            }

            _connection.Execute(@"insert into [SmartFailtureCategory](ID,Name,[Status],Remark) values (@ID,@Name,@Status,@Remark)", list, _transaction);

            Console.WriteLine("未成交结束导入");
        }

        /// <summary>
        /// 回访类型
        /// </summary>
        public static void CallBackCategory()
        {
            Console.WriteLine("回访类型开始导入");
            var list = _sourceSqlConnection.Query<DataTransferChannel>(@"select mc as Name,ms as Remark from YHZDB where ZDFLB_bh='ZDHFLX'");

            int i = 10000;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";
                u.HospitalID = _hospitalID;
                u.Status = CommonStatus.Use;
                i++;
            }

            _connection.Execute(@"insert into [SmartCallbackCategory](ID,Name,[Status],Remark) 
values (@ID,@Name,@Status,@Remark)", list, _transaction);

            Console.WriteLine("回访类型结束导入");
        }


        /// <summary>
        /// 回访组
        /// </summary>
        public static void CallBackGroup()
        {
            Console.WriteLine("回访组开始导入");
            var list = _sourceSqlConnection.Query<CallBackGroup>(@"sELECT [lsh] as OldID
      ,[mc] as Name
      ,1 as Status
      ,[mc] as Remark
  FROM [KHHFJHDY]");

            var detailList = _sourceSqlConnection.Query<SmartCallbackGroupDetail>(@"SELECT [KHHFJHDY_lsh] as OldSetID
      ,[hfjg] as Days
      ,case when b.mc is null then '其他' else b.mc end as CategoryName
      ,'['+a.mc+']'+a.hfzt as Name
  FROM [KHHFJHDYMX] a
	left join YHZDB b on a.ZDHFLX_bh=b.bh and b.ZDFLB_bh='ZDHFLX' where KHHFJHDY_lsh is not null and KHHFJHDY_lsh in (sELECT [lsh]
  FROM [KHHFJHDY])");

            var newList = _connection.Query<DataTransferChannel>(@"select ID,Name from SmartCallbackCategory", null, _transaction);

            int i = 10000;
            foreach (var u in list)
            {
                u.ID = i;
                i++;
            }
            foreach (var u in detailList)
            {
                u.ID = i;
                u.SetID = list.Where(x => x.OldID == u.OldSetID).FirstOrDefault().ID;
                u.CategoryID = newList.Where(x => x.Name == u.CategoryName).FirstOrDefault().ID;
                if (u.Name == null)
                {
                    u.Name = "";
                }
                i++;
            }

            _connection.Execute("insert into [SmartCallbackSet](ID,Name,[Status],Remark) values (@ID,@Name,@Status,@Remark)",
                 list, _transaction);

            _connection.Execute("insert into [SmartCallbackSetDetail](ID,[SetID],[CategoryID],[Name],[Days]) values (@ID,@SetID,@CategoryID,@Name,@Days)",
                       detailList, _transaction);
            _connection.Execute(@"delete
from SmartCallbackSet where ID not in (select SetID from SmartCallbackSetDetail)", null, _transaction);

            Console.WriteLine("回访组结束导入");
        }


        /// <summary>
        /// 咨询症状
        /// </summary>
        public static void Symptom()
        {
            Console.WriteLine("咨询症状开始导入");
            var list = _sourceSqlConnection.Query<DataTransferChannel>(@"select [mc] as Name,sfky as Status,
[ms] as Remark from YHZDB where ZDFLB_bh='ZDZXXM'");

            int i = 10000;
            int sort = 0;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";
                u.HospitalID = _hospitalID;
                u.SortNo = sort;
                i++;
                sort++;
            }
            _connection.Execute(@"delete from [SmartSymptom]", list, _transaction);
            _connection.Execute(@"insert into [SmartSymptom](ID,Name,[Status],SortNo,Remark) 
values (@ID,@Name,@Status,@SortNo,@Remark)", list, _transaction);

            Console.WriteLine("咨询症状结束导入");
        }


        /// <summary>
        /// 单位
        /// </summary>
        public static void Unit()
        {
            Console.WriteLine("单位开始导入");
            var list = _sourceSqlConnection.Query<DataTransferChannel>(@"select mc as Name,
sfky as status from YHZDB where ZDFLB_bh='ZDYPDW'");

            int i = 10000;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";
                i++;
            }
            _connection.Execute("delete from SmartUnit",
                    list, _transaction);

            _connection.Execute("insert into SmartUnit(ID,Name) values (@ID,@Name)",
                    list, _transaction);
            Console.WriteLine("单位结束导入");
        }

        /// <summary>
        /// 供应商
        /// </summary>
        public static void Factory()
        {
            Console.WriteLine("生产厂商开始导入");
            var list = _sourceSqlConnection.Query<DataTransferChannel>(@"select mc as Name,sfky as Status,sm as Remark  from SCS");

            DateTime now = DateTime.Now;
            int i = 10000;
            foreach (var u in list)
            {
                u.CreateTime = now;
                u.CreateUserID = 1;
                u.ID = i;
                i++;
            }

            _connection.Execute(@"insert into SmartFactory(ID,Name,Remark,Status,CreateTime,CreateUserID) 
values (@ID,@Name,@Remark,@Status,@CreateTime,@CreateUserID)", list, _transaction);

            Console.WriteLine("生产厂商结束导入");
        }


        /// <summary>
        /// 产品
        /// </summary>
        public static void Product()
        {
            Console.WriteLine("产品开始导入");
            var list = _sourceSqlConnection.Query<Product>(@"select zpt_name as Name,zpt_oldid as PinYin,
zpt_pducoms_amt as Price,b.unit_name as UnitName,c.unit_name as MinUnitName,zpt_addunit as Scale,d.pdt_name as CategoryID1,
e.pdt_name as CategoryID2,f.pdt_name as CategoryID3,g.pdt_name as CategoryID4,zpt_order as Size,1 as IsSale,0 as IsEvaluate,zpt_price as SalePrice,
                        case zpt_status when 'STP' then '0' else '1' end as Status
						,case  zpt_recprj when 'hc' then '耗材库' 
						when 'ALL' then '药房' when 'kf' then '库房'  
						else '药房' end as WarehouseName
from zsb_product a
left join aps_units b on b.unit_code=a.zpt_unit
left join aps_units c on c.unit_code=a.zpt_sunit
left join zsb_pdutype1 d on a.zpt_ptype1=d.pdt_code
left join zsb_pdutype2 e on a.zpt_ptype2=e.pdt_code
left join zsb_pdutype3 f on a.zpt_ptype3=f.pdt_code
left join zsb_pdutype4 g on a.zpt_ptype4=g.pdt_code
 where zpt_type='SAL'

");
            var warehouseList = _connection.Query<DataTransferChannel>(@"select * from SmartWarehouse", null, _transaction);
            var productCategoryList = _connection.Query<DataTransferChannel>(@"select * from SmartProductCategory", null, _transaction);
            var chargeCategoryategoryList = _connection.Query<DataTransferChannel>(@"select * from SmartChargeCategory", null, _transaction);
            var unitList = _connection.Query<DataTransferChannel>(@"select * from SmartUnit", null, _transaction);

            var chargeResult = new List<Charge>();
            int i = 20000;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";
                u.IsSendPoint = CommonStatus.Stop;
                if (!u.CategoryID4.IsNullOrEmpty())
                {
                    u.CategoryID = productCategoryList.Where(x => x.Name == u.CategoryID4).FirstOrDefault().ID;
                }
                else if (!u.CategoryID3.IsNullOrEmpty())
                {
                    u.CategoryID = productCategoryList.Where(x => x.Name == u.CategoryID3).FirstOrDefault().ID;
                }
                else if (!u.CategoryID2.IsNullOrEmpty())
                {
                    u.CategoryID = productCategoryList.Where(x => x.Name == u.CategoryID2).FirstOrDefault().ID;
                }
                else if (!u.CategoryID1.IsNullOrEmpty())
                {
                    u.CategoryID = productCategoryList.Where(x => x.Name == u.CategoryID1).FirstOrDefault().ID;
                }
                u.UnitID = unitList.Where(x => x.Name == u.UnitName).FirstOrDefault().ID;
                u.MiniUnitID = unitList.Where(x => x.Name == u.MinUnitName).FirstOrDefault().ID;

                u.WarehouseID = warehouseList.Where(x => x.Name == u.WarehouseName).FirstOrDefault().ID;
                if (u.PinYin == null)
                {
                    u.PinYin = "";
                }
                if (u.IsSale == CommonStatus.Use)
                {
                    long? chargeCategoryID = null;
                    if (!u.CategoryID4.IsNullOrEmpty())
                    {
                        chargeCategoryID = chargeCategoryategoryList.Where(x => x.Name == u.CategoryID4).FirstOrDefault().ID;
                    }
                    else if (!u.CategoryID3.IsNullOrEmpty())
                    {
                        chargeCategoryID = chargeCategoryategoryList.Where(x => x.Name == u.CategoryID3).FirstOrDefault().ID;
                    }
                    else if (!u.CategoryID2.IsNullOrEmpty())
                    {
                        chargeCategoryID = chargeCategoryategoryList.Where(x => x.Name == u.CategoryID2).FirstOrDefault().ID;
                    }
                    else if (!u.CategoryID1.IsNullOrEmpty())
                    {
                        chargeCategoryID = chargeCategoryategoryList.Where(x => x.Name == u.CategoryID1).FirstOrDefault().ID;
                    }
                    u.ChargeCategoryID = chargeCategoryID;

                    chargeResult.Add(new Charge()
                    {
                        ID = i,
                        CategoryID = chargeCategoryID,
                        IsEvaluate = u.IsEvaluate,
                        Name = u.Name,
                        PinYin = u.PinYin,
                        Price = u.SalePrice,
                        ProductAdd = 0,
                        ProductID = i,
                        Remark = "数据迁移",
                        Size = u.Size,
                        Status = u.Status,
                        Type = ChargeType.Product,
                        UnitID = u.UnitID,
                        IsSendPoint = CommonStatus.Stop
                    });
                }

                i++;
            }


            _connection.Execute(@"insert into SmartProduct(ID,Name,PinYin,CategoryID,Size,Price,[Status],Remark,UnitID,MiniUnitID,Scale,IsSale,SalePrice,WarehouseID,IsEvaluate,ChargeCategoryID,IsSendPoint)
 values(@ID, @Name, @PinYin, @CategoryID, @Size, @Price, @Status, @Remark, @UnitID, @MiniUnitID, @Scale,@IsSale,@SalePrice,@WarehouseID,@IsEvaluate,@ChargeCategoryID,@IsSendPoint)",
                   list, _transaction);
            if (chargeResult.Count() > 0)
            {
                _connection.Execute(@"insert into SmartCharge(ID,Name,CategoryID,PinYin,Price,Status,Remark,UnitID,Size,
ProductAdd,IsEvaluate,ProductID,Type,IsSendPoint) 
values(@ID, @Name, @CategoryID, @PinYin, @Price, @Status, @Remark, @UnitID,@Size,@ProductAdd,@IsEvaluate,@ProductID,@Type,@IsSendPoint)", chargeResult, _transaction);
            }

            Console.WriteLine("产品结束导入");
        }

        /// <summary>
        /// 产品分类
        /// </summary>
        public static void ProductCategory()
        {
            Console.WriteLine("物品分类开始导入");
            var list = _sourceSqlConnection.Query<ProductCategory>(@"select id as ID,mc as Name,null as PID,
1 as Status,'数据迁移' as Remark,xh as SortNo from  YHZDB where ZDFLB_bh='ZDSRXM'   and bh=mc2 and ys in (1,2)

union all
select a.id as ID,a.mc as Name,b.id as PID,
1 as Status,'数据迁移' as Remark,a.xh as SortNo 
from  YHZDB a
inner join YHZDB b on a.mc2=b.bh
where a.ZDFLB_bh='ZDSRXM' and a.sfky=1 and a.bh<>a.mc2 and a.ys in (1,2)");




            _connection.Execute("insert into SmartProductCategory(ID,Name,PID,SortNo,Remark) values (@ID,@Name,@PID,@SortNo,@Remark)",
                    list, _transaction);
            Console.WriteLine("物品分类结束导入");
        }

        /// <summary>
        /// 产品同步项目分类
        /// </summary>
        public static void ProductToChargeCategory()
        {
            Console.WriteLine("物品分类开始导入");
            var list = _sourceSqlConnection.Query<ProductCategory>(@"select pdt_code as OldID,pdt_name as Name,'0' as OldPID,
case when pdt_status='STP' then 0 else 1 end as Status,pdt_remark as Remark from zsb_pdutype1 where pdt_zpttype='SAL'
                union all
                select pdt_code as old_id,pdt_name as product_category_name,pdt_type as parent_id,
case when pdt_status='STP' then 0 else 1 end as status,pdt_remark as Remark from zsb_pdutype2 where pdt_zpttype='SAL'
                union all
                select pdt_code as old_id,pdt_name as product_category_name,pdt_type2 as parent_id,
case when pdt_status='STP' then 0 else 1 end as status,pdt_remark as Remark from zsb_pdutype3 where pdt_zpttype='SAL'
                union all
                select pdt_code as old_id,pdt_name as product_category_name,pdt_type3 as parent_id,
case when pdt_status='STP' then 0 else 1 end as status,pdt_remark as Remark from zsb_pdutype4 where pdt_zpttype='SAL'");


            int i = 20000;
            foreach (var u in list)
            {
                u.ID = i;
                if (u.Remark.IsNullOrEmpty())
                {
                    u.Remark = "数据迁移";
                }
                u.SortNo = 0;
                i++;
            }

            foreach (var u in list)
            {
                if (u.OldPID != "0")
                {
                    var temp = list.Where(x => x.OldID == u.OldPID).FirstOrDefault();
                    if (temp != null)
                    {
                        u.PID = temp.ID;
                    }
                }
                else
                {
                    //**************************************************************************************************************************************
                    u.PID = 15239785906324480;
                }
            }

            _connection.Execute("insert into SmartChargeCategory(ID,Name,ParentID,SortNo,Remark) values (@ID,@Name,@PID,@SortNo,@Remark)",
                     list, _transaction);
            Console.WriteLine("物品分类结束导入");
        }

        /// <summary>
        /// 项目分类
        /// </summary>
        public static void ChargeCategory()
        {
            Console.WriteLine("项目分类开始导入");
            var list = _sourceSqlConnection.Query<ProductCategory>(@"select id as ID,mc as Name,null as PID,
1 as Status,'数据迁移' as Remark,xh as SortNo from  YHZDB where ZDFLB_bh='ZDSRXM' and sfky=1 and bh=mc2 and ys=3

union all
select a.id as ID,a.mc as Name,b.id as PID,
1 as Status,'数据迁移' as Remark,a.xh as SortNo 
from  YHZDB a
inner join YHZDB b on a.mc2=b.bh
where a.ZDFLB_bh='ZDSRXM' and a.sfky=1 and a.bh<>a.mc2 and a.ys=3");


            _connection.Execute("insert into SmartChargeCategory(ID,Name,ParentID,SortNo,Remark) values (@ID,@Name,@PID,@SortNo,@Remark)",
                     list, _transaction);
            Console.WriteLine("项目分类结束导入");
        }

        /// <summary>
        /// 项目
        /// </summary>
        public static void Charge()
        {
            Console.WriteLine("项目开始导入");
            var list = _sourceSqlConnection.Query<Product>(@"select distinct a.mc as Name,'' as PinYin,b.id as CategoryID,'' as Size,dj as SalePrice,
                        yxsj,'' as Remark
from SFXMB a
left join YHZDB b on a.ZDSRXM_bh=b.bh and  b.ZDFLB_bh='ZDSRXM'

");

            var chargeResult = new List<Charge>();
            int i = 10000;
            foreach (var u in list)
            {
                u.ID = i;
                u.Remark = "数据迁移";

                u.UnitID = 15819956000015360;


                if (u.yxsj >= DateTime.Today)
                {
                    u.Status = CommonStatus.Use;
                }
                else
                {
                    u.Status = CommonStatus.Stop;
                }

                if (u.CategoryID == null)
                {
                    u.CategoryID = 0;
                }

                chargeResult.Add(new Charge()
                {
                    ID = i,
                    CategoryID = u.CategoryID,
                    IsEvaluate = CommonStatus.Use,
                    Name = u.Name,
                    PinYin = u.Name.PinYin(),
                    Price = u.SalePrice,
                    ProductAdd = 1,
                    Remark = u.Remark,
                    Size = u.Size,
                    Status = u.Status,
                    Type = ChargeType.Charge,
                    UnitID = u.UnitID
                });


                i++;
            }


            if (chargeResult.Count() > 0)
            {
                _connection.Execute(@"insert into SmartCharge(ID,Name,CategoryID,PinYin,Price,Status,Remark,UnitID,Size,
ProductAdd,IsEvaluate,Type) 
values(@ID, @Name, @CategoryID, @PinYin, @Price, @Status, @Remark, @UnitID,@Size,@ProductAdd,@IsEvaluate,@Type)", chargeResult, _transaction);

                _connection.Execute(@"update SmartCharge set CategoryID=188 where CategoryID not in (select ID from SmartChargeCategory)", null, _transaction);
            }

            Console.WriteLine("项目结束导入");
        }

        /// <summary>
        /// 项目套餐
        /// </summary>
        public static void ChargeSet()
        {
            //Console.WriteLine("(S)中下身吸脂基础型".PinYin());
            Console.WriteLine("项目套餐开始导入");
            var listCharge = _connection.Query<Charge>(@"select * from SmartCharge", null, _transaction);
            var list = _sourceSqlConnection.Query<ChargeSet>(@"select pth_code as OldID,pth_name as Name, pth_tolamt as Price, pth_oldid as PinYin,
case when [pth_status]='STP' then 0 else 1 end as Status from zsb_pdutol_h");

            var listDetaik = _sourceSqlConnection.Query<SmartChargeSetDetail>(@"select pth_code as OldSetID,
a.zpt_num as Num,a.zpt_price as Amount,b.zpt_name as  ChargeName
from zsb_pdutol_det a
inner join zsb_product b on a.zpt_code=b.zpt_code
");
            int i = 10000, j = 10000;
            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                if (u.PinYin.IsNullOrEmpty())
                {
                    u.PinYin = u.Name.PinYin();
                }
                u.ID = i;
                u.TimeLimit = 0;
                u.TimeStart = 0;
                u.Days = 0;
                i++;
                u.CreateUserID = 1;
                u.CreateTime = now;
            }

            foreach (var u in listDetaik)
            {
                u.ID = j;
                u.ChargeID = listCharge.Where(x => x.Name.Trim() == u.ChargeName.Replace('\t', ' ').Trim()).FirstOrDefault().ID.Value;
                u.SetID = list.Where(x => x.OldID == u.OldSetID).FirstOrDefault().ID;
                j++;
            }

            _connection.Execute(@"insert into SmartChargeSetDetail(ID,SetID,ChargeID,Num,Amount) 
                                            VALUES(@ID, @SetID, @ChargeID, @Num, @Amount)", listDetaik, _transaction);
            _connection.Execute(@"insert into SmartChargeSet(ID,Name,Price,Status,Remark,PinYin,TimeLimit,TimeStart,Days,HospitalID,CreateUserID,CreateTime) 
                                    VALUES(@ID, @Name, @Price, @Status, @Remark, @PinYin, @TimeLimit, @TimeStart, @Days, @HospitalID,@CreateUserID,@CreateTime)", list, _transaction);

            //_connection.Execute(@"update SmartChargeSet set PinYin='' where PinYin is null", null, _transaction);
            Console.WriteLine("项目套餐结束导入");
        }

        /// <summary>
        /// N次项目数量套餐
        /// </summary>
        public static void NumChargeSet()
        {
            Console.WriteLine("项目套餐开始导入");
            var listCharge = _connection.Query<Charge>(@"select * from SmartCharge", null, _transaction);
            //            var list = _sourceSqlConnection.Query<ChargeSet>(@"select pth_code as OldID,pth_name as Name, pth_tolamt as Price, pth_oldid as PinYin,
            //case when [pth_status]='STP' then 0 else 1 end as Status from zsb_pdutol_h");

            var listDetaik = _sourceSqlConnection.Query<SmartChargeSetDetail>(@"select zpt_price as Amount,zpt_num as Num,zpt_uprice as Price,
zpt_name as ChargeName,
case zpt_status when 'STP' then '0' else '1' end as Status,zpt_oldid as PinYin,
zpt_remark as Remark  from zsb_product where zpt_num>1 and zpt_status<>'STP'
");

            var list = new List<ChargeSet>();
            int i = 100000, j = 100000;
            DateTime now = DateTime.Now;
            foreach (var u in listDetaik)
            {
                if (u.PinYin == null)
                {
                    u.PinYin = u.ChargeName.PinYin();
                }
                u.ID = j;
                u.ChargeID = listCharge.Where(x => x.Name == u.ChargeName.Replace('\t', ' ')).FirstOrDefault().ID.Value;
                u.SetID = i;
                list.Add(new DTO.ChargeSet()
                {
                    Days = 0,
                    HospitalID = _hospitalID,
                    Name = u.ChargeName,
                    PinYin = u.PinYin,
                    Price = u.Amount,
                    Status = u.Status,
                    Remark = u.Remark,
                    TimeLimit = 0,
                    TimeStart = 0,
                    ID = i,
                    CreateTime = now,
                    CreateUserID = 1
                }); ;
                j++;
                i++;
            }

            _connection.Execute(@"insert into SmartChargeSetDetail(ID,SetID,ChargeID,Num,Amount) 
                                            VALUES(@ID, @SetID, @ChargeID, @Num, @Amount)", listDetaik, _transaction);
            _connection.Execute(@"insert into SmartChargeSet(ID,Name,Price,Status,Remark,PinYin,TimeLimit,TimeStart,Days,HospitalID,CreateTime,CreateUserID) 
                                    VALUES(@ID, @Name, @Price, @Status, @Remark, @PinYin, @TimeLimit, @TimeStart, @Days, @HospitalID,@CreateTime,@CreateUserID)", list, _transaction);

            _connection.Execute(@"update SmartChargeSet set PinYin='' where PinYin is null", null, _transaction);
            Console.WriteLine("项目套餐结束导入");
        }

        /// <summary>
        /// 用户
        /// </summary>
        public static void User()
        {
            Console.WriteLine("用户开始导入");
            var list = _sourceSqlConnection.Query<User>(@"select case when b.yhm is null then a.bh else b.yhm end as Account,xm as Name,xb as Gender,
case zt when '1' then '1' else '0' end as Status,c.mc as DeptName,a.xh as Sort,b.dlmm as Password
from YG a
LEFT join XTYH b on a.ID=b.YG_id
left join KS c on a.KS_id=c.id
where a.bh<>'system' and a.bh<>'00103'
");

            var deptList = _connection.Query<DataTransferChannel>(@"select ID,Name from SmartDept", null, _transaction);

            List<UserRole> roleList = new List<UserRole>();
            DateTime now = DateTime.Now;
            int i = 10001;
            foreach (var u in list)
            {
                u.ID = i;
                u.HospitalID = _hospitalID;
                u.Discount = 1;
                u.CreateTime = now;
                u.CreateUserID = _hospitalID;
                u.Remark = "数据迁移";
                u.Password = HashHelper.GetMd5("123456");
                u.DeptID = deptList.Where(x => x.Name == u.DeptName).FirstOrDefault().ID;
                roleList.Add(new UserRole()
                {
                    RoleID = 15819937568523264,
                    UserID = u.ID,
                    ID = u.ID
                });
                i++;
            }

            _connection.Execute(
                    "insert into SmartUser([ID],[Account],[Password],[Name],[Gender],[DeptID],[Status],[Remark],[Phone],[HospitalID],[Discount],[CreateTime],[CreateUserID],Sort) " +
                    "values(@ID,@Account,@Password,@Name,@Gender,@DeptID,@Status,@Remark,@Phone,@HospitalID,@Discount,@CreateTime,@CreateUserID,@Sort)",
                     list, _transaction);

            _connection.Execute("insert into [SmartUserRole]([ID],[UserID],[RoleID]) values(@ID,@UserID,@RoleID)", roleList, _transaction);

            Console.WriteLine("用户结束导入");
        }

        /// <summary>
        /// 顾客
        /// </summary>
        public static void Customer()
        {
            Console.WriteLine("顾客开始迁移");
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\所有的客户.xlsx")))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                List<DataTransferCommon> customerList = new List<DataTransferCommon>();
                List<DataTransferCommon> channelList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> symptomList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                customerList = _connection.Query<DataTransferCommon>(@"select ID,Mobile as Name,[MobileBackup] as Account from [SmartCustomer]", null, _transaction).ToList();
                channelList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartChannel]", null, _transaction).ToList();
                symptomList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartSymptom]", null, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID order by Status desc", new { HospitalID = 1 }, _transaction);


                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable customerAddList = new DataTable("SmartCustomer");
                customerAddList.Columns.Add("ID", typeof(long));
                customerAddList.Columns.Add("Name", typeof(string));
                customerAddList.Columns.Add("Gender", typeof(int));
                customerAddList.Columns.Add("Remark", typeof(string));
                customerAddList.Columns.Add("Birthday", typeof(string));
                customerAddList.Columns.Add("ChannelID", typeof(long));
                customerAddList.Columns.Add("CreateTime", typeof(string));
                customerAddList.Columns.Add("CreateUserHospitalID", typeof(long));
                customerAddList.Columns.Add("CreateUserID", typeof(long));
                customerAddList.Columns.Add("CurrentConsultSymptomID", typeof(long));
                customerAddList.Columns.Add("MemberCategoryID", typeof(long));
                customerAddList.Columns.Add("Mobile", typeof(string));
                customerAddList.Columns.Add("Source", typeof(int));
                customerAddList.Columns.Add("PromoterID", typeof(long));
                customerAddList.Columns.Add("PromoterPhone", typeof(string));
                customerAddList.Columns.Add("Commission", typeof(decimal));
                customerAddList.Columns.Add("Point", typeof(decimal));
                customerAddList.Columns.Add("MobileBackup", typeof(string));
                customerAddList.Columns.Add("HospitalID", typeof(long));
                customerAddList.Columns.Add("Address", typeof(string));
                customerAddList.Columns.Add("Custom8", typeof(string));
                customerAddList.Columns.Add("Custom9", typeof(string));
                customerAddList.Columns.Add("Custom10", typeof(string));
                DataTable ownerShipAddList = new DataTable("SmartOwnerShip");
                ownerShipAddList.Columns.Add("CustomerID", typeof(long));
                ownerShipAddList.Columns.Add("EndTime", typeof(string));
                ownerShipAddList.Columns.Add("StartTime", typeof(string));
                ownerShipAddList.Columns.Add("HospitalID", typeof(long));
                ownerShipAddList.Columns.Add("Remark", typeof(string));
                ownerShipAddList.Columns.Add("Type", typeof(int));
                ownerShipAddList.Columns.Add("UserID", typeof(long));
                DataTransferCommon symptomTemp = null;
                DataTransferCommon channelTemp = null;
                DateTime createTime;
                DataTransferCommon createUserTemp = null;
                GenderEnum gender = GenderEnum.Girl;
                DataTransferCommon exploitUserTemp = null;
                DataTransferCommon managerUserTemp = null;
                DataTransferCommon customerUserTemp = null;
                long customerID;
                decimal point = 0;
                decimal commission = 0;
                List<object> commissionList = new List<object>();
                List<object> pointList = new List<object>();
                DateTime now = DateTime.Now;
                DataTransferCommon customerTemp = null;
                //校验

                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 4].Value == null)
                    {
                        break;
                    }
                    exploitUserTemp = null;
                    managerUserTemp = null;
                    customerTemp = null;

                    //顾客姓名
                    if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 1].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客姓名不能为空！");
                    }
                    //手机号、备用手机号
                    if (worksheet.Cells[row, 6].Value == null || worksheet.Cells[row, 6].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 6].Value = "";
                        if (worksheet.Cells[row, 7].Value != null && !worksheet.Cells[row, 7].Value.ToString().Trim().IsNullOrEmpty())
                        {
                            worksheet.Cells[row, 6].Value = worksheet.Cells[row, 7].Value;
                            worksheet.Cells[row, 7].Value = "";
                        }
                        //result.Message = "第" + row + "行手机号不能为空！";
                        //return result;
                    }
                    /*if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        result.Message = "第" + row + "行用户名不能为空！";
                        return result;
                    }*/
                    //性别
                    if (worksheet.Cells[row, 2].Value == null || worksheet.Cells[row, 2].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 2].Value = "女";
                        //result.Message = "第" + row + "行性别不能为空！";
                        //return result;
                    }
                    //if (worksheet.Cells[row, 5].Value == null || worksheet.Cells[row, 5].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    result.Message = "第" + row + "行生日不能为空！";
                    //    return result;
                    //}
                    //登记时间
                    if (worksheet.Cells[row, 13].Value == null || worksheet.Cells[row, 13].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行登记时间不能为空！");
                        //result.Message = "第" + row + "行登记时间不能为空！";
                        //return result;
                    }
                    //登记人
                    //if (worksheet.Cells[row, 7].Value == null || worksheet.Cells[row, 7].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    worksheet.Cells[row, 7].Value = "超级管理员";
                    //    //result.Message = "第" + row + "行登记人不能为空！";
                    //    //return result;
                    //}
                    //渠道
                    if (worksheet.Cells[row, 17].Value == null || worksheet.Cells[row, 17].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 17].Value = "数据迁移空渠道";
                        //result.Message = "第" + row + "行渠道不能为空！";
                        //return result;
                    }
                    /*if (worksheet.Cells[row, 9].Value == null || worksheet.Cells[row, 9].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        result.Message = "第" + row + "行症状不能为空！";
                        return result;
                    }*/
                    //剩余积分
                    //if (worksheet.Cells[row, 12].Value == null || worksheet.Cells[row, 12].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    worksheet.Cells[row, 12].Value = 0;
                    //    //result.Message = "第" + row + "行剩余积分不能为空！";
                    //    //return result;
                    //}
                    //if (worksheet.Cells[row, 13].Value == null || worksheet.Cells[row, 13].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    worksheet.Cells[row, 13].Value = 0;
                    //    //result.Message = "第" + row + "行剩余佣金不能为空！";
                    //    //return result;
                    //}

                    //if (worksheet.Cells[row, 3].Value != null)
                    //{
                    //    if (customerList.AsParallel().Any(u => u.Name == worksheet.Cells[row, 3].Value.ToString().Trim() || u.Account == worksheet.Cells[row, 3].Value.ToString().Trim()
                    //    || u.Name == worksheet.Cells[row, 2].Value.ToString().Trim() || u.Account == worksheet.Cells[row, 2].Value.ToString().Trim()))
                    //    {
                    //        continue;
                    //        //result.Message = "第" + row + "行手机号或者备用手机号已存在！";
                    //        //return result;
                    //    }
                    //}
                    //else
                    //{
                    //    if (customerList.AsParallel().Any(u => u.Name == worksheet.Cells[row, 2].Value.ToString().Trim() || u.Account == worksheet.Cells[row, 2].Value.ToString().Trim()))
                    //    {
                    //        continue;
                    //        //result.Message = "第" + row + "行手机号已存在！";
                    //        //return result;
                    //    }
                    //}

                    channelTemp = channelList.Where(u => u.Name == worksheet.Cells[row, 17].Value.ToString().Trim()).FirstOrDefault();
                    if (channelTemp == null)
                    {
                        channelTemp = new DataTransferCommon()
                        {
                            ID = 15820093232088064
                        };
                    }
                    symptomTemp = null;
                    //if (worksheet.Cells[row, 9].Value != null && !worksheet.Cells[row, 9].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    symptomTemp = symptomList.Where(u => u.Name == worksheet.Cells[row, 9].Value.ToString().Trim()).FirstOrDefault();
                    //    if (symptomTemp == null)
                    //    {
                    //        throw new Exception("第" + row + "行症状不存在！");
                    //    }
                    //}

                    if (worksheet.Cells[row, 2].Value.ToString().Trim() == "男")
                    {
                        gender = GenderEnum.Boy;
                    }
                    else
                    {
                        gender = GenderEnum.Girl;
                    }


                    if (!DateTime.TryParse(worksheet.Cells[row, 13].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行登记时间异常！");
                    }
                    createTime = createTime.AddSeconds(1);
                    createUserTemp = new DataTransferCommon()
                    {
                        ID = 1,
                        Name = "超级管理员"
                    };

                    if (worksheet.Cells[row, 11].Value != null)
                    {
                        exploitUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 11].Value.ToString()).FirstOrDefault();
                        if (exploitUserTemp == null)
                        {
                            throw new Exception("第" + row + "行网电咨询师不存在！");
                        }
                    }
                    if (worksheet.Cells[row, 9].Value != null)
                    {
                        managerUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 9].Value.ToString()).FirstOrDefault();
                        if (managerUserTemp == null)
                        {
                            throw new Exception("第" + row + "行现场咨询师不存在！");
                        }
                    }
                    if (worksheet.Cells[row, 10].Value != null)
                    {
                        customerUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 10].Value.ToString()).FirstOrDefault();
                        if (customerUserTemp == null)
                        {
                            throw new Exception("第" + row + "行咨询客服不存在！");
                        }
                    }
                    customerID = OrderAutoNumber.Instance().Number(OrderAutoNumber.customerStr);

                    point = 0;

                    commission = 0;



                    DataRow dr = customerAddList.NewRow();
                    //if (worksheet.Cells[row, 14].Value != null)
                    //{
                    //    /*customerTemp = customerList.Where(u => u.Name == worksheet.Cells[row, 14].Value.ToString() || u.Account == worksheet.Cells[row, 14].Value.ToString()).FirstOrDefault();
                    //    if (customerTemp == null)
                    //    {
                    //        result.Message = "第" + row + "行推荐人手机号不存在！";
                    //        return result;
                    //    }*/
                    //    dr["PromoterPhone"] = worksheet.Cells[row, 14].Value.ToString().Trim();
                    //}

                    if (worksheet.Cells[row, 1].Value.ToString().Trim().Length > 20)
                    {
                        dr["Name"] = worksheet.Cells[row, 1].Value.ToString().Trim().Substring(0, 20);
                    }
                    else
                    {
                        dr["Name"] = worksheet.Cells[row, 1].Value.ToString().Trim();
                    }
                    dr["ID"] = customerID;

                    dr["Gender"] = gender.CastTo<int>();

                    if (worksheet.Cells[row, 12].Value == null)
                    {
                        dr["Remark"] = "";
                    }
                    else
                    {
                        if (worksheet.Cells[row, 12].Value.ToString().Trim().Length > 2000)
                        {
                            dr["Remark"] = worksheet.Cells[row, 12].Value.ToString().Trim().Substring(0, 2000);
                        }
                        else
                        {
                            dr["Remark"] = worksheet.Cells[row, 12].Value.ToString().Trim();
                        }
                    }

                    //if (worksheet.Cells[row, 5].Value != null && worksheet.Cells[row, 5].Value.ToString().Trim() != "")
                    //{
                    //    dr["Birthday"] = DateTime.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
                    //}


                    if (worksheet.Cells[row, 3].Value != null && worksheet.Cells[row, 3].Value.ToString().Trim() != "")
                    {
                        var birthday = DateTime.Today.AddYears(int.Parse(worksheet.Cells[row, 3].Value.ToString()) * -1);
                        dr["Birthday"] = birthday.ToShortDateString();
                        if (worksheet.Cells[row, 4].Value != null && worksheet.Cells[row, 4].Value.ToString().Trim() != "")
                        {
                            if (worksheet.Cells[row, 4].Value.ToString().Length == 4)
                            {
                                dr["Birthday"] = birthday.Year + "-" + worksheet.Cells[row, 4].Value.ToString().Substring(0, 2) + "-" + worksheet.Cells[row, 4].Value.ToString().Substring(2, 2);
                            }
                        }
                    }

                    //dr["Birthday"] = "";
                    dr["ChannelID"] = channelTemp.ID;
                    dr["CreateTime"] = createTime.ToString();
                    dr["CreateUserHospitalID"] = 1;
                    dr["HospitalID"] = 1;
                    dr["CreateUserID"] = createUserTemp.ID;
                    if (symptomTemp != null)
                    {
                        dr["CurrentConsultSymptomID"] = symptomTemp.ID;
                    }
                    dr["MemberCategoryID"] = 0;
                    dr["Point"] = point;
                    dr["Commission"] = commission;
                    /*if (customerTemp != null)
                    {
                        dr["PromoterID"] = customerTemp.ID;
                    }*/
                    if (point > 0)
                    {
                        pointList.Add(new
                        {
                            CustomerID = customerID,
                            CreateUserID = 1,
                            CreateTime = now,
                            Type = PointType.DataImport,
                            Amount = point,
                            Remark = "数据迁移",
                            HospitalID = 1,
                            ConsumeAmount = 0,
                            FromHospitalID = 1,
                            ID = SingleIdWork.Instance(1).nextId()
                        });
                    }
                    if (commission > 0)
                    {
                        commissionList.Add(new
                        {
                            CustomerID = customerID,
                            CreateTime = now,
                            CreateUserID = 1,
                            Type = CommissionType.DataImport,
                            HospitalID = 1,
                            Commission = commission,
                            Remark = "数据迁移",
                            FromHospitalID = 1,
                            ID = SingleIdWork.Instance(1).nextId(),
                        });
                    }
                    if (worksheet.Cells[row, 6].Value.ToString().Trim().Length > 20)
                    {
                        dr["Mobile"] = worksheet.Cells[row, 6].Value.ToString().Trim().Substring(0, 20);
                    }
                    else
                    {
                        dr["Mobile"] = worksheet.Cells[row, 6].Value.ToString().Trim();
                    }
                    dr["Source"] = CustomerRegisterType.DataTransfer.CastTo<int>();
                    if (worksheet.Cells[row, 7].Value == null)
                    {
                        dr["MobileBackup"] = "";
                    }
                    else
                    {
                        if (worksheet.Cells[row, 7].Value.ToString().Trim().Length > 50)
                        {
                            dr["MobileBackup"] = worksheet.Cells[row, 7].Value.ToString().Trim().Substring(0, 50);
                        }
                        else
                        {
                            dr["MobileBackup"] = worksheet.Cells[row, 7].Value.ToString().Trim();
                        }
                    }

                    if (worksheet.Cells[row, 21].Value == null)
                    {
                        dr["Address"] = "";
                    }
                    else
                    {
                        dr["Address"] = worksheet.Cells[row, 21].Value.ToString().Trim();
                    }


                    if (worksheet.Cells[row, 5].Value == null)
                    {
                        dr["Custom10"] = "";
                    }
                    else
                    {
                        //********************************************************************************************************************************
                        dr["Custom10"] = worksheet.Cells[row, 5].Value.ToString().Trim();
                    }
                    customerAddList.Rows.Add(dr);

                    customerList.Add(new DataTransferCommon()
                    {
                        ID = customerID,
                        Name = dr["Mobile"].ToString(),
                        Account = dr["MobileBackup"] == null ? "" : dr["MobileBackup"].ToString()
                    });

                    if (exploitUserTemp != null)
                    {
                        DataRow dr2 = ownerShipAddList.NewRow();
                        dr2["CustomerID"] = customerID;
                        dr2["EndTime"] = "9999-12-31 23:59:59";
                        dr2["StartTime"] = createTime.ToString();
                        dr2["HospitalID"] = 1;
                        dr2["Remark"] = "数据迁移";
                        dr2["Type"] = OwnerShipType.Exploit.CastTo<int>();
                        dr2["UserID"] = exploitUserTemp.ID;
                        ownerShipAddList.Rows.Add(dr2);
                    }
                    if (customerUserTemp != null)
                    {
                        DataRow dr2 = ownerShipAddList.NewRow();
                        dr2["CustomerID"] = customerID;
                        dr2["EndTime"] = "9999-12-31 23:59:59";
                        dr2["StartTime"] = createTime.ToString();
                        dr2["HospitalID"] = 1;
                        dr2["Remark"] = "数据迁移";
                        dr2["Type"] = OwnerShipType.CustomerCall.CastTo<int>();
                        dr2["UserID"] = customerUserTemp.ID;
                        ownerShipAddList.Rows.Add(dr2);
                    }
                    if (managerUserTemp != null)
                    {
                        DataRow dr2 = ownerShipAddList.NewRow();
                        dr2["CustomerID"] = customerID;
                        dr2["EndTime"] = "9999-12-31 23:59:59";
                        dr2["StartTime"] = createTime.ToString();
                        dr2["HospitalID"] = 1;
                        dr2["Remark"] = "数据迁移";
                        dr2["Type"] = OwnerShipType.Manager.CastTo<int>();
                        dr2["UserID"] = managerUserTemp.ID;
                        ownerShipAddList.Rows.Add(dr2);
                    }
                }




                ///导入数据库

                if (customerAddList.Rows.Count > 0)
                {
                    customerAddList.Columns.Remove("PromoterPhone");
                    SqlBulkCopyByDataTable("SmartCustomer", customerAddList);
                }
                if (ownerShipAddList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartOwnerShip", ownerShipAddList);
                }

                if (pointList.Count > 0)
                {
                    _connection.Execute(@"insert into SmartPoint(ID,CustomerID,CreateUserID,CreateTime,Type,Amount,Remark,HospitalID,ConsumeAmount,FromHospitalID) 
values(@ID,@CustomerID,@CreateUserID,@CreateTime,@Type,@Amount,@Remark,@HospitalID,@ConsumeAmount,@FromHospitalID)", pointList, _transaction);
                }
                if (commissionList.Count > 0)
                {
                    _connection.Execute(@"insert into SmartCommissionRecord(ID,CustomerID,CreateTime,CreateUserID,Type,HospitalID,Commission,Remark,FromHospitalID) 
values(@ID,@CustomerID,@CreateTime,@CreateUserID,@Type,@HospitalID,@Commission,@Remark,@FromHospitalID)", commissionList, _transaction);

                }
                //生成顾客医院子表
                _connection.Execute(@"insert into [SmartCustomerHospital]([CustomerID],[HospitalID],[Point],[Commission])
select a.ID as CustomerID,b.ID as HospitalID,0,0 from SmartCustomer a,SmartHospital b where a.ID not in (select distinct CustomerID from SmartCustomerHospital)", null, _transaction);
                //更新顾客医院子表
                _connection.Execute(@"update [SmartCustomerHospital] set Point=b.Amount 
  from [SmartCustomerHospital] a
  inner join (
  select a.CustomerID,a.HospitalID,sum([Amount]) as Amount
  from [SmartPoint] a group by a.CustomerID,a.HospitalID) as b on a.CustomerID=b.CustomerID and a.HospitalID=b.HospitalID", null, _transaction);
                _connection.Execute(@"update [SmartCustomerHospital] set [Commission]=b.Amount 
  from [SmartCustomerHospital] a
  inner join (
  select a.CustomerID,a.HospitalID,sum([Commission]) as Amount
  from [SmartCommissionRecord] a group by a.CustomerID,a.HospitalID) as b on a.CustomerID=b.CustomerID and a.HospitalID=b.HospitalID", null, _transaction);
            }

            Console.WriteLine("顾客结束迁移");
        }

        /// <summary>
        /// 标签记录
        /// </summary>
        public static void CustomerTag()
        {
            Console.WriteLine("标签记录开始迁移");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\8888.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                IEnumerable<DataTransferCommon> toolList = new List<DataTransferCommon>();
                toolList = _connection.Query<DataTransferCommon>(@"select ID,[Content] as Name from [SmartTag]", null, _transaction);



                DataTransferCommon toolTemp = null;
                long id = 0;
                List<object> result = new List<object>();
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }


                    toolTemp = toolList.Where(u => u.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();


                    id = SingleIdWork.Instance(1).nextId();

                    var data = new
                    {
                        ID = id,
                        CustomerID = 0,
                        TagID = toolTemp.ID,
                        CreateUserID = 1,
                        CreateTime = DateTime.Now,
                        Custom10 = worksheet.Cells[row, 2].Value.ToString().Trim()
                    };
                    result.Add(data);
                }

                ///导入数据库
                _connection.Execute(@"ALTER TABLE [SmartCustomerTag] ADD [Custom10] nvarchar(255)", null, _transaction);

                _connection.Execute(@"insert into SmartCustomerTag values(@ID,@CustomerID,@TagID,@CreateUserID,@CreateTime,@Custom10)", result, _transaction);
                _connection.Execute(@"update SmartCustomerTag set CustomerID=b.ID 
from SmartCustomerTag a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);

                _connection.Execute(@"delete from SmartCustomerTag where CustomerID=0", null, _transaction);
                _connection.Execute(@"ALTER TABLE [SmartCustomerTag] DROP COLUMN [Custom10]", null, _transaction);
            }

            Console.WriteLine("标签记录结束迁移");
        }

        /// <summary>
        /// 标签记录
        /// </summary>
        public static void CustomerTag2()
        {
            Console.WriteLine("标签记录开始迁移");


            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\客户档案查询.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable customerAddList = new DataTable("SmartTest");
                customerAddList.Columns.Add("No", typeof(string));
                customerAddList.Columns.Add("Phone", typeof(string));
                customerAddList.Columns.Add("Doc", typeof(string));
                customerAddList.Columns.Add("IDCard", typeof(string));
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null)
                    {
                        break;
                    }

                    DataRow dr = customerAddList.NewRow();
                    if (worksheet.Cells[row, 3].Value != null)
                    {
                        dr["No"] = worksheet.Cells[row, 3].Value.ToString().Trim();
                    }
                    else
                    {
                        dr["No"] = "";
                    }

                    if (worksheet.Cells[row, 5].Value != null)
                    {
                        dr["Phone"] = worksheet.Cells[row, 5].Value.ToString().Trim();
                    }
                    else
                    {
                        dr["Phone"] = "";
                    }
                    if (worksheet.Cells[row, 4].Value != null)
                    {
                        dr["Doc"] = worksheet.Cells[row, 4].Value.ToString().Trim();
                    }
                    else
                    {
                        dr["Doc"] = "";
                    }
                    if (worksheet.Cells[row, 22].Value != null)
                    {
                        dr["IDCard"] = worksheet.Cells[row, 22].Value.ToString().Trim();
                    }
                    else
                    {
                        dr["IDCard"] = "";
                    }
                    customerAddList.Rows.Add(dr);
                }

                SqlBulkCopyByDataTable("SmartTest", customerAddList);

                _connection.Execute(@"create table SmartTest
(
No nvarchar(200),
Phone nvarchar(200),
Doc nvarchar(200),
IDCard nvarchar(200)
)", null, _transaction);

                _connection.Execute(@"update SmartCustomer set Custom1=b.DOC
from SmartCustomer a
inner join Smarttest b on a.Custom10=b.No and a.Custom10 is not null and a.Custom10<>'' and b.No<>'' and b.DOc<>''


update SmartCustomer set Custom1=b.DOC
from SmartCustomer a
inner join Smarttest b on a.Mobile=b.Phone and a.Mobile is not null and a.Mobile<>'' and b.No='' and b.DOc<>'' and b.Phone<>''



update SmartCustomer set Custom2=b.IDCard

from SmartCustomer a
inner join Smarttest b on a.Custom10=b.No and a.Custom10 is not null and a.Custom10<>'' and b.No<>'' and b.IDCard<>''


update SmartCustomer set Custom2=b.IDCard
from SmartCustomer a
inner join Smarttest b on a.Mobile=b.Phone and a.Mobile is not null and a.Mobile<>'' and b.No='' and b.IDCard<>'' and b.Phone<>''", null, _transaction);

            }



            Console.WriteLine("标签记录结束迁移");
        }

        /// <summary>
        /// 科室客服
        /// </summary>
        public static void FactoryNew()
        {
            Console.WriteLine("工厂开始迁移");


            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\淄博壹美\\最新仓库\\Factory.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                List<DataTransferCommon> result = new List<DataTransferCommon>();

                var list = _connection.Query<string>(@"select Name from SmartFactory", null, _transaction);

                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null)
                    {
                        break;
                    }
                    if (list.Where(x => x.Trim() == worksheet.Cells[row, 1].Value.ToString().Trim()).Count() == 0)
                    {
                        result.Add(new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                        });
                    }

                }

                ///导入数据库
                //_connection.Execute(@"delete from  SmartCustomerTag where TagID=@TagID", new { TagID = 14996523617928192 }, _transaction);
                // _connection.Execute(@"insert into Test values(@Name)", result, _transaction) ;

                if (result.Count > 0)
                {
                    _connection.Execute(@"insert into SmartFactory values(@ID,@Name,1,'Excel导入','2021-05-07',1)", result, _transaction);

                }

            }



            Console.WriteLine("工厂结束迁移");
        }

        /// <summary>
        /// 科室客服
        /// </summary>
        public static void CustomerKF()
        {
            Console.WriteLine("科室客服开始迁移");


            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\淄博壹美\\科室客服5.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                List<CallBack> result = new List<CallBack>();


                DataTable consultList = new DataTable("Test");
                consultList.Columns.Add("ID", typeof(string));

                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null)
                    {
                        break;
                    }
                    DataRow dr = consultList.NewRow();
                    dr["ID"] = worksheet.Cells[row, 2].Value.ToString().Trim();
                    consultList.Rows.Add(dr);
                    //result.Add(new DTO.CallBack()
                    //{
                    //     Name= worksheet.Cells[row, 2].Value.ToString().Trim()
                    //});
                }

                ///导入数据库
                //_connection.Execute(@"delete from  SmartCustomerTag where TagID=@TagID", new { TagID = 14996523617928192 }, _transaction);
                // _connection.Execute(@"insert into Test values(@Name)", result, _transaction) ;

                if (consultList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("Test", consultList);
                }

            }



            Console.WriteLine("科室客服结束迁移");
        }

        /// <summary>
        /// 网电咨询
        /// </summary>
        public static void ConsultExploit()
        {
            Console.WriteLine("网电咨询记录开始迁移");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\南京环亚\\电话网络情况明细表.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                IEnumerable<DataTransferCommon> toolList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> symptomList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();

                //customerList = _connection.Query<DataTransferCommon>(@"select Custom10 as Name,ID from [SmartCustomer]");
                toolList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartTool]", null, _transaction);
                symptomList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartSymptom]", null, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID", new { HospitalID = 1 }, _transaction);


                DataTable consultList = new DataTable("SmartConsult");
                consultList.Columns.Add("ID", typeof(long));
                consultList.Columns.Add("CustomerID", typeof(long));
                consultList.Columns.Add("CreateUserID", typeof(long));
                consultList.Columns.Add("CreateTime", typeof(DateTime));
                consultList.Columns.Add("Tool", typeof(long));
                consultList.Columns.Add("Content", typeof(string));
                consultList.Columns.Add("HospitalID", typeof(long));
                consultList.Columns.Add("Custom10", typeof(string));


                DataTable detailList = new DataTable("SmartConsultSymptomDetail");
                detailList.Columns.Add("ConsultID", typeof(long));
                detailList.Columns.Add("SymptomID", typeof(long));

                DataTransferCommon symptomTemp = null;
                DataTransferCommon toolTemp = null;
                DateTime createTime;
                DataTransferCommon createUserTemp = null;
                long id = 0;
                string content = "";
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    if (worksheet.Cells[row, 5].Value == null || worksheet.Cells[row, 5].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 5].Value = "超级管理员";
                        //result.Message = "第" + row + "行咨询师不能为空！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 2].Value == null || worksheet.Cells[row, 2].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行咨询时间不能为空！");
                    }
                    //if (worksheet.Cells[row, 4].Value == null || worksheet.Cells[row, 4].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    worksheet.Cells[row, 4].Value = "当面";
                    //    //result.Message = "第" + row + "行沟通工具不能为空！";
                    //    //return result;
                    //}
                    if (worksheet.Cells[row, 7].Value == null || worksheet.Cells[row, 7].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 7].Value = "空咨询";
                        //result.Message = "第" + row + "行咨询症状不能为空！";
                        //return result;
                    }


                    //customerTemp = customerList.AsParallel().Where(u => u.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();
                    //if (customerTemp == null)
                    //{
                    //    result.Message = "第" + row + "行该顾客手机号不存在！";
                    //    return result;
                    //}
                    symptomTemp = symptomList.Where(u => u.Name == worksheet.Cells[row, 7].Value.ToString().Trim()).FirstOrDefault();
                    //********************************************************************************************************************************************************************************************************
                    if (symptomTemp == null)
                    {
                        //symptomTemp = symptomList.Where(u => u.Name == "其它").FirstOrDefault();
                        if (symptomTemp == null)
                        {
                            //throw new Exception("第" + row + "行症状不存在！");
                            symptomTemp = new DataTransferCommon()
                            {
                                ID = 15553291689231360
                            };
                        }
                    }
                    createUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();
                    if (createUserTemp == null)
                    {
                        createUserTemp = new DataTransferCommon()
                        {
                            ID = 1,
                            Name = "超级管理员"
                        };
                        //result.Message = "第" + row + "行咨询师不存在！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 13].Value == null)
                    {
                        worksheet.Cells[row, 13].Value = "商务通";
                    }
                    toolTemp = toolList.Where(u => u.Name == worksheet.Cells[row, 13].Value.ToString().Trim()).FirstOrDefault();
                    if (toolTemp == null)
                    {
                        toolTemp = toolList.Where(u => u.Name == "商务通").FirstOrDefault();
                    }

                    if (!DateTime.TryParse(worksheet.Cells[row, 2].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行登记时间异常！");
                    }

                    id = SingleIdWork.Instance(1).nextId();

                    DataRow dr = consultList.NewRow();
                    dr["ID"] = id;
                    //dr["CustomerID"] = new Random().Next(958266,1430913);
                    dr["CustomerID"] = 0;
                    dr["CreateUserID"] = createUserTemp.ID;
                    dr["CreateTime"] = createTime;
                    dr["Tool"] = toolTemp.ID;
                    dr["HospitalID"] = 1;
                    dr["Custom10"] = worksheet.Cells[row, 3].Value.ToString().Trim();
                    content = "";
                    if (worksheet.Cells[row, 8].Value == null)
                    {
                        content = "";
                    }
                    else
                    {
                        content = worksheet.Cells[row, 8].Value.ToString().Trim();
                        if (content.Length > 1900)
                        {
                            content = content.Substring(0, 1900);
                        }
                    }
                    dr["Content"] = content;

                    consultList.Rows.Add(dr);


                    DataRow dr2 = detailList.NewRow();
                    dr2["ConsultID"] = id;
                    dr2["SymptomID"] = symptomTemp.ID;
                    detailList.Rows.Add(dr2);

                }

                ///导入数据库
                _connection.Execute(@"ALTER TABLE [SmartConsult] ADD [Custom10] nvarchar(255)", null, _transaction);

                if (consultList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartConsult", consultList);
                }
                if (detailList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartConsultSymptomDetail", detailList);
                }

                _connection.Execute(@"update SmartConsult set CustomerID=b.ID 
from SmartConsult a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);

                //1、更新首次咨询时间、最后咨询时间、咨询次数
                _connection.Execute(@"update SmartCustomer set FirstConsultTime=min,LastConsultTime=max,ConsultTimes=count 
  from SmartCustomer a
  inner join (select CustomerID,MIN(createtime) as min,MAX(CreateTime) as max,count(CustomerID) as count 
  from SmartConsult group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);

                _connection.Execute(@"delete from SmartConsultSymptomDetail where ConsultID in (select ID from SmartConsult where CustomerID=0)", null, _transaction);
                _connection.Execute(@"delete from SmartConsult where CustomerID=0", null, _transaction);
                _connection.Execute(@"ALTER TABLE [SmartConsult] DROP COLUMN [Custom10]", null, _transaction);

                _connection.Execute(@"update SmartCustomer set CurrentConsultSymptomID=c.SymptomID
from SmartCustomer a
inner join SmartConsult b on a.ID=b.CustomerID and a.LastConsultTime=b.CreateTime
inner join SmartConsultSymptomDetail c on b.ID=c.ConsultID", null, _transaction);

                _connection.Execute(@"update SmartCustomer set CurrentConsultSymptomID=null where CurrentConsultSymptomID=15240289315734528", null, _transaction);
            }

            Console.WriteLine("咨询记录结束迁移");
        }

        /// <summary>
        /// 现场咨询
        /// </summary>
        public static void ConsultManager()
        {
            Console.WriteLine("现场咨询记录开始迁移");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\客户形象设计历史.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                IEnumerable<DataTransferCommon> toolList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> symptomList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                //customerList = _connection.Query<DataTransferCommon>(@"select Custom10 as Name,ID from [SmartCustomer]");
                toolList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartTool]", null, _transaction);
                symptomList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartSymptom]", null, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID order by Status desc", new { HospitalID = 1 }, _transaction);

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable consultList = new DataTable("SmartConsult");
                consultList.Columns.Add("ID", typeof(long));
                consultList.Columns.Add("CustomerID", typeof(long));
                consultList.Columns.Add("CreateUserID", typeof(long));
                consultList.Columns.Add("CreateTime", typeof(DateTime));
                consultList.Columns.Add("Tool", typeof(long));
                consultList.Columns.Add("Content", typeof(string));
                consultList.Columns.Add("HospitalID", typeof(long));
                consultList.Columns.Add("Custom10", typeof(string));


                DataTable detailList = new DataTable("SmartConsultSymptomDetail");
                detailList.Columns.Add("ConsultID", typeof(long));
                detailList.Columns.Add("SymptomID", typeof(long));

                DataTransferCommon symptomTemp = null;
                DataTransferCommon toolTemp = null;
                DateTime createTime;
                DataTransferCommon createUserTemp = null;
                long id = 0;
                string content = "";
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    if (worksheet.Cells[row, 6].Value == null || worksheet.Cells[row, 6].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    if (worksheet.Cells[row, 13].Value == null || worksheet.Cells[row, 13].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 13].Value = "超级管理员";
                        //result.Message = "第" + row + "行咨询师不能为空！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行咨询时间不能为空！");
                    }
                    //if (worksheet.Cells[row, 4].Value == null || worksheet.Cells[row, 4].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    worksheet.Cells[row, 4].Value = "当面";
                    //    //result.Message = "第" + row + "行沟通工具不能为空！";
                    //    //return result;
                    //}
                    if (worksheet.Cells[row, 20].Value == null || worksheet.Cells[row, 20].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 20].Value = "空";
                        //result.Message = "第" + row + "行咨询症状不能为空！";
                        //return result;
                    }


                    //customerTemp = customerList.AsParallel().Where(u => u.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();
                    //if (customerTemp == null)
                    //{
                    //    result.Message = "第" + row + "行该顾客手机号不存在！";
                    //    return result;
                    //}
                    symptomTemp = symptomList.Where(u => u.Name == worksheet.Cells[row, 20].Value.ToString().Trim()).FirstOrDefault();
                    if (symptomTemp == null)
                    {
                        //symptomTemp = symptomList.Where(u => u.Name == "其它").FirstOrDefault();
                        if (symptomTemp == null)
                        {
                            //throw new Exception("第" + row + "行症状不存在！");
                            symptomTemp = new DataTransferCommon()
                            {
                                ID = 15820297174598656
                            };
                        }
                    }
                    createUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 13].Value.ToString().Trim()).FirstOrDefault();
                    if (createUserTemp == null)
                    {
                        createUserTemp = new DataTransferCommon()
                        {
                            ID = 1,
                            Name = "超级管理员"
                        };
                        //result.Message = "第" + row + "行咨询师不存在！";
                        //return result;
                    }
                    toolTemp = toolList.Where(u => u.Name == "电话").FirstOrDefault();
                    if (toolTemp == null)
                    {
                        throw new Exception("第" + row + "行工具不存在！");
                    }

                    if (!DateTime.TryParse(worksheet.Cells[row, 3].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行登记时间异常！");
                    }

                    id = SingleIdWork.Instance(1).nextId();

                    DataRow dr = consultList.NewRow();
                    dr["ID"] = id;
                    //dr["CustomerID"] = new Random().Next(958266,1430913);
                    dr["CustomerID"] = 0;
                    dr["CreateUserID"] = createUserTemp.ID;
                    dr["CreateTime"] = createTime;
                    dr["Tool"] = toolTemp.ID;
                    dr["HospitalID"] = 1;
                    dr["Custom10"] = worksheet.Cells[row, 6].Value.ToString().Trim();
                    content = "";
                    if (worksheet.Cells[row, 22].Value == null)
                    {
                        content = "";
                        if (worksheet.Cells[row, 25].Value != null)
                        {
                            content = worksheet.Cells[row, 25].Value.ToString().Trim();
                        }
                    }
                    else
                    {
                        content = worksheet.Cells[row, 22].Value.ToString().Trim();

                    }
                    if (content.Length > 2000)
                    {
                        content = content.Substring(0, 2000);
                    }
                    dr["Content"] = content;

                    consultList.Rows.Add(dr);


                    DataRow dr2 = detailList.NewRow();
                    dr2["ConsultID"] = id;
                    dr2["SymptomID"] = symptomTemp.ID;
                    detailList.Rows.Add(dr2);

                }

                ///导入数据库
                _connection.Execute(@"ALTER TABLE [SmartConsult] ADD [Custom10] nvarchar(255)", null, _transaction);

                if (consultList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartConsult", consultList);
                }
                if (detailList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartConsultSymptomDetail", detailList);
                }

                _connection.Execute(@"update SmartConsult set CustomerID=b.ID 
from SmartConsult a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);

                //1、更新首次咨询时间、最后咨询时间、咨询次数
                _connection.Execute(@"update SmartCustomer set FirstConsultTime=min,LastConsultTime=max,ConsultTimes=count 
  from SmartCustomer a
  inner join (select CustomerID,MIN(createtime) as min,MAX(CreateTime) as max,count(CustomerID) as count 
  from SmartConsult group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);


                _connection.Execute(@"delete from SmartConsult where CustomerID=0", null, _transaction);
                _connection.Execute(@"ALTER TABLE [SmartConsult] DROP COLUMN [Custom10]", null, _transaction);
                _connection.Execute(@"update SmartCustomer set CurrentConsultSymptomID=c.SymptomID
from SmartCustomer a
inner join SmartConsult b on a.ID=b.CustomerID and a.LastConsultTime=b.CreateTime 
inner join SmartConsultSymptomDetail c on b.ID=c.ConsultID", null, _transaction);

                _connection.Execute(@"update SmartCustomer set CurrentConsultSymptomID=null where CurrentConsultSymptomID=15820297174598656", null, _transaction);

            }

            Console.WriteLine("咨询记录结束迁移");
        }


        /// <summary>
        /// 回访计划记录
        /// </summary>
        public static void CallBackTask()
        {
            Console.WriteLine("回访计划记录开始迁移");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\CallBackTask.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                IEnumerable<DataTransferCommon> customerList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> callbackCategoryList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();

                //customerList = _connection.Query<DataTransferCommon>(@"select Mobile as Name,ID from [SmartCustomer]");
                callbackCategoryList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartCallbackCategory]", null, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID order by Status desc", new { HospitalID = 1 }, _transaction);


                DataTable callbackList = new DataTable("SmartCallback");
                callbackList.Columns.Add("ID", typeof(long));
                callbackList.Columns.Add("CustomerID", typeof(long));
                callbackList.Columns.Add("CreateUserID", typeof(long));
                callbackList.Columns.Add("CreateTime", typeof(DateTime));
                callbackList.Columns.Add("CategoryID", typeof(long));
                callbackList.Columns.Add("Name", typeof(string));
                callbackList.Columns.Add("UserID", typeof(long));
                callbackList.Columns.Add("TaskTime", typeof(DateTime));
                callbackList.Columns.Add("Status", typeof(int));
                callbackList.Columns.Add("HospitalID", typeof(long));
                callbackList.Columns.Add("Custom10", typeof(string));

                DataTransferCommon categoryTemp = null;
                DataTransferCommon customerTemp = null;
                DateTime createTime;
                DateTime taskTime;
                DataTransferCommon createUserTemp = null;
                DataTransferCommon taskUserTemp = null;
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    #region
                    if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 1].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    if (worksheet.Cells[row, 2].Value == null || worksheet.Cells[row, 2].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 2].Value = "超级管理员";
                        //result.Message = "第" + row + "行回访计划创建人不能为空！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行回访计划创建时间不能为空！");
                    }

                    if (worksheet.Cells[row, 4].Value == null || worksheet.Cells[row, 4].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 4].Value = "其他";
                        //throw new Exception("第" + row + "行回访类型不能为空！");
                    }
                    if (worksheet.Cells[row, 5].Value == null || worksheet.Cells[row, 5].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 5].Value = "超级管理员";
                        //result.Message = "第" + row + "行提醒人不能为空！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 6].Value == null || worksheet.Cells[row, 6].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行提醒时间不能为空！");
                    }
                    if (worksheet.Cells[row, 7].Value == null || worksheet.Cells[row, 7].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 7].Value = "";
                        //result.Message = "第" + row + "行提醒内容不能为空！";
                        //return result;
                    }
                    #endregion

                    #region
                    customerTemp = new DataTransferCommon()
                    {
                        ID = 0
                    };

                    createUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 2].Value.ToString().Trim()).FirstOrDefault();
                    if (createUserTemp == null)
                    {
                        createUserTemp = new DataTransferCommon()
                        {
                            ID = 1
                        };
                    }
                    if (!DateTime.TryParse(worksheet.Cells[row, 3].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行回访创建时间异常！");
                    }
                    categoryTemp = callbackCategoryList.Where(u => u.Name == worksheet.Cells[row, 4].Value.ToString().Trim()).FirstOrDefault();

                    //****************************************************************************************************************************************************************************************************************************
                    if (categoryTemp == null)
                    {
                        categoryTemp = new DataTransferCommon()
                        {
                            ID = 10001
                        };
                    }

                    taskUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();
                    if (taskUserTemp == null)
                    {
                        taskUserTemp = new DataTransferCommon()
                        {
                            ID = 1
                        };
                    }
                    if (!DateTime.TryParse(worksheet.Cells[row, 6].Value.ToString().Trim(), out taskTime))
                    {
                        throw new Exception("第" + row + "行提醒时间异常！");
                    }

                    #endregion



                    DataRow dr = callbackList.NewRow();
                    dr["ID"] = SingleIdWork.Instance(1).nextId();
                    //dr["CustomerID"] = new Random().Next(958266, 1430913);
                    dr["CustomerID"] = customerTemp.ID;
                    dr["CreateUserID"] = createUserTemp.ID;
                    dr["CreateTime"] = createTime;
                    dr["CategoryID"] = categoryTemp.ID;
                    if (worksheet.Cells[row, 7].Value == null)
                    {
                        dr["Name"] = "";
                    }
                    else
                    {

                        if (worksheet.Cells[row, 7].Value.ToString().Trim().Length > 50)
                        {
                            dr["Name"] = worksheet.Cells[row, 7].Value.ToString().Trim().Substring(0, 49);
                        }
                        else
                        {
                            dr["Name"] = worksheet.Cells[row, 7].Value.ToString().Trim();
                        }

                        if (dr["Name"].ToString() == "NULL")
                        {
                            dr["Name"] = "";
                        }
                    }
                    dr["UserID"] = taskUserTemp.ID;
                    dr["TaskTime"] = taskTime;
                    dr["Status"] = 0;
                    dr["HospitalID"] = 1;
                    dr["Custom10"] = worksheet.Cells[row, 1].Value.ToString().Trim();

                    callbackList.Rows.Add(dr);
                }

                ///导入数据库
                if (callbackList.Rows.Count > 0)
                {
                    _connection.Execute(@"ALTER TABLE [SmartCallback] ADD [Custom10] nvarchar(255)", null, _transaction);
                    SqlBulkCopyByDataTable("SmartCallback", callbackList);
                    _connection.Execute(@"update SmartCallback set CustomerID=b.ID from SmartCallback a,SmartCustomer b where a.Custom10=b.Custom10", null, _transaction);

                    _connection.Execute(@"delete from SmartCallback where CustomerID=0", null, _transaction);
                }
            }

            Console.WriteLine("回访计划记录结束迁移");
        }


        /// <summary>
        /// 回访记录
        /// </summary>
        public static void CallBack()
        {
            Console.WriteLine("回访记录开始迁移");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\CallBack.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                IEnumerable<DataTransferCommon> customerList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> toolList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> callbackCategoryList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                //customerList = _connection.Query<DataTransferCommon>(@"select Mobile as Name,ID from [SmartCustomer]");
                toolList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartTool]", null, _transaction);
                callbackCategoryList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartCallbackCategory]", null, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID order by Status desc", new { HospitalID = 1 }, _transaction);

                DataTable callbackList = new DataTable("SmartCallback");
                callbackList.Columns.Add("ID", typeof(long));
                callbackList.Columns.Add("CustomerID", typeof(long));
                callbackList.Columns.Add("CreateUserID", typeof(long));
                callbackList.Columns.Add("CreateTime", typeof(DateTime));
                callbackList.Columns.Add("Tool", typeof(long));
                callbackList.Columns.Add("Content", typeof(string));
                callbackList.Columns.Add("CategoryID", typeof(long));
                callbackList.Columns.Add("Name", typeof(string));
                callbackList.Columns.Add("UserID", typeof(long));
                callbackList.Columns.Add("TaskTime", typeof(DateTime));
                callbackList.Columns.Add("TaskCreateTime", typeof(DateTime));
                callbackList.Columns.Add("TaskCreateUserID", typeof(long));
                callbackList.Columns.Add("Status", typeof(int));
                callbackList.Columns.Add("HospitalID", typeof(long));
                callbackList.Columns.Add("Custom10", typeof(string));

                DataTransferCommon categoryTemp = null;
                DataTransferCommon toolTemp = null;
                DataTransferCommon customerTemp = null;
                DateTime createTime;
                DateTime taskTime;
                DateTime taskCreateTime;
                DataTransferCommon createUserTemp = null;
                DataTransferCommon taskUserTemp = null;
                DataTransferCommon taskCreateUserTemp = null;
                string name = "";
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    #region
                    if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 1].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    if (worksheet.Cells[row, 2].Value == null || worksheet.Cells[row, 2].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 2].Value = "超级管理员";
                        //result.Message = "第" + row + "行回访计划创建人不能为空！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行回访计划创建时间不能为空！");
                    }
                    if (worksheet.Cells[row, 4].Value == null || worksheet.Cells[row, 4].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 4].Value = "电话";
                        //result.Message = "第" + row + "行沟通工具不能为空！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 5].Value == null || worksheet.Cells[row, 5].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 5].Value = "其他";
                    }
                    if (worksheet.Cells[row, 7].Value == null || worksheet.Cells[row, 7].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 7].Value = "超级管理员";
                        //result.Message = "第" + row + "行提醒人不能为空！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 8].Value == null || worksheet.Cells[row, 8].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行提醒时间不能为空！");
                    }
                    if (worksheet.Cells[row, 9].Value == null || worksheet.Cells[row, 9].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        //throw new Exception("第" + row + "行回访内容不能为空！");
                    }
                    if (worksheet.Cells[row, 11].Value == null || worksheet.Cells[row, 11].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行回访时间不能为空！");
                    }
                    if (worksheet.Cells[row, 10].Value == null || worksheet.Cells[row, 10].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 10].Value = "超级管理员";
                        //result.Message = "第" + row + "行回访人不能为空！";
                        //return result;
                    }
                    #endregion

                    #region
                    customerTemp = new DataTransferCommon() { ID = 0 };


                    createUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 2].Value.ToString().Trim()).FirstOrDefault();
                    if (createUserTemp == null)
                    {
                        createUserTemp = new DataTransferCommon()
                        {
                            ID = 1,
                        };
                        //result.Message = "第" + row + "行创建人不存在！";
                        //return result;
                    }
                    if (!DateTime.TryParse(worksheet.Cells[row, 3].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行回访创建时间异常！");
                    }
                    toolTemp = toolList.Where(u => u.Name == worksheet.Cells[row, 4].Value.ToString().Trim()).FirstOrDefault();
                    if (toolTemp == null)
                    {
                        throw new Exception("第" + row + "行沟通工具不存在！");
                    }
                    categoryTemp = callbackCategoryList.Where(u => u.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();
                    //***************************************************************************************************************************************************************************
                    if (categoryTemp == null)
                    {

                        categoryTemp = new DataTransferCommon()
                        {
                            ID = 10001,
                        };
                        //result.Message = "第" + row + "行回访类型不存在！";
                        //return result;
                    }

                    if (worksheet.Cells[row, 6].Value == null)
                    {
                        name = "";
                    }
                    else
                    {
                        if (worksheet.Cells[row, 6].Value.ToString().Trim().Length > 50)
                        {
                            name = worksheet.Cells[row, 6].Value.ToString().Trim().Substring(0, 49);
                        }
                        else
                        {
                            name = worksheet.Cells[row, 6].Value.ToString().Trim();
                        }
                        if (name == "NULL")
                        {
                            name = "";
                        }

                    }

                    taskUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 7].Value.ToString().Trim()).FirstOrDefault();
                    if (taskUserTemp == null)
                    {
                        taskUserTemp = new DataTransferCommon()
                        {
                            ID = 1,
                        };
                        //result.Message = "第" + row + "行提醒人不存在！";
                        //return result;
                    }
                    if (!DateTime.TryParse(worksheet.Cells[row, 8].Value.ToString().Trim(), out taskTime))
                    {
                        throw new Exception("第" + row + "行提醒时间异常！");
                    }

                    taskCreateUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 10].Value.ToString().Trim()).FirstOrDefault();
                    if (taskCreateUserTemp == null)
                    {
                        taskCreateUserTemp = new DataTransferCommon()
                        {
                            ID = 1,
                        };
                        //result.Message = "第" + row + "行回访人不存在！";
                        //return result;
                    }
                    if (!DateTime.TryParse(worksheet.Cells[row, 11].Value.ToString().Trim(), out taskCreateTime))
                    {
                        throw new Exception("第" + row + "行回访时间异常！");
                    }

                    #endregion



                    DataRow dr = callbackList.NewRow();
                    dr["ID"] = SingleIdWork.Instance(1).nextId();
                    //dr["CustomerID"] = new Random().Next(958266,1430913);
                    dr["CustomerID"] = customerTemp.ID;
                    dr["CreateUserID"] = createUserTemp.ID;
                    dr["CreateTime"] = createTime;
                    dr["Tool"] = toolTemp.ID;
                    if (worksheet.Cells[row, 9].Value == null)
                    {
                        dr["Content"] = "";
                    }
                    else
                    {
                        if (worksheet.Cells[row, 9].Value.ToString().Trim().Length > 500)
                        {
                            dr["Content"] = worksheet.Cells[row, 9].Value.ToString().Trim().Substring(0, 499);
                        }
                        else
                        {
                            dr["Content"] = worksheet.Cells[row, 9].Value.ToString().Trim();
                        }

                        if (dr["Content"].ToString() == "NULL")
                        {
                            dr["Content"] = "";
                        }
                    }
                    dr["CategoryID"] = categoryTemp.ID;
                    dr["Name"] = name;
                    dr["UserID"] = taskUserTemp.ID;
                    dr["TaskTime"] = taskTime;
                    dr["TaskCreateTime"] = taskCreateTime;
                    dr["TaskCreateUserID"] = taskCreateUserTemp.ID;
                    dr["Status"] = 1;
                    dr["HospitalID"] = 1;
                    dr["Custom10"] = worksheet.Cells[row, 1].Value.ToString().Trim();

                    callbackList.Rows.Add(dr);
                }

                ///导入数据库
                if (callbackList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartCallback", callbackList);
                }

                //1、更新首次回访时间、最后回访时间、回访次数
                _connection.Execute(@"update SmartCallback set CustomerID=b.ID from SmartCallback a,SmartCustomer b where a.Custom10=b.Custom10", null, _transaction);

                _connection.Execute(@"update SmartCustomer set FirstCallbackTime=min,LastCallbackTime=max,CallbackTime=count 
  from SmartCustomer a
  inner join (select CustomerID,MIN(TaskCreateTime) as min,MAX(TaskCreateTime) as max,count(CustomerID) as count 
  from SmartCallback where Status=1 group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);

                _connection.Execute(@"update SmartCallback set CustomerID=b.ID 
from SmartCallback a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);
                _connection.Execute(@"delete from SmartCallback where CustomerID=0", null, _transaction);
                _connection.Execute(@"ALTER TABLE [SmartCallback] DROP COLUMN [Custom10]", null, _transaction);
            }

            Console.WriteLine("回访记录结束迁移");
        }


        /// <summary>
        /// 上门记录
        /// </summary>
        public static void Visit()
        {
            Console.WriteLine("上门记录开始迁移");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\分诊情况统计.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                IEnumerable<DataTransferCommon> customerList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> deptList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> symptomList = new List<DataTransferCommon>();
                customerList = _connection.Query<DataTransferCommon>(@"select Custom10 as Name,ID from [SmartCustomer]", null, _transaction);
                deptList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartDept]", null, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID order by Status desc", new { HospitalID = 1 }, _transaction);
                symptomList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartSymptom]", null, _transaction);

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable visitList = new DataTable("SmartVisit");
                visitList.Columns.Add("CustomerID", typeof(long));
                visitList.Columns.Add("CreateUserID", typeof(long));
                visitList.Columns.Add("CreateTime", typeof(DateTime));
                visitList.Columns.Add("HospitalID", typeof(long));
                visitList.Columns.Add("VisitType", typeof(int));
                visitList.Columns.Add("DeptID", typeof(long));
                visitList.Columns.Add("UserID", typeof(long));
                visitList.Columns.Add("IsConsume", typeof(int));
                visitList.Columns.Add("CurrentExploitUserID", typeof(long));
                visitList.Columns.Add("CurrentManagerUserID", typeof(long));
                visitList.Columns.Add("Custom10", typeof(string));
                visitList.Columns.Add("TodaySymptomID", typeof(long));


                DataTable userVisitList = new DataTable("SmartTriage");
                userVisitList.Columns.Add("CustomerID", typeof(long));
                userVisitList.Columns.Add("CreateUserID", typeof(long));
                userVisitList.Columns.Add("CreateTime", typeof(DateTime));
                userVisitList.Columns.Add("AssignUserID", typeof(long));
                userVisitList.Columns.Add("Remark", typeof(string));
                userVisitList.Columns.Add("HospitalID", typeof(long));
                userVisitList.Columns.Add("VisitType", typeof(int));
                userVisitList.Columns.Add("CurrentExploitUserID", typeof(long));
                userVisitList.Columns.Add("CurrentManagerUserID", typeof(long));

                DataTable deptVisitList = new DataTable("SmartDeptVisit");
                deptVisitList.Columns.Add("CustomerID", typeof(long));
                deptVisitList.Columns.Add("CreateUserID", typeof(long));
                deptVisitList.Columns.Add("CreateTime", typeof(DateTime));
                deptVisitList.Columns.Add("DeptID", typeof(long));
                deptVisitList.Columns.Add("HospitalID", typeof(long));
                deptVisitList.Columns.Add("VisitType", typeof(int));
                deptVisitList.Columns.Add("CurrentExploitUserID", typeof(long));
                deptVisitList.Columns.Add("CurrentManagerUserID", typeof(long));


                DataTransferCommon deptTemp = null;
                DataTransferCommon customerTemp = null;
                DataTransferCommon symptomTemp = null;
                DateTime createTime;
                VisitType? visitType;
                int dealType;
                DataTransferCommon createUserTemp = null;
                DataTransferCommon userTemp = null;
                DataTransferCommon exploitTemp = null;
                DataTransferCommon managerTemp = null;
                for (int row = 2; row <= rowCount; row++)
                {
                    userTemp = null;
                    exploitTemp = null;
                    managerTemp = null;
                    deptTemp = null;
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    #region
                    if (worksheet.Cells[row, 4].Value == null || worksheet.Cells[row, 4].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    if (worksheet.Cells[row, 16].Value == null || worksheet.Cells[row, 16].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 16].Value = "超级管理员";
                    }
                    if (worksheet.Cells[row, 10].Value == null || worksheet.Cells[row, 10].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行分诊时间不能为空！");
                    }

                    if (worksheet.Cells[row, 24].Value == null || worksheet.Cells[row, 24].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行到诊类型不能为空！");
                    }
                    if (worksheet.Cells[row, 25].Value == null || worksheet.Cells[row, 25].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行当日是否成交不能为空！");
                    }

                    if (worksheet.Cells[row, 33].Value == null || worksheet.Cells[row, 33].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 33].Value = "其它";
                    }
                    #endregion

                    #region
                    symptomTemp = symptomList.Where(u => u.Name == worksheet.Cells[row, 33].Value.ToString().Trim()).FirstOrDefault();
                    if (symptomTemp == null)
                    {
                        //symptomTemp = symptomList.Where(u => u.Name == "其它").FirstOrDefault();
                        if (symptomTemp == null)
                        {
                            //throw new Exception("第" + row + "行症状不存在！");
                            symptomTemp = new DataTransferCommon()
                            {
                                ID = 15796086509306880
                            };
                        }
                    }

                    createUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 16].Value.ToString().Trim()).FirstOrDefault();
                    if (createUserTemp == null)
                    {
                        createUserTemp = new DataTransferCommon()
                        {
                            ID = 1,
                            Name = "超级管理员"
                        };
                        //result.Message = "第" + row + "行分诊人员不存在！";
                        //return result;
                    }
                    if (!DateTime.TryParse(worksheet.Cells[row, 10].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行分诊时间异常！");
                    }



                    visitType = TransferVisitType(worksheet.Cells[row, 24].Value.ToString().Trim());
                    if (visitType == null)
                    {
                        throw new Exception("第" + row + "行到诊状态异常！");
                    }

                    if (worksheet.Cells[row, 25].Value.ToString().Trim() == "未成交")
                    {
                        dealType = 0;
                    }
                    else
                    {
                        dealType = 1;
                    }
                    if (worksheet.Cells[row, 14].Value != null && !worksheet.Cells[row, 14].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        exploitTemp = userList.Where(u => u.Name == worksheet.Cells[row, 14].Value.ToString().Trim()).FirstOrDefault();
                        //if (exploitTemp == null)
                        //{
                        //    result.Message = "第" + row + "行归属网电不存在！";
                        //    return result;
                        //}
                    }
                    if (worksheet.Cells[row, 15].Value != null && !worksheet.Cells[row, 15].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        managerTemp = userList.Where(u => u.Name == worksheet.Cells[row, 15].Value.ToString().Trim()).FirstOrDefault();
                        //if (managerTemp == null)
                        //{
                        //    result.Message = "第" + row + "行归属现场不存在！";
                        //    return result;
                        //}
                    }

                    #endregion


                    DataRow dr = visitList.NewRow();
                    //dr["CustomerID"] = new Random().Next(958266, 1430913);
                    dr["CustomerID"] = 0;
                    dr["CreateUserID"] = createUserTemp.ID;
                    dr["CreateTime"] = createTime;
                    dr["HospitalID"] = 1;
                    dr["VisitType"] = visitType.Value.CastTo<int>();
                    dr["Custom10"] = worksheet.Cells[row, 4].Value.ToString().Trim();


                    dr["IsConsume"] = dealType;
                    if (exploitTemp != null)
                    {
                        dr["CurrentExploitUserID"] = exploitTemp.ID;
                    }
                    if (managerTemp != null)
                    {
                        dr["CurrentManagerUserID"] = managerTemp.ID;
                    }

                    dr["TodaySymptomID"] = symptomTemp.ID;

                    visitList.Rows.Add(dr);
                }

                ///导入数据库
                _connection.Execute(@"ALTER TABLE [SmartVisit] ADD [Custom10] nvarchar(255)", null, _transaction);

                if (visitList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartVisit", visitList);
                }
                if (userVisitList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartTriage", userVisitList);
                }
                if (deptVisitList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartDeptVisit", deptVisitList);
                }

                _connection.Execute(@"update SmartVisit set CustomerID=b.ID 
from SmartVisit a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);

                //1、更新顾客首次上门时间、最后上门时间、上门次数
                _connection.Execute(@"update SmartCustomer set FirstVisitTime=min,LastVisitTime=max,VisitTimes=count 
  from SmartCustomer a
  inner join (select CustomerID,MIN(createtime) as min,MAX(CreateTime) as max,count(CustomerID) as count 
  from SmartVisit group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);
                //2、更新首次上门医院
                _connection.Execute(@"update SmartCustomer set FirstVisitHospitalID=b.HospitalID
  from SmartCustomer a,SmartVisit b where a.ID=b.CustomerID and a.FirstVisitTime=b.CreateTime", null, _transaction);
                //3、更新最后上门医院
                _connection.Execute(@"update SmartCustomer set LastVisitHospitalID=b.HospitalID
  from SmartCustomer a,SmartVisit b where a.ID=b.CustomerID and a.LastVisitTime=b.CreateTime", null, _transaction);



                _connection.Execute(@"delete from SmartVisit where CustomerID=0", null, _transaction);
                _connection.Execute(@"ALTER TABLE [SmartVisit] DROP COLUMN [Custom10]", null, _transaction);

            }

            Console.WriteLine("上门记录结束迁移");
        }


        /// <summary>
        /// 优惠券
        /// </summary>
        public static void Coupon()
        {
            Console.WriteLine("优惠券记录开始迁移");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\朔州丽都\\消费券情况明细表.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                IEnumerable<DataTransferCommon> customerList = new List<DataTransferCommon>();
                List<DataTransferCommon> couponCategoryList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                //customerList = _connection.Query<DataTransferCommon>(@"select Custom10 as Name,ID from [SmartCustomer]");
                couponCategoryList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartCouponCategory]", null, _transaction).ToList();
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID", new { HospitalID = 1 }, _transaction);

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable visitList = new DataTable("SmartCoupon");
                visitList.Columns.Add("ID", typeof(long));
                visitList.Columns.Add("CustomerID", typeof(long));
                visitList.Columns.Add("CreateUserID", typeof(long));
                visitList.Columns.Add("CreateTime", typeof(DateTime));
                visitList.Columns.Add("HospitalID", typeof(long));
                visitList.Columns.Add("Access", typeof(int));
                visitList.Columns.Add("CategoryID", typeof(long));
                visitList.Columns.Add("Amount", typeof(decimal));
                visitList.Columns.Add("Rest", typeof(decimal));
                visitList.Columns.Add("Remark", typeof(string));
                visitList.Columns.Add("ExpirationDate", typeof(DateTime));
                visitList.Columns.Add("Custom10", typeof(string));


                DataTransferCommon couponCategoryTemp = null;
                DataTransferCommon customerTemp = null;
                decimal amount = 0;
                string remark = "";
                DateTime now = DateTime.Now;
                DateTime expirationDate = DateTime.MaxValue.Date;
                List<CouponCategoryInfo> couponAddList = new List<CouponCategoryInfo>();
                for (int row = 2; row <= rowCount; row++)
                {
                    expirationDate = DateTime.MaxValue.Date;
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    #region
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    if (worksheet.Cells[row, 6].Value == null || worksheet.Cells[row, 6].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 6].Value = "通用代金券";
                    }
                    #endregion

                    #region
                    //customerTemp = customerList.AsParallel().Where(u => u.Name == worksheet.Cells[row, 1].Value.ToString().Trim()).FirstOrDefault();
                    //if (customerTemp == null)
                    //{
                    //    result.Message = "第" + row + "行该顾客手机号不存在！";
                    //    return result;
                    //}

                    couponCategoryTemp = couponCategoryList.Where(u => u.Name == worksheet.Cells[row, 6].Value.ToString().Trim()).FirstOrDefault();
                    if (couponCategoryTemp == null)
                    {
                        couponCategoryTemp = new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId()
                        };
                        couponCategoryList.Add(new DataTransferCommon()
                        {
                            ID = couponCategoryTemp.ID,
                            Name = worksheet.Cells[row, 6].Value.ToString().Trim()
                        });
                        couponAddList.Add(new CouponCategoryInfo()
                        {
                            ID = couponCategoryTemp.ID,
                            Name = worksheet.Cells[row, 6].Value.ToString().Trim(),
                            Remark = "数据迁移补录",
                            ScopeLimit = 1,
                            Status = CommonStatus.Use,
                            TimeLimit = 1,
                            ChargeID = 0,
                            ChargeCategoryID = 0
                        });
                        //result.Message = "第" + row + "行代金券类型不存在！";
                        //return result;
                    }
                    if (!decimal.TryParse(worksheet.Cells[row, 9].Value.ToString().Trim(), out amount))
                    {
                        throw new Exception("第" + row + "行剩余余额异常！");
                    }
                    if (amount <= 0)
                    {
                        throw new Exception("第" + row + "行剩余余额不能小于等于0！");
                    }

                    if (worksheet.Cells[row, 10].Value != null && worksheet.Cells[row, 10].Value.ToString().Trim() != "")
                    {
                        if (!DateTime.TryParse(worksheet.Cells[row, 10].Value.ToString().Trim(), out expirationDate))
                        {
                            throw new Exception("第" + row + "行过期日期异常！");
                        }
                    }

                    if (worksheet.Cells[row, 13].Value != null)
                    {
                        remark = worksheet.Cells[row, 13].Value.ToString().Trim();
                    }

                    if (remark.IsNullOrEmpty())
                    {
                        remark = "数据迁移获取";
                    }
                    if (remark.Length > 50)
                    {
                        remark = remark.Substring(0, 49);
                    }
                    #endregion


                    DataRow dr = visitList.NewRow();
                    dr["ID"] = SingleIdWork.Instance(1).nextId();
                    //dr["CustomerID"] = new Random().Next(958266, 1430913);
                    dr["CustomerID"] = 0;
                    dr["CreateUserID"] = 1;
                    dr["CreateTime"] = DateTime.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
                    dr["HospitalID"] = 1;
                    dr["Access"] = 10;
                    dr["CategoryID"] = couponCategoryTemp.ID;
                    dr["Amount"] = amount;
                    dr["Rest"] = amount;
                    dr["Remark"] = remark;
                    dr["ExpirationDate"] = expirationDate.Date;
                    dr["Custom10"] = worksheet.Cells[row, 3].Value.ToString().Trim();
                    visitList.Rows.Add(dr);
                }

                ///导入数据库

                if (couponAddList.Count() > 0)
                {
                    _connection.Execute(@"insert into SmartCouponCategory(ID,Name,ScopeLimit,ChargeID,ChargeCategoryID,TimeLimit,Remark,Status) 
                        VALUES(@ID, @Name, @ScopeLimit, @ChargeID, @ChargeCategoryID, @TimeLimit, @Remark, @Status)", couponAddList, _transaction);

                    _connection.Execute("insert into SmartCouponCategoryHospital(ID,CouponCategoryID,HospitalID) VALUES(@ID, @CouponCategoryID, @HospitalID)",
                        couponAddList.Select(u => new { ID = SingleIdWork.Instance(1).nextId(), CouponCategoryID = u.ID, HospitalID = 1 }), _transaction);
                }

                _connection.Execute(@"ALTER TABLE [SmartCoupon] ADD [Custom10] nvarchar(255)", null, _transaction);


                if (visitList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartCoupon", visitList);
                }

                _connection.Execute(@"update SmartCoupon set CustomerID=b.ID 
from SmartCoupon a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);

                _connection.Execute(@"update SmartCustomer set Coupon=CouponAmount
  from SmartCustomer a
  inner join (select CustomerID,sum(Rest) as CouponAmount 
  from SmartCoupon where ExpirationDate>=@Date group by CustomerID) as b on a.ID=b.CustomerID", new { Date = DateTime.Today }, _transaction);

                _connection.Execute(@"delete from SmartCoupon where CustomerID=0", null, _transaction);
                _connection.Execute(@"ALTER TABLE [SmartCoupon] DROP COLUMN [Custom10]", null, _transaction);
            }

            Console.WriteLine("优惠券记录结束迁移");
        }


        /// <summary>
        /// 预收款
        /// </summary>
        public static void Deposit()
        {
            Console.WriteLine("预收款记录开始迁移");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\客户定金和项目结余情况.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                IEnumerable<DataTransferCommon> customerList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> depositCategoryList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                customerList = _connection.Query<DataTransferCommon>(@"select Custom10 as Name,ID from [SmartCustomer]", null, _transaction);
                depositCategoryList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartDepositCharge]", null, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID order by Status desc", new { HospitalID = 1 }, _transaction);

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable visitList = new DataTable("SmartDeposit");
                visitList.Columns.Add("ID", typeof(long));
                visitList.Columns.Add("CustomerID", typeof(long));
                visitList.Columns.Add("CreateUserID", typeof(long));
                visitList.Columns.Add("CreateTime", typeof(DateTime));
                visitList.Columns.Add("HospitalID", typeof(long));
                visitList.Columns.Add("Access", typeof(int));
                visitList.Columns.Add("ChargeID", typeof(long));
                visitList.Columns.Add("Amount", typeof(decimal));
                visitList.Columns.Add("Rest", typeof(decimal));
                visitList.Columns.Add("Remark", typeof(string));
                visitList.Columns.Add("BuyExploitUserID", typeof(long));
                visitList.Columns.Add("BuyManagerUserID", typeof(long));
                visitList.Columns.Add("BuyOrderUserID", typeof(long));
                visitList.Columns.Add("BuyVisitType", typeof(int));
                visitList.Columns.Add("Custom10", typeof(string));


                DataTransferCommon depositCategoryTemp = null;
                DataTransferCommon customerTemp = null;
                VisitType? visitType;
                decimal amount = 0;
                DataTransferCommon orderUserTemp = null;
                DataTransferCommon exploitTemp = null;
                DataTransferCommon managerTemp = null;
                string remark = "";
                DateTime now = DateTime.Now;
                for (int row = 2; row <= rowCount; row++)
                {
                    orderUserTemp = null;
                    exploitTemp = null;
                    managerTemp = null;
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    #region
                    if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 1].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    if (worksheet.Cells[row, 2].Value == null || worksheet.Cells[row, 2].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 2].Value = "通用预收款";
                    }
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行剩余余额不能为空！");
                    }
                    #endregion

                    #region
                    //customerTemp = customerList.AsParallel().Where(u => u.Name == worksheet.Cells[row, 1].Value.ToString().Trim()).FirstOrDefault();
                    //if (customerTemp == null)
                    //{
                    //    result.Message = "第" + row + "行该顾客手机号不存在！";
                    //    return result;
                    //}

                    depositCategoryTemp = depositCategoryList.Where(u => u.Name == worksheet.Cells[row, 2].Value.ToString().Trim()).FirstOrDefault();
                    if (depositCategoryTemp == null)
                    {
                        throw new Exception("第" + row + "行预收款类型不存在！");
                    }
                    if (!decimal.TryParse(worksheet.Cells[row, 3].Value.ToString().Trim(), out amount))
                    {
                        throw new Exception("第" + row + "行剩余余额异常！");
                    }
                    if (amount <= 0)
                    {
                        throw new Exception("第" + row + "行剩余余额不能小于等于0！");
                    }
                    if (worksheet.Cells[row, 4].Value != null && !worksheet.Cells[row, 4].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        exploitTemp = userList.Where(u => u.Name == worksheet.Cells[row, 4].Value.ToString().Trim()).FirstOrDefault();
                        if (exploitTemp == null)
                        {
                            //throw new Exception("第" + row + "行归属网电不存在！");
                        }
                    }
                    if (worksheet.Cells[row, 5].Value != null && !worksheet.Cells[row, 5].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        managerTemp = userList.Where(u => u.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();
                        if (managerTemp == null)
                        {
                            //throw new Exception()
                            //result.Message = "第" + row + "行归属现场不存在！";
                            //return result;
                        }
                    }
                    if (worksheet.Cells[row, 6].Value != null && !worksheet.Cells[row, 6].Value.ToString().Trim().IsNullOrEmpty())
                    {
                    }

                    visitType = TransferVisitType("再消费");
                    if (visitType == null)
                    {
                        throw new Exception("第" + row + "行到诊状态异常！");
                    }
                    if (worksheet.Cells[row, 8].Value != null)
                    {
                        remark = worksheet.Cells[row, 8].Value.ToString().Trim();
                    }

                    if (remark.IsNullOrEmpty())
                    {
                        remark = "数据迁移获取";
                    }
                    if (remark.Length > 50)
                    {
                        remark = remark.Substring(0, 49);
                    }
                    #endregion


                    DataRow dr = visitList.NewRow();
                    dr["ID"] = SingleIdWork.Instance(1).nextId();
                    //dr["CustomerID"] = new Random().Next(958266, 1430913);
                    dr["CustomerID"] = 0;
                    dr["CreateUserID"] = 1;
                    dr["CreateTime"] = now;
                    dr["HospitalID"] = 1;
                    dr["Access"] = 6;
                    dr["ChargeID"] = depositCategoryTemp.ID;
                    dr["Amount"] = amount;
                    dr["Rest"] = amount;
                    dr["Remark"] = remark;
                    dr["Custom10"] = worksheet.Cells[row, 1].Value.ToString().Trim();
                    if (exploitTemp != null)
                    {
                        dr["BuyExploitUserID"] = exploitTemp.ID;
                    }
                    if (managerTemp != null)
                    {
                        dr["BuyManagerUserID"] = managerTemp.ID;
                    }
                    dr["BuyOrderUserID"] = 1;
                    dr["BuyVisitType"] = visitType.Value.CastTo<int>();

                    visitList.Rows.Add(dr);
                }

                ///导入数据库
                _connection.Execute(@"ALTER TABLE [SmartDeposit] ADD [Custom10] nvarchar(255)", null, _transaction);

                if (visitList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartDeposit", visitList);
                }

                _connection.Execute(@"update SmartDeposit set CustomerID=b.ID 
from SmartDeposit a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);
                _connection.Execute(@"update SmartCustomer set Deposit=DepositAmount
  from SmartCustomer a
  inner join (select CustomerID,sum(Rest) as DepositAmount 
  from SmartDeposit group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);

                _connection.Execute(@"delete from SmartDeposit where CustomerID=0", null, _transaction);
                _connection.Execute(@"ALTER TABLE [SmartDeposit] DROP COLUMN [Custom10]", null, _transaction);
            }

            Console.WriteLine("预收款记录结束迁移");
        }


        /// <summary>
        /// 订单
        /// </summary>
        public static void Order()
        {
            Console.WriteLine("订单记录开始迁移");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\客户订购项目情况.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                List<DataTransferCommon> chargeList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> chargeSetList = new List<DataTransferCommon>();
                //customerList = _connection.Query<DataTransferCommon>(@"select Custom10 as Name,ID from [SmartCustomer]");
                chargeList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartCharge]  order by Status desc", null, _transaction).ToList();
                chargeSetList = _connection.Query<DataTransferCommon>(@"select ID,[Name],Price from [SmartChargeSet]  order by Status desc", new { HospitalID = 1 }, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID order by Status desc", new { HospitalID = 1 }, _transaction);

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;



                DataTransferCommon chargeTemp = null;
                DataTransferCommon chargeSetTemp = null;
                //DataTransferCommon customerTemp = null;
                VisitType? visitType;
                DataTransferCommon createUserTemp = null;
                DataTransferCommon exploitTemp = null;
                DataTransferCommon managerTemp = null;
                string remark = "";
                DateTime createTime;
                DateTime expirationDate;
                Dictionary<int, List<DataTransferOrder>> list = new Dictionary<int, List<DataTransferOrder>>();
                int chargeSetNum = 0;
                int num = 1;
                int restNum = 0;
                decimal originAmount;
                decimal amount;
                decimal cashAmount;
                decimal cardAmount;
                decimal depositAmount;
                decimal couponAmount;
                decimal debtAmount;
                int orderID = 200000;

                List<object> chargeAddList = new List<object>();
                //List<object> customerAddList = new List<object>();
                for (int row = 3; row <= rowCount; row++)
                {
                    exploitTemp = null;
                    managerTemp = null;
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }


                    #region
                    //if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 1].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    result.Message = "第" + row + "行订单编号不能为空！";
                    //    return result;
                    //}
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行下单时间不能为空！");
                    }
                    if (worksheet.Cells[row, 21].Value == null || worksheet.Cells[row, 21].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 21].Value = "超级管理员";
                        //result.Message = "第" + row + "行下单人不能为空！";
                        //return result;
                    }
                    if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 1].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    if (worksheet.Cells[row, 4].Value == null || worksheet.Cells[row, 4].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行项目名称不能为空！");
                    }
                    if (worksheet.Cells[row, 5].Value == null || worksheet.Cells[row, 5].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行项目数量不能为空！");
                    }
                    if (worksheet.Cells[row, 11].Value == null || worksheet.Cells[row, 11].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行剩余数量不能为空！");
                    }
                    if (worksheet.Cells[row, 7].Value == null || worksheet.Cells[row, 7].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行原始金额不能为空！");
                    }
                    if (worksheet.Cells[row, 7].Value == null || worksheet.Cells[row, 7].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行成交金额不能为空！");
                    }
                    if (worksheet.Cells[row, 15].Value == null || worksheet.Cells[row, 15].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行现金支付不能为空！");
                    }

                    #endregion

                    DataTransferOrder temp = new DataTransferOrder();
                    #region
                    if (!DateTime.TryParse(worksheet.Cells[row, 3].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行下单时间异常！");
                    }
                    temp.CreateTime = createTime.AddSeconds(1);


                    if (worksheet.Cells[row, 21].Value == null)
                    {
                        createUserTemp = new DataTransferCommon()
                        {
                            ID = 1
                        };
                    }
                    else
                    {
                        createUserTemp = userList.Where(u => u.Name == worksheet.Cells[row, 21].Value.ToString().Trim()).FirstOrDefault();
                        if (createUserTemp == null)
                        {
                            createUserTemp = new DataTransferCommon()
                            {
                                ID = 1
                            };
                            //result.Message = "第" + row + "行下单人不存在！";
                            //return result;
                        }
                    }

                    temp.CreateUserID = createUserTemp.ID;
                    temp.CustomerID = 0;
                    //customerTemp = customerList.AsParallel().Where(u => u.Name == worksheet.Cells[row, 4].Value.ToString().Trim()).FirstOrDefault();
                    //if (customerTemp == null)
                    //{
                    //    customerTemp = new DataTransferCommon()
                    //    {
                    //        ID = OrderAutoNumber.Instance().Number(OrderAutoNumber.customerStr),
                    //    };
                    //    customerAddList.Add(new
                    //    {
                    //        ID = customerTemp.ID,
                    //        Name = worksheet.Cells[row, 4].Value.ToString().Trim(),
                    //        Gender = 2,
                    //        Mobile = worksheet.Cells[row, 4].Value.ToString().Trim(),
                    //        CreateTime = createTime,
                    //        ChannelID = 14603201187021884,
                    //        CreateUserID = hospitalID,
                    //        CreateUserHospitalID = hospitalID,
                    //        Deposit = 0,
                    //        Coupon = 0,
                    //        Point = 0,
                    //        VisitTimes = 0,
                    //        ConsultTimes = 0,
                    //        MemberCategoryID = 0,
                    //        CashCardTotalAmount = 0,
                    //        Source = CustomerRegisterType.DataTransfer,
                    //        NewReward = CustomerNewRewardType.NotPaid,
                    //        Remark = "数据迁移",
                    //        HospitalID = hospitalID
                    //    });
                    //    //result.Message = "第" + row + "行该顾客手机号不存在！";
                    //    //return result;
                    //}
                    //temp.CustomerID = customerTemp.ID;

                    //if (worksheet.Cells[row, 5].Value != null && worksheet.Cells[row, 5].Value.ToString().Trim() != "")
                    //{
                    //    chargeSetTemp = chargeSetList.AsParallel().Where(u => u.Name == worksheet.Cells[row, 5].Value.ToString().Trim()).FirstOrDefault();
                    //    if (chargeSetTemp == null)
                    //    {
                    //        result.Message = "第" + row + "行套餐不存在！";
                    //        return result;
                    //    }
                    //    temp.SetID = chargeSetTemp.ID;
                    //    temp.SetPrice = chargeSetTemp.Price;
                    //    if (worksheet.Cells[row, 6].Value == null || worksheet.Cells[row, 6].Value.ToString().Trim().IsNullOrEmpty())
                    //    {
                    //        result.Message = "第" + row + "行套餐数量异常！";
                    //        return result;
                    //    }
                    //    if (!int.TryParse(worksheet.Cells[row, 6].Value.ToString().Trim(), out chargeSetNum))
                    //    {
                    //        result.Message = "第" + row + "行套餐数量异常！";
                    //        return result;
                    //    }
                    //    temp.SetNum = chargeSetNum;
                    //    temp.SetFinalPrice = temp.SetNum * temp.SetPrice;
                    //}

                    chargeTemp = chargeList.Where(u => u.Name.Trim() == worksheet.Cells[row, 4].Value.ToString().Trim().Trim()).FirstOrDefault();
                    if (chargeTemp == null)
                    {
                        //continue;
                        throw new Exception("第" + row + "行项目不存在！");
                        //********************************************************************************************************************************************************************************************
                        chargeTemp = new DataTransferCommon()
                        {
                            ID = 15553333419377664
                        };
                        //chargeTemp = new DataTransferCommon()
                        //{
                        //    ID = SingleIdWork.Instance(1).nextId()
                        //};

                        //chargeAddList.Add(new
                        //{
                        //    ID = chargeTemp.ID,
                        //    Name = worksheet.Cells[row, 7].Value.ToString().Trim(),
                        //    CategoryID = 10061,
                        //    PinYin = worksheet.Cells[row, 7].Value.ToString().Trim(),
                        //    Price = 0,
                        //    Status = 1,
                        //    Remark = "数据迁移自动创建",
                        //    UnitID = 10021,
                        //    Size = "",
                        //    ProductAdd = 1,
                        //    IsEvaluate = 1,
                        //    Type = 1
                        //});
                        //chargeList.Add(new DataTransferCommon()
                        //{
                        //    ID = chargeTemp.ID,
                        //    Name = worksheet.Cells[row, 7].Value.ToString().Trim()
                        //});
                        //result.Message = "第" + row + "行项目不存在！";
                        //return result;
                    }
                    temp.ChargeID = chargeTemp.ID;
                    if (!int.TryParse(worksheet.Cells[row, 5].Value.ToString().Trim(), out num))
                    {
                        throw new Exception("第" + row + "行项目购买数量异常！");
                    }
                    temp.Num = num;
                    if (!int.TryParse(worksheet.Cells[row, 11].Value.ToString().Trim(), out restNum))
                    {
                        throw new Exception("第" + row + "行项目剩余数量异常！");
                    }
                    temp.RestNum = restNum;
                    //if (worksheet.Cells[row, 10].Value != null && !worksheet.Cells[row, 10].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    if (!DateTime.TryParse(worksheet.Cells[row, 10].Value.ToString().Trim(), out expirationDate))
                    //    {
                    //        result.Message = "第" + row + "行过期时间异常！";
                    //        return result;
                    //    }
                    //    temp.ExpirationDate = expirationDate.Date;
                    //}

                    //if (worksheet.Cells[row, 20].Value.ToString().Trim() == "过期")
                    //{
                    //    if (worksheet.Cells[row, 7].Value.ToString().Trim().Contains("年卡"))
                    //    {
                    //        temp.ExpirationDate = temp.CreateTime.AddYears(1);
                    //    }
                    //    else if (worksheet.Cells[row, 7].Value.ToString().Trim().Contains("季卡"))
                    //    {
                    //        temp.ExpirationDate = temp.CreateTime.AddMonths(3);
                    //    }
                    //    else
                    //    {
                    //        temp.ExpirationDate = DateTime.Parse("2021-01-01");
                    //    }

                    //}
                    //else if (worksheet.Cells[row, 20].Value.ToString().Trim() == "退费")
                    //{
                    //    temp.RestNum = 0;
                    //}

                    if (!decimal.TryParse(worksheet.Cells[row, 7].Value.ToString().Trim(), out originAmount))
                    {
                        throw new Exception("第" + row + "行原始金额异常！");
                    }
                    temp.Price = originAmount;
                    if (!decimal.TryParse(worksheet.Cells[row, 7].Value.ToString().Trim(), out amount))
                    {
                        throw new Exception("第" + row + "行成交金额异常！");
                    }
                    temp.FinalPrice = amount;
                    if (!decimal.TryParse(worksheet.Cells[row, 15].Value.ToString().Trim(), out cashAmount))
                    {
                        throw new Exception("第" + row + "行现金支付异常！");
                    }
                    if (!decimal.TryParse(worksheet.Cells[row, 16].Value.ToString().Trim(), out cardAmount))
                    {
                        throw new Exception("第" + row + "行卡支付异常！");
                    }
                    temp.CashAmount = cashAmount + cardAmount;
                    if (!decimal.TryParse(worksheet.Cells[row, 17].Value.ToString().Trim(), out depositAmount))
                    {
                        throw new Exception("第" + row + "行预售款支付异常！");
                    }
                    temp.DepositAmount = depositAmount;
                    if (!decimal.TryParse(worksheet.Cells[row, 18].Value.ToString().Trim(), out couponAmount))
                    {
                        throw new Exception("第" + row + "行代金券支付异常！");
                    }
                    temp.CouponAmount = couponAmount;
                    //if (!decimal.TryParse(worksheet.Cells[row, 16].Value.ToString().Trim(), out commissionAmount))
                    //{
                    //    result.Message = "第" + row + "行佣金支付异常！";
                    //    return result;
                    //}
                    temp.CommissionAmount = 0;
                    if (!decimal.TryParse(worksheet.Cells[row, 19].Value.ToString().Trim(), out debtAmount))
                    {
                        throw new Exception("第" + row + "行剩余欠款异常！");
                    }
                    temp.DebtAmount = debtAmount;
                    if (temp.FinalPrice != temp.CashAmount + temp.DepositAmount + temp.CouponAmount + temp.CommissionAmount + temp.DebtAmount)
                    {
                        throw new Exception("第" + row + "行成交金额不等于现金+预收款+券+佣金+欠款！");
                        //temp.CashAmount = temp.FinalPrice - (temp.DepositAmount + temp.CouponAmount + temp.CommissionAmount + temp.DebtAmount);
                    }
                    visitType = TransferVisitType(worksheet.Cells[row, 24].Value.ToString().Trim());
                    if (visitType == null)
                    {
                        throw new Exception("第" + row + "行到诊状态异常！");
                    }
                    temp.VisitType = visitType.Value;

                    //if (worksheet.Cells[row, 19].Value != null && !worksheet.Cells[row, 19].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    exploitTemp = userList.Where(u => u.Name == worksheet.Cells[row, 19].Value.ToString().Trim()).FirstOrDefault();
                    //    if (exploitTemp == null)
                    //    {
                    //        result.Message = "第" + row + "行归属网电不存在！";
                    //        return result;
                    //    }
                    //    temp.ExploitUserID = exploitTemp.ID;
                    //}
                    if (worksheet.Cells[row, 21].Value != null && !worksheet.Cells[row, 21].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        managerTemp = userList.Where(u => u.Name == worksheet.Cells[row, 21].Value.ToString().Trim()).FirstOrDefault();
                        if (managerTemp == null)
                        {
                            //result.Message = "第" + row + "行归属现场不存在！";
                            //return result;
                        }
                        else
                        {
                            temp.ManagerUserID = managerTemp.ID;
                        }
                    }
                    temp.DealType = DealType.Yes;
                    remark = "数据迁移";

                    temp.Remark = remark;
                    temp.Custom10 = worksheet.Cells[row, 1].Value.ToString().Trim();
                    #endregion

                    orderID = 10000 + row;
                    if (list.ContainsKey(orderID))
                    {
                        list[orderID].Add(temp);
                    }
                    else
                    {
                        list.Add(orderID, new List<DataTransferOrder>() { temp });
                    }
                }

                if (list.Values.Count() > 0)
                {
                    #region datetable
                    DataTable orderList = new DataTable("SmartOrder");
                    orderList.Columns.Add("ID", typeof(long));
                    orderList.Columns.Add("CustomerID", typeof(long));
                    orderList.Columns.Add("CreateUserID", typeof(long));
                    orderList.Columns.Add("CreateTime", typeof(DateTime));
                    orderList.Columns.Add("TotalPrice", typeof(decimal));
                    orderList.Columns.Add("FinalPrice", typeof(decimal));
                    orderList.Columns.Add("PaidStatus", typeof(int));
                    orderList.Columns.Add("Remark", typeof(string));
                    orderList.Columns.Add("AuditStatus", typeof(int));
                    orderList.Columns.Add("PaidTime", typeof(DateTime));
                    orderList.Columns.Add("DebtAmount", typeof(decimal));
                    orderList.Columns.Add("VisitType", typeof(int));
                    orderList.Columns.Add("SourceType", typeof(int));
                    orderList.Columns.Add("RainFlyType", typeof(int));
                    orderList.Columns.Add("ExploitUserID", typeof(long));
                    orderList.Columns.Add("ManagerUserID", typeof(long));
                    orderList.Columns.Add("DealType", typeof(int));
                    orderList.Columns.Add("HospitalID", typeof(long));
                    orderList.Columns.Add("Custom10", typeof(string));

                    decimal debtAmountTemp = 0;


                    DataTable detailList = new DataTable("SmartOrderDetail");
                    detailList.Columns.Add("ID", typeof(long));
                    detailList.Columns.Add("OrderID", typeof(long));
                    detailList.Columns.Add("ChargeID", typeof(long));
                    detailList.Columns.Add("Price", typeof(decimal));
                    detailList.Columns.Add("Num", typeof(int));
                    detailList.Columns.Add("FinalPrice", typeof(decimal));
                    detailList.Columns.Add("RestNum", typeof(int));
                    detailList.Columns.Add("SetID", typeof(long));
                    detailList.Columns.Add("SetNum", typeof(int));
                    detailList.Columns.Add("SetPrice", typeof(decimal));
                    detailList.Columns.Add("SetFinalPrice", typeof(decimal));
                    detailList.Columns.Add("ExpirationDate", typeof(DateTime));

                    DataTable cashierList = new DataTable("SmartCashierCharge");
                    cashierList.Columns.Add("CashierID", typeof(long));
                    cashierList.Columns.Add("ReferID", typeof(long));
                    cashierList.Columns.Add("CashCardAmount", typeof(decimal));
                    cashierList.Columns.Add("DepositAmount", typeof(decimal));
                    cashierList.Columns.Add("CouponAmount", typeof(decimal));
                    cashierList.Columns.Add("DebtAmount", typeof(decimal));
                    cashierList.Columns.Add("Amount", typeof(decimal));
                    cashierList.Columns.Add("HospitalID", typeof(long));
                    cashierList.Columns.Add("CommissionAmount", typeof(decimal));
                    cashierList.Columns.Add("CreateTime", typeof(DateTime));
                    cashierList.Columns.Add("OrderType", typeof(int));
                    cashierList.Columns.Add("CustomerID", typeof(long));
                    cashierList.Columns.Add("ChargeID", typeof(long));
                    cashierList.Columns.Add("SetID", typeof(long));
                    cashierList.Columns.Add("SetNum", typeof(int));
                    cashierList.Columns.Add("Num", typeof(int));
                    cashierList.Columns.Add("OriginAmount", typeof(decimal));
                    cashierList.Columns.Add("VisitType", typeof(int));
                    cashierList.Columns.Add("ExploitUserID", typeof(long));
                    cashierList.Columns.Add("ManagerUserID", typeof(long));
                    cashierList.Columns.Add("OrderID", typeof(long));
                    cashierList.Columns.Add("SourceType", typeof(int));
                    cashierList.Columns.Add("RainFlyType", typeof(int));
                    cashierList.Columns.Add("OrderUserID", typeof(long));
                    cashierList.Columns.Add("BuyExploitUserID", typeof(long));
                    cashierList.Columns.Add("BuyManagerUserID", typeof(long));
                    cashierList.Columns.Add("BuyOrderUserID", typeof(long));
                    cashierList.Columns.Add("BuyVisitType", typeof(int));
                    cashierList.Columns.Add("DealType", typeof(int));
                    cashierList.Columns.Add("Custom10", typeof(string));

                    #endregion
                    foreach (var u in list.Values.AsParallel())
                    {
                        #region order
                        var id = OrderAutoNumber.Instance().Number(OrderAutoNumber.orderStr);
                        var order = orderList.NewRow();
                        order["ID"] = id;
                        order["CustomerID"] = u.FirstOrDefault().CustomerID;
                        order["CreateUserID"] = u.FirstOrDefault().CreateUserID;
                        order["CreateTime"] = u.FirstOrDefault().CreateTime;
                        order["TotalPrice"] = u.Sum(x => x.Price);
                        order["FinalPrice"] = u.Sum(x => x.FinalPrice);
                        debtAmountTemp = u.Sum(x => x.DebtAmount);
                        order["DebtAmount"] = debtAmountTemp;
                        order["PaidTime"] = u.FirstOrDefault().CreateTime;
                        order["VisitType"] = u.FirstOrDefault().VisitType.CastTo<int>();
                        order["SourceType"] = 7;
                        order["RainFlyType"] = 0;
                        if (u.FirstOrDefault().ExploitUserID != null)
                        {
                            order["ExploitUserID"] = u.FirstOrDefault().ExploitUserID;
                        }
                        if (u.FirstOrDefault().ManagerUserID != null)
                        {
                            order["ManagerUserID"] = u.FirstOrDefault().ManagerUserID;
                        }
                        order["DealType"] = u.FirstOrDefault().DealType.CastTo<int>();
                        order["HospitalID"] = 1;
                        if (debtAmountTemp > 0)
                        {
                            order["PaidStatus"] = 3;
                        }
                        else
                        {
                            order["PaidStatus"] = 2;
                        }
                        order["Remark"] = u.FirstOrDefault().Remark;
                        order["AuditStatus"] = 4;

                        order["Custom10"] = u.FirstOrDefault().Custom10;
                        orderList.Rows.Add(order);
                        #endregion

                        foreach (var x in u)
                        {
                            #region detail
                            var detail = detailList.NewRow();
                            var detailID = SingleIdWork.Instance(1).nextId();
                            detail["ID"] = detailID;
                            detail["OrderID"] = id;
                            detail["ChargeID"] = x.ChargeID;
                            detail["Price"] = x.Price;
                            detail["Num"] = x.Num;
                            detail["FinalPrice"] = x.FinalPrice;
                            detail["RestNum"] = x.RestNum;
                            if (x.SetID != null)
                            {
                                detail["SetID"] = x.SetID;
                                detail["SetNum"] = x.SetNum;
                                detail["SetPrice"] = x.SetPrice;
                                detail["SetFinalPrice"] = x.SetFinalPrice;
                            }
                            if (x.ExpirationDate != null)
                            {
                                detail["ExpirationDate"] = x.ExpirationDate;
                            }
                            detailList.Rows.Add(detail);
                            #endregion

                            #region cashier
                            var cashier = cashierList.NewRow();
                            cashier["CashierID"] = 0;
                            cashier["ReferID"] = detailID;
                            cashier["CashCardAmount"] = x.CashAmount;
                            cashier["DepositAmount"] = x.DepositAmount;
                            cashier["CouponAmount"] = x.CouponAmount;
                            cashier["DebtAmount"] = x.DebtAmount;
                            cashier["Amount"] = x.FinalPrice;
                            cashier["HospitalID"] = 1;
                            cashier["CommissionAmount"] = x.CommissionAmount;
                            cashier["CreateTime"] = x.CreateTime;
                            cashier["OrderType"] = 1;
                            cashier["CustomerID"] = x.CustomerID;
                            cashier["ChargeID"] = x.ChargeID;
                            if (x.SetID != null)
                            {
                                cashier["SetID"] = x.SetID;
                                cashier["SetNum"] = x.SetNum;
                            }
                            cashier["Num"] = x.Num;
                            cashier["OriginAmount"] = x.Price;
                            cashier["VisitType"] = x.VisitType.CastTo<int>();
                            if (x.ExploitUserID != null)
                            {
                                cashier["ExploitUserID"] = x.ExploitUserID;
                                cashier["BuyExploitUserID"] = x.ExploitUserID;
                            }
                            if (x.ManagerUserID != null)
                            {
                                cashier["ManagerUserID"] = x.ManagerUserID;
                                cashier["BuyManagerUserID"] = x.ManagerUserID;
                            }
                            cashier["OrderID"] = id;
                            cashier["SourceType"] = 7;
                            cashier["RainFlyType"] = 0;
                            cashier["OrderUserID"] = x.CreateUserID;
                            cashier["BuyVisitType"] = x.VisitType.CastTo<int>();
                            cashier["BuyOrderUserID"] = x.CreateUserID;
                            cashier["DealType"] = x.DealType.CastTo<int>();
                            cashier["Custom10"] = x.Custom10;
                            cashierList.Rows.Add(cashier);
                            #endregion
                        }
                    }

                    ///导入数据库
                    //if (customerAddList.Count() > 0)
                    //{
                    //    var sql = @"insert into [SmartCustomer]([ID],[Name],[Gender],[Mobile],[CreateTime],[ChannelID],[CreateUserID],[CreateUserHospitalID],[Deposit],
                    //        [Coupon],[Point],[VisitTimes],[ConsultTimes],
                    //        [MemberCategoryID],[CashCardTotalAmount],Source,NewReward,Remark,HospitalID) 
                    //        values(@ID,@Name,@Gender,@Mobile,@CreateTime,@ChannelID,@CreateUserID,@CreateUserHospitalID,@Deposit,
                    //        @Coupon,@Point,@VisitTimes,@ConsultTimes,
                    //        @MemberCategoryID,@CashCardTotalAmount,@Source,@NewReward,@Remark,@HospitalID)";
                    //    //_connection.Execute(sql, customerAddList, _transaction);
                    //}
                    if (chargeAddList.Count() > 0)
                    {
                        _connection.Execute(@"insert into SmartCharge(ID,Name,CategoryID,PinYin,Price,Status,Remark,UnitID,Size,ProductAdd,IsEvaluate,Type)
 values(@ID, @Name, @CategoryID, @PinYin, @Price, @Status, @Remark, @UnitID,@Size,@ProductAdd,@IsEvaluate,@Type)", chargeAddList, _transaction);
                    }

                    _connection.Execute(@"ALTER TABLE [SmartOrder] ADD [Custom10] nvarchar(255)", null, _transaction);
                    _connection.Execute(@"ALTER TABLE [SmartCashierCharge] ADD [Custom10] nvarchar(255)", null, _transaction);

                    SqlBulkCopyByDataTable("SmartOrder", orderList);
                    SqlBulkCopyByDataTable("SmartOrderDetail", detailList);
                    SqlBulkCopyByDataTable("SmartCashierCharge", cashierList);

                    _connection.Execute(@"update SmartOrder set CustomerID=b.ID 
from SmartOrder a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);

                    _connection.Execute(@"update SmartCashierCharge set CustomerID=b.ID 
from SmartCashierCharge a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);

                    //1、更新首次成交时间、最后成交时间、成交次数
                    _connection.Execute(@"update SmartCustomer set FirstDealTime=min,LastDealTime=max,DealTime=count 
  from SmartCustomer a
  inner join (select CustomerID,MIN(createtime) as min,MAX(CreateTime) as max,count(CustomerID) as count 
  from SmartOrder where PaidStatus in (2,3) and DealType=1 group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);
                    //1、更新首次消费时间、最后消费时间、消费次数
                    _connection.Execute(@"  update SmartCustomer set FirstConsumeTime=min,LastConsumeTime=max,ConsumeTime=count 
  from SmartCustomer a
  inner join (select CustomerID,MIN(createtime) as min,MAX(CreateTime) as max,count(CustomerID) as count 
  from SmartOrder where PaidStatus in (2,3) group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);

                    //1、更新累计消费金额
                    _connection.Execute(@"update SmartCustomer set CashCardTotalAmount=b.Amount 
  from SmartCustomer a
  inner join (
  select a.CustomerID,sum(case when a.OrderType in (1,2) then a.CashCardAmount+a.DepositAmount+a.DebtAmount+a.CommissionAmount 
  else (a.CashCardAmount+a.DepositAmount+a.DebtAmount+a.CommissionAmount)*-1 end) as Amount
  from SmartCashierCharge a
  where a.OrderType in (1,2,4,8) group by a.CustomerID) as b on a.ID=b.CustomerID", null, _transaction);
                    //2、更新会员等级
                    //                    _connection.Execute(@"update SmartCustomer set MemberCategoryID=c.ID 
                    //from SmartCustomer a
                    //inner join (
                    //select a.ID,max(b.Level) as Level
                    //from SmartCustomer a
                    //inner join SmartMemberCategory as b on a.CashCardTotalAmount>b.Amount group by a.ID) as b on a.ID=b.ID
                    //inner join SmartMemberCategory as c on b.Level=c.Level", null, _transaction);



                    _connection.Execute(@"update SmartCashierCharge set ExploitUserID=b.UserID,BuyExploitUserID=b.UserID 
from SmartCashierCharge a
inner join SmartOwnerShip b on a.CustomerID=b.CUstomerID 
and b.EndTime>getDate() and b.Type=1
where a.ExploitUserID is null", null, _transaction);
                    _connection.Execute(@"update SmartOrder set ExploitUserID=b.UserID
from SmartOrder a
inner join SmartOwnerShip b on a.CustomerID=b.CUstomerID 
and b.EndTime>getDate() and b.Type=1
where a.ExploitUserID is null", null, _transaction);

                    _connection.Execute(@"update SmartCashierCharge set CustomerCallUserID=b.UserID,BuyCustomerCallUserID=b.UserID 
from SmartCashierCharge a
inner join SmartOwnerShip b on a.CustomerID=b.CUstomerID 
and b.EndTime>getDate() and b.Type=5
where a.CustomerCallUserID is null", null, _transaction);
                    _connection.Execute(@"update SmartOrder set CustomerCallUserID=b.UserID
from SmartOrder a
inner join SmartOwnerShip b on a.CustomerID=b.CUstomerID 
and b.EndTime>getDate() and b.Type=5
where a.CustomerCallUserID is null", null, _transaction);

                    _connection.Execute(@"update SmartCashierCharge set ManagerUserID=b.UserID,BuyManagerUserID=b.UserID 
from SmartCashierCharge a
inner join SmartOwnerShip b on a.CustomerID=b.CUstomerID 
and b.EndTime>getDate() and b.Type=2
where a.ManagerUserID is null", null, _transaction);
                    _connection.Execute(@"update SmartOrder set ManagerUserID=b.UserID
from SmartOrder a
inner join SmartOwnerShip b on a.CustomerID=b.CUstomerID 
and b.EndTime>getDate() and b.Type=2
where a.ManagerUserID is null", null, _transaction);

                    //                    _connection.Execute(@"update SmartOrder set VisitType =case when a.CreateTime=b.FirstDealTime and a.CreateTime=b.FirstVisitTime then 1
                    //when a.CreateTime=b.FirstDealTime and a.CreateTime>b.FirstVisitTime then 2
                    //else 4 end
                    //from SmartOrder a
                    //inner join SmartCustomer b on a.CustomerID=b.ID", null, _transaction);

                    //                    _connection.Execute(@"update SmartCashierCharge set VisitType =case when a.CreateTime=b.FirstDealTime and a.CreateTime=b.FirstVisitTime then 1
                    //when a.CreateTime=b.FirstDealTime and a.CreateTime>b.FirstVisitTime then 2
                    //else 4 end,
                    //BuyVisitType=case when a.CreateTime=b.FirstDealTime and a.CreateTime=b.FirstVisitTime then 1
                    //when a.CreateTime=b.FirstDealTime and a.CreateTime>b.FirstVisitTime then 2
                    //else 4 end
                    //from SmartCashierCharge a
                    //inner join SmartCustomer b on a.CustomerID=b.ID", null, _transaction);

                    _connection.Execute(@"delete from SmartOrder where CustomerID=0", null, _transaction);
                    _connection.Execute(@"ALTER TABLE [SmartOrder] DROP COLUMN [Custom10]", null, _transaction);

                    _connection.Execute(@"delete from SmartCashierCharge where CustomerID=0", null, _transaction);
                    _connection.Execute(@"ALTER TABLE [SmartCashierCharge] DROP COLUMN [Custom10]", null, _transaction);

                    _connection.Execute(@"delete from SmartCashier", null, _transaction);
                    _connection.Execute(@"insert into SmartCashier
select a.OrderID,a.HospitalID,a.CustomerID,a.OrderType,a.OrderID,1,a.CreateTime,a.Amount,a.CashCardAmount,0,0,a.CouponAmount,
a.DebtAmount,1,'数据迁移补录',a.CommissionAmount,a.RainFlyType,1
from SmartCashierCharge a", null, _transaction);

                    _connection.Execute(@"update SmartCashierCharge set CashierID=OrderID", null, _transaction);
                }

                Console.WriteLine("订单记录结束迁移");
            }
        }


        /// <summary>
        /// 订单
        /// </summary>
        public static void OrderNJHY()
        {
            Console.WriteLine("订单记录开始迁移");

            var tempList = _connection.Query<CashierCharge>(@"select a.ID as CustomerID,a.Custom8 as Amount,a.CreateTime as CreateTime,15696967422166016 as ChargeID  
from SmartCustomer a
where a.Custom8<>'0' and a.ID>252698", null, _transaction);

            List<Order> orderList = new List<Order>();
            List<OrderDetail> detailList = new List<OrderDetail>();
            List<CashierCharge> cashierChargeList = new List<CashierCharge>();
            List<Visit> visitList = new List<Visit>();
            foreach (var u in tempList)
            {
                var id = OrderAutoNumber.Instance().Number(OrderAutoNumber.orderStr);
                var detailID = SingleIdWork.Instance(1).nextId();
                orderList.Add(new Order()
                {
                    ID = id,
                    CustomerID = u.CustomerID,
                    CreateTime = u.CreateTime,
                    CreateUserID = 1,
                    TotalPrice = u.Amount,
                    FinalPrice = u.Amount,
                    PaidStatus = 2,
                    Remark = "系统补录累计消费金额",
                    AuditStatus = 4,
                    PaidTime = u.CreateTime,
                    DebtAmount = 0,
                    HospitalID = 1,
                    VisitType = 1,
                    SourceType = 7,
                    DealType = 1,

                });
                detailList.Add(new OrderDetail()
                {
                    ID = detailID,
                    OrderID = id,
                    ChargeID = u.ChargeID,
                    Price = u.Amount,
                    Num = 1,
                    FinalPrice = u.Amount,
                    RestNum = 0,
                });
                cashierChargeList.Add(new CashierCharge()
                {
                    Amount = u.Amount,
                    ChargeID = u.ChargeID,
                    OrderID = id,
                    BuyOrderUserID = 1,
                    BuyVisitType = 1,
                    CashCardAmount = u.Amount,
                    CashierID = 0,
                    CommissionAmount = 0,
                    CouponAmount = 0,
                    CreateTime = u.CreateTime,
                    CustomerID = u.CustomerID,
                    DealType = 1,
                    DebtAmount = 0,
                    DepositAmount = 0,
                    HospitalID = 1,
                    Num = 1,
                    OrderType = 1,
                    OrderUserID = 1,
                    OriginAmount = u.Amount,
                    ReferID = detailID,
                    SourceType = 7,
                    VisitType = 1
                });

                //visitList.Add(new DTO.Visit()
                //{
                //    CustomerID = u.CustomerID,
                //    CreateTime = u.CreateTime,
                //    CreateUserID = 1,
                //    UserID = 1,
                //    HospitalID = 1,
                //    VisitType = 1,
                //    IsConsume = 1
                //});
            }

            //_connection.Execute(@"insert into SmartVisit(CustomerID,CreateTime,CreateUserID,UserID,HospitalID,VisitType,IsConsume) values(@CustomerID,@CreateTime,@CreateUserID,@UserID,@HospitalID,@VisitType,@IsConsume)", visitList, _transaction);

            Console.WriteLine("11111111111111");
            _connection.Execute(@"insert into SmartOrder(ID,CustomerID,CreateTime,CreateUserID,TotalPrice,FinalPrice,
PaidStatus,Remark,AuditStatus,PaidTime,DebtAmount,HospitalID,VisitType,SourceType,DealType) values(@ID,@CustomerID,@CreateTime,@CreateUserID,@TotalPrice,@FinalPrice,
@PaidStatus,@Remark,@AuditStatus,@PaidTime,@DebtAmount,@HospitalID,@VisitType,@SourceType,@DealType)", orderList, _transaction);
            Console.WriteLine("2222222222222");

            _connection.Execute(@"insert into SmartOrderDetail(ID,OrderID,ChargeID,Price,Num,FinalPrice,RestNum) values(@ID,@OrderID,@ChargeID,@Price,@Num,@FinalPrice,@RestNum)", detailList, _transaction);
            Console.WriteLine("333333333333333333");
            _connection.Execute(@"insert into SmartCashierCharge(Amount,ChargeID,OrderID,BuyOrderUserID,BuyVisitType,CashCardAmount,CashierID,CommissionAmount,CouponAmount,
CreateTime,CustomerID,DealType,DebtAmount,DepositAmount,HospitalID,Num,OrderType,OrderUserID,OriginAmount,ReferID,SourceType,VisitType) 
values(@Amount,@ChargeID,@OrderID,@BuyOrderUserID,@BuyVisitType,@CashCardAmount,@CashierID,@CommissionAmount,@CouponAmount,
@CreateTime,@CustomerID,@DealType,@DebtAmount,@DepositAmount,@HospitalID,@Num,@OrderType,@OrderUserID,@OriginAmount,@ReferID,@SourceType,@VisitType)", cashierChargeList, _transaction);

            Console.WriteLine("订单记录结束迁移");

        }

        public static void Order2()
        {
            Console.WriteLine("订单记录开始迁移");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\青岛壹美\\客户订购项目情况表2.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                List<DataTransferCommon> chargeList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> chargeSetList = new List<DataTransferCommon>();
                //customerList = _connection.Query<DataTransferCommon>(@"select Custom10 as Name,ID from [SmartCustomer]");

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                chargeList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartCharge]", null, _transaction).ToList();

                DataTransferCommon chargeTemp = null;
                DataTransferCommon chargeSetTemp = null;
                //DataTransferCommon customerTemp = null;
                VisitType? visitType;
                DataTransferCommon createUserTemp = null;
                DataTransferCommon exploitTemp = null;
                DataTransferCommon managerTemp = null;
                string remark = "";
                DateTime createTime;
                DateTime expirationDate;
                Dictionary<int, List<DataTransferOrder>> list = new Dictionary<int, List<DataTransferOrder>>();
                int chargeSetNum = 0;
                int num = 1;
                int restNum = 0;
                decimal originAmount;
                decimal amount;
                decimal cashAmount;
                decimal debtAmount;
                int orderID = 10000;

                List<object> resultList = new List<object>();


                int aaa = 0;
                for (int row = 2; row <= rowCount; row++)
                {
                    exploitTemp = null;
                    managerTemp = null;
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }


                    if (!DateTime.TryParse(worksheet.Cells[row, 6].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行下单时间异常！");
                    }
                    var createTimeNew = createTime.AddSeconds(1);

                    chargeTemp = chargeList.Where(u => u.Name == worksheet.Cells[row, 7].Value.ToString().Trim()).FirstOrDefault();
                    var custom10 = worksheet.Cells[row, 1].Value.ToString().Trim().Split('<')[1].Split('>')[0];



                    if (!decimal.TryParse(worksheet.Cells[row, 11].Value.ToString().Trim(), out originAmount))
                    {
                        throw new Exception("第" + row + "行原始金额异常！");
                    }
                    if (!decimal.TryParse(worksheet.Cells[row, 12].Value.ToString().Trim(), out amount))
                    {
                        throw new Exception("第" + row + "行成交金额异常！");
                    }

                    if (worksheet.Cells[row, 20].Value.ToString().Trim() == "取消" || worksheet.Cells[row, 20].Value.ToString().Trim() == "未收费")
                    {
                        resultList.Add(new
                        {
                            Custom10 = custom10,
                            ChargeID = chargeTemp.ID,
                            OriginAmount = originAmount,
                            TotalAmount = amount,
                            CreateTime = createTimeNew,
                            Num = worksheet.Cells[row, 8].Value.ToString().Trim(),
                        });
                    }

                }

                _connection.Execute(@"insert into SmartTestOrder values(@Custom10,@ChargeID,@OriginAmount,@TotalAmount,@CreateTime,@Num)", resultList, _transaction);

                Console.WriteLine("订单记录结束迁移" + aaa);
            }
        }

        /// <summary>
        /// 划扣
        /// </summary>
        public static void Operation()
        {
            Console.WriteLine("划扣记录开始迁移");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\俊泰美好\\客户消费明细表.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                IEnumerable<DataTransferCommon> deptList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> userList = new List<DataTransferCommon>();
                IEnumerable<DataTransferCommon> chargeList = new List<DataTransferCommon>();
                deptList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartDept] where HospitalID=@HospitalID", new { HospitalID = 1 }, _transaction);
                userList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartUser] where HospitalID=@HospitalID order by Status desc", new { HospitalID = 1 }, _transaction);
                chargeList = _connection.Query<DataTransferCommon>(@"select ID,[Name] from [SmartCharge]  order by Status desc", null, _transaction);

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable operatorList = new DataTable("SmartOperator");
                operatorList.Columns.Add("ID", typeof(long));
                operatorList.Columns.Add("OperationID", typeof(long));
                operatorList.Columns.Add("UserID", typeof(long));
                operatorList.Columns.Add("PositionID", typeof(long));


                DataTable visitList = new DataTable("SmartOperation");
                visitList.Columns.Add("ID", typeof(long));
                visitList.Columns.Add("CustomerID", typeof(long));
                visitList.Columns.Add("CreateUserID", typeof(long));
                visitList.Columns.Add("CreateTime", typeof(DateTime));
                visitList.Columns.Add("HospitalID", typeof(long));
                visitList.Columns.Add("Remark", typeof(string));
                visitList.Columns.Add("ChargeID", typeof(long));
                visitList.Columns.Add("Num", typeof(int));
                visitList.Columns.Add("DeptID", typeof(long));
                visitList.Columns.Add("DoctorID", typeof(long));
                visitList.Columns.Add("OrderDetailID", typeof(long));
                visitList.Columns.Add("CustomerStatus", typeof(int));
                visitList.Columns.Add("Custom10", typeof(string));


                DataTransferCommon chargeTemp = null;
                DataTransferCommon customerTemp = null;
                int num = 1;
                DataTransferCommon createUserTemp = null;
                DataTransferCommon deptTemp = null;
                DataTransferCommon doctorTemp = null;
                string remark = "";
                DateTime createTime;
                for (int row = 3; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    #region
                    if (worksheet.Cells[row, 3].Value == null || worksheet.Cells[row, 3].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行顾客手机号不能为空！");
                    }
                    //if (worksheet.Cells[row, 25].Value == null || worksheet.Cells[row, 25].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    worksheet.Cells[row, 25].Value = "超级管理员";
                    //}
                    if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 1].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        throw new Exception("第" + row + "行划扣时间不能为空！");
                    }
                    if (worksheet.Cells[row, 5].Value == null || worksheet.Cells[row, 5].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 5].Value = "财务部";
                        //throw new Exception("第" + row + "行划扣科室不能为空！");
                    }
                    //if (worksheet.Cells[row, 8].Value == null || worksheet.Cells[row, 8].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    continue;
                    //    //throw new Exception("第" + row + "行划扣项目不能为空！");
                    //}
                    if (worksheet.Cells[row, 26].Value == null || worksheet.Cells[row, 26].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 26].Value = 1;
                        //throw new Exception("第" + row + "行划扣数量不能为空！");
                    }
                    if (worksheet.Cells[row, 44].Value == null || worksheet.Cells[row, 44].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        worksheet.Cells[row, 44].Value = "超级管理员";
                    }
                    #endregion

                    #region
                    //customerTemp = customerList.AsParallel().Where(u => u.Name == worksheet.Cells[row, 1].Value.ToString().Trim()).FirstOrDefault();
                    //if (customerTemp == null)
                    //{
                    //    continue;
                    //    //result.Message = "第" + row + "行该顾客手机号不存在！";
                    //    //return result;
                    //}

                    createUserTemp = new DataTransferCommon()
                    {
                        ID = 1
                    };
                    if (!DateTime.TryParse(worksheet.Cells[row, 1].Value.ToString().Trim(), out createTime))
                    {
                        throw new Exception("第" + row + "行划扣时间异常！");
                    }

                    deptTemp = deptList.Where(u => u.Name.Trim() == worksheet.Cells[row, 62].Value.ToString().Trim()).FirstOrDefault();
                    if (deptTemp == null)
                    {
                        //deptTemp = new DataTransferCommon()
                        //{
                        //    ID = 10014
                        //};
                        throw new Exception("第" + row + "行科室不存在！");
                    }

                    chargeTemp = chargeList.Where(u => u.Name == worksheet.Cells[row, 18].Value.ToString().Trim()).FirstOrDefault();
                    if (chargeTemp == null)
                    {
                        //continue;
                        throw new Exception("第" + row + "行划扣项目不存在！");

                        //result.Message = "第" + row + "行划扣项目不存在！";
                        //return result;
                    }

                    if (!int.TryParse(worksheet.Cells[row, 26].Value.ToString().Trim(), out num))
                    {
                        throw new Exception("第" + row + "行划扣数量异常！");
                    }

                    //string name = "";
                    //*********************************************************************************************************************************************************************************************************************
                    if (worksheet.Cells[row, 44].Value == null || worksheet.Cells[row, 44].Value.ToString().Trim().IsNullOrEmpty())
                    {
                        doctorTemp = new DataTransferCommon()
                        {
                            ID = 1
                        };
                    }
                    else
                    {
                        doctorTemp = userList.Where(u => u.Name.Trim() == worksheet.Cells[row, 44].Value.ToString().Trim()).FirstOrDefault();
                        if (doctorTemp == null)
                        {
                            doctorTemp = new DataTransferCommon()
                            {
                                ID = 1
                            };

                        }
                    }
                    //if (worksheet.Cells[row, 16].Value != null && !worksheet.Cells[row, 16].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    name = worksheet.Cells[row, 16].Value.ToString().Trim();
                    //}
                    //else
                    //{
                    //    if (name != "" && worksheet.Cells[row, 17].Value != null && !worksheet.Cells[row, 17].Value.ToString().Trim().IsNullOrEmpty())
                    //    {
                    //        name = worksheet.Cells[row, 17].Value.ToString().Trim();
                    //    }
                    //}
                    //doctorTemp = userList.Where(u => u.Name == name).FirstOrDefault();
                    //if (doctorTemp == null)
                    //{
                    //    //doctorTemp = new DataTransferCommon() { ID = 1 };

                    //    throw new Exception("第" + row + "行主治医生不能为空！");

                    //}
                    remark = "数据迁移";
                    //if (worksheet.Cells[row, 27].Value != null)
                    //{
                    //    remark = worksheet.Cells[row, 27].Value.ToString().Trim();
                    //}

                    //if (remark.IsNullOrEmpty())
                    //{
                    //    remark = "数据迁移";
                    //}
                    //if (remark.Length > 50)
                    //{
                    //    remark = remark.Substring(0, 49);
                    //}
                    #endregion


                    DataRow dr = visitList.NewRow();
                    var operationID = SingleIdWork.Instance(1).nextId();
                    dr["ID"] = operationID;
                    //dr["CustomerID"] = new Random().Next(958266, 1430913);
                    dr["CustomerID"] = 0;
                    dr["CreateUserID"] = createUserTemp.ID;
                    dr["CreateTime"] = createTime;
                    dr["HospitalID"] = 1;
                    dr["ChargeID"] = chargeTemp.ID;
                    dr["Num"] = num;
                    dr["Remark"] = remark;
                    dr["DeptID"] = deptTemp.ID;
                    dr["DoctorID"] = doctorTemp.ID;
                    dr["OrderDetailID"] = 0;
                    dr["CustomerStatus"] = 0;
                    dr["Custom10"] = worksheet.Cells[row, 3].Value.ToString().Trim();

                    visitList.Rows.Add(dr);

                    //*********************************************************************************************************************************************************************************************************************  麻醉师
                    //if (worksheet.Cells[row, 17].Value != null && !worksheet.Cells[row, 17].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    var temp = userList.Where(u => u.Name.Trim() == worksheet.Cells[row, 17].Value.ToString().Trim()).FirstOrDefault();
                    //    if (temp != null)
                    //    {
                    //        DataRow dr2 = operatorList.NewRow();
                    //        dr2["ID"] = SingleIdWork.Instance(1).nextId();
                    //        dr2["OperationID"] = operationID;
                    //        dr2["UserID"] = temp.ID;
                    //        dr2["PositionID"] = 15553349128487936;
                    //        operatorList.Rows.Add(dr2);
                    //    }
                    //}
                    //*********************************************************************************************************************************************************************************************************************  外科医助
                    //if (worksheet.Cells[row, 18].Value != null && !worksheet.Cells[row, 18].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    var temp = userList.Where(u => u.Name.Trim() == worksheet.Cells[row, 18].Value.ToString().Trim()).FirstOrDefault();
                    //    if (temp != null)
                    //    {
                    //        DataRow dr3 = operatorList.NewRow();
                    //        dr3["ID"] = SingleIdWork.Instance(1).nextId();
                    //        dr3["OperationID"] = operationID;
                    //        dr3["UserID"] = temp.ID;
                    //        dr3["PositionID"] = 15553349029495808;
                    //        operatorList.Rows.Add(dr3);
                    //    }
                    //}
                    //*********************************************************************************************************************************************************************************************************************  美容师
                    //if (worksheet.Cells[row, 20].Value != null && !worksheet.Cells[row, 20].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    var temp = userList.Where(u => u.Name.Trim() == worksheet.Cells[row, 20].Value.ToString().Trim()).FirstOrDefault();
                    //    if (temp != null)
                    //    {
                    //        DataRow dr4 = operatorList.NewRow();
                    //        dr4["ID"] = SingleIdWork.Instance(1).nextId();
                    //        dr4["OperationID"] = operationID;
                    //        dr4["UserID"] = temp.ID;
                    //        dr4["PositionID"] = 15553348874617856;
                    //        operatorList.Rows.Add(dr4);
                    //    }
                    //}
                    //*********************************************************************************************************************************************************************************************************************  外科护士
                    //if (worksheet.Cells[row, 21].Value != null && !worksheet.Cells[row, 21].Value.ToString().Trim().IsNullOrEmpty())
                    //{
                    //    var temp = userList.Where(u => u.Name.Trim() == worksheet.Cells[row, 21].Value.ToString().Trim()).FirstOrDefault();
                    //    if (temp != null)
                    //    {
                    //        DataRow dr5 = operatorList.NewRow();
                    //        dr5["ID"] = SingleIdWork.Instance(1).nextId();
                    //        dr5["OperationID"] = operationID;
                    //        dr5["UserID"] = temp.ID;
                    //        dr5["PositionID"] = 15553348586357760;
                    //        operatorList.Rows.Add(dr5);
                    //    }
                    //}
                }

                ///导入数据库
                _connection.Execute(@"ALTER TABLE [SmartOperation] ADD [Custom10] nvarchar(255)", null, _transaction);
                if (visitList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartOperation", visitList);
                }

                if (operatorList.Rows.Count > 0)
                {
                    SqlBulkCopyByDataTable("SmartOperator", operatorList);
                }

                _connection.Execute(@"update SmartOperation set CustomerID=b.ID 
from SmartOperation a 
inner join SmartCustomer b on a.Custom10=b.Custom10", null, _transaction);

                _connection.Execute(@"delete from SmartOperator where OperationID in (select distinct CustomerID from SmartOperation where CustomerID=0)", null, _transaction);
                _connection.Execute(@"delete from SmartOperation where CustomerID=0", null, _transaction);
                _connection.Execute(@"ALTER TABLE [SmartOperation] DROP COLUMN [Custom10]", null, _transaction);

                Console.WriteLine("划扣记录结束迁移");
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="connectionStr">连接字符串</param>
        /// <param name="dataTableName">数据库表名称</param>
        /// <param name="sourceDataTable"></param>
        /// <param name="batchSize"></param>
        public static void SqlBulkCopyByDataTable(string dataTableName, DataTable sourceDataTable, int batchSize = 100000)
        {
            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(_connection, SqlBulkCopyOptions.Default, _transaction))
            {
                try
                {
                    sqlBulkCopy.DestinationTableName = dataTableName;
                    sqlBulkCopy.BatchSize = batchSize;
                    for (int i = 0; i < sourceDataTable.Columns.Count; i++)
                    {
                        sqlBulkCopy.ColumnMappings.Add(sourceDataTable.Columns[i].ColumnName, sourceDataTable.Columns[i].ColumnName);
                    }
                    sqlBulkCopy.WriteToServer(sourceDataTable);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 更新划扣
        /// </summary>
        public static void CaculateOrderRestNum()
        {
            Console.WriteLine("订单剩余次数开始计算");
            var orderDetailList = _connection.Query<OrderDetail>(@"select b.ID as OrderDetailID,b.Num,b.Num as RestNum,a.CustomerID,b.ChargeID,a.CreateTime  
from SmartOrder a
inner join SmartOrderDetail b on a.ID=b.OrderID
where a.PaidStatus in (2,3) ", null, _transaction);
            Console.WriteLine(@"111111111111111111111111");

            var operationList = _connection.Query<OrderDetail>(@"select a.ID as OperationID,a.CustomerID,a.OrderDetailID,a.Num,a.ChargeID 
from SmartOperation a where a.OrderDetailID=0  order by a.CreateTime", null, _transaction);
            Console.WriteLine(@"2222222222222");

            DataTable visitList = new DataTable("SmartOperationTest");
            visitList.Columns.Add("OperationID", typeof(long));
            visitList.Columns.Add("OrderDetailID", typeof(long));
            int i = 0;

            foreach (var u in operationList)
            {
                var temp = orderDetailList.AsParallel().Where(x => x.CustomerID == u.CustomerID && x.ChargeID == u.ChargeID && u.Num <= x.RestNum).OrderBy(x => x.RestNum).FirstOrDefault();
                if (temp == null)
                {
                    i++;
                }
                else
                {
                    u.OrderDetailID = temp.OrderDetailID;
                    temp.RestNum -= u.Num;
                }

                DataRow dr = visitList.NewRow();
                dr["OperationID"] = u.OperationID;
                dr["OrderDetailID"] = u.OrderDetailID;

                visitList.Rows.Add(dr);
            }

            Console.WriteLine(@"111111111111111111111111");

            _connection.Execute(@"create table SmartOperationTest
(
OperationID bigint ,
OrderDetailID bigint 
)", null, _transaction);
            if (visitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartOperationTest", visitList);
            }

            _connection.Execute(@"update SmartOperation set OrderDetailID=b.OrderDetailID 
from SmartOperation a 
inner join SmartOperationTest b on a.ID=b.OperationID", null, _transaction);

            _connection.Execute(@"drop table SmartOperationTest", null, _transaction);

            Console.WriteLine("用户结束导入");
        }

        public static void Point()
        {
            Console.WriteLine("积分开始迁移");
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\潍坊壹美\\客户资料表point.xlsx")))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;

                DataTable customerAddList = new DataTable("SmartTest");
                customerAddList.Columns.Add("ID", typeof(long));
                customerAddList.Columns.Add("CustomerID", typeof(string));
                customerAddList.Columns.Add("Point", typeof(decimal));


                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }

                    DataRow dr = customerAddList.NewRow();
                    dr["ID"] = SingleIdWork.Instance(1).nextId();
                    dr["CustomerID"] = worksheet.Cells[row, 1].Value.ToString().Trim();
                    dr["Point"] = decimal.Parse(worksheet.Cells[row, 2].Value.ToString().Trim());

                    customerAddList.Rows.Add(dr);


                }


                if (customerAddList.Rows.Count > 0)
                {
                    //SqlBulkCopyByDataTable("SmartTest", customerAddList);
                    _connection.Execute(@"insert into SmartPoint select b.ID,1,@Time,@Type,a.Point,'补录历史积分' ,0,1,1,null,null,a.ID
from SmartTest a
inner join SmartCustomer b on a.CustomerID=b.Custom10 ", new { Type = PointType.DataImport, Time = DateTime.Now, }, _transaction);
                    //更新顾客医院子表
                    _connection.Execute(@"update [SmartCustomerHospital] set Point=b.Amount 
  from [SmartCustomerHospital] a
  inner join (
  select a.CustomerID,a.HospitalID,sum([Amount]) as Amount
  from [SmartPoint] a group by a.CustomerID,a.HospitalID) as b on a.CustomerID=b.CustomerID and a.HospitalID=b.HospitalID", null, _transaction);
                    _connection.Execute(@"update [SmartCustomer] set Point=b.Amount 
  from [SmartCustomer] a
  inner join (
  select a.CustomerID,sum([Amount]) as Amount
  from [SmartPoint] a  group by a.CustomerID) as b on a.ID=b.CustomerID", null, _transaction);
                }


            }

            Console.WriteLine("积分结束迁移");
        }

        private static VisitType? TransferVisitType(string temp)
        {
            VisitType? visitType = null;
            if (VisitType.First.ToDescription() == temp)
            {
                visitType = VisitType.First;
            }
            else if (VisitType.Twice.ToDescription() == temp)
            {
                visitType = VisitType.Twice;
            }
            else if (VisitType.Check.ToDescription() == temp)
            {
                visitType = VisitType.Check;
            }
            else if (VisitType.Again.ToDescription() == temp)
            {
                visitType = VisitType.Again;
            }
            else if (VisitType.NoCome.ToDescription() == temp)
            {
                visitType = VisitType.NoCome;
            }
            else
            {
                visitType = VisitType.First;
            }

            return visitType;
        }

    }
}

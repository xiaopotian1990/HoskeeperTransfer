using Dapper;
using HoskeeperTransfer.DTO;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace HoskeeperTransfer
{
    class JinYiWeiNewProgram
    {
        private static long _hospitalID = 1;
        private static long _channelID = 429;
        private static SqlConnection _connection;
        private static MySqlConnection _mySqlConnection;
        private static SqlTransaction _transaction;
        private static long _tool = 14663754418897920;//电话工具
        //private static long _symptomID = 14663754418897920;//无症状
        private static long _callBackCategoryOfSH = 14663756704973824;
        private static long _callBackCategoryOfWD = 14663756476875776;
        private static long _callBackCategoryOfXC = 14663756587729920;

        private static long _couponCategoryID = 14961071147172864;
        private static long _depositCategoryID = 14961071468217344;
        private static int _callbackNum = 200000;
        static void Main(string[] args)
        {
            try
            {
                //北医圣心
                _connection = new SqlConnection("Data Source=47.105.89.85;Initial Catalog=Hoskeeper;Persist Security Info=True;User ID=sa;Password=Ytym#!@2020123456;MultipleActiveResultSets = true;connect timeout=90000");
                //_mySqlConnection = new MySqlConnection("server=30243r285b.zicp.vip;port=33015;database=guard_erp;uid=nzzn;pwd=nzzn@2023;charset=utf8;");
                // _mySqlConnection.Open();
                _connection.Open();
                _transaction = _connection.BeginTransaction();

                //Supplier();
                //Factory();
                //Channel();
                //Unit();
                //Dept();
                //ItemGroup();
                //Item();
                //Symptom();
                //ChargeCategory();
                //Charge();
                //ProductCategory();
                //User();
                //Warehouse();
                //Product();
                //CouponCategory();
                //DepositCategory();
                //CallBackCategory();
                //CallBackGroup();
                //ChargeSet();
                //FailtureCategory();
                //CardCategory();
                //Tag();


                //Customer();
                //Consult();
                //CallBackTask();
                //CallBack();
                //Visit();
                //Coupon();
                //Deposit();

                //Order();
                //BackOrder();
                //BackDeposit();
                //DepositOrder();
                //DebtOrder();
                //Cashier();
                //Point();
                //CustomerTag();
                //OperationOld();
                //TahGroup();
                //Photo();
                //CaculateOrderRestNum();
                //AddItem();
                AddCallBackGroup();
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
                _mySqlConnection.Close();
            }
        }
        /// <summary>
        /// 回访组
        /// </summary>
        public static void AddCallBackGroup()
        {
            Console.WriteLine("回访组导入开始！");
            Dictionary<string, List<DataTransferChannel>> dic = new Dictionary<string, List<DataTransferChannel>>();

            var categoryList=_connection.Query<DataTransferCommon>(@"select ID,Name from SmartCallbackCategory order by status desc", null, _transaction);

            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\烟台壹美Over\\1111111.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                Dictionary<string, CallBackGroup> setList = new Dictionary<string, CallBackGroup>();
                List<SmartCallbackGroupDetail> detailList = new List<SmartCallbackGroupDetail>();

                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    CallBackGroup set;
                    //Console.WriteLine(worksheet.Cells[row, 1].Value.ToString());
                    if (setList.ContainsKey(worksheet.Cells[row, 1].Value.ToString()))
                    {
                        set = setList[worksheet.Cells[row, 1].Value.ToString()];
                    }
                    else
                    {
                        set = new CallBackGroup()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 1].Value.ToString(),
                            Status = CommonStatus.Use,
                        };
                        setList.Add(worksheet.Cells[row, 1].Value.ToString(), set);
                    }

                    detailList.Add(new SmartCallbackGroupDetail()
                    {
                        ID = SingleIdWork.Instance(1).nextId(),
                        Name = worksheet.Cells[row, 4].Value.ToString(),
                        Days = int.Parse(worksheet.Cells[row, 3].Value.ToString()),
                        CategoryID = categoryList.Where(x => x.Name == worksheet.Cells[row, 2].Value.ToString()).FirstOrDefault().ID,
                        SetID = set.ID,
                    });
                }
                //_connection.Execute("delete from [SmartChargeCategory]",
                //      itemGroupList.Values, _transaction);
                //_connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo) values (@ID,@Name,@SortNo)",
                //       itemGroupList.Values, _transaction);
                //_connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo,ParentID) values (@ID,@Name,@SortNo,@GroupID)",
                //       itemCategoryList.Values, _transaction);
                //_connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo,ParentID) values (@ID,@Name,@SortNo,@GroupID)",
                //       itemList.Values, _transaction);

                _connection.Execute("delete from [SmartCallBackSet]",
                      setList.Values, _transaction);
                _connection.Execute("insert into [SmartCallBackSet](ID,Name,Status) values (@ID,@Name,@Status)",
                       setList.Values, _transaction);

                _connection.Execute("delete from [SmartCallBackSetDetail]",
                      detailList, _transaction);
                _connection.Execute("insert into [SmartCallBackSetDetail](ID,Name,Days,CategoryID,SetID) values (@ID,@Name,@Days,@CategoryID,@SetID)",
                       detailList, _transaction);

            }



            Console.WriteLine("渠道导入结束！");
        }

        public static void AddItemGroup()
        {
            Console.WriteLine("渠道导入开始！");
            Dictionary<string, List<DataTransferChannel>> dic = new Dictionary<string, List<DataTransferChannel>>();
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\东方整形\\项目管理.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                Dictionary<string, DataTransferCommon> itemGroupList = new Dictionary<string, DataTransferCommon>();
                Dictionary<string, DataTransferCommon> itemCategoryList = new Dictionary<string, DataTransferCommon>();
                Dictionary<string, DataTransferCommon> itemList = new Dictionary<string, DataTransferCommon>();

                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null && worksheet.Cells[row, 3].Value == null)
                    {
                        break;
                    }
                    DataTransferCommon itemGroup;
                    Console.WriteLine(worksheet.Cells[row, 1].Value.ToString());
                    if (itemGroupList.ContainsKey(worksheet.Cells[row, 1].Value.ToString()))
                    {
                        itemGroup = itemGroupList[worksheet.Cells[row, 1].Value.ToString()];
                    }
                    else
                    {
                        itemGroup = new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 1].Value.ToString(),
                            SortNo = 1,
                        };
                        itemGroupList.Add(worksheet.Cells[row, 1].Value.ToString(), itemGroup);
                    }

                    DataTransferCommon itemCategory;
                    if (itemCategoryList.ContainsKey(worksheet.Cells[row, 2].Value.ToString()))
                    {
                        itemCategory = itemCategoryList[worksheet.Cells[row, 2].Value.ToString()];
                    }
                    else
                    {
                        itemCategory = new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 2].Value.ToString(),
                            SortNo = 1,
                            GroupID = itemGroup.ID
                        };
                        itemCategoryList.Add(worksheet.Cells[row, 2].Value.ToString(), itemCategory);
                    }

                    DataTransferCommon item;
                    if (itemList.ContainsKey(worksheet.Cells[row, 3].Value.ToString() + itemCategory.ID))
                    {
                        item = itemList[worksheet.Cells[row, 3].Value.ToString() + itemCategory.ID];
                    }
                    else
                    {
                        item = new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 3].Value.ToString(),
                            SortNo = 1,
                            GroupID = itemCategory.ID
                        };
                        itemList.Add(worksheet.Cells[row, 3].Value.ToString() + itemCategory.ID, item);
                    }

                }
                //_connection.Execute("delete from [SmartChargeCategory]",
                //      itemGroupList.Values, _transaction);
                //_connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo) values (@ID,@Name,@SortNo)",
                //       itemGroupList.Values, _transaction);
                //_connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo,ParentID) values (@ID,@Name,@SortNo,@GroupID)",
                //       itemCategoryList.Values, _transaction);
                //_connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo,ParentID) values (@ID,@Name,@SortNo,@GroupID)",
                //       itemList.Values, _transaction);

                _connection.Execute("delete from [SmartItemGroup]",
                      itemGroupList.Values, _transaction);
                _connection.Execute("insert into [SmartItemGroup](ID,Name,SortNo) values (@ID,@Name,@SortNo)",
                       itemGroupList.Values, _transaction);

                _connection.Execute("delete from [SmartItemChargeCategory]",
                      itemCategoryList.Values, _transaction);
                _connection.Execute("insert into [SmartItemChargeCategory](ID,Name,SortNo,GroupID) values (@ID,@Name,@SortNo,@GroupID)",
                       itemCategoryList.Values, _transaction);

                _connection.Execute("delete from [SmartItem]",
                                      itemList.Values, _transaction);
                _connection.Execute("insert into [SmartItem](ID,Name,SortNo,GroupID) values (@ID,@Name,@SortNo,@GroupID)",
                       itemList.Values, _transaction);

            }



            Console.WriteLine("渠道导入结束！");
        }

        public static void AddItem()
        {
            Console.WriteLine("渠道导入开始！");
            Dictionary<string, List<DataTransferChannel>> dic = new Dictionary<string, List<DataTransferChannel>>();
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\各医院\\北医圣心\\项目管理.xlsx")))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                Dictionary<string, DataTransferCommon> itemGroupList = new Dictionary<string, DataTransferCommon>();
                Dictionary<string, DataTransferCommon> itemCategoryList = new Dictionary<string, DataTransferCommon>();
                Dictionary<string, DataTransferCommon> itemList = new Dictionary<string, DataTransferCommon>();
                Dictionary<string, DataTransferCommon> categoryList = new Dictionary<string, DataTransferCommon>();


                List<DataTransferCommon> updateChargeItemList = new List<DataTransferCommon>();

                for (int row = 2; row <= rowCount; row++)
                {
                    DataTransferCommon itemGroup;
                    if (itemGroupList.ContainsKey(worksheet.Cells[row, 12].Value.ToString()))
                    {
                        itemGroup = itemGroupList[worksheet.Cells[row, 12].Value.ToString()];
                    }
                    else
                    {
                        itemGroup = new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 12].Value.ToString(),
                            SortNo = 1,
                        };
                        itemGroupList.Add(worksheet.Cells[row, 12].Value.ToString(), itemGroup);
                    }

                    DataTransferCommon itemCategory;
                    if (itemCategoryList.ContainsKey(worksheet.Cells[row, 13].Value.ToString()))
                    {
                        itemCategory = itemCategoryList[worksheet.Cells[row, 13].Value.ToString()];
                    }
                    else
                    {
                        itemCategory = new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 13].Value.ToString(),
                            SortNo = 1,
                            GroupID = itemGroup.ID
                        };
                        itemCategoryList.Add(worksheet.Cells[row, 13].Value.ToString(), itemCategory);
                    }

                    DataTransferCommon item;
                    if (itemList.ContainsKey(worksheet.Cells[row, 14].Value.ToString() + itemCategory.ID))
                    {
                        item = itemList[worksheet.Cells[row, 14].Value.ToString() + itemCategory.ID];
                    }
                    else
                    {
                        item = new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 14].Value.ToString(),
                            SortNo = 1,
                            GroupID = itemCategory.ID
                        };
                        itemList.Add(worksheet.Cells[row, 14].Value.ToString() + itemCategory.ID, item);
                    }

                    DataTransferCommon category;
                    if (categoryList.ContainsKey(worksheet.Cells[row, 3].Value.ToString()))
                    {
                        category = categoryList[worksheet.Cells[row, 3].Value.ToString()];
                    }
                    else
                    {
                        category = new DataTransferCommon()
                        {
                            ID = SingleIdWork.Instance(1).nextId(),
                            Name = worksheet.Cells[row, 3].Value.ToString(),
                            SortNo = 1,
                            GroupID = itemCategory.ID
                        };
                        categoryList.Add(worksheet.Cells[row, 3].Value.ToString(), category);
                    }

                    updateChargeItemList.Add(new DataTransferCommon()
                    {
                        ID = long.Parse(worksheet.Cells[row, 1].Value.ToString()),
                        GroupID = item.ID,
                        CategoryID = category.ID
                    });
                }
                _connection.Execute("delete from [SmartChargeCategory]",
                      itemGroupList.Values, _transaction);
                _connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo) values (@ID,@Name,@SortNo)",
                       itemGroupList.Values, _transaction);
                _connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo,ParentID) values (@ID,@Name,@SortNo,@GroupID)",
                       itemCategoryList.Values, _transaction);
                _connection.Execute("insert into [SmartChargeCategory](ID,Name,SortNo,ParentID) values (@ID,@Name,@SortNo,@GroupID)",
                       categoryList.Values, _transaction);

                _connection.Execute("delete from [SmartItemGroup]",
                      itemGroupList.Values, _transaction);
                _connection.Execute("insert into [SmartItemGroup](ID,Name,SortNo) values (@ID,@Name,@SortNo)",
                       itemGroupList.Values, _transaction);

                _connection.Execute("delete from [SmartItemChargeCategory]",
                      itemCategoryList.Values, _transaction);
                _connection.Execute("insert into [SmartItemChargeCategory](ID,Name,SortNo,GroupID) values (@ID,@Name,@SortNo,@GroupID)",
                       itemCategoryList.Values, _transaction);

                _connection.Execute("delete from [SmartItem]",
                                      itemList.Values, _transaction);
                _connection.Execute("insert into [SmartItem](ID,Name,SortNo,GroupID) values (@ID,@Name,@SortNo,@GroupID)",
                       itemList.Values, _transaction);


                _connection.Execute("update SmartCharge set CategoryID=@CategoryID,ItemID=@GroupID where ID=@ID",
                       updateChargeItemList, _transaction);
            }



            Console.WriteLine("渠道导入结束！");
        }

        public static void CaculateOrderRestNum()
        {
            Console.WriteLine("订单剩余次数开始计算");
            var orderDetailList = _connection.Query<OrderDetail>(@"select b.ID as OrderDetailID,b.Num,b.RestNum as RestNum,a.CustomerID,b.ChargeID,a.CreateTime  
from SmartOrder a
inner join SmartOrderDetail b on a.ID=b.OrderID
where a.PaidStatus in (2,3) and b.RestNum>0", null, _transaction);
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

            Console.WriteLine(i);

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

            _connection.Execute(@"update SmartOrderDetail set RestNum=a.RestNum-c.Num  
from SmartOrderDetail a 
inner join SmartOperationTest b on a.ID=b.OrderDetailID
inner join SmartOperation c on b.OperationID=c.ID 
", null, _transaction);

            _connection.Execute(@"update SmartOperation set OrderDetailID=b.OrderDetailID 
from SmartOperation a 
inner join SmartOperationTest b on a.ID=b.OperationID", null, _transaction);

            _connection.Execute(@"drop table SmartOperationTest", null, _transaction);

            Console.WriteLine("用户结束导入");
        }

        public static void Photo()
        {
            Console.WriteLine("照片开始迁移");
            DataTable callbackList = new DataTable("SmartPhoto");
            callbackList.Columns.Add("ID", typeof(long));
            callbackList.Columns.Add("CustomerID", typeof(long));
            callbackList.Columns.Add("CreateUserID", typeof(long));
            callbackList.Columns.Add("CreateTime", typeof(DateTime));
            callbackList.Columns.Add("ChargeID", typeof(long));
            callbackList.Columns.Add("Remark", typeof(string));
            callbackList.Columns.Add("ImageUrl", typeof(string));
            callbackList.Columns.Add("SymptomID", typeof(long));
            callbackList.Columns.Add("Type", typeof(int));
            callbackList.Columns.Add("ReducedImage", typeof(string));


            var list = _mySqlConnection.Query<Photo>(@$"select ID,CustomerID,CreateUserID,CreateTime,ChargeID,Remark,ImageUrl,SymptomID,Type,ZoomImageUrl as ReducedImage  from SmartPhoto", null, null, true, 6000);

            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                DataRow dr = callbackList.NewRow();
                dr["ID"] = u.ID;
                //dr["CustomerID"] = new Random().Next(958266,1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                if (u.ChargeID != null)
                {
                    dr["ChargeID"] = u.ChargeID;

                }
                if (u.SymptomID != null)
                {
                    dr["SymptomID"] = u.SymptomID;
                }
                dr["Remark"] = u.Remark;


                dr["ImageUrl"] = @"http://10.0.0.2/" + u.CustomerID + @"/" + u.ImageUrl.Substring(1, u.ImageUrl.Length - 1);
                dr["Type"] = u.Type;
                dr["ReducedImage"] = @"http://10.0.0.2/" + u.CustomerID + @"/" + u.ReducedImage.Substring(1, u.ReducedImage.Length - 1);

                callbackList.Rows.Add(dr);
            }

            if (callbackList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartPhoto", callbackList);
            }

            Console.WriteLine("照片结束迁移");
        }

        public static void CallBackTaskBL()
        {
            Console.WriteLine("回访计划记录开始迁移");
            DataTable callbackList = new DataTable("SmartTest");
            callbackList.Columns.Add("ID", typeof(long));
            callbackList.Columns.Add("Name", typeof(string));


            var list = _mySqlConnection.Query<CallBack>(@$"select a.ID,a.Name  
from SmartCallback a where a.Status=0 and a.TaskTime>'2020-12-01' and Name is not null and Name <> ''", null, null, true, 6000);

            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                DataRow dr = callbackList.NewRow();
                dr["ID"] = u.ID;
                if (u.Name == null)
                {
                    u.Name = "";
                }
                if (u.Name.Length > 50)
                {
                    dr["Name"] = u.Name.Substring(0, 50);
                }
                else
                {
                    dr["Name"] = u.Name;
                }

                callbackList.Rows.Add(dr);
            }

            if (callbackList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartTest", callbackList);


            }

            Console.WriteLine("回访计划记录结束迁移");
        }

        public static void TahGroup()
        {
            Console.WriteLine("渠道导入开始！");
            Dictionary<string, List<DataTransferChannel>> dic = new Dictionary<string, List<DataTransferChannel>>();
            using (var package = new ExcelPackage(new System.IO.FileInfo("D:\\哪吒智能\\昆明丽都\\标签模板.xlsx")))
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
                    if (worksheet.Cells[row, 1].Value == null)
                    {
                        throw new Exception("第" + row + "行标签组不能为空！");
                    }
                    groupName = worksheet.Cells[row, 1].Value.ToString().Trim();
                    if (worksheet.Cells[row, 2].Value == null)
                    {
                        throw new Exception("第" + row + "行标签不能为空！");
                    }
                    channelName = worksheet.Cells[row, 2].Value.ToString().Trim();



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
                            Remark = u.Remark,
                            Name = u.Name,
                            ChannelGroupID = channelGroupID,
                            Status = CommonStatus.Use
                        }); ;

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
                    _connection.Execute("insert into [SmartTag](ID,Content,TagGroupID,Status) values (@ID,@Name,@ChannelGroupID,@Status)",
                       channelAddList, _transaction);
                }
                if (groupAddList.Count > 0)
                {
                    _connection.Execute("insert into SmartTagGroup(ID,Name,Remark) values(@ID, @Name, @Remark)",
                 groupAddList, _transaction);
                }
            }



            Console.WriteLine("渠道导入结束！");
        }

        /// <summary>
        /// 收银方式
        /// </summary>
        public static void Tag()
        {
            Console.WriteLine("标签开始导入");
            var list = _mySqlConnection.Query<Tag>(@"select ID,Name as Content 
from SmartTag");

            foreach (var u in list)
            {
                u.TagGroupID = 1;
                u.Status = CommonStatus.Use;
                u.NotCallBack = CommonStatus.Stop;
                u.NotSend = CommonStatus.Stop;
                u.NotSSM = CommonStatus.Stop;
            }

            _connection.Execute(@"insert into SmartTagGroup values(1,'未分组','初始化添加')", null, _transaction);

            _connection.Execute("insert into [SmartTag]([ID],[Content],[Status],NotCallBack,NotSend,NotSSM,TagGroupID) values (@ID,@Content,@Status,@NotCallBack,@NotSend,@NotSSM,@TagGroupID)",
                   list, _transaction);

            Console.WriteLine("标签结束导入");
        }

        /// <summary>
        /// 收银方式
        /// </summary>
        public static void CardCategory()
        {
            Console.WriteLine("收银方式开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select ID,Name,Remark,Status 
from SmartCardCategory  where ID not in (1,2) ");

            _connection.Execute(@"insert into [SmartCardCategory](ID,Name,[Status],Remark) values (@ID,@Name,@Status,@Remark)", list, _transaction);

            Console.WriteLine("收银方式结束导入");
        }

        /// <summary>
        /// 未成交
        /// </summary>
        public static void FailtureCategory()
        {
            Console.WriteLine("未成交开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select ID,Name,Remark,Status 
from SmartFailtureCategory");

            _connection.Execute(@"insert into [SmartFailtureCategory](ID,Name,[Status],Remark) values (@ID,@Name,@Status,@Remark)", list, _transaction);

            Console.WriteLine("未成交结束导入");
        }

        /// <summary>
        /// 回访类型
        /// </summary>
        public static void CallBackCategory()
        {
            Console.WriteLine("回访类型开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select ID,Name,Status,Remark 
  from SmartCallbackCategory");


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
            var list = _mySqlConnection.Query<CallBackGroup>(@"select ID,Name,Remark 
from SmartCallbackSet a");

            var detailList = _mySqlConnection.Query<SmartCallbackGroupDetail>(@"select a.ID,a.SetID,a.CategoryID,a.Name,a.Days
from SmartCallbackSetDetail a 
");

            foreach (var u in list)
            {
                u.Status = CommonStatus.Use;
            }

            _connection.Execute("insert into [SmartCallbackSet](ID,Name,[Status],Remark) values (@ID,@Name,@Status,@Remark)",
                 list, _transaction);

            _connection.Execute("insert into [SmartCallbackSetDetail](ID,[SetID],[CategoryID],[Name],[Days]) values (@ID,@SetID,@CategoryID,@Name,@Days)",
                       detailList, _transaction);

            Console.WriteLine("回访组结束导入");
        }

        /// <summary>
        /// 供应商
        /// </summary>
        public static void Factory()
        {
            Console.WriteLine("生产厂商开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select a.ID,a.Name,a.Remark,a.PinYin 
  from SmartFactory a");

            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                u.Status = CommonStatus.Use;
                u.CreateTime = now;
                u.CreateUserID = 1;
            }

            _connection.Execute(@"insert into SmartFactory(ID,Name,Remark,Status,CreateTime,CreateUserID) 
values (@ID,@Name,@Remark,@Status,@CreateTime,@CreateUserID)", list, _transaction);

            Console.WriteLine("生产厂商结束导入");
        }

        /// <summary>
        /// 供应商
        /// </summary>
        public static void Supplier()
        {
            Console.WriteLine("供应商开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select ID,Name,LinkMan,Contact,Remark,PinYin from SmartSupplier");

            foreach (var u in list)
            {
                if (u.Remark.IsNullOrEmpty())
                {
                    u.Remark = "数据迁移";
                }
                u.HospitalID = _hospitalID;
                if (u.LinkMan == null)
                {
                    u.LinkMan = "";
                }
                if (u.Contact == null)
                {
                    u.Contact = "";
                }
            }

            _connection.Execute(@"insert into SmartSupplier(ID,Name,LinkMan,Contact,Remark,PinYin,HospitalID) 
values (@ID,@Name,@LinkMan,@Contact,@Remark,@PinYin,@HospitalID)", list, _transaction);

            Console.WriteLine("供应商结束导入");
        }

        /// <summary>
        /// 渠道
        /// </summary>
        public static void Channel()
        {
            Console.WriteLine("渠道导入开始！");
            var channeGrouplList = _mySqlConnection.Query<DataTransferChannel>(@"select c.ID,c.Name,c.SortNo,c.Remark  from SmartChannelGroup c");
            var channeList = _mySqlConnection.Query<DataTransferChannel>(@"select a.ID,a.Name,a.Status,a.SortNo,a.Remark,b.GroupID as ChannelGroupID
from SmartChannel a 
left join SmartChannelGroupDetail b on a.ID=b.ChannelID");


            foreach (var u in channeList)
            {
                if (u.Remark.IsNullOrEmpty())
                {
                    u.Remark = "数据迁移导入";
                }
                else
                {
                    u.Remark = u.Remark + "，数据迁移导入";
                }
            }
            foreach (var u in channeGrouplList)
            {
                if (u.Remark.IsNullOrEmpty())
                {
                    u.Remark = "数据迁移导入";
                }
                else
                {
                    u.Remark = u.Remark + "，数据迁移导入";
                }
            }

            if (channeList.Count() > 0)
            {
                _connection.Execute("insert into [SmartChannel](ID,Name,[Status],SortNo,Remark,ChannelGroupID) values (@ID,@Name,@Status,@SortNo,@Remark,@ChannelGroupID)",
                   channeList, _transaction);
            }
            if (channeGrouplList.Count() > 0)
            {
                _connection.Execute("insert into SmartChannelGroup(ID,Name,SortNo,Remark) values(@ID, @Name,@SortNo, @Remark)", channeGrouplList, _transaction);
            }

            Console.WriteLine("渠道导入结束！");
        }

        /// <summary>
        /// 产品分类
        /// </summary>
        public static void ProductCategory()
        {
            Console.WriteLine("物品分类开始导入");
            var list = _mySqlConnection.Query<ProductCategory>(@"select ID,Name,Remark 
from SmartProductCategory");

            _connection.Execute("insert into SmartProductCategory(ID,Name,PID,SortNo,Remark) values (@ID,@Name,@PID,@SortNo,@Remark)",
                    list, _transaction);
            //_connection.Execute("insert into SmartChargeCategory(ID,Name,ParentID,SortNo,Remark) values (@ID,@Name,@PID,@SortNo,@Remark)",
            //         list, _transaction);
            Console.WriteLine("物品分类结束导入");
        }

        /// <summary>
        /// 单位
        /// </summary>
        public static void Unit()
        {
            Console.WriteLine("单位开始导入");
            var list = _mySqlConnection.Query<ProductCategory>(@"select ID,Name from SmartUnit");

            _connection.Execute("insert into SmartUnit(ID,Name) values (@ID,@Name)",
                    list, _transaction);
            Console.WriteLine("单位结束导入");
        }

        /// <summary>
        /// 仓库
        /// </summary>
        public static void Warehouse()
        {
            Console.WriteLine("仓库开始导入");
            var list = _mySqlConnection.Query<Product>(@"select a.ID,a.Name, a.Remark,a.Type as UnitType,1 as HospitalID 
from SmartWarehouse a
");

            var listDetail = _mySqlConnection.Query<Product>(@"select *
from SmartWarehouseManager a
");


            _connection.Execute(@"insert into SmartWarehouse(ID,Name,Remark,UnitType,HospitalID)
 values(@ID,@Name,@Remark,@UnitType,@HospitalID)", list, _transaction);

            _connection.Execute(@"insert into SmartWarehouseManager(ID,WarehouseID,UserID)
 values(@ID,@WarehouseID,@UserID)", listDetail, _transaction);

            Console.WriteLine("仓库结束导入");
        }

        /// <summary>
        /// 产品
        /// </summary>
        public static void Product()
        {
            Console.WriteLine("产品开始导入");
            var list = _mySqlConnection.Query<Product>(@"select a.ID,a.Name,a.PinYin,a.CategoryID,a.Size,a.Price,a.Status,a.Remark,
a.UnitID,a.UnitID as MiniUnitID,1 as Scale,b.Price as SalePrice,b.CategoryID as ChargeCategoryID,a.ChargeID

from SmartProduct a
left join SmartCharge b on a.ChargeID=b.ID
");
            foreach (var u in list)
            {
                if (u.ChargeID == null)
                {
                    u.IsSale = CommonStatus.Stop;
                }
                else
                {
                    u.IsSale = CommonStatus.Use;
                }
                if (u.CategoryID == null)
                {
                    u.CategoryID = 11;
                }
            }

            var warehouseList = _mySqlConnection.Query<Product>(@"select ProductID,WarehouseID,1 as HospitalID 
from SmartRetailconfig where ProductID is not null and WarehouseID is not null");
            if (warehouseList.Count() > 0)
            {
                _connection.Execute(@"insert into SmartProductWarehouse(ProductID,WarehouseID,HospitalID) values(@ProductID,@WarehouseID,@HospitalID)", warehouseList, _transaction);
            }

            _connection.Execute(@"insert into SmartProduct(ID,Name,PinYin,CategoryID,Size,Price,[Status],Remark,UnitID,MiniUnitID,Scale,
IsSale,SalePrice,WarehouseID,IsEvaluate,ChargeCategoryID)
 values(@ID, @Name, @PinYin, @CategoryID, @Size, @Price, @Status, @Remark, @UnitID, @MiniUnitID, @Scale,@IsSale,@SalePrice,
@WarehouseID,@IsEvaluate,@ChargeCategoryID)",
                   list, _transaction);

            Console.WriteLine("产品结束导入");
        }

        /// <summary>
        /// 项目分类
        /// </summary>
        public static void ChargeCategory()
        {
            Console.WriteLine("项目分类开始导入");
            var list = _mySqlConnection.Query<ChargeCategory>(@"  select a.ID,a.Name,a.ParentID,a.Remark,a.SortNo from SmartChargeCategory a");

            foreach (var u in list)
            {
                if (u.ParentID == 0)
                {
                    u.ParentID = null;
                }
            }

            _connection.Execute("insert into SmartChargeCategory(ID,Name,ParentID,SortNo,Remark) values (@ID,@Name,@ParentID,@SortNo,@Remark)",
                     list, _transaction);
            Console.WriteLine("项目分类结束导入");
        }

        /// <summary>
        /// 项目
        /// </summary>
        public static void Charge()
        {
            Console.WriteLine("项目开始导入");
            var list = _mySqlConnection.Query<Charge>(@"select a.ID,a.Name,a.CategoryID,a.PinYin,a.Price,a.Status,a.Remark,a.UnitID,a.Size,a.ProductAdd,b.ItemID,a.ProductType,c.ID as ProductID 
  from SmartCharge a
  left join SmartItemChargeDetail b on a.ID=b.ChargeID 
	left join SmartProduct c on a.ID=c.ChargeID 
");
            var productList = _mySqlConnection.Query<ChargeProductDetail>(@"select a.ID,a.ChargeID,a.ProductID,a.Num as MinNum,a.MaxNum
  from SmartChargeProductDetail a");
            foreach (var u in list)
            {
                u.IsEvaluate = CommonStatus.Use;
                if (u.ProductID != null)
                {
                    u.Type = ChargeType.Product;
                }
                else
                {
                    u.Type = ChargeType.Charge;
                }

                if (u.CategoryID == null)
                {
                    u.CategoryID = 999;
                }
            }

            _connection.Execute(@"insert into SmartCharge(ID,Name,CategoryID,PinYin,Price,Status,Remark,UnitID,Size,ProductAdd,IsEvaluate,ItemID,Type,ProductID)
 values(@ID, @Name, @CategoryID, @PinYin, @Price, @Status, @Remark, @UnitID,@Size,@ProductAdd,@IsEvaluate,@ItemID,@Type,@ProductID)", list, _transaction);  //收费项目

            if (productList.Count() > 0)
            {
                _connection.Execute(@"insert into SmartChargeProductDetail(ID,ChargeID,ProductID,MinNum,MaxNum) 
 VALUES(@ID, @ChargeID, @ProductID, @MinNum, @MaxNum)", productList, _transaction);
            }

            Console.WriteLine("项目结束导入");
        }


        /// <summary>
        /// 部门
        /// </summary>
        public static void Dept()
        {
            Console.WriteLine("部门开始导入");
            var list = _mySqlConnection.Query<Dept>(@$"select ID,Name,Remark,OpenStatus as IsTriage,SortNo from SmartDept");

            foreach (var u in list)
            {
                u.HospitalID = 1;
                if (u.Remark.IsNullOrEmpty())
                {
                    u.Remark = "数据迁移导入";
                }
                else
                {
                    u.Remark += "，数据迁移导入";
                }
                u.OpenStatus = CommonStatus.Use;
            }

            _connection.Execute("insert into SmartDept(ID,Name,Remark,OpenStatus,SortNo,HospitalID,IsTriage) values (@ID,@Name,@Remark,@OpenStatus,@SortNo,@HospitalID,@IsTriage)",
                    list, _transaction);

            Console.WriteLine("部门结束导入");
        }

        /// <summary>
        /// 代金券
        /// </summary>
        public static void CouponCategory()
        {
            Console.WriteLine("代金券类型开始导入");
            var list = _mySqlConnection.Query<CouponCategoryInfo>(@$"select a.ID,a.Name,a.ScopeLimit,a.ChargeIDs as ChargeID,a.ChargeCategoryIDs as ChargeCategoryID,a.TimeLimit,a.EndDate,a.Days,a.Remark,a.Status 
from SmartCouponCategory a
");
            var hospitalList = new List<object>();
            var couponChargeList = new List<object>();
            var couponChargeCategoryList = new List<object>();

            foreach (var u in list)
            {
                if (u.ScopeLimit == 2 && u.ChargeCategoryID != null)
                {
                    couponChargeCategoryList.Add(new
                    {
                        ID = SingleIdWork.Instance(1).nextId(),
                        CouponCategoryID = u.ID,
                        ChargeCategoryID = u.ChargeCategoryID
                    });
                }

                if (u.ScopeLimit == 3 && u.ChargeID != null)
                {
                    couponChargeList.Add(new
                    {
                        ID = SingleIdWork.Instance(1).nextId(),
                        CouponCategoryID = u.ID,
                        ChargeID = u.ChargeID
                    });
                }
                hospitalList.Add(new
                {
                    ID = SingleIdWork.Instance(1).nextId(),
                    CouponCategoryID = u.ID,
                    HospitalID = 1
                });
            }

            _connection.Execute(@"insert into SmartCouponCategory(ID,Name,ScopeLimit,ChargeID,ChargeCategoryID,TimeLimit,EndDate,Days,Remark,Status) 
                        VALUES(@ID, @Name, @ScopeLimit, @ChargeID, @ChargeCategoryID, @TimeLimit, @EndDate, @Days, @Remark, @Status)", list, _transaction);

            _connection.Execute(@"insert into SmartCouponCategoryHospital(ID,CouponCategoryID,HospitalID) VALUES(@ID, @CouponCategoryID, @HospitalID)", hospitalList, _transaction);
            if (couponChargeList.Count() > 0)
            {
                _connection.Execute(@"insert into [SmartCouponCategoryCharge](ID,CouponCategoryID,ChargeID) VALUES(@ID, @CouponCategoryID, @ChargeID)", couponChargeList, _transaction);
            }
            if (couponChargeCategoryList.Count() > 0)
            {
                _connection.Execute(@"insert into SmartCouponCategoryChargeCategory(ID,CouponCategoryID,ChargeCategoryID) VALUES(@ID, @CouponCategoryID, @ChargeCategoryID)", couponChargeCategoryList, _transaction);
            }

            Console.WriteLine("代金券类型结束导入");
        }


        /// <summary>
        /// 预收款类型
        /// </summary>
        public static void DepositCategory()
        {
            Console.WriteLine("预收款类型开始导入");
            var list = _mySqlConnection.Query<DepositChargeInfo>(@$"SELECT ID,Name,Price,Status,ScopeLimit,HasCoupon,CouponCategoryID,CouponAmount,Remark
FROM SmartDepositCharge
");
            var hospitalList = new List<object>();
            var couponChargeList = new List<object>();
            var couponChargeCategoryList = new List<object>();

            foreach (var u in list)
            {
                if (u.ScopeLimit == 2 && u.ChargeCategoryID != null)
                {
                    couponChargeCategoryList.Add(new
                    {
                        ID = SingleIdWork.Instance(1).nextId(),
                        CouponCategoryID = u.ID,
                        ChargeCategoryID = u.ChargeCategoryID
                    });
                }

                if (u.ScopeLimit == 3 && u.ChargeID != null)
                {
                    couponChargeList.Add(new
                    {
                        ID = SingleIdWork.Instance(1).nextId(),
                        CouponCategoryID = u.ID,
                        ChargeID = u.ChargeID
                    });
                }
                hospitalList.Add(new
                {
                    ID = SingleIdWork.Instance(1).nextId(),
                    CouponCategoryID = u.ID,
                    HospitalID = 1
                });
                u.IsShopOnly = 0;
            }

            _connection.Execute(@"insert into SmartDepositCharge(ID,Name,Price,Status,ScopeLimit,ChargeID,ChargeCategoryID,
HasCoupon,CouponCategoryID,CouponAmount,Remark,IsShopOnly) 
                    VALUES(@ID, @Name, @Price, @Status, @ScopeLimit, @ChargeID, @ChargeCategoryID, @HasCoupon, @CouponCategoryID, @CouponAmount,@Remark,@IsShopOnly)", list, _transaction);

            _connection.Execute("insert into SmartDepositChargeHospital(ID,DepositChargeID,HospitalID) VALUES(@ID, @CouponCategoryID, @HospitalID)", hospitalList, _transaction); //预收款适用医院映射表

            if (couponChargeList.Count() > 0)
            {
                _connection.Execute(@"insert into [SmartDepositChargeCharge](ID,DepositChargeID,ChargeID) VALUES(@ID, @CouponCategoryID, @ChargeID)", couponChargeList, _transaction);
            }
            if (couponChargeCategoryList.Count() > 0)
            {
                _connection.Execute(@"insert into SmartDepositChargeChargeCategory(ID,DepositChargeID,ChargeCategoryID) VALUES(@ID, @CouponCategoryID, @ChargeCategoryID)", couponChargeCategoryList, _transaction);
            }
            _connection.Execute(@"insert into SmartDepositCouponSend  select ID,CouponCategoryID,CouponAmount from SmartDepositCharge where HasCoupon=1", null, _transaction);
            Console.WriteLine("预收款类型结束导入");
        }


        /// <summary>
        /// 用户
        /// </summary>
        public static void User()
        {
            Console.WriteLine("用户开始导入");
            var list = _mySqlConnection.Query<User>(@"select a.ID,a.Account,a.Password,a.Name,a.Gender,a.DeptID,a.Status,a.Remark,a.Phone,a.Mobile 
from SmartUser a where a.ID<>1
");

            DateTime now = DateTime.Now;
            List<UserRole> roleList = new List<UserRole>();
            foreach (var u in list)
            {
                u.HospitalID = _hospitalID;
                u.Discount = 1;
                u.CreateTime = now;
                u.CreateUserID = _hospitalID;
                if (u.Password.IsNullOrEmpty())
                {
                    u.Password = "123456";
                }
                u.Password = HashHelper.GetMd5(u.Password);

                roleList.Add(new UserRole()
                {
                    RoleID = 16527970138637312,
                    UserID = u.ID,
                    ID = u.ID
                });
            }

            _connection.Execute(
                    "insert into SmartUser([ID],[Account],[Password],[Name],[Gender],[DeptID],[Status],[Remark],[Phone],[HospitalID],[Discount],[CreateTime],[CreateUserID],Mobile) " +
                    "values(@ID,@Account,@Password,@Name,@Gender,@DeptID,@Status,@Remark,@Phone,@HospitalID,@Discount,@CreateTime,@CreateUserID,@Mobile)",
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
            customerAddList.Columns.Add("MemberCategoryID", typeof(long));
            customerAddList.Columns.Add("Mobile", typeof(string));
            customerAddList.Columns.Add("Source", typeof(int));
            customerAddList.Columns.Add("PromoterID", typeof(long));
            customerAddList.Columns.Add("Commission", typeof(decimal));
            customerAddList.Columns.Add("Point", typeof(decimal));
            customerAddList.Columns.Add("MobileBackup", typeof(string));
            customerAddList.Columns.Add("QQ", typeof(string));
            customerAddList.Columns.Add("WeChat", typeof(string));
            customerAddList.Columns.Add("CurrentConsultSymptomID", typeof(long));
            customerAddList.Columns.Add("Custom1", typeof(string));
            customerAddList.Columns.Add("Custom2", typeof(string));
            customerAddList.Columns.Add("Custom3", typeof(string));
            customerAddList.Columns.Add("Custom4", typeof(string));
            customerAddList.Columns.Add("Custom5", typeof(string));
            customerAddList.Columns.Add("Custom6", typeof(string));
            customerAddList.Columns.Add("Custom7", typeof(string));
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

            DataTable pointList = new DataTable("SmartPoint");
            pointList.Columns.Add("CustomerID", typeof(long));
            pointList.Columns.Add("CreateUserID", typeof(long));
            pointList.Columns.Add("CreateTime", typeof(string));
            pointList.Columns.Add("Type", typeof(int));
            pointList.Columns.Add("Amount", typeof(decimal));
            pointList.Columns.Add("Remark", typeof(string));
            pointList.Columns.Add("HospitalID", typeof(long));
            pointList.Columns.Add("ConsumeAmount", typeof(decimal));
            pointList.Columns.Add("FromHospitalID", typeof(long));

            var list = _mySqlConnection.Query<Customer>(@$"SELECT a.ID,a.Name,Gender,Mobile,Mobile2 as MobileBackup,CreateTime,ChannelID,a.Remark,CreateUserID,0 as Point
      ,BirthYear,BirthDay as BirthDay2 ,c.Name+b.Name+b0.`Name`+a.Address as Address,SymptomID,d.Userid as CurrentExploitUserID,e.UserID as CurrentManagerUserID,a.FN as Custom3,Custom1,Custom2,Custom4,Custom5,Custom6,Custom7,Custom8,Custom9,Custom10 
FROM SmartCustomer a
left join SmartDistrict b0 on a.DistrictID=b0.ID
left join SmartCity b on b0.CityID=b.ID
left join SmartProvince c on b.ProvinceID=c.ID
left join smartownership d on a.ID=d.CustomerID and d.Type=1 and d.EndTime='2099-12-31 00:00:00'
left join smartownership e on a.ID=e.CustomerID and e.Type=2 and e.EndTime='2099-12-31 00:00:00'", null, null, true, 6000);

            List<object> commissionList = new List<object>();
            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                if (u.ChannelID == null)
                {
                    u.ChannelID = 10;
                }
                u.Address = u.ProName + u.CityName + u.Address;

                if (u.CreateTime == null)
                {
                    u.CreateTime = DateTime.Parse("2021-01-01");
                }


                DataRow dr = customerAddList.NewRow();
                dr["ID"] = u.ID;
                dr["Name"] = u.Name;
                dr["Gender"] = u.Gender;
                if (u.Remark.IsNullOrEmpty())
                {
                    dr["Remark"] = "数据迁移";
                }
                else
                {
                    if (u.Remark.Length > 1999)
                    {
                        u.Remark = u.Remark.Substring(0, 1999);
                    }
                    dr["Remark"] = u.Remark;
                }
                if (u.BirthYear != null)
                {
                    if (u.BirthDay2.IsNullOrEmpty())
                    {
                        dr["Birthday"] = DateTime.Parse(u.BirthYear + "-07-01").ToShortDateString();
                    }
                    else
                    {
                        dr["Birthday"] = DateTime.Parse(u.BirthYear + "-" + u.BirthDay2).ToShortDateString();
                    }
                }
                dr["Custom1"] = u.Custom1;
                dr["Custom2"] = u.Custom2;
                dr["Custom3"] = u.Custom3;
                dr["Custom4"] = u.Custom4;
                dr["Custom5"] = u.Custom5;
                dr["Custom6"] = u.Custom6;
                dr["Custom7"] = u.Custom7;
                dr["Custom8"] = u.Custom8;
                dr["Custom9"] = u.Custom9;
                dr["Custom10"] = u.Custom10;

                //if (u.Age != null)
                //{
                //    if (u.Birthday != null)
                //    {
                //        int month = u.Birthday.Value.Month;
                //        int day = u.Birthday.Value.Day;
                //        if (month == 2)
                //        {
                //            if (day >= 29)
                //            {
                //                day = 28;
                //            }
                //        }
                //        dr["Birthday"] = DateTime.Parse(DateTime.Today.Year + "/" + month + "/" + day).ToShortDateString();
                //    }
                //    else
                //    {
                //        dr["Birthday"] = u.CreateTime.Value.AddYears(u.Age.Value * -1).ToShortDateString();
                //    }
                //}


                dr["CreateTime"] = u.CreateTime.ToString();
                dr["ChannelID"] = u.ChannelID;
                dr["CreateUserHospitalID"] = _hospitalID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["MemberCategoryID"] = 0;
                //if (u.Mobile.Length >= 19)
                //{
                //    u.Mobile = u.Mobile.Substring(0, 19);
                //}
                dr["Mobile"] = u.Mobile;
                dr["Source"] = 7;
                dr["Commission"] = u.Commission;
                dr["Point"] = u.Point;
                //if (!u.MobileBackup.IsNullOrEmpty() && u.MobileBackup.Length > 19)
                //{
                //    u.MobileBackup = u.MobileBackup.Substring(0, 19);
                //}
                dr["MobileBackup"] = u.MobileBackup;
                dr["QQ"] = u.QQ;
                dr["WeChat"] = u.WeChat;
                //dr["Custom2"] = u.Custom2;

                if (u.CurrentConsultSymptomID != null)
                {
                    dr["CurrentConsultSymptomID"] = u.CurrentConsultSymptomID;
                }

                customerAddList.Rows.Add(dr);


                if (u.Point > 0)
                {
                    DataRow drPoint = pointList.NewRow();
                    drPoint["CustomerID"] = u.ID;
                    drPoint["CreateUserID"] = _hospitalID;
                    drPoint["CreateTime"] = now.ToString();
                    drPoint["Type"] = 26;
                    drPoint["Amount"] = u.Point;
                    drPoint["Remark"] = "数据迁移";
                    drPoint["HospitalID"] = _hospitalID;
                    drPoint["ConsumeAmount"] = 0;
                    drPoint["FromHospitalID"] = _hospitalID;

                    pointList.Rows.Add(drPoint);
                }
                if (u.Commission > 0)
                {
                    commissionList.Add(new
                    {
                        CustomerID = u.ID,
                        CreateTime = now,
                        CreateUserID = _hospitalID,
                        Type = 5,
                        HospitalID = _hospitalID,
                        Commission = u.Commission,
                        Remark = "数据迁移",
                        FromHospitalID = _hospitalID
                    });
                }

                if (u.CurrentExploitUserID != null)
                {
                    DataRow dr2 = ownerShipAddList.NewRow();
                    dr2["CustomerID"] = u.ID;
                    dr2["EndTime"] = "9999-12-31 23:59:59";
                    dr2["StartTime"] = u.CreateTime.ToString();
                    dr2["HospitalID"] = _hospitalID;
                    dr2["Remark"] = "数据迁移";
                    dr2["Type"] = 1;
                    dr2["UserID"] = u.CurrentExploitUserID;
                    ownerShipAddList.Rows.Add(dr2);
                }
                if (u.CurrentManagerUserID != null)
                {
                    DataRow dr2 = ownerShipAddList.NewRow();
                    dr2["CustomerID"] = u.ID;
                    dr2["EndTime"] = "9999-12-31 23:59:59";
                    dr2["StartTime"] = u.CreateTime.ToString();
                    dr2["HospitalID"] = _hospitalID;
                    dr2["Remark"] = "数据迁移";
                    dr2["Type"] = 2;
                    dr2["UserID"] = u.CurrentManagerUserID;
                    ownerShipAddList.Rows.Add(dr2);
                }
            }

            if (customerAddList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartCustomer", customerAddList);
            }
            if (ownerShipAddList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartOwnerShip", ownerShipAddList);
            }

            if (pointList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartPoint", pointList);
            }
            if (commissionList.Count > 0)
            {
                _connection.Execute(@"insert into SmartCommissionRecord(CustomerID,CreateTime,CreateUserID,Type,HospitalID,Commission,Remark,FromHospitalID) 
values(@CustomerID,@CreateTime,@CreateUserID,@Type,@HospitalID,@Commission,@Remark,@FromHospitalID)", commissionList, _transaction);

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

            Console.WriteLine("顾客结束迁移");
        }

        // <summary>
        /// 报表项目组
        /// </summary>
        public static void ItemGroup()
        {
            Console.WriteLine("报表项目组开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select ID,Name,SortNo,Remark from SmartItemGroup ");

            foreach (var u in list)
            {
                if (u.Remark.IsNullOrEmpty())
                {
                    u.Remark = "数据迁移导入";
                }
                else
                {
                    u.Remark += "，数据迁移导入";
                }
            }

            _connection.Execute(@"insert into SmartItemGroup(ID,Name,SortNo,Remark) values (@ID,@Name,@SortNo,@Remark)", list, _transaction);

            Console.WriteLine("报表项目组开始导入");
        }

        // <summary>
        /// 报表项目
        /// </summary>
        public static void Item()
        {
            Console.WriteLine("报表项目开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"
select a.ID,a.Name,a.SortNo,a.Remark,b.GroupID 
from SmartItem a
left join SmartItemGroupDetail b on a.ID=b.ItemID");

            foreach (var u in list)
            {
                if (u.Remark.IsNullOrEmpty())
                {
                    u.Remark = "数据迁移导入";
                }
                else
                {
                    u.Remark += "，数据迁移导入";
                }
            }
            _connection.Execute(@"insert into SmartItemChargeCategory(ID,Name,SortNo,Remark,GroupID) values(@ID, @Name,@SortNo,@Remark,@GroupID)", list, _transaction);
            _connection.Execute(@"insert into SmartItem(ID,Name,SortNo,Remark,GroupID) values(@ID, @Name,@SortNo,@Remark,@ID)", list, _transaction);

            Console.WriteLine("报表项目结束导入");
        }

        // <summary>
        /// 咨询症状
        /// </summary>
        public static void Symptom()
        {
            Console.WriteLine("咨询症状开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select a.ID,a.Name,a.SortNo,a.Status,a.Remark,b.ItemID 
from SmartSymptom a
left join SmartItemSymptomDetail b on a.ID=b.SymptomID");

            foreach (var u in list)
            {
                if (u.Remark.IsNullOrEmpty())
                {
                    u.Remark = "数据迁移导入";
                }
                else
                {
                    u.Remark += "，数据迁移导入";
                }
            }

            _connection.Execute(@"insert into [SmartSymptom](ID,Name,[Status],SortNo,Remark,ItemID) 
values (@ID,@Name,@Status,@SortNo,@Remark,@ItemID)", list, _transaction);

            Console.WriteLine("咨询症状结束导入");
        }

        /// <summary>
        /// 咨询
        /// </summary>
        public static void Consult()
        {
            Console.WriteLine("咨询记录开始迁移");
            DataTable consultList = new DataTable("SmartConsult");
            consultList.Columns.Add("ID", typeof(long));
            consultList.Columns.Add("CustomerID", typeof(long));
            consultList.Columns.Add("CreateUserID", typeof(long));
            consultList.Columns.Add("CreateTime", typeof(DateTime));
            consultList.Columns.Add("Tool", typeof(long));
            consultList.Columns.Add("Content", typeof(string));
            consultList.Columns.Add("HospitalID", typeof(long));

            DataTable detailList = new DataTable("SmartConsultSymptomDetail");
            detailList.Columns.Add("ConsultID", typeof(long));
            detailList.Columns.Add("SymptomID", typeof(long));


            var list = _mySqlConnection.Query<Consult>(@"select a.ID,a.CustomerID,a.CreateUserID,a.CreateTime,a.Content
  from SmartConsult a", null, null, true, 60000);



            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                DataRow dr = consultList.NewRow();
                dr["ID"] = u.ID;
                //dr["CustomerID"] = new Random().Next(958266,1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["Tool"] = 16552323585328128;
                dr["HospitalID"] = _hospitalID;
                if (u.Content == null)
                {
                    u.Content = "";
                }
                if (u.Content.Length >= 1999)
                {
                    dr["Content"] = u.Content.Substring(0, 1999);
                }
                else
                {
                    dr["Content"] = u.Content;
                }
                consultList.Rows.Add(dr);



            }

            var symptomList = _mySqlConnection.Query<ConsultDetail>(@"select ID as ConsultID,SymptomID
from SmartConsult a
union all 
select ID as ConsultID,Symptom2ID
from SmartConsult a where Symptom2ID is not null and Symptom2ID<>''
union all
select ID as ConsultID,Symptom3ID
from SmartConsult a where Symptom3ID is not null and Symptom3ID<>''
union all
select ID as ConsultID,Symptom4ID
from SmartConsult a where Symptom4ID is not null and Symptom4ID<>''
union all
select ID as ConsultID,Symptom5ID
from SmartConsult a where Symptom5ID is not null and Symptom5ID<>''", null, null, true, 60000);
            foreach (var u in symptomList)
            {
                DataRow dr2 = detailList.NewRow();
                dr2["ConsultID"] = u.ConsultID;
                dr2["SymptomID"] = u.SymptomID;
                detailList.Rows.Add(dr2);
            }

            if (consultList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartConsult", consultList);
            }
            if (detailList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartConsultSymptomDetail", detailList);
            }

            //1、更新首次咨询时间、最后咨询时间、咨询次数
            _connection.Execute(@"update SmartCustomer set FirstConsultTime=min,LastConsultTime=max,ConsultTimes=count 
  from SmartCustomer a
  inner join (select CustomerID,MIN(createtime) as min,MAX(CreateTime) as max,count(CustomerID) as count 
  from SmartConsult group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);

            Console.WriteLine("咨询记录结束迁移");
        }


        /// <summary>
        /// 回访计划记录
        /// </summary>
        public static void CallBackTask()
        {
            Console.WriteLine("回访计划记录开始迁移");
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


            var list = _mySqlConnection.Query<CallBack>(@$"select a.ID,a.CustomerID,a.TaskCreateTime as CreateTime,a.TaskCreateUserID as CreateUserID,a.UserID,a.TaskTime,a.CategoryID,a.Name  
from SmartCallback a where a.Status=0 and a.TaskTime>='2023-10-01'", null, null, true, 6000);

            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                DataRow dr = callbackList.NewRow();
                dr["ID"] = u.ID;
                //dr["CustomerID"] = new Random().Next(958266,1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["CategoryID"] = u.CategoryID;
                if (u.Name == null)
                {
                    u.Name = "";
                }
                if (u.Name.Length > 200)
                {
                    dr["Name"] = u.Name.Substring(0, 200);
                }
                else
                {
                    dr["Name"] = u.Name;
                }

                dr["UserID"] = u.UserID;
                dr["TaskTime"] = u.TaskTime;
                dr["Status"] = 0;
                dr["HospitalID"] = _hospitalID;

                callbackList.Rows.Add(dr);
            }

            if (callbackList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartCallback", callbackList);
            }

            Console.WriteLine("回访计划记录结束迁移");
        }


        /// <summary>
        /// 回访记录
        /// </summary>
        public static void CallBack()
        {
            Console.WriteLine("回访记录开始迁移");
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
            callbackList.Columns.Add("LastUpdateTime", typeof(DateTime));
            callbackList.Columns.Add("LastUpdateUserID", typeof(long));
            callbackList.Columns.Add("ResultType", typeof(long));

            var list = _mySqlConnection.Query<CallBack>(@$"select a.ID,a.CustomerID,a.TaskCreateTime as CreateTime,a.TaskCreateUserID as CreateUserID,a.UserID,a.TaskTime,a.CategoryID,
a.CreateTime as TaskCreateTime,a.CreateUserID as TaskCreateUserID,a.Content,a.Name 
from SmartCallback a where a.Status=1", null);

            foreach (var u in list)
            {
                DataRow dr = callbackList.NewRow();
                dr["ID"] = u.ID;
                //dr["CustomerID"] = new Random().Next(958266,1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["CategoryID"] = u.CategoryID;
                if (u.Name == null)
                {
                    u.Name = "";
                }
                if (u.Name != null && u.Name.Length > 500)
                {
                    dr["Name"] = u.Name.Substring(0, 500);
                }
                else
                {
                    dr["Name"] = u.Name;
                }
                if (u.Content == null)
                {
                    u.Content = "";
                }
                if (u.Content != null && u.Content.Length > 1999)
                {
                    dr["Content"] = u.Content.Substring(0, 1999);
                }
                else
                {
                    dr["Content"] = u.Content;
                }

                dr["UserID"] = u.UserID;
                dr["TaskTime"] = u.TaskTime;
                dr["Status"] = 1;
                dr["HospitalID"] = _hospitalID;
                dr["Tool"] = 16552323585328128;
                dr["TaskCreateTime"] = u.TaskCreateTime;
                dr["TaskCreateUserID"] = u.TaskCreateUserID;
                dr["LastUpdateTime"] = u.TaskCreateTime;
                dr["LastUpdateUserID"] = u.TaskCreateUserID;
                dr["ResultType"] = 16528052897645568;


                callbackList.Rows.Add(dr);
            }
            if (callbackList.Rows.Count > 0)
            {
                Console.WriteLine("1111111111111");
                SqlBulkCopyByDataTable("SmartCallback", callbackList);
                Console.WriteLine("222222222222");

                //1、更新首次回访时间、最后回访时间、回访次数
                _connection.Execute(@"update SmartCustomer set FirstCallbackTime=min,LastCallbackTime=max,CallbackTime=count 
  from SmartCustomer a
  inner join (select CustomerID,MIN(TaskCreateTime) as min,MAX(TaskCreateTime) as max,count(CustomerID) as count 
  from SmartCallback where Status=1 group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);
            }

            Console.WriteLine("回访记录结束迁移");
        }


        /// <summary>
        /// 上门记录
        /// </summary>
        public static void Visit()
        {
            Console.WriteLine("上门记录开始迁移");
            DataTable visitList = new DataTable("SmartVisit");
            visitList.Columns.Add("ID", typeof(long));
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
            visitList.Columns.Add("TodaySymptomID", typeof(long));



            var list = _mySqlConnection.Query<Visit>(@"select distinct a.ID,a.CustomerID,a.CreateTime,a.CreateUserID,
c.UserID as ExploitUserID,d.UserID as ManagerUserID,b.SymptomID as TodaySymptomID,a.Type as VisitType 
from SmartVisit a
inner join SmartCustomer b on a.CustomerID=b.ID
left join smartownership c on a.CustomerID=c.CustomerID and a.createtime between c.StartTime and c.EndTime and c.Type=1
left join smartownership d on a.CustomerID=d.CustomerID and a.createtime between d.StartTime and d.EndTime and d.Type=2", null, null, true, 60000);

            foreach (var u in list)
            {
                DataRow dr = visitList.NewRow();
                //dr["CustomerID"] = new Random().Next(958266, 1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["ID"] = u.ID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["HospitalID"] = _hospitalID;
                dr["VisitType"] = u.VisitType;

                dr["IsConsume"] = u.DealType;
                if (u.ExploitUserID != null)
                {
                    dr["CurrentExploitUserID"] = u.ExploitUserID;
                }
                if (u.ManagerUserID != null)
                {
                    dr["CurrentManagerUserID"] = u.ManagerUserID;
                }
                dr["TodaySymptomID"] = u.TodaySymptomID;
                visitList.Rows.Add(dr);
            }


            if (visitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartVisit", visitList);
            }


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

            Console.WriteLine("上门记录结束迁移");
        }


        /// <summary>
        /// 优惠券
        /// </summary>
        public static void Coupon()
        {
            Console.WriteLine("优惠券记录开始迁移");
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
            visitList.Columns.Add("ExpirationDate", typeof(string));


            var list = _mySqlConnection.Query<Coupon>(@"select a.ID,a.CustomerID,a.CreateUserID,a.CreateTime,a.Access,a.CategoryID,a.Rest as Amount,a.Remark
from SmartCoupon a where a.Rest>0", new { Date = DateTime.Today.ToLocalUnixTimestamp() }, null, true, 6000);

            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                DataRow dr = visitList.NewRow();
                dr["ID"] = u.ID;
                //dr["CustomerID"] = new Random().Next(958266, 1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = _hospitalID;
                dr["CreateTime"] = now;
                dr["HospitalID"] = _hospitalID;
                dr["Access"] = 10;
                dr["CategoryID"] = u.CategoryID;
                dr["Amount"] = u.Amount;
                dr["Rest"] = u.Amount;
                dr["Remark"] = "数据迁移";
                dr["ExpirationDate"] = DateTime.MaxValue.Date;

                visitList.Rows.Add(dr);
            }

            if (visitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartCoupon", visitList);
            }
            _connection.Execute(@"update SmartCustomer set Coupon=CouponAmount
  from SmartCustomer a
  inner join (select CustomerID,sum(Rest) as CouponAmount 
  from SmartCoupon group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);

            Console.WriteLine("优惠券记录结束迁移");
        }


        /// <summary>
        /// 预收款
        /// </summary>
        public static void Deposit()
        {
            Console.WriteLine("预收款记录开始迁移");
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


            var list = _mySqlConnection.Query<Deposit>(@"select a.ID,a.CustomerID,a.CreateUserID,a.CreateTime,a.Access,a.ChargeID,a.Rest as Amount,a.Remark,
c.UserID as ExploitUserID,d.UserID  as ManagerUserID 
from SmartDeposit a

left join smartownership c on a.CustomerID=c.CustomerID and a.createtime between c.StartTime and c.EndTime and c.Type=1
left join smartownership d on a.CustomerID=d.CustomerID and a.createtime between d.StartTime and d.EndTime and d.Type=2
where a.Rest>0", null, null, true, 6000);

            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                DataRow dr = visitList.NewRow();
                dr["ID"] = u.ID;
                //dr["CustomerID"] = new Random().Next(958266, 1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = _hospitalID;
                dr["CreateTime"] = now;
                dr["HospitalID"] = _hospitalID;
                dr["Access"] = 6;
                dr["ChargeID"] = u.ChargeID;
                dr["Amount"] = u.Amount;
                dr["Rest"] = u.Amount;
                dr["Remark"] = "数据迁移";
                if (u.ExploitUserID != null)
                {
                    dr["BuyExploitUserID"] = u.ExploitUserID;
                }
                if (u.ManagerUserID != null)
                {
                    dr["BuyManagerUserID"] = u.ManagerUserID;
                }
                dr["BuyOrderUserID"] = _hospitalID;
                dr["BuyVisitType"] = 1;

                visitList.Rows.Add(dr);
            }

            if (visitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartDeposit", visitList);
            }
            _connection.Execute(@"update SmartCustomer set Deposit=DepositAmount
  from SmartCustomer a
  inner join (select CustomerID,sum(Rest) as DepositAmount 
  from SmartDeposit group by CustomerID) as b on a.ID=b.CustomerID", null, _transaction);

            Console.WriteLine("预收款记录结束迁移");
        }

        /// <summary>
        /// 项目套餐
        /// </summary>
        public static void ChargeSet()
        {
            //Console.WriteLine("(S)中下身吸脂基础型".PinYin());
            Console.WriteLine("项目套餐开始导入");
            var list = _mySqlConnection.Query<ChargeSet>(@"select a.ID,a.Name,a.Price,a.Status,a.Remark,a.PinYin,a.TimeLimit,a.TimeStart,a.Days 
from SmartChargeSet a");

            var listDetaik = _mySqlConnection.Query<SmartChargeSetDetail>(@"select a.ID,a.SetID,a.Num,a.Amount,a.ChargeID
from SmartChargeSetDetail a
");
            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                if (u.PinYin.IsNullOrEmpty())
                {
                    u.PinYin = u.Name.PinYin();
                }
                u.CreateUserID = 1;
                u.CreateTime = now;
                u.HospitalID = 1;
            }

            _connection.Execute(@"insert into SmartChargeSetDetail(ID,SetID,ChargeID,Num,Amount) 
                                            VALUES(@ID, @SetID, @ChargeID, @Num, @Amount)", listDetaik, _transaction);
            _connection.Execute(@"insert into SmartChargeSet(ID,Name,Price,Status,Remark,PinYin,TimeLimit,TimeStart,Days,HospitalID,CreateUserID,CreateTime) 
                                    VALUES(@ID, @Name, @Price, @Status, @Remark, @PinYin, @TimeLimit, @TimeStart, @Days, @HospitalID,@CreateUserID,@CreateTime)", list, _transaction);

            //_connection.Execute(@"update SmartChargeSet set PinYin='' where PinYin is null", null, _transaction);
            Console.WriteLine("项目套餐结束导入");
        }

        /// <summary>
        /// 退预收款开始导入
        /// </summary>
        public static void BackDeposit()
        {
            //Console.WriteLine("(S)中下身吸脂基础型".PinYin());
            Console.WriteLine("退预收款开始导入");
            var list = _mySqlConnection.Query<DepositOrder>(@"select a.ID,a.CustomerID,a.PaidTime as CreateTime,a.CreateUserID,0 AS Point,0 AS Coupon,a.Amount,a.Remark,
e.UserID as ExploitUserID,d.UserID as ManagerUserID 
from smartdepositrebateorder a
inner join SmartCustomer b on a.CustomerID=b.ID
left join smartownership e on a.CustomerID=e.CustomerID and a.createtime between e.StartTime and e.EndTime and e.Type=1
left join smartownership d on a.CustomerID=d.CustomerID and a.createtime between d.StartTime and d.EndTime and d.Type=2
where a.PaidStatus=2
");

            var detailList = _mySqlConnection.Query<DepositOrderDetial>(@"select c.ID,c.OrderID,a.CustomerID,a.PaidTime as CreateTime,a.CreateUserID,0 AS Point,0 AS Coupon,c.Amount,a.Remark,
e.UserID as ExploitUserID,F.UserID as ManagerUserID,c.DepositID,d.ChargeID as DepositChargeID
from smartdepositrebateorder a
inner join SmartCustomer b on a.CustomerID=b.ID
inner join smartdepositrebateorderdetail c on a.ID=c.OrderID
left join SmartDeposit d on c.DepositID=d.ID
left join smartownership e on a.CustomerID=e.CustomerID and a.createtime between e.StartTime and e.EndTime and e.Type=1
left join smartownership F on a.CustomerID=F.CustomerID and a.createtime between F.StartTime and F.EndTime and F.Type=2
where a.PaidStatus=2
");
            foreach (var u in list)
            {
                u.PaidStatus = 2;
                u.PaidTime = u.CreateTime;
                u.HospitalID = 1;
                u.VisitType = VisitType.First;
                u.SourceType = 7;
                u.RainFlyType = 0;
                u.Point = 0;
                u.AuditStatus = 2;
                u.Point = 0;
                u.Coupon = 0;
                u.Deposit = u.Amount;



            }

            var cashierList = new List<object>();
            foreach (var u in detailList)
            {
                u.PaidStatus = 2;
                u.PaidTime = u.CreateTime;
                u.HospitalID = 1;
                u.VisitType = VisitType.First;
                u.SourceType = 7;
                u.CashierID = 0;
                u.BuyOrderID = 0;
                if (u.DepositChargeID == null)
                {
                    u.DepositChargeID = 1;
                }
                cashierList.Add(new
                {
                    CashierID = 0,
                    ReferID = u.ID,
                    CashCardAmount = u.Amount,
                    DepositAmount = 0,
                    CouponAmount = 0,
                    DebtAmount = 0,
                    CommissionAmount = 0,
                    Amount = u.Amount,
                    HospitalID = 1,
                    CreateTime = u.CreateTime,
                    OrderType = 5,
                    CustomerID = u.CustomerID,
                    OriginAmount = u.Amount,
                    DepositID = u.DepositChargeID,
                    u.VisitType,
                    u.ExploitUserID,
                    u.ManagerUserID,
                    u.OrderID,
                    u.SourceType,
                    RainFlyType = 0,
                    OrderUserID = u.CreateUserID,
                    BuyExploitUserID = u.ExploitUserID,
                    BuyManagerUserID = u.ManagerUserID,
                    BuyOrderUserID = u.CreateUserID,
                    BuyVisitType = u.VisitType
                });
            }
            Console.WriteLine("11111111111");
            _connection.Execute(
                    @"insert into [SmartDepositRebateOrder]([ID],[HospitalID],[CustomerID],[CreateTime],[CreateUserID],[Deposit],[Amount],[AuditStatus],[PaidStatus],[Remark],Coupon,Point,VisitType,SourceType,ExploitUserID,ManagerUserID) 
                        values(@ID,@HospitalID,@CustomerID,@CreateTime,@CreateUserID,@Deposit,@Amount,@AuditStatus,@PaidStatus,@Remark,@Coupon,@Point,@VisitType,@SourceType,@ExploitUserID,@ManagerUserID)",
                    list, _transaction);
            Console.WriteLine("22222222222");

            _connection.Execute(
                   @"insert into [SmartDepositRebateOrderDetail]([ID],[OrderID],[DepositID],[Amount],BuyExploitUserID,BuyManagerUserID,BuyOrderUserID,BuyVisitType,DepositChargeID) 
                    values(@ID,@OrderID,@DepositID,@Amount,@ExploitUserID,@ManagerUserID,@CreateUserID,@VisitType,@DepositChargeID)", detailList, _transaction);
            Console.WriteLine("333333333333333");

            _connection.Execute(
                        @"insert into [SmartCashierCharge]([CashierID],[ReferID],[CashCardAmount],[DepositAmount],[CouponAmount],[DebtAmount],[Amount],[HospitalID],[CommissionAmount],[CreateTime],[OrderType],[CustomerID],
                        OriginAmount,DepositID,VisitType,ExploitUserID,ManagerUserID,OrderID,SourceType,RainFlyType,OrderUserID,BuyExploitUserID,BuyManagerUserID,BuyOrderUserID,BuyVisitType) 
                        values(@CashierID,@ReferID,@CashCardAmount,@DepositAmount,@CouponAmount,@DebtAmount,@Amount,@HospitalID,@CommissionAmount,@CreateTime,@OrderType,@CustomerID,
                        @OriginAmount,@DepositID,@VisitType,@ExploitUserID,@ManagerUserID,@OrderID,@SourceType,@RainFlyType,@OrderUserID,@BuyExploitUserID,@BuyManagerUserID,@BuyOrderUserID,@BuyVisitType)",
                        cashierList, _transaction);
            Console.WriteLine("444444444444444444444444444!");

            //_connection.Execute(@"update SmartChargeSet set PinYin='' where PinYin is null", null, _transaction);
            Console.WriteLine("退预收款结束导入");
        }

        /// <summary>
        /// 退款单开始导入
        /// </summary>
        public static void BackOrder()
        {
            //Console.WriteLine("(S)中下身吸脂基础型".PinYin());
            Console.WriteLine("退款单开始导入");
            var list = _mySqlConnection.Query<DepositOrder>(@"select a.ID,a.CustomerID,a.PaidTime as CreateTime,a.Amount,a.Remark,
c.UserID as ExploitUserID,d.UserID as ManagerUserID,a.CreateUserID
from SmartBackOrder a
inner join SmartCustomer b on a.CustomerID=b.ID
left join smartownership c on a.CustomerID=c.CustomerID and a.createtime between c.StartTime and c.EndTime and c.Type=1
left join smartownership d on a.CustomerID=d.CustomerID and a.createtime between d.StartTime and d.EndTime and d.Type=2
where a.PaidStatus=2");
            Console.WriteLine("1111111");

            var detailList = _mySqlConnection.Query<DepositOrderDetial>(@"select a.CustomerID,a.PaidTime as CreateTime,a.Remark,
e.UserID as ExploitUserID,d.UserID as ManagerUserID,c.ID,c.OrderID,c.ChargeID,c.Num,c.Amount,c.DetailID,a.CreateUserID,g.DepositAmount,g.CashierID  
from SmartBackOrder a
inner join SmartCustomer b on a.CustomerID=b.ID
inner join SmartBackOrderDetail c on a.ID=c.OrderID
left join smartownership e on a.CustomerID=e.CustomerID and a.createtime between e.StartTime and e.EndTime and e.Type=1
left join smartownership d on a.CustomerID=d.CustomerID and a.createtime between d.StartTime and d.EndTime and d.Type=2
inner join smartcashier f on a.ID=f.OrderID and f.OrderType in (4)
inner join smartcashierdetail g on f.ID=g.CashierID and c.DetailID=g.ReferID
where a.PaidStatus=2
");
            Console.WriteLine("222222222222222");

            foreach (var u in list)
            {
                u.PaidStatus = 2;
                u.PaidTime = u.CreateTime;
                u.HospitalID = 1;
                u.VisitType = VisitType.First;
                u.SourceType = 7;
                u.RainFlyType = 0;
                u.Point = 0;
                u.AuditStatus = 2;


            }

            var cashierList = new List<object>();
            foreach (var u in detailList)
            {
                u.PaidStatus = 2;
                u.PaidTime = u.CreateTime;
                u.HospitalID = 1;
                u.VisitType = VisitType.First;
                u.SourceType = 7;
                //u.CashierID = 0;
                u.BuyOrderID = 0;
                cashierList.Add(new
                {
                    CashierID = u.CashierID,
                    ReferID = u.ID,
                    CashCardAmount = u.Amount - (u.DepositAmount * -1),
                    DepositAmount = u.DepositAmount * -1,
                    CouponAmount = 0,
                    DebtAmount = 0,
                    Amount = u.Amount,
                    HospitalID = 1,
                    CommissionAmount = 0,
                    CreateTime = u.CreateTime,
                    OrderType = 4,
                    CustomerID = u.CustomerID,
                    ChargeID = u.ChargeID,
                    u.Num,
                    OriginAmount = u.Amount,
                    VisitType = VisitType.First,
                    u.ExploitUserID,
                    u.ManagerUserID,

                    u.OrderID,
                    u.SourceType,
                    RainFlyType = 0,
                    OrderUserID = u.CreateUserID,
                    BuyExploitUserID = u.ExploitUserID,
                    BuyManagerUserID = u.ManagerUserID,
                    BuyOrderUserID = u.CreateUserID,
                    BuyVisitType = VisitType.First
                });
            }
            Console.WriteLine("333333333333333");

            _connection.Execute(
                    @"insert into [SmartBackOrder]([ID],[HospitalID],[CustomerID],[CreateUserID],[CreateTime],[Amount],[Point],[PaidStatus],[Remark],[AuditStatus],VisitType,SourceType,ExploitUserID,ManagerUserID,RainFlyType) 
                        values(@ID,@HospitalID,@CustomerID,@CreateUserID,@CreateTime,@Amount,@Point,@PaidStatus,@Remark,@AuditStatus,@VisitType,@SourceType,@ExploitUserID,@ManagerUserID,@RainFlyType)",
                    list, _transaction);
            Console.WriteLine("44444444444444");

            _connection.Execute(
                   @"insert into [SmartBackOrderDetail]([ID],[OrderID],[ChargeID],[Num],[Amount],[DetailID],BuyOrderID,BuyExploitUserID,BuyManagerUserID,BuyOrderUserID,BuyVisitType) 
                    values(@ID,@OrderID,@ChargeID,@Num,@Amount,@DetailID,@BuyOrderID,@ExploitUserID,@ManagerUserID,@CreateUserID,@VisitType)", detailList, _transaction);
            Console.WriteLine("55555555555555555");

            _connection.Execute(
                        @"insert into [SmartCashierCharge]([CashierID],[ReferID],[CashCardAmount],[DepositAmount],[CouponAmount],[DebtAmount],[Amount],[HospitalID],[CommissionAmount],[CreateTime],[OrderType],[CustomerID],
                        ChargeID,Num,OriginAmount,VisitType,ExploitUserID,ManagerUserID,OrderID,SourceType,RainFlyType,OrderUserID,BuyExploitUserID,BuyManagerUserID,BuyOrderUserID,BuyVisitType) 
                        values(@CashierID,@ReferID,@CashCardAmount,@DepositAmount,@CouponAmount,@DebtAmount,@Amount,@HospitalID,@CommissionAmount,@CreateTime,@OrderType,@CustomerID,
                        @ChargeID,@Num,@OriginAmount,@VisitType,@ExploitUserID,@ManagerUserID,@OrderID,@SourceType,@RainFlyType,@OrderUserID,@BuyExploitUserID,@BuyManagerUserID,@BuyOrderUserID,@BuyVisitType)",
                        cashierList, _transaction);

            //_connection.Execute(@"update SmartChargeSet set PinYin='' where PinYin is null", null, _transaction);
            Console.WriteLine("退款单结束导入");
        }

        /// <summary>
        /// 预收款订单
        /// </summary>
        public static void DepositOrder()
        {
            //Console.WriteLine("(S)中下身吸脂基础型".PinYin());
            Console.WriteLine("预收款订单开始导入");
            var list = _mySqlConnection.Query<DepositOrder>(@"select a.ID,a.CustomerID,a.PaidTime as CreateTime,a.Amount,a.CreateUserID,a.Remark,
e.UserID as ExploitUserID,d.UserID as ManagerUserID   
from SmartDepositOrder a 
inner join SmartCustomer b on a.CustomerID=b.ID
left join smartownership e on a.CustomerID=e.CustomerID and a.createtime between e.StartTime and e.EndTime and e.Type=1
left join smartownership d on a.CustomerID=d.CustomerID and a.createtime between d.StartTime and d.EndTime and d.Type=2
where a.PaidStatus=2");

            var detailList = _mySqlConnection.Query<DepositOrderDetial>(@"select c.ID,a.CustomerID,a.PaidTime as CreateTime,a.Amount,a.CreateUserID,a.Remark,
e.UserID as ExploitUserID,d.UserID as ManagerUserID,
c.Price,c.ChargeID,c.Num,c.Total,c.OrderID   
from SmartDepositOrder a 
inner join SmartCustomer b on a.CustomerID=b.ID
inner join SmartDepositOrderDetail c on c.OrderID=a.ID
left join smartownership e on a.CustomerID=e.CustomerID and a.createtime between e.StartTime and e.EndTime and e.Type=1
left join smartownership d on a.CustomerID=d.CustomerID and a.createtime between d.StartTime and d.EndTime and d.Type=2
where a.PaidStatus=2
");
            foreach (var u in list)
            {
                u.PaidStatus = 2;
                u.PaidTime = u.CreateTime;
                u.HospitalID = 1;
                u.VisitType = VisitType.First;
                u.SourceType = 7;
            }

            var cashierList = new List<object>();
            foreach (var u in detailList)
            {
                u.PaidStatus = 2;
                u.PaidTime = u.CreateTime;
                u.HospitalID = 1;
                u.VisitType = VisitType.First;
                u.SourceType = 7;
                u.CashierID = 0;
                cashierList.Add(new
                {
                    CashierID = 0,
                    ReferID = u.ID,
                    CashCardAmount = u.Total,
                    DepositAmount = 0,
                    CouponAmount = 0,
                    DebtAmount = 0,
                    CommissionAmount = 0,
                    Amount = u.Total,
                    HospitalID = 1,
                    CreateTime = u.CreateTime,
                    OrderType = 3,
                    CustomerID = u.CustomerID,
                    u.Num,
                    OriginAmount = u.Total,
                    DepositID = u.ChargeID,
                    u.VisitType,
                    u.ExploitUserID,
                    u.ManagerUserID,
                    u.OrderID,
                    u.SourceType,
                    RainFlyType = 0,
                    OrderUserID = u.CreateUserID,
                    BuyExploitUserID = u.ExploitUserID,
                    BuyManagerUserID = u.ManagerUserID,
                    BuyOrderUserID = u.CreateUserID,
                    BuyVisitType = u.VisitType,
                    DealType = 0,
                });
            }
            _connection.Execute(
                   @"insert into [SmartDepositOrder]([ID],[HospitalID],[CustomerID],[CreateUserID],[CreateTime],[Amount],[Remark],[PaidStatus],VisitType,SourceType,ExploitUserID,ManagerUserID) 
                        values(@ID,@HospitalID,@CustomerID,@CreateUserID,@CreateTime,@Amount,@Remark,@PaidStatus,@VisitType,@SourceType,@ExploitUserID,@ManagerUserID)",
                   list, _transaction);

            _connection.Execute(
                   @"insert into [SmartDepositOrderDetail]([ID],[OrderID],[ChargeID],[Price],[Num],[Total]) 
                    values(@ID,@OrderID,@ChargeID,@Price,@Num,@Total)", detailList, _transaction);

            _connection.Execute(
                        @"insert into [SmartCashierCharge]([CashierID],[ReferID],[CashCardAmount],[DepositAmount],[CouponAmount],[DebtAmount],[Amount],[HospitalID],[CommissionAmount],[CreateTime],[OrderType],[CustomerID],
                        Num,OriginAmount,DepositID,VisitType,ExploitUserID,ManagerUserID,OrderID,SourceType,RainFlyType,OrderUserID,BuyExploitUserID,
BuyManagerUserID,BuyOrderUserID,BuyVisitType,DealType) 
                        values(@CashierID,@ReferID,@CashCardAmount,@DepositAmount,@CouponAmount,@DebtAmount,@Amount,@HospitalID,@CommissionAmount,@CreateTime,@OrderType,@CustomerID,
                        @Num,@OriginAmount,@DepositID,@VisitType,@ExploitUserID,@ManagerUserID,@OrderID,@SourceType,@RainFlyType,@OrderUserID,
@BuyExploitUserID,@BuyManagerUserID,@BuyOrderUserID,@BuyVisitType,@DealType)",
                        cashierList, _transaction);

            //_connection.Execute(@"update SmartChargeSet set PinYin='' where PinYin is null", null, _transaction);
            Console.WriteLine("预收款订单结束导入");
        }

        /// <summary>
        /// 订单
        /// </summary>
        public static void Order()
        {
            Console.WriteLine("订单记录开始迁移");

            #region SmartOrder
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
            var orders = _mySqlConnection.Query<Order>(@"select  a.ID,a.CustomerID,a.CreateUserID,a.TotalPrice,a.FinalPrice,
a.PaidStatus,a.Remark,a.PaidTime as CreateTime,a.DebtAmount,d.UserID as ManagerUserID,c.UserID as ExploitUserID
from SmartOrder a 
inner join SmartCustomer b on a.CustomerID=b.ID
left join smartownership c on a.CustomerID=c.CustomerID and a.createtime between c.StartTime and c.EndTime and c.Type=1
left join smartownership d on a.CustomerID=d.CustomerID and a.createtime between d.StartTime and d.EndTime and d.Type=2
where a.PaidStatus in(2,3)", null, null, true, 60000);

            foreach (var u in orders)
            {
                var order = orderList.NewRow();
                order["ID"] = u.ID;
                order["CustomerID"] = u.CustomerID;
                order["CreateUserID"] = u.CreateUserID;
                order["CreateTime"] = u.CreateTime;
                order["TotalPrice"] = u.TotalPrice;
                order["FinalPrice"] = u.FinalPrice;
                order["DebtAmount"] = u.DebtAmount;
                order["PaidTime"] = u.CreateTime;
                order["VisitType"] = VisitType.First;
                order["SourceType"] = 7;
                order["RainFlyType"] = 0;
                if (u.ExploitUserID != null)
                {
                    order["ExploitUserID"] = u.ExploitUserID;
                }
                if (u.ManagerUserID != null)
                {
                    order["ManagerUserID"] = u.ManagerUserID;
                }
                order["DealType"] = 1;
                order["HospitalID"] = _hospitalID;

                order["PaidStatus"] = u.PaidStatus;

                if (u.Remark.Length >= 900)
                {
                    u.Remark = u.Remark.Substring(0, 900);
                }
                else
                {
                    order["Remark"] = u.Remark;
                }

                order["AuditStatus"] = 4;
                orderList.Rows.Add(order);
            }
            #endregion


            #region SmartOrderDetail
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

            var details = _mySqlConnection.Query<OrderDetail>(@"select a.ID as OrderDetailID,a.OrderID,a.ChargeID,a.Price,a.FinalPrice,a.RestNum,a.SetID,
e.UserID as ExploitUserID,d.UserID as ManagerUserID,a.Num,g.DepositAmount,g.CouponAmount,g.DebtAmount,b.CreateUserID,b.PaidTime as CreateTime,b.CustomerID,g.CashierID    
from SmartOrderDetail a
inner join SmartOrder b on a.OrderID=b.ID and b.PaidStatus in (2,3)
left join smartownership e on b.CustomerID=e.CustomerID and b.createtime between e.StartTime and e.EndTime and e.Type=1
left join smartownership d on b.CustomerID=d.CustomerID and b.createtime between d.StartTime and d.EndTime and d.Type=2
inner join smartcashier f on b.ID=f.OrderID and f.OrderType in (1,2)
inner join smartcashierdetail g on f.ID=g.CashierID and a.id=g.ReferID
");
            foreach (var x in details)
            {
                #region detail
                var detail = detailList.NewRow();
                detail["ID"] = x.OrderDetailID;
                detail["OrderID"] = x.OrderID;
                detail["ChargeID"] = x.ChargeID;
                detail["Price"] = x.Price;
                detail["Num"] = x.Num;
                detail["FinalPrice"] = x.FinalPrice;
                detail["RestNum"] = x.RestNum;
                if (x.SetID != null)
                {
                    detail["SetID"] = x.SetID;
                    detail["SetNum"] = 1;
                    detail["SetPrice"] = 0;
                    detail["SetFinalPrice"] = 0;
                }

                //detail["ExpirationDate"] = x.ExpirationDate;

                detailList.Rows.Add(detail);
                #endregion

                #region cashier
                var cashier = cashierList.NewRow();
                cashier["CashierID"] = x.CashierID;
                cashier["ReferID"] = x.OrderDetailID;
                cashier["CashCardAmount"] = x.FinalPrice - x.DepositAmount - x.CouponAmount - x.DebtAmount;
                cashier["DepositAmount"] = x.DepositAmount;
                cashier["CouponAmount"] = x.CouponAmount;
                cashier["DebtAmount"] = x.DebtAmount;
                cashier["Amount"] = x.FinalPrice;
                cashier["HospitalID"] = _hospitalID;
                cashier["CommissionAmount"] = 0;
                cashier["CreateTime"] = x.CreateTime;
                cashier["OrderType"] = 1;
                cashier["CustomerID"] = x.CustomerID;
                cashier["ChargeID"] = x.ChargeID;
                if (x.SetID != null)
                {
                    cashier["SetID"] = x.SetID;
                    cashier["SetNum"] = 1;
                }
                cashier["Num"] = x.Num;
                cashier["OriginAmount"] = x.Price;
                cashier["VisitType"] = VisitType.First;
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
                cashier["OrderID"] = x.OrderID;
                cashier["SourceType"] = 7;
                cashier["RainFlyType"] = 0;
                cashier["OrderUserID"] = x.CreateUserID;
                cashier["BuyVisitType"] = VisitType.First;
                cashier["BuyOrderUserID"] = x.CreateUserID;
                cashier["DealType"] = 1;
                cashierList.Rows.Add(cashier);
                #endregion
            }
            #endregion

            ///导入数据库
            SqlBulkCopyByDataTable("SmartOrder", orderList);
            SqlBulkCopyByDataTable("SmartOrderDetail", detailList);
            SqlBulkCopyByDataTable("SmartCashierCharge", cashierList);


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


            Console.WriteLine("订单记录结束迁移");
        }

        public static void DebtOrder()
        {
            Console.WriteLine("欠款订单记录开始迁移");

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

            var details = _mySqlConnection.Query<OrderDetail>(@"select a.ID as OrderDetailID,a.OrderID,a.ChargeID,a.Price,g.Amount AS FinalPrice,a.Num,a.SetID,
e.UserID as ExploitUserID,d.UserID as ManagerUserID,a.Num,g.DepositAmount,g.CouponAmount,g.DebtAmount,b.CreateUserID,b.PaidTime as CreateTime,b.CustomerID,f.ID as CashierID      
from SmartOrderDetail a
inner join SmartOrder b on a.OrderID=b.ID and b.PaidStatus in (2,3) 
left join smartownership e on b.CustomerID=e.CustomerID and b.createtime between e.StartTime and e.EndTime and e.Type=1
left join smartownership d on b.CustomerID=d.CustomerID and b.createtime between d.StartTime and d.EndTime and d.Type=2
inner join smartcashier f on b.ID=f.OrderID and f.OrderType in (10) 
inner join smartcashierdetail g on f.ID=g.CashierID and a.id=g.ReferID;
");
            foreach (var x in details)
            {

                var cashier = cashierList.NewRow();
                cashier["CashierID"] = x.CashierID;
                cashier["ReferID"] = x.OrderDetailID;
                cashier["CashCardAmount"] = x.FinalPrice - x.DepositAmount - x.CouponAmount - x.DebtAmount;
                cashier["DepositAmount"] = x.DepositAmount;
                cashier["CouponAmount"] = x.CouponAmount;
                cashier["DebtAmount"] = x.DebtAmount;
                cashier["Amount"] = x.FinalPrice;
                cashier["HospitalID"] = _hospitalID;
                cashier["CommissionAmount"] = 0;
                cashier["CreateTime"] = x.CreateTime;
                cashier["OrderType"] = 6;
                cashier["CustomerID"] = x.CustomerID;
                cashier["ChargeID"] = x.ChargeID;
                if (x.SetID != null)
                {
                    cashier["SetID"] = x.SetID;
                    cashier["SetNum"] = 1;
                }
                cashier["Num"] = x.Num;
                cashier["OriginAmount"] = x.Price;
                cashier["VisitType"] = VisitType.First;
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
                cashier["OrderID"] = x.OrderID;
                cashier["SourceType"] = 7;
                cashier["RainFlyType"] = 0;
                cashier["OrderUserID"] = x.CreateUserID;
                cashier["BuyVisitType"] = VisitType.First;
                cashier["BuyOrderUserID"] = x.CreateUserID;
                cashier["DealType"] = 1;
                cashierList.Rows.Add(cashier);
            }

            ///导入数据库
            SqlBulkCopyByDataTable("SmartCashierCharge", cashierList);


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
  select a.CustomerID,sum(case when a.OrderType in (1,2,3) then a.CashCardAmount+a.DepositAmount+a.CommissionAmount 
  else (a.CashCardAmount+a.DepositAmount+a.CommissionAmount)*-1 end) as Amount
  from SmartCashierCharge a
  where a.OrderType in (1,2,4,8,3,5) group by a.CustomerID) as b on a.ID=b.CustomerID", null, _transaction);
            //2、更新会员等级
            //            _connection.Execute(@"update SmartCustomer set MemberCategoryID=c.ID 
            //from SmartCustomer a
            //inner join (
            //select a.ID,max(b.Level) as Level
            //from SmartCustomer a
            //inner join SmartMemberCategory as b on a.CashCardTotalAmount>b.Amount group by a.ID) as b on a.ID=b.ID
            //inner join SmartMemberCategory as c on b.Level=c.Level", null, _transaction);

            Console.WriteLine("订单记录结束迁移");
        }


        public static void Cashier()
        {
            Console.WriteLine("收银记录开始迁移");

            DataTable cashierList = new DataTable("SmartCashier");
            cashierList.Columns.Add("ID", typeof(long));
            cashierList.Columns.Add("HospitalID", typeof(long));
            cashierList.Columns.Add("OrderType", typeof(int));
            cashierList.Columns.Add("CustomerID", typeof(long));
            cashierList.Columns.Add("OrderID", typeof(long));
            cashierList.Columns.Add("CreateUserID", typeof(long));
            cashierList.Columns.Add("CreateTime", typeof(DateTime));
            cashierList.Columns.Add("Amount", typeof(decimal));
            cashierList.Columns.Add("Cash", typeof(decimal));
            cashierList.Columns.Add("Card", typeof(decimal));
            cashierList.Columns.Add("Deposit", typeof(decimal));
            cashierList.Columns.Add("Coupon", typeof(decimal));
            cashierList.Columns.Add("Debt", typeof(decimal));
            cashierList.Columns.Add("Commission", typeof(decimal));
            cashierList.Columns.Add("Remark", typeof(string));
            cashierList.Columns.Add("Status", typeof(int));

            var details = _mySqlConnection.Query<Cashier>(@"select ID,CustomerID,case when OrderType=10 then 6 else OrderType end as OrderType,OrderID,CreateUserID,
CreateTime,Amount,Cash,Card,Deposit,Coupon,Debt,Remark 
from smartcashier
");
            foreach (var x in details)
            {

                var cashier = cashierList.NewRow();
                cashier["ID"] = x.ID;
                cashier["HospitalID"] = 1;
                cashier["OrderType"] = x.OrderType;
                cashier["CustomerID"] = x.CustomerID;
                cashier["OrderID"] = x.OrderID;
                cashier["CreateUserID"] = x.CreateUserID;
                cashier["CreateTime"] = x.CreateTime;
                cashier["Amount"] = x.Amount;
                cashier["Cash"] = x.Cash;
                cashier["Card"] = x.Card;
                cashier["Deposit"] = x.Deposit;
                cashier["Coupon"] = x.Coupon;
                cashier["Debt"] = x.Debt;
                cashier["Commission"] = x.Commission;
                cashier["Remark"] = x.Remark;
                cashier["Status"] = x.Status;




                cashierList.Rows.Add(cashier);
            }

            ///导入数据库
            SqlBulkCopyByDataTable("SmartCashier", cashierList);



            Console.WriteLine("订单记录结束迁移");
        }


        public static void Point()
        {
            Console.WriteLine("积分记录开始迁移");

            DataTable cashierList = new DataTable("SmartPoint");
            cashierList.Columns.Add("ID", typeof(long));
            cashierList.Columns.Add("HospitalID", typeof(long));
            cashierList.Columns.Add("Type", typeof(int));
            cashierList.Columns.Add("CustomerID", typeof(long));
            cashierList.Columns.Add("CreateUserID", typeof(long));
            cashierList.Columns.Add("CreateTime", typeof(DateTime));
            cashierList.Columns.Add("Amount", typeof(decimal));
            cashierList.Columns.Add("ConsumeAmount", typeof(decimal));

            cashierList.Columns.Add("Remark", typeof(string));

            var details = _mySqlConnection.Query<Cashier>(@"select *
from SmartPoint a where a.AMount<>0
");
            foreach (var x in details)
            {

                var cashier = cashierList.NewRow();
                cashier["ID"] = x.ID;
                cashier["HospitalID"] = 1;
                cashier["Type"] = x.Type;
                cashier["CustomerID"] = x.CustomerID;
                cashier["CreateUserID"] = x.CreateUserID;
                cashier["CreateTime"] = x.CreateTime;
                cashier["Amount"] = x.Amount;
                cashier["ConsumeAmount"] = x.ConsumeAmount;

                cashier["Remark"] = x.Remark;




                cashierList.Rows.Add(cashier);
            }

            ///导入数据库
            SqlBulkCopyByDataTable("SmartPoint", cashierList);



            Console.WriteLine("订单记录结束迁移");
        }


        public static void CustomerTag()
        {
            Console.WriteLine("顾客标签开始迁移");

            DataTable cashierList = new DataTable("SmartCustomerTag");
            cashierList.Columns.Add("ID", typeof(long));
            cashierList.Columns.Add("CustomerID", typeof(long));
            cashierList.Columns.Add("TagID", typeof(int));

            cashierList.Columns.Add("CreateUserID", typeof(long));
            cashierList.Columns.Add("CreateTime", typeof(DateTime));


            var details = _mySqlConnection.Query<Cashier>(@"select * from SmartCustomertag
");
            foreach (var x in details)
            {

                var cashier = cashierList.NewRow();
                cashier["ID"] = x.ID;
                cashier["TagID"] = x.TagID;
                cashier["CustomerID"] = x.CustomerID;
                cashier["CreateUserID"] = x.CreateUserID;
                cashier["CreateTime"] = x.CreateTime;





                cashierList.Rows.Add(cashier);
            }

            ///导入数据库
            SqlBulkCopyByDataTable("SmartCustomerTag", cashierList);



            Console.WriteLine("订单记录结束迁移");
        }

        /// <summary>
        /// 划扣
        /// </summary>
        public static void OperationOld()
        {
            Console.WriteLine("划扣记录开始迁移");
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

            DataTable operatorList = new DataTable("SmartOperator");
            operatorList.Columns.Add("ID", typeof(long));
            operatorList.Columns.Add("OperationID", typeof(long));
            operatorList.Columns.Add("UserID", typeof(long));
            operatorList.Columns.Add("PositionID", typeof(long));

            var list = _mySqlConnection.Query<Operation>(@"select a.ID,a.Num,a.CreateUserID,a.CreateTime,a.Remark,a.OrderDetailID,
1 as Status,a.CustomerStatus,a.CustomerID,
d.ChargeID 
from SmartOperation a
inner join SmartOrderDetail d on a.OrderDetailID=d.ID", null, null, true, 6000);

            foreach (var u in list)
            {
                DataRow dr = visitList.NewRow();
                dr["ID"] = u.ID;
                //dr["CustomerID"] = new Random().Next(958266, 1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["HospitalID"] = _hospitalID;
                dr["ChargeID"] = u.ChargeID;
                dr["Num"] = u.Num;
                dr["Remark"] = u.Remark;
                if (u.DeptID != null)
                {
                    dr["DeptID"] = u.DeptID;
                }
                else
                {
                    dr["DeptID"] = 0;
                }
                if (u.DoctorID == null)
                {
                    dr["DoctorID"] = 0;
                }
                else
                {
                    dr["DoctorID"] = u.DoctorID;
                }

                dr["OrderDetailID"] = u.OrderDetailID;
                dr["CustomerStatus"] = 0;

                visitList.Rows.Add(dr);
            }

            var operatorTempList = _mySqlConnection.Query<Operator>(@"SELECT  ID
      ,OperationID
      ,UserID,case when PositionID is null then 1 else PositionID end as PositionID
  FROM SmartOperator", null, null, true, 6000);

            //var positionList = _connection.Query<Channel>(@"select ID,Name from SmartPosition", null, _transaction);

            foreach (var u in operatorTempList)
            {
                DataRow dr = operatorList.NewRow();
                dr["ID"] = u.ID;


                dr["OperationID"] = u.OperationID;
                dr["UserID"] = u.UserID;
                dr["PositionID"] = u.PositionID;


                operatorList.Rows.Add(dr);
            }
            if (visitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartOperation", visitList);
                SqlBulkCopyByDataTable("SmartOperator", operatorList);

            }




            Console.WriteLine("划扣记录结束迁移");
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
                    sqlBulkCopy.BulkCopyTimeout = 10000000;
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
    }
}

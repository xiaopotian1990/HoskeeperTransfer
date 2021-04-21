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
    class HoskeeperProgram
    {
        private static long _hospitalID = 1;
        private static long _channelID = 439;
        private static SqlConnection _connection;
        private static MySqlConnection _mySqlConnection;
        private static SqlTransaction _transaction;
        private static long _tool = 15213905621173248;//电话工具
        //private static long _symptomID = 14663754418897920;//无症状
        private static long _callBackCategoryOfSH = 14663756704973824;
        private static long _callBackCategoryOfWD = 14663756476875776;
        private static long _callBackCategoryOfXC = 14663756587729920;

        private static long _couponCategoryID = 14961071147172864;
        private static long _depositCategoryID = 14961071468217344;
        private static int _callbackNum = 200000;
        static void HoskeeperMain(string[] args)
        {
            try
            {
                _connection = new SqlConnection("Data Source=a39.107.231.232;Initial Catalog=Hoskeeper;Persist Security Info=True;User ID=sa;Password=7frV4W&cjl1DrWUn;MultipleActiveResultSets = true;connect timeout=90000");
                _mySqlConnection = new MySqlConnection("server=a39.106.132.6;database=hoskeeper_dltl;uid=root;pwd=qwer1234;charset=utf8;");
                _connection.Open();
                _transaction = _connection.BeginTransaction();

                //Tag();
                //Supplier();
                Factory();
                //Channel();
                //Unit();
                //ChargeCategory();
                //Charge();
                //ProductCategory();
                //Product();
                //Dept();
                //User();
                //CouponCategory();
                //DepositCategory();
                //Symptom();
                //ChargeSet();


                //Customer();
                //Consult();
                //CallBackTask();
                //CallBack();
                //Visit();
                //Coupon();
                //Deposit();

                //Order();
                //Operation();
                //BackOrder();
                //OperationOld();
                Rebate();

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
        /// 标签
        /// </summary>
        public static void Tag()
        {
            Console.WriteLine("标签开始导入");
            var list = _mySqlConnection.Query<Tag>(@"select a.tag_name as Content,a.status as Status,b.type_name as TagGroupName 
from r_tag_files a
left join doc_tag_type b on a.type_id=b.type_id
where a.is_del=0");

            var groupList = _connection.Query<Tag>(@"select ID,Name as Content from SmartTagGroup", null, _transaction);

            int i = 10000;
            foreach (var u in list)
            {
                u.TagGroupID = groupList.Where(x => x.Content == u.TagGroupName).FirstOrDefault().ID;
                u.NotCallBack = CommonStatus.Stop;
                u.NotSend = CommonStatus.Stop;
                u.NotSSM = CommonStatus.Stop;
                u.ID = i;
                i++;
            }


            _connection.Execute("insert into [SmartTag]([ID],[Content],[Status],NotCallBack,NotSend,NotSSM,TagGroupID) values (@ID,@Content,@Status,@NotCallBack,@NotSend,@NotSSM,@TagGroupID)",
                   list, _transaction);

            Console.WriteLine("标签结束导入");
        }

        /// <summary>
        /// 供应商
        /// </summary>
        public static void Supplier()
        {
            Console.WriteLine("供应商开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select a.supplier_name as Name,a.supplier_pinyin as PinYin,a.contact_name as LinkMan,a.contact_way as Contact,a.`status` as Status 
from doc_supplier_info a where is_del=0");

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
        /// 供应商
        /// </summary>
        public static void Factory()
        {
            Console.WriteLine("生产厂商开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"
select a.mfr_name as Name,a.pinyin_code as PinYin,a.`status` as Status from doc_manufacturer a where a.is_del=0
");

            DateTime now = DateTime.Now;
            int i = 10000;
            foreach (var u in list)
            {
                u.CreateTime = now;
                u.CreateUserID = 1;
                u.ID = i;
                i++;
                u.Remark = "数据迁移";
            }

            _connection.Execute(@"insert into SmartFactory(ID,Name,Remark,Status,CreateTime,CreateUserID) 
values (@ID,@Name,@Remark,@Status,@CreateTime,@CreateUserID)", list, _transaction);

            Console.WriteLine("生产厂商结束导入");
        }

        /// <summary>
        /// 渠道
        /// </summary>
        public static void Channel()
        {
            Console.WriteLine("渠道导入开始！");
            var list = _mySqlConnection.Query<Channel>(@"
select a.id as ID,a.channel_name as Name,b.group_name as GroupName,a.`status` as Status
from doc_channel a
left join doc_channel_group b on a.group_id=b.group_id where a.Is_del=0");

            if (list.Count() > 0)
            {
                Dictionary<string, List<DataTransferChannel>> dic = new Dictionary<string, List<DataTransferChannel>>();

                foreach (var u in list)
                {
                    if (u.GroupName == null)
                    {
                        u.GroupName = "";
                    }
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
                        ID = u.ID,
                        Name = u.Name,
                        Remark = "批量导入",
                        SortNo = 0,
                        Status = u.Status
                    });
                }

                //数据加工
                int num = 100;
                List<DataTransferChannel> channelAddList = new List<DataTransferChannel>();
                List<DataTransferChannelGroup> groupAddList = new List<DataTransferChannelGroup>();
                List<DataTransferChannelGroupDetail> detailAddlist = new List<DataTransferChannelGroupDetail>();
                foreach (var key in dic.Keys)
                {
                    //var groupID = SingleIdWork.Instance(Key.WorkID).nextId();
                    if (!key.IsNullOrEmpty())
                    {
                        groupAddList.Add(new DataTransferChannelGroup()
                        {
                            ID = num,
                            Name = key,
                            Remark = "批量导入",
                            SortNo = 1
                        });
                    }

                    var temp = dic[key];
                    foreach (var u in temp)
                    {
                        long? groupID = num;
                        if (key.IsNullOrEmpty())
                        {
                            groupID = null;
                        }
                        channelAddList.Add(new DataTransferChannel()
                        {
                            ID = u.ID,
                            SortNo = u.SortNo,
                            Remark = u.Remark,
                            Name = u.Name,
                            Status = u.Status,
                            ChannelGroupID = groupID,
                        });

                        //if (!key.IsNullOrEmpty())
                        //{
                        //    detailAddlist.Add(new DataTransferChannelGroupDetail()
                        //    {
                        //        ChannelID = u.ID,
                        //        GroupID = num,
                        //        ID = u.ID
                        //    });
                        //}
                    }
                    num++;
                }
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
                //if (detailAddlist.Count > 0)
                //{
                //    _connection.Execute("insert into SmartChannelGroupDetail(ID,GroupID,ChannelID) values(@ID, @GroupID, @ChannelID)",
                //       detailAddlist, _transaction); //渠道组映射
                //}
            }

            Console.WriteLine("渠道导入结束！");
        }

        /// <summary>
        /// 产品分类
        /// </summary>
        public static void ProductCategory()
        {
            Console.WriteLine("物品分类开始导入");
            var list = _mySqlConnection.Query<ProductCategory>(@"select a.id as ID,a.product_category_name as Name,b.id  as PID,
a.`status` as Status,0 as SortNo,'数据迁移' as  Remark
from doc_product_category a
left join doc_product_category b on a.parent_id = b.product_category_id
where a.is_del=0");

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
            var list = _mySqlConnection.Query<ProductCategory>(@"select id as ID,unit_name as Name from doc_unit");

            _connection.Execute("insert into SmartUnit(ID,Name) values (@ID,@Name)",
                    list, _transaction);
            Console.WriteLine("单位结束导入");
        }

        /// <summary>
        /// 产品
        /// </summary>
        public static void Product()
        {
            Console.WriteLine("产品开始导入");
            var list = _mySqlConnection.Query<Product>(@"select a.id as ID,a.product_name as Name,a.pinyin_code as PinYin,
b.id as CategoryID,a.size as Size,a.refer_price as Price,a.`status` as Status,'数据迁移' as Remark, c.id as MiniUnitID,d.id as UnitID,a.num as Scale,a.is_sale as IsSale,sale_price as SalePrice
from doc_product a 
left join doc_product_category b on a.product_category_id=b.product_category_id
left join doc_unit c on a.use_unit_id=c.unit_id
left join doc_unit d  on a.stock_unit_id=d.unit_id
where a.is_del=0;
");
            var chargeResult = new List<Charge>();
            int i = 20000;
            foreach (var u in list)
            {
                u.IsEvaluate = CommonStatus.Stop;
                u.WarehouseID = 15213125084136448;
                u.ChargeCategoryID = 15213054196270080;
                if (u.IsSale == CommonStatus.Use)
                {
                    chargeResult.Add(new Charge()
                    {
                        ID = i + u.ID,
                        CategoryID = 15213054196270080,
                        IsEvaluate = u.IsEvaluate,
                        Name = u.Name,
                        PinYin = u.PinYin,
                        Price = u.SalePrice,
                        ProductAdd = 0,
                        ProductID = u.ID,
                        Remark = "数据迁移",
                        Size = u.Size,
                        Status = u.Status,
                        Type = ChargeType.Product,
                        UnitID = u.UnitID
                    });
                    i++;
                }

            }

            _connection.Execute(@"insert into SmartProduct(ID,Name,PinYin,CategoryID,Size,Price,[Status],Remark,UnitID,MiniUnitID,Scale,IsSale,SalePrice,WarehouseID,IsEvaluate,ChargeCategoryID)
 values(@ID, @Name, @PinYin, @CategoryID, @Size, @Price, @Status, @Remark, @UnitID, @MiniUnitID, @Scale,@IsSale,@SalePrice,@WarehouseID,@IsEvaluate,@ChargeCategoryID)",
                   list, _transaction);

            if (chargeResult.Count() > 0)
            {
                _connection.Execute(@"insert into SmartCharge(ID,Name,CategoryID,PinYin,Price,Status,Remark,UnitID,Size,
ProductAdd,IsEvaluate,ProductID,Type) 
values(@ID, @Name, @CategoryID, @PinYin, @Price, @Status, @Remark, @UnitID,@Size,@ProductAdd,@IsEvaluate,@ProductID,@Type)", chargeResult, _transaction);
            }
            Console.WriteLine("产品结束导入");
        }

        /// <summary>
        /// 项目分类
        /// </summary>
        public static void ChargeCategory()
        {
            Console.WriteLine("项目分类开始导入");
            var list = _mySqlConnection.Query<ChargeCategory>(@"select a.id as ID,a.project_category_name as Name,b.id as ParentID,0 as SortNo,'数据迁移' as Remark
from doc_project_category a 
left join doc_project_category b on a.parent_project_category_id=b.project_category_id
where a.is_del=0
");

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
            var list = _mySqlConnection.Query<Charge>(@"select a.id as ID,a.project_name as Name,a.pym as PinYin,a.project_size as Size,
b.id as CategoryID,a.price as Price,a.`status` as Status,c.id as UnitID,a.is_evaluate as IsEvaluate,1 as ProductAdd
from doc_project a
inner join doc_project_category b on a.project_category_id=b.project_category_id
inner join doc_unit c on a.unit_id=c.unit_id
where a.is_del=0
");

            _connection.Execute(@"insert into SmartCharge(ID,Name,CategoryID,PinYin,Price,Status,Remark,UnitID,Size,ProductAdd,IsEvaluate)
 values(@ID, @Name, @CategoryID, @PinYin, @Price, @Status, @Remark, @UnitID,@Size,@ProductAdd,@IsEvaluate)", list, _transaction);  //收费项目
            Console.WriteLine("项目结束导入");
        }


        /// <summary>
        /// 部门
        /// </summary>
        public static void Dept()
        {
            Console.WriteLine("部门开始导入");
            var list = _mySqlConnection.Query<Dept>(@$"select a.id as ID,a.dept_name as Name,a.`status` as OpenStatus,'数据迁移' as Remark,0 as SortNo, {_hospitalID} as HospitalID 
from doc_dept a where a.is_del=0
");

            _connection.Execute("insert into SmartDept(ID,Name,Remark,OpenStatus,SortNo,HospitalID) values (@ID,@Name,@Remark,@OpenStatus,@SortNo,@HospitalID)",
                    list, _transaction);

            Console.WriteLine("部门结束导入");
        }

        /// <summary>
        /// 代金券
        /// </summary>
        public static void CouponCategory()
        {
            Console.WriteLine("代金券类型开始导入");
            var list = _mySqlConnection.Query<CouponCategoryInfo>(@$"select a.id as ID,a.coupon_name as Name,a.`status` as Status,a.valid_day as Days
from mk_coupon a where a.coupon_type=1 and a.coupon_id in (select coupon_id from op_customer_coupon where rest>0)
");
            var hospitalList = new List<object>();
            foreach (var u in list)
            {
                if (u.Days >= 1000)
                {
                    u.Days = null;
                    u.TimeLimit = 1;
                }
                else
                {
                    u.TimeLimit = 3;
                }
                u.Remark = "数据迁移";
                u.ScopeLimit = 1;
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
            Console.WriteLine("代金券类型结束导入");
        }


        /// <summary>
        /// 预收款类型
        /// </summary>
        public static void DepositCategory()
        {
            Console.WriteLine("预收款类型开始导入");
            var list = _mySqlConnection.Query<DepositChargeInfo>(@$"select a.id as ID,a.recharge_amount as Price,a.deposit_name as Name,a.`status` as Status
,b.coupon_amount as CouponAmount,1 as ScopeLimit,c.id as CouponCategoryID,'数据迁移' as Remark ,0 as IsShopOnly
from mk_deposit a
left join r_deposit_coupon b 
left join mk_coupon c on b.coupon_id=c.coupon_id
on a.policy_id=b.deposit_id where a.policy_id in (select distinct a.deposit_id
from op_customer_deposit a where a.rest_amount>0)

");
            var hospitalList = new List<object>();
            foreach (var u in list)
            {
                if (u.CouponCategoryID == null)
                {
                    u.HasCoupon = 0;
                }
                else
                {
                    u.HasCoupon = 1;
                }
                hospitalList.Add(new
                {
                    ID = SingleIdWork.Instance(1).nextId(),
                    DepositChargeID = u.ID,
                    HospitalID = 1
                });
            }

            _connection.Execute(@"insert into SmartDepositCharge(ID,Name,Price,Status,ScopeLimit,ChargeID,ChargeCategoryID,
HasCoupon,CouponCategoryID,CouponAmount,Remark,IsShopOnly) 
                    VALUES(@ID, @Name, @Price, @Status, @ScopeLimit, @ChargeID, @ChargeCategoryID, @HasCoupon, @CouponCategoryID, @CouponAmount,@Remark,@IsShopOnly)", list, _transaction);

            _connection.Execute("insert into SmartDepositChargeHospital(ID,DepositChargeID,HospitalID) VALUES(@ID, @DepositChargeID, @HospitalID)", hospitalList, _transaction); //预收款适用医院映射表
            Console.WriteLine("预收款类型结束导入");
        }


        /// <summary>
        /// 用户
        /// </summary>
        public static void User()
        {
            Console.WriteLine("用户开始导入");
            var temp = _mySqlConnection.Query<User>(@"select a.id as ID,a.mobile as Account,a.user_name as Name,a.sex as Gender,c.id as DeptID,a.`status` as Status,a.mobile as Mobile
from doc_user a
left join r_user_dept b on a.user_id=b.user_id
left join doc_dept c on b.dept_id=c.dept_id
where a.is_del=0
");

            Dictionary<long, User> dic = new Dictionary<long, User>();
            foreach (var u in temp)
            {
                if (!dic.ContainsKey(u.ID))
                {
                    dic.Add(u.ID, u);
                }
            }

            List<User> list = new List<User>();
            List<UserRole> roleList = new List<UserRole>();
            DateTime now = DateTime.Now;
            foreach (var u in dic.Values)
            {
                u.HospitalID = _hospitalID;
                u.Discount = 1;
                u.CreateTime = now;
                u.CreateUserID = _hospitalID;
                u.Remark = "数据迁移";
                u.Password = HashHelper.GetMd5("123456");
                list.Add(u);
                roleList.Add(new UserRole()
                {
                    RoleID = 1,
                    UserID = u.ID,
                    ID = u.ID
                });
            }

            _connection.Execute(
                    "insert into SmartUser([ID],[Account],[Password],[Name],[Gender],[DeptID],[Status],[Remark],[Phone],[HospitalID],[Discount],[CreateTime],[CreateUserID]) " +
                    "values(@ID,@Account,@Password,@Name,@Gender,@DeptID,@Status,@Remark,@Phone,@HospitalID,@Discount,@CreateTime,@CreateUserID)",
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
            customerAddList.Columns.Add("WeChatBind", typeof(string));
            customerAddList.Columns.Add("WechatBindTime", typeof(DateTime));
            customerAddList.Columns.Add("ImageUrl", typeof(string));

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

            var list = _mySqlConnection.Query<Customer>(@$"select a.id as ID, a.`name` as Name,a.phone as Mobile,
REPLACE(REPLACE(a.backup_number, CHAR(10),''), CHAR(13), '') as MobileBackup,a.sex as Gender,
case when FROM_UNIXTIME(a.birthday/1000) is null then null else FROM_UNIXTIME(a.birthday/1000) end  as Birthday,FROM_UNIXTIME(a.create_time/1000) as CreateTime,
case when c.id is null then 1 else c.id end as CreateUserID,d.ID as ChannelID,null as CurrentConsultSymptomID,f.id as CurrentExploitUserID,
h.id as CurrentManagerUserID,b.integral as Point,b.coin as Commission,i.ID as  PromoterID,REPLACE(REPLACE(a.remark, CHAR(10),''), CHAR(13), '') as Remark,cw.openid_ser as OpenID,cw.create_date as WeChatCreateTime,cw.nick_name as NickName,
a.sex as Sex,cw.province as Province,cw.city as City,cw.country as Country,cw.openid_ser as WeChatBind,cw.create_date as WechatBindTime,
cw.avatar_url as ImageUrl,cw.avatar_url as HeadImgUrl
from cu_customer a
left join r_customer_assets b on a.customer_id=b.customer_id
left join doc_user c on a.create_user_id=c.user_id
left join doc_channel d on a.resource_id=d.channel_id
left join r_customer_ascription e on a.customer_id=e.customer_id and e.consultant_type=1 and e.`status`=1
left join doc_user f on e.consultant=f.user_id
left join r_customer_ascription g on a.customer_id=g.customer_id and g.consultant_type=2 and g.`status`=1
left join doc_user h on g.consultant=h.user_id
left join cu_customer i on a.referrer_id=i.customer_id
left join cu_wechat cw on a.customer_id=cw.customer_id", null, null, true, 6000);

            List<object> commissionList = new List<object>();
            DateTime now = DateTime.Now;

            List<Customer> wechatList = new List<Customer>();

            foreach (var u in list)
            {
                if (u.ChannelID == null)
                {
                    u.ChannelID = _channelID;
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
                    if (u.Remark.Length > 199)
                    {
                        u.Remark = u.Remark.Substring(0, 199);
                    }
                    dr["Remark"] = u.Remark;
                }
                if (u.Birthday != null)
                {
                    dr["Birthday"] = u.Birthday.Value.ToShortDateString();
                }

                if (u.CreateTime == null)
                {
                    dr["CreateTime"] = "2020/1/1";
                }
                else
                {
                    dr["CreateTime"] = u.CreateTime.ToString();
                }
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
                dr["WeChatBind"] = u.WeChatBind;
                if (u.WechatBindTime != null)
                {
                    dr["WechatBindTime"] = u.WechatBindTime;
                }
                dr["ImageUrl"] = u.ImageUrl;



                if (u.PromoterID != null)
                {
                    dr["PromoterID"] = u.PromoterID;
                }
                customerAddList.Rows.Add(dr);

                if (!u.OpenID.IsNullOrEmpty())
                {
                    wechatList.Add(new Customer()
                    {
                        OpenID = u.OpenID,
                        NickName = u.NickName,
                        WeChatCreateTime = u.WeChatCreateTime,
                        Sex = u.Sex,
                        Province = u.Province,
                        City = u.City,
                        Country = u.Country,
                        HeadImgUrl = u.HeadImgUrl,
                        Type = 1,
                        ID = u.ID,
                    });
                }


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
            if (wechatList.Count > 0)
            {
                _connection.Execute(@"insert into SmartWeChat(OpenID,CustomerID,NickName,CreateTime,Sex,Province,City,Country,HeadImgUrl,Type) 
values(@OpenID,@ID,@NickName,@WeChatCreateTime,@Sex,@Province,@City,@Country,@HeadImgUrl,@Type)", wechatList, _transaction);
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
        /// 咨询症状
        /// </summary>
        public static void Symptom()
        {
            Console.WriteLine("咨询症状开始导入");
            var list = _mySqlConnection.Query<DataTransferChannel>(@"select distinct b.id as ID, b.project_category_name as Name 
from op_consult_project_category a
inner
join doc_project_category b on a.project_category_id = b.project_category_id");

            foreach (var u in list)
            {
                u.Remark = "数据迁移";
                u.SortNo = 0;
                u.Status = CommonStatus.Use;
            }

            _connection.Execute(@"insert into [SmartSymptom](ID,Name,[Status],SortNo,Remark) 
values (@ID,@Name,@Status,@SortNo,@Remark)", list, _transaction);

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
            consultList.Columns.Add("EvaluationLevel", typeof(int));
            consultList.Columns.Add("EvaluationContent", typeof(string));
            consultList.Columns.Add("EvaluationTime", typeof(DateTime));

            DataTable detailList = new DataTable("SmartConsultSymptomDetail");
            detailList.Columns.Add("ConsultID", typeof(long));
            detailList.Columns.Add("SymptomID", typeof(long));


            var list = _mySqlConnection.Query<Consult>(@"select distinct a.id as ID,b.id as CustomerID,c.id  as CreateUserID,
FROM_UNIXTIME(a.create_time/1000) as CreateTime,a.remark as Content,d.evaluation_level as EvaluationLevel,
d.evaluation_content as EvaluationContent,FROM_UNIXTIME(d.create_time/1000) as EvaluationTime
from op_consult a
inner join cu_customer b on a.customer_id=b.customer_id
inner join doc_user c on a.create_user_id=c.user_id
left join op_consult_evaluation d on a.consult_id=d.consult_id order by a.id", null, null, true, 6000);



            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                DataRow dr = consultList.NewRow();
                dr["ID"] = u.ID;
                //dr["CustomerID"] = new Random().Next(958266,1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["Tool"] = _tool;
                dr["HospitalID"] = _hospitalID;
                if (u.Content.Length >= 1999)
                {
                    dr["Content"] = u.Content.Substring(0, 1999);
                }
                else
                {
                    dr["Content"] = u.Content;
                }
                if (u.EvaluationLevel != null)
                {
                    dr["EvaluationLevel"] = u.EvaluationLevel;
                }
                if (u.EvaluationTime != null)
                {
                    dr["EvaluationTime"] = u.EvaluationTime;
                }
                dr["EvaluationContent"] = u.EvaluationContent;
                consultList.Rows.Add(dr);



            }

            var symptomList = _mySqlConnection.Query<ConsultDetail>(@"select a.id as ConsultID,c.id as SymptomID
from op_consult a
inner join op_consult_project_category b on a.consult_id=b.consult_id
inner join doc_project_category c on b.project_category_id=c.project_category_id", null, null, true, 6000);
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


            var list = _mySqlConnection.Query<CallBack>(@$"select b.id as CustomerID,c.id as  CreateUserID,
FROM_UNIXTIME(a.create_time/1000) as CreateTime,
case when a.type=1 then {_callBackCategoryOfSH}  when a.type=2 then {+_callBackCategoryOfWD} else {_callBackCategoryOfXC}  end as CategoryID,
c.id as  UserID,FROM_UNIXTIME(a.call_back_time/1000) as TaskTime,a.call_back_content as Name 
from doc_call_back a
inner join cu_customer b on a.customer_id=b.customer_id
inner join doc_user c on a.create_user_id=c.user_id
where a.`status`= 0", null, null, true, 6000);

            DateTime now = DateTime.Now;
            int num = 10000;
            foreach (var u in list)
            {
                DataRow dr = callbackList.NewRow();
                dr["ID"] = num;
                //dr["CustomerID"] = new Random().Next(958266,1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["CategoryID"] = u.CategoryID;
                if (u.Name.Length > 50)
                {
                    dr["Name"] = u.Name.Substring(0, 50);
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
                num++;
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


            var list = _mySqlConnection.Query<CallBack>(@$"select b.id as CustomerID,c.id as CreateUserID,FROM_UNIXTIME(a.create_time/1000) as CreateTime,
case when a.callback_type=1 then {_callBackCategoryOfWD} else {_callBackCategoryOfXC} end as CategoryID,'' as Name,c.id as UserID,
FROM_UNIXTIME(a.create_time/1000) as TaskTime,a.record as Content,c.id as TaskCreateUserID,FROM_UNIXTIME(a.create_time/1000) as TaskCreateTime
from r_customer_callback a
inner join cu_customer b on a.customer_id=b.customer_id
inner join doc_user c on a.create_user_id=c.user_id
union all
select b.id,c.id,FROM_UNIXTIME(a.finish_time/1000),{_callBackCategoryOfSH},'',c.id,FROM_UNIXTIME(a.finish_time/1000) ,
a.visit_record,c.id,FROM_UNIXTIME(a.finish_time/1000) 
from doc_postoperative_visit a
inner join cu_customer b on a.customer_id=b.customer_id
inner join doc_user c on a.update_user=c.user_id", null, null, true, 6000);

            int num = _callbackNum;
            foreach (var u in list)
            {
                DataRow dr = callbackList.NewRow();
                dr["ID"] = num;
                //dr["CustomerID"] = new Random().Next(958266,1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["CategoryID"] = u.CategoryID;
                if (u.Name != null && u.Name.Length > 50)
                {
                    dr["Name"] = u.Name.Substring(0, 50);
                }
                else
                {
                    dr["Name"] = u.Name;
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
                dr["Tool"] = _tool;
                dr["TaskCreateTime"] = u.TaskCreateTime;
                dr["TaskCreateUserID"] = u.TaskCreateUserID;

                callbackList.Rows.Add(dr);
                num++;
            }
            if (callbackList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartCallback", callbackList);
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


            var list = _mySqlConnection.Query<Visit>(@"select b.id as CustomerID,c.id as CreateUserID,FROM_UNIXTIME(a.split_time/1000) as CreateTime,d.id as UserID,
case when a.property=1 then 1 when a.property=2 then 2 when a.property=3 then 4 else 3 end as VisitType,
case when a.order_id is null then 0 else 1 end as DealType,f.id as ExploitUserID,h.id as ManagerUserID 
from r_customer_split a
inner join cu_customer b on a.customer_id=b.customer_id
inner join doc_user c on a.create_user=c.user_id
left join doc_user d on a.consultant_id=d.user_id
left join r_customer_ascription e on a.customer_id=e.customer_id and e.consultant_type=1 and e.`status`=1
left join doc_user f on e.consultant=f.user_id
left join r_customer_ascription g on a.customer_id=g.customer_id and g.consultant_type=2 and g.`status`=1
left join doc_user h on g.consultant=h.user_id", null, null, true, 6000);

            foreach (var u in list)
            {
                DataRow dr = visitList.NewRow();
                //dr["CustomerID"] = new Random().Next(958266, 1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.UserID;
                dr["CreateTime"] = u.CreateTime;
                dr["HospitalID"] = _hospitalID;
                dr["VisitType"] = u.VisitType;

                if (u.UserID != null)
                {
                    dr["UserID"] = u.UserID;

                    DataRow dr2 = userVisitList.NewRow();
                    dr2["CustomerID"] = u.CustomerID;
                    dr2["CreateUserID"] = u.CreateUserID;
                    dr2["CreateTime"] = u.CreateTime;
                    dr2["HospitalID"] = _hospitalID;
                    dr2["VisitType"] = u.VisitType;
                    dr2["AssignUserID"] = u.UserID;
                    dr2["Remark"] = "数据迁移分配";
                    if (u.ExploitUserID != null)
                    {
                        dr2["CurrentExploitUserID"] = u.ExploitUserID;
                    }
                    if (u.ManagerUserID != null)
                    {
                        dr2["CurrentManagerUserID"] = u.ManagerUserID;
                    }
                    userVisitList.Rows.Add(dr2);
                }
                dr["IsConsume"] = u.DealType;
                if (u.ExploitUserID != null)
                {
                    dr["CurrentExploitUserID"] = u.ExploitUserID;
                }
                if (u.ManagerUserID != null)
                {
                    dr["CurrentManagerUserID"] = u.ManagerUserID;
                }

                visitList.Rows.Add(dr);
            }


            if (visitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartVisit", visitList);
            }
            if (userVisitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartTriage", userVisitList);
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


            var list = _mySqlConnection.Query<Coupon>(@"select a.id as ID,c.id as CustomerID,a.rest as Amount,
FROM_UNIXTIME(a.valid_date_end/1000)  as ExpirationDate,b.id as CategoryID
from op_customer_coupon a
inner join mk_coupon b on a.coupon_id=b.coupon_id
inner join cu_customer c on a.customer_id=c.customer_id
where a.rest>=1 and a.valid_date_end>=@Date", new { Date = DateTime.Today.ToLocalUnixTimestamp() }, null, true, 6000);

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
                dr["ExpirationDate"] = u.ExpirationDate;

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


            var list = _mySqlConnection.Query<Deposit>(@"select a.id as ID,c.id as CustomerID,a.rest_amount as Amount
,f.id as ExploitUserID,h.id as ManagerUserID,b.id as ChargeID
from op_customer_deposit a
inner join mk_deposit b on a.deposit_id=b.policy_id
inner join cu_customer c on a.customer_id=c.customer_id
left join r_customer_ascription e on a.customer_id=e.customer_id and e.consultant_type=1 and e.`status`=1
left join doc_user f on e.consultant=f.user_id
left join r_customer_ascription g on a.customer_id=g.customer_id and g.consultant_type=2 and g.`status`=1
left join doc_user h on g.consultant=h.user_id
where a.rest_amount>0", null, null, true, 6000);

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
                dr["BuyVisitType"] = 2;

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
            var list = _mySqlConnection.Query<ChargeSet>(@"select a.id as ID,a.combo_name as Name,a.pym as PinYin,a.total_price as Price,a.`status` as Status 
from mk_combo a where a.is_del=0");

            var listDetaik = _mySqlConnection.Query<SmartChargeSetDetail>(@"select a.id as ID,b.id as SetID,a.size as Num,a.combo_amount as Amount,c.id as ChargeID 
from r_combo_project a
inner join mk_combo b on a.comboid=b.comboid
inner join doc_project c on a.project_id=c.project_id
where b.is_del=0
");
            DateTime now = DateTime.Now;
            foreach (var u in list)
            {
                if (u.PinYin.IsNullOrEmpty())
                {
                    u.PinYin = u.Name.PinYin();
                }
                u.TimeLimit = 0;
                u.TimeStart = 0;
                u.Days = 0;
                u.CreateUserID = 1;
                u.CreateTime = now;
            }

            _connection.Execute(@"insert into SmartChargeSetDetail(ID,SetID,ChargeID,Num,Amount) 
                                            VALUES(@ID, @SetID, @ChargeID, @Num, @Amount)", listDetaik, _transaction);
            _connection.Execute(@"insert into SmartChargeSet(ID,Name,Price,Status,Remark,PinYin,TimeLimit,TimeStart,Days,HospitalID,CreateUserID,CreateTime) 
                                    VALUES(@ID, @Name, @Price, @Status, @Remark, @PinYin, @TimeLimit, @TimeStart, @Days, @HospitalID,@CreateUserID,@CreateTime)", list, _transaction);

            //_connection.Execute(@"update SmartChargeSet set PinYin='' where PinYin is null", null, _transaction);
            Console.WriteLine("项目套餐结束导入");
        }


        /// <summary>
        /// 订单
        /// </summary>
        public static void Order()
        {
            Console.WriteLine("订单记录开始迁移");

            var tempList = _mySqlConnection.Query<Order>(@"select a.id as ID,FROM_UNIXTIME(a.paid_time/1000) as CreateTime,
d.id as CreateUserID,c.id as CustomerID,
g.id as ChargeID,b.num as Num,b.rest_num as RestNum,b.original_price as Price,b.total_price  as FinalPrice,
b.total_price-b.use_deposit-b.use_send_deposit-b.use_coin-b.use_coupon-b.use_guarantee as CashAmount,
b.use_deposit as DepositAmount,b.use_coupon+b.use_send_deposit as CouponAmount,b.use_coin as CommissionAmount,b.use_guarantee as DebtAmount,
case when a.customer_property=1 then 1 when a.customer_property=2 then 2 when a.customer_property=3 then 4 else 3 end as VisitType,
e.id as ExploitUserID,f.id as ManagerUserID,1 as DealType,b.ID as OrderDetailID,i.id as SetID  
from op_order a
inner join r_order_project b on a.order_id=b.order_id
inner join cu_customer c on a.customer_id=c.customer_id
inner join doc_user d on a.create_user_id=d.user_id
left join doc_user e on a.online_id=e.user_id
left join doc_user f on a.live_id=f.user_id
inner join doc_project g on b.project_id=g.project_id
left join mk_combo i on b.project_combo_id=i.comboid
where a.`status`=2 order by a.paid_time", null, null, true, 60000);

            DateTime now = DateTime.Now;

            Dictionary<long, List<Order>> list = new Dictionary<long, List<Order>>();
            foreach (var u in tempList)
            {
                Order temp = new Order();
                temp.CreateTime = u.CreateTime;
                if (temp.CreateTime == null)
                {
                    temp.CreateTime = DateTime.Parse("2020/1/1");
                }
                temp.CreateUserID = u.CreateUserID;
                temp.CustomerID = u.CustomerID;
                temp.ChargeID = u.ChargeID;
                temp.Num = u.Num;
                temp.RestNum = u.RestNum;
                temp.Price = u.Price;
                temp.FinalPrice = u.FinalPrice;
                temp.CashAmount = u.CashAmount;
                temp.DepositAmount = u.DepositAmount;
                temp.CouponAmount = u.CouponAmount;
                temp.CommissionAmount = u.CommissionAmount;
                temp.DebtAmount = u.DebtAmount;
                temp.VisitType = u.VisitType;
                temp.SetID = u.SetID;
                temp.OrderDetailID = u.OrderDetailID;
                if (u.SetID != null)
                {
                    temp.SetNum = 1;
                    u.SetNum = 1;
                    u.SetPrice = u.FinalPrice;
                    temp.SetPrice = u.FinalPrice;
                    u.SetFinalPrice = u.FinalPrice;
                    temp.SetFinalPrice = u.FinalPrice;
                }

                if (u.ExploitUserID != null)
                {
                    temp.ExploitUserID = u.ExploitUserID;
                }
                if (u.ManagerUserID != null)
                {
                    temp.ManagerUserID = u.ManagerUserID;
                }
                temp.DealType = u.DealType;
                temp.Remark = "数据迁移";


                if (list.ContainsKey(u.ID))
                {
                    list[u.ID].Add(temp);
                }
                else
                {
                    list.Add(u.ID, new List<Order>() { temp });
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
                #endregion
                int num = 10000;
                //int detailNum = 10000;
                foreach (var u in list.Values.AsParallel())
                {
                    #region order
                    var id = num;
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
                    order["VisitType"] = u.FirstOrDefault().VisitType;
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
                    order["DealType"] = u.FirstOrDefault().DealType;
                    order["HospitalID"] = _hospitalID;
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
                    orderList.Rows.Add(order);
                    #endregion

                    foreach (var x in u)
                    {
                        //detailNum++;
                        #region detail
                        var detail = detailList.NewRow();
                        var detailID = x.OrderDetailID;
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
                        cashier["HospitalID"] = _hospitalID;
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
                        cashier["VisitType"] = x.VisitType;
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
                        cashier["BuyVisitType"] = x.VisitType;
                        cashier["BuyOrderUserID"] = x.CreateUserID;
                        cashier["DealType"] = x.DealType;
                        cashierList.Rows.Add(cashier);
                        #endregion

                    }
                    num++;

                }

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

                //1、更新累计消费金额
                _connection.Execute(@"update SmartCustomer set CashCardTotalAmount=b.Amount 
  from SmartCustomer a
  inner join (
  select a.CustomerID,sum(case when a.OrderType in (1,2) then a.CashCardAmount+a.DepositAmount+a.DebtAmount+a.CommissionAmount 
  else (a.CashCardAmount+a.DepositAmount+a.DebtAmount+a.CommissionAmount)*-1 end) as Amount
  from SmartCashierCharge a
  where a.OrderType in (1,2,4,8) group by a.CustomerID) as b on a.ID=b.CustomerID", null, _transaction);
                //2、更新会员等级
                _connection.Execute(@"update SmartCustomer set MemberCategoryID=c.ID 
from SmartCustomer a
inner join (
select a.ID,max(b.Level) as Level
from SmartCustomer a
inner join SmartMemberCategory as b on a.CashCardTotalAmount>b.Amount group by a.ID) as b on a.ID=b.ID
inner join SmartMemberCategory as c on b.Level=c.Level", null, _transaction);
            }

            Console.WriteLine("订单记录结束迁移");
        }

        /// <summary>
        /// 退款单开始导入
        /// </summary>
        public static void BackOrder()
        {
            //Console.WriteLine("(S)中下身吸脂基础型".PinYin());
            Console.WriteLine("退款单开始导入");
            var list = _mySqlConnection.Query<DepositOrder>(@"select a.id,b.id as CustomerID,a.project_amount as Amount, FROM_UNIXTIME(a.pay_time/1000) as CreateTime,
a.reason_id as Remark,c.id as ManagerUserID,d.id as ExploitUserID,e.id as CreateUserID,a.deposit_amount
from op_order_back a
inner join cu_customer b on a.customer_id=b.customer_id
left join doc_user c on a.live_id=c.user_id
left join doc_user d on a.online_id=d.user_id
left join doc_user e on a.create_user_id=e.user_id
where a.`status`=2 and a.order_back_id in (select distinct order_back_id from r_order_back_project)");

            var detailList = _mySqlConnection.Query<DepositOrderDetial>(@"select a.id,b.id as CustomerID, FROM_UNIXTIME(a.pay_time/1000) as CreateTime,
a.reason_id as Remark,c.id as ManagerUserID,d.id as ExploitUserID,e.id as CreateUserID,f.id as DetailID,
f.num as Num,f.back_amount as Amount,g.id as ChargeID,case when a.back_type=1 then 0 else f.back_amount end as DepositAmount 
from op_order_back a
inner join cu_customer b on a.customer_id=b.customer_id
left join doc_user c on a.live_id=c.user_id
left join doc_user d on a.online_id=d.user_id
left join doc_user e on a.create_user_id=e.user_id
inner join r_order_back_project f on a.order_back_id=f.order_back_id
inner join doc_project g on f.project_id=g.project_id
where a.`status`=2
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
                cashierList.Add(new
                {
                    CashierID = 0,
                    ReferID = u.ID,
                    CashCardAmount = u.Amount - u.DepositAmount,
                    DepositAmount = u.DepositAmount,
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
            _connection.Execute(
                    @"insert into [SmartBackOrder]([ID],[HospitalID],[CustomerID],[CreateUserID],[CreateTime],[Amount],[Point],[PaidStatus],[Remark],[AuditStatus],VisitType,SourceType,ExploitUserID,ManagerUserID,RainFlyType) 
                        values(@ID,@HospitalID,@CustomerID,@CreateUserID,@CreateTime,@Amount,@Point,@PaidStatus,@Remark,@AuditStatus,@VisitType,@SourceType,@ExploitUserID,@ManagerUserID,@RainFlyType)",
                    list, _transaction);

            _connection.Execute(
                   @"insert into [SmartBackOrderDetail]([ID],[OrderID],[ChargeID],[Num],[Amount],[DetailID],BuyOrderID,BuyExploitUserID,BuyManagerUserID,BuyOrderUserID,BuyVisitType) 
                    values(@ID,@OrderID,@ChargeID,@Num,@Amount,@DetailID,@BuyOrderID,@ExploitUserID,@ManagerUserID,@CreateUserID,@VisitType)", detailList, _transaction);

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
        /// 划扣
        /// </summary>
        public static void Operation()
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
            visitList.Columns.Add("EvaluationTime", typeof(DateTime));
            visitList.Columns.Add("EvaluationLevel", typeof(int));
            visitList.Columns.Add("EvaluationContent", typeof(string));
            visitList.Columns.Add("BuyHospitalID", typeof(long));
            visitList.Columns.Add("RainFlyType", typeof(int));




            var list = _mySqlConnection.Query<Operation>(@"select a.deduct_count as Num,g.id as CreateUserID,FROM_UNIXTIME(a.create_time/1000) as CreateTime,a.remark as Remark,c.id as OrderDetailID,
e.id as CustomerID,f.id as ChargeID,h.id as DeptID,i.id as DoctorID,
case when j.create_time is null then null else FROM_UNIXTIME(j.create_time/1000) end as EvaluationTime,j.cure_level as EvaluationLevel,j.cure_level_tags as EvaluationContent
FROM op_deduct_record  a
inner join tc_confirm_project  b ON a.confirm_project_id = b.id
INNER JOIN r_order_project  c ON a.order_project_id = c.order_project_id
INNER JOIN cu_customer  e ON e.customer_id = a.customer_id
INNER JOIN doc_project  f ON c.project_id = f.project_id
left join doc_user g on a.creater=g.user_id
left join doc_dept h on h.dept_id=b.dept_id
left join doc_user i on b.doctor_id=i.user_id
left join op_order_project_evaluation j on j.deduct_id=a.deduct_id
where a.is_del=0", null, null, true, 6000);

            DateTime now = DateTime.Now;
            var num = 100000;
            foreach (var u in list)
            {
                DataRow dr = visitList.NewRow();
                dr["ID"] = num;
                //dr["CustomerID"] = new Random().Next(958266, 1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = u.CreateUserID;
                dr["CreateTime"] = u.CreateTime;
                dr["HospitalID"] = _hospitalID;
                dr["ChargeID"] = u.ChargeID;
                dr["Num"] = u.Num;
                dr["Remark"] = u.Remark;
                dr["DeptID"] = u.DeptID;
                dr["DoctorID"] = u.DoctorID;
                dr["OrderDetailID"] = u.OrderDetailID;
                dr["CustomerStatus"] = 0;
                if (u.EvaluationTime != null)
                {
                    dr["EvaluationTime"] = u.EvaluationTime;
                    dr["EvaluationLevel"] = u.EvaluationLevel;
                    dr["EvaluationContent"] = u.EvaluationContent;
                }

                dr["BuyHospitalID"] = _hospitalID;
                dr["RainFlyType"] = 0;



                visitList.Rows.Add(dr);
                num++;
            }

            if (visitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartOperation", visitList);
            }

            Console.WriteLine("划扣记录结束迁移");
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



            var list = _mySqlConnection.Query<Operation>(@"select c.id as CustomerID,FROM_UNIXTIME(a.create_time/1000) as CreateTime,a.num as Num,b.id as ChargeID,a.remark as Remark
from op_old_operation a
inner join doc_project b on a.project_id=b.project_id
inner join cu_customer c on a.customer_id=c.customer_id", null, null, true, 6000);

            DateTime now = DateTime.Now;
            var num = 100000;
            foreach (var u in list)
            {
                DataRow dr = visitList.NewRow();
                dr["ID"] = num;
                //dr["CustomerID"] = new Random().Next(958266, 1430913);
                dr["CustomerID"] = u.CustomerID;
                dr["CreateUserID"] = 1;
                dr["CreateTime"] = u.CreateTime;
                dr["HospitalID"] = _hospitalID;
                dr["ChargeID"] = u.ChargeID;
                dr["Num"] = u.Num;
                dr["Remark"] = u.Remark;
                dr["DeptID"] = 0;
                dr["DoctorID"] = 0;
                dr["OrderDetailID"] = 0;
                dr["CustomerStatus"] = 0;

                visitList.Rows.Add(dr);
                num++;
            }

            if (visitList.Rows.Count > 0)
            {
                SqlBulkCopyByDataTable("SmartOperation", visitList);
            }

            Console.WriteLine("划扣记录结束迁移");
        }

        /// <summary>
        /// 提点
        /// </summary>
        public static void Rebate()
        {
            Console.WriteLine("提点导入开始！");
            string str = @"9927,9928,9929,9930,9931,9932,9933,9934,9935,9936,10440,10487,10488,10489,10490,10491,10492,10493,10494,10495,10496,10497,
10498,10499,10500,10501,10502,10503,10504,10505,10506,10507,10508,10509,10510,10511,10512,10513,10514,10515,10534,10571,27553,27573,27590,27594,27601,
27607,27609,27611,27752,27757,27867,27873,27894,27953,27955,27957,27959,27961,27963,27965,27967,27969,27971,27973,27975,27977,27979,27981,27983,27985,27987,
27989,27991,27993,27995,27997,27999,28002,28004,28006,28008,28010,28012,28014,28016,28018,28020,28022,28024,28026,28028,28030,28032,28034,28037,28039,28041,
28043,28045,28047,28049,28051,28053,28055,28057,28059,28061,28064,28066,28069,28071,28073,28075,28077,28079,28081,28083,28085,28087,28089,28091,28093,28095,
28097,28099,28101,28103,28105,28107,28109,28114,28116,28118,28120,28122,28124,28126,28128,28130,28132,28134,28136,28138,28140,28142,28144,28146,28148,28151,28153,
28155,28157,28160,28173,28177,28179,28181,28183,28187,28196,28205,28207,28236,28238,28256,28258,28269,28271,28273,28315,28322,28332,28340,28342,28344,28346,28387,
28410,28414,28431,28464,28466,28473,28475,28484,28486,28488,28490,28529,28532,28534,28538,28540,28549,28551,28553,28558,28580,28582,28584,28586,28599,28610,28612,
28615,28619,28621,28634,28637,28639,28641,28643,28657,28666,28668,28685,28687,28689,28691,28704,28707,28709,28715,28735,28739,28768,28770,28773,28775,28782,28785,
28787,28789,28791,28794,28805,28809,28817,28819,28824,28826,28830,28833,28839,28853,28855,28858,28861,28863,28865,28868,28877,28896,28921,28923,28933,28935,28938,
28940,28946,28950,28952,28954,28963,28966,28978,28989,28993,28996,28998,29003,29005,29007,29009,29011,29013,29022,29024,29029,29036,29041,29051,29053,29055,29058,
29061,29063,29065,29067,29069,29071,29073,29075,29077,29079,29081,29083,29085,29088,29090,29092,29094,29096,29098,29100";

            var list = str.Split(',');

            var data = new List<Rebate>();
            foreach (var u in list)
            {
                long ID = SingleIdWork.Instance(1).nextId();
                data.Add(new Rebate()
                {
                    ID = ID,
                    ChargeID = long.Parse(u),
                    Level1 = 0,
                    Level2 = 0,
                    Level3 = 0,
                    Level4 = 0,
                    Level5 = 0,
                    Status = CommonStatus.Use,
                    Discount = 50,
                    Type = 17,
                    TimeLimit = 1,
                });
                //data.Add(new Rebate()
                //{
                //    ID = SingleIdWork.Instance(1).nextId(),
                //    Level1 = 10,
                //    Level2 = 0,
                //    Level3 = 0,
                //    Level4 = 0,
                //    Level5 = 0,
                //    Status = CommonStatus.Use,
                //    Discount = 50,
                //    Type = 18,
                //    TimeLimit = 1,
                //    PID = ID,
                //    IsOld = 0
                //});
                //data.Add(new Rebate()
                //{
                //    ID = SingleIdWork.Instance(1).nextId(),
                //    Level1 = 5,
                //    Level2 = 0,
                //    Level3 = 0,
                //    Level4 = 0,
                //    Level5 = 0,
                //    Status = CommonStatus.Use,
                //    Discount = 50,
                //    Type = 19,
                //    TimeLimit = 1,
                //    PID = ID,
                //    IsOld = 1
                //});
            }

            _connection.Execute(@"insert into SmartRebate(ID,ChargeID,Level1,Level2,Level3,Level4,Level5,Status,Discount,Type,TimeLimit,PID,IsOld) 
values(@ID,@ChargeID,@Level1,@Level2,@Level3,@Level4,@Level5,@Status,@Discount,@Type,@TimeLimit,@PID,@IsOld)", data, _transaction);

            Console.WriteLine("提点导入结束！");

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
    }
}

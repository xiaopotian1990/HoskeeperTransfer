using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HoskeeperTransfer
{
    class OrderAutoNumber
    {
        public static readonly string customerStr = "customer";
        public static readonly string cashierStr = "cashier";
        public static readonly string orderStr = "order";
        public static readonly string orderBackStr = "orderBack";
        public static readonly string indemnityStr = "indemnity";
        public static readonly string depositStr = "deposit";
        public static readonly string depositBackStr = "depositBack";
        private static readonly object _object = new object();


        private static long customerNumber = 10000;
        private static readonly object customerObject = new object();

        private static long orderNumber = 10000;
        private static readonly object orderObject = new object();

        private static long depositBackNumber = 10000;
        private static readonly object depositBackObject = new object();

        private static long depositNumber = 10000;
        private static readonly object depositObject = new object();

        private static long orderBackNumber = 10000;
        private static readonly object orderBackObject = new object();

        private static long indemnityNumber = 10000;
        private static readonly object indemnityObject = new object();

        private static long cashierNumber = 10000;
        private static readonly object cashierObject = new object();


        private static OrderAutoNumber _instance = null;
        private OrderAutoNumber()
        {
        }

        public static OrderAutoNumber Instance()
        {
            if (_instance == null)
            {
                lock (_object)
                {
                    if (null == _instance)
                    {
                        _instance = new OrderAutoNumber();
                        _instance.Init();
                    }
                }
            }

            return _instance;
        }


        /// <summary>
        /// 库存相关单号生成
        /// </summary>
        /// <param name="workID">机器ID</param>
        /// <param name="qz">标志： JH代表入库，TH代表出库，DB代表调拨，PD代表盘点,SY代表领用</param>
        /// <returns></returns>
        public long Number(string qz)
        {
            long number = 0;

            if (qz == orderStr)
            {
                lock (orderObject)
                {
                    orderNumber++;
                    number = orderNumber;
                }
            }
            else if (qz == orderBackStr)
            {
                lock (orderBackObject)
                {
                    orderBackNumber++;
                    number = orderBackNumber;
                }
            }
            else if (qz == indemnityStr)
            {
                lock (indemnityObject)
                {
                    indemnityNumber++;
                    number = indemnityNumber;
                }
            }
            else if (qz == depositStr)
            {
                lock (depositObject)
                {
                    depositNumber++;
                    number = depositNumber;
                }
            }
            else if (qz == depositBackStr)
            {
                lock (depositBackObject)
                {
                    depositBackNumber++;
                    number = depositBackNumber;
                }
            }
            else if (qz == cashierStr)
            {
                lock (cashierObject)
                {
                    cashierNumber++;
                    number = cashierNumber;
                }
            }
            else if (qz == customerStr)
            {
                lock (customerObject)
                {
                    customerNumber++;
                    number = customerNumber;
                }
            }

            return number;
        }


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            SqlConnection _connection = null;
            try
            {
                _connection = new SqlConnection("Data Source=47.105.89.85;Initial Catalog=Hoskeeper;Persist Security Info=True;User ID=sa;Password=Ytym#!@2020123456;MultipleActiveResultSets = true;connect timeout=900000000");
                _connection.Open();

                var customerTemp = _connection.Query<long>(@"select top 1 ID from SmartCustomer order by ID desc").FirstOrDefault();
                if (customerTemp > 0)
                {
                    customerNumber = customerTemp;
                }

                var orderTemp = _connection.Query<long>(@"select top 1 ID from SmartOrder order by ID desc").FirstOrDefault();
                if (orderTemp > 0)
                {
                    orderNumber = orderTemp;
                }

                var depositTemp = _connection.Query<long>(@"select top 1 ID from SmartDepositOrder order by ID desc").FirstOrDefault();
                if (depositTemp > 0)
                {
                    depositNumber = depositTemp;
                }

                var orderBackTemp = _connection.Query<long>(@"select top 1 ID from SmartBackOrder order by ID desc").FirstOrDefault();
                if (orderBackTemp > 0)
                {
                    orderBackNumber = orderBackTemp;
                }

                var indemnityTemp = _connection.Query<long>(@"select top 1 ID from SmartIndemnity order by ID desc").FirstOrDefault();
                if (indemnityTemp > 0)
                {
                    indemnityNumber = indemnityTemp;
                }

                var depositBackTemp = _connection.Query<long>(@"select top 1 ID from SmartDepositRebateOrder order by ID desc").FirstOrDefault();
                if (depositBackTemp > 0)
                {
                    depositBackNumber = depositBackTemp;
                }

                var cashierTemp = _connection.Query<long>(@"select top 1 ID from SmartCashier order by ID desc").FirstOrDefault();
                if (cashierTemp > 0)
                {
                    cashierNumber = cashierTemp;
                }
            }
            finally
            {
                _connection.Close();
            }

        }
    }
}

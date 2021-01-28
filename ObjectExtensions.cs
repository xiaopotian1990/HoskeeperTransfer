using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Text;

namespace HoskeeperTransfer
{
    /// <summary>
    /// 基类型<see cref="Object"/>扩展辅助操作类
    /// </summary>
    /// <remarks>
    /// 2015-12-01:创建. zoulu </br>
    /// </remarks>
    public static class ObjectExtensions
    {
        #region 公共方法

        /// <summary>
        /// 把对象类型转换为指定类型
        /// </summary>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="conversionType"> 转化后的动态类型 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static object CastTo(this object value, Type conversionType)
        {
            if (value == null)
            {
                return null;
            }
            //判断类型是否为Nullable类型
            //if (conversionType.IsNullableType())
            //{
            //    conversionType = conversionType.GetUnNullableType();
            //}
            //判断是否为枚举类型
            if (conversionType.IsEnum)
            {
                return Enum.Parse(conversionType, value.ToString());
            }
            //判断是否为GUID类型
            if (conversionType == typeof(Guid))
            {
                return Guid.Parse(value.ToString());
            }
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 转化后的动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static T CastTo<T>(this object value)
        {
            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }
            object result = CastTo(value, typeof(T));
            return (T)result;
        }

        /// <summary>
        /// 把对象类型转化为指定类型
        /// </summary>
        /// <typeparam name="T"> 转化后的动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <returns> 转化后的指定类型的对象，转化失败引发异常。 </returns>
        public static T CastToWithNull<T>(this object value)
        {
            if (value == null)
                return default(T);
            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }
            object result = CastTo(value, typeof(T));
            return (T)result;
        }

        /// <summary>
        /// 把对象类型转化为指定类型，转化失败时返回指定的默认值
        /// </summary>
        /// <typeparam name="T"> 转化后的动态类型 </typeparam>
        /// <param name="value"> 要转化的源对象 </param>
        /// <param name="defaultValue"> 转化失败返回的指定默认值 </param>
        /// <returns> 转化后的指定类型对象，转化失败时返回指定的默认值 </returns>
        public static T CastTo<T>(this object value, T defaultValue)
        {
            try
            {
                return CastTo<T>(value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 判断当前值是否介于指定范围内
        /// </summary>
        /// <typeparam name="T"> 动态类型 </typeparam>
        /// <param name="value"> 动态类型对象 </param>
        /// <param name="start"> 范围起点 </param>
        /// <param name="end"> 范围终点 </param>
        /// <param name="leftEqual"> 是否可等于上限（默认等于） </param>
        /// <param name="rightEqual"> 是否可等于下限（默认等于） </param>
        /// <returns> 是否介于 </returns>
        public static bool IsBetween<T>(this IComparable<T> value, T start, T end, bool leftEqual = false, bool rightEqual = false) where T : IComparable
        {
            bool flag = leftEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            return flag && (rightEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0);
        }

        /// <summary>
        /// 将对象序列化为JSON字符串，不支持存在循环引用的对象
        /// </summary>
        /// <typeparam name="T">动态类型</typeparam>
        /// <param name="value">动态类型对象</param>
        /// <returns>JSON字符串</returns>
        public static string ToJsonString<T>(this T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 将对象[主要是匿名对象]转换为dynamic
        /// </summary>
        /// <remarks>
        /// EntityFramework 使用 linq 查询匿名结果的方式很好用，性能又好，爽呆了，但由于匿名结果的可访问性是 internal，
        /// 可是不能直接返回给View（View在编译之后是另外一个程序集了，internal不能跨程序集）使用，
        /// 来个扩展方法专门把匿名对象转换为dynamic吧
        /// Demo:
        /// //Controller 端，转换为dynamic
        /// var data = new { Id = 1, Name = "GMF" };
        /// dynamic result = data.ToDynamic();
        /// Viewbag.Result = result;
        /// ...
        /// //View 端，可以直接用了
        /// dynamic result = Viewbag.Result;
        /// @result.Id
        /// @result.Name
        /// ...
        /// </remarks>
        public static dynamic ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            Type type = value.GetType();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor property in properties)
            {
                var val = property.GetValue(value);
                if (property.PropertyType.FullName.StartsWith("<>f__AnonymousType"))
                {
                    dynamic dval = val.ToDynamic();
                    expando.Add(property.Name, dval);
                }
                else
                {
                    expando.Add(property.Name, val);
                }
            }
            return expando as ExpandoObject;
        }
        #endregion
    }
}

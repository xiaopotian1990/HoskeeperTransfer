﻿using Microsoft.International.Converters.PinYinConverter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HoskeeperTransfer
{
    public static class StringEx
    {
        #region 正则表达式

        /// <summary>
        /// 指示所指定的正则表达式在指定的输入字符串中是否找到了匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false</returns>
        public static bool IsMatch(this string value, string pattern)
        {
            if (value == null)
            {
                return false;
            }
            return Regex.IsMatch(value, pattern);
        }

        /// <summary>
        /// 在指定的输入字符串中搜索指定的正则表达式的第一个匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns>一个对象，包含有关匹配项的信息</returns>
        public static string Match(this string value, string pattern)
        {
            if (value == null)
            {
                return null;
            }
            return Regex.Match(value, pattern).Value;
        }

        /// <summary>
        /// 在指定的输入字符串中搜索指定的正则表达式的所有匹配项的字符串集合
        /// </summary>
        /// <param name="value"> 要搜索匹配项的字符串 </param>
        /// <param name="pattern"> 要匹配的正则表达式模式 </param>
        /// <returns> 一个集合，包含有关匹配项的字符串值 </returns>
        public static IEnumerable<string> Matches(this string value, string pattern)
        {
            if (value == null)
            {
                return new string[] { };
            }
            MatchCollection matches = Regex.Matches(value, pattern);
            return from Match match in matches select match.Value;
        }

        /// <summary>
        /// 是否电子邮件
        /// </summary>
        public static bool IsEmail(this string value)
        {
            const string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否是IP地址
        /// </summary>
        public static bool IsIpAddress(this string value)
        {
            const string pattern = @"^(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5])$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否是整数
        /// </summary>
        public static bool IsNumeric(this string value)
        {
            const string pattern = @"^\-?[0-9]+$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否是Unicode字符串
        /// </summary>
        public static bool IsUnicode(this string value)
        {
            const string pattern = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否Url字符串
        /// </summary>
        public static bool IsUrl(this string value)
        {
            const string pattern = @"^(http|https|ftp|rtsp|mms):(\/\/|\\\\)[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;]*$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否身份证号，验证如下3种情况：
        /// 1.身份证号码为15位数字；
        /// 2.身份证号码为18位数字；
        /// 3.身份证号码为17位数字+1个字母
        /// </summary>
        public static bool IsIdentityCard(this string value)
        {
            const string pattern = @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$";
            return value.IsMatch(pattern);
        }

        /// <summary>
        /// 是否手机号码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isRestrict">是否按严格格式验证</param>
        public static bool IsMobileNumber(this string value, bool isRestrict = false)
        {
            string pattern = isRestrict ? @"^[1][3-8]\d{9}$" : @"^[1]\d{10}$";
            return value.IsMatch(pattern);
        }

        #endregion

        #region 其他操作

        /// <summary>
        /// 指示指定的字符串是 null 还是 System.String.Empty 字符串
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 为指定格式的字符串填充相应对象来生成字符串
        /// </summary>
        /// <param name="format">字符串格式，占位符以{n}表示</param>
        /// <param name="args">用于填充占位符的参数</param>
        /// <returns>格式化后的字符串</returns>
        [DebuggerStepThrough]
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        /// 将字符串反转
        /// </summary>
        /// <param name="value">要反转的字符串</param>
        public static string ReverseString(this string value)
        {
            return new string(value.Reverse().ToArray());
        }

        /// <summary>
        /// 判断指定路径是否图片文件
        /// </summary>
        public static bool IsImageFile(this string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }
            byte[] filedata = File.ReadAllBytes(filename);
            if (filedata.Length == 0)
            {
                return false;
            }
            ushort code = BitConverter.ToUInt16(filedata, 0);
            switch (code)
            {
                case 0x4D42: //bmp
                case 0xD8FF: //jpg
                case 0x4947: //gif
                case 0x5089: //png
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 以指定字符串作为分隔符将指定字符串分隔成数组
        /// </summary>
        /// <param name="value">要分割的字符串</param>
        /// <param name="strSplit">字符串类型的分隔符</param>
        /// <param name="removeEmptyEntries">是否移除数据中元素为空字符串的项</param>
        /// <returns>分割后的数据</returns>
        public static string[] Split(this string value, string strSplit, bool removeEmptyEntries = false)
        {
            return value.Split(new[] { strSplit }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        /// <summary>
        /// 获取字符串的MD5 Hash值
        /// </summary>
        public static string ToMd5Hash(this string value)
        {
            return HashHelper.GetMd5(value);
        }

        /// <summary>
        /// 支持汉字的字符串长度，汉字长度计为2
        /// </summary>
        /// <param name="value">参数字符串</param>
        /// <returns>当前字符串的长度，汉字长度为2</returns>
        public static int TextLength(this string value)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] bytes = ascii.GetBytes(value);
            foreach (byte b in bytes)
            {
                if (b == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }

        /// <summary>
        /// 将JSON字符串还原为对象
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="json">JSON字符串 </param>
        /// <returns></returns>
        public static T FromJsonString<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 给URL添加查询参数
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <param name="queries">要添加的参数，形如："id=1,cid=2"</param>
        /// <returns></returns>
        public static string AddQueryString(this string url, params string[] queries)
        {
            foreach (string query in queries)
            {
                if (!url.Contains("?"))
                {
                    url += "?";
                }
                else if (!url.EndsWith("&"))
                {
                    url += "&";
                }

                url = url + query;
            }
            return url;
        }

        /// <summary>
        /// 给URL添加 # 参数
        /// </summary>
        /// <param name="url">URL字符串</param>
        /// <param name="query">要添加的参数</param>
        /// <returns></returns>
        public static string AddHashFragment(this string url, string query)
        {
            if (!url.Contains("#"))
            {
                url += "#";
            }

            return url + query;
        }

        /// <summary>
        /// 将字符串转换为<see cref="byte"/>[]数组，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        public static byte[] ToBytes(this string value, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 将<see cref="byte"/>[]数组转换为字符串，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        public static string ToString(this byte[] bytes, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// 将时间戳转化为日期
        /// </summary>
        /// <param name="datestr"></param>
        /// <returns></returns>
        public static DateTime ToDate(this string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 将时间戳转化为日期
        /// </summary>
        /// <param name="datestr"></param>
        /// <returns></returns>
        public static DateTime ToDate(this int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }


        /// <summary>
        /// 字符串转为UniCode码字符串
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StringToUnicode(this string s)
        {
            char[] charbuffers = s.ToCharArray();
            byte[] buffer;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < charbuffers.Length; i++)
            {
                buffer = System.Text.Encoding.Unicode.GetBytes(charbuffers[i].ToString());
                sb.Append(String.Format("//u{0:X2}{1:X2}", buffer[1], buffer[0]));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Unicode字符串转为正常字符串
        /// </summary>
        /// <param name="srcText"></param>
        /// <returns></returns>
        public static string UnicodeToString(this string srcText)
        {
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        }

        /// <summary>
        /// 获取拼音码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PinYin(this string str)
        {
            if (str.IsNullOrEmpty())
            {
                return "";
            }
            string tempStr = "";
            foreach (char c in str)
            {
                if ((int)c >= 33 && (int)c <= 126)
                {//字母和符号原样保留
                    tempStr += c.ToString();
                }
                else
                {//累加拼音声母
                    tempStr += GetPYChar(c.ToString());
                }
            }
            return tempStr;
        }

        /// <summary>
        /// 获取首字母拼音
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static char GetPYChar(string name)
        {
            var c = name.First();
            if (('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z'))
            {
                return c;
            }
            else
            {
                try
                {
                    ChineseChar cc = new ChineseChar(c);
                    if (cc.Pinyins.Count > 0 && cc.Pinyins[0].Length > 0)
                    {
                        return cc.Pinyins[0][0];
                    }
                }
                catch (Exception ex)
                {
                    return c;
                }
                return c;
            }
        }

        #endregion
    }
}

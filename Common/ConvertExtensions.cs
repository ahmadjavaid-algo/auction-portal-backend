using System.Text.RegularExpressions;
using AuctionPortal.Common.Core;

namespace AuctionPortal.Common.Infrastructure
{
    public static class ConvertExtensions
    {
        /// <summary>
        /// GetIntegerValue Converts the sourceValue to int.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static int GetUnsignedIntegerValue(this object sourceValue)
        {
            var value = sourceValue.GetUnsignedIntegerValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return default(int);
        }

        /// <summary>
        /// GetUnsignedShortValue Converts the sourceValue to int.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static int GetUnsignedShortValue(this object sourceValue)
        {
            var value = sourceValue.GetUnsignedShortValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return default(int);
        }

        /// <summary>
        /// GetByteValue Converts the sourceValue to byte.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static byte GetByteValue(this object sourceValue)
        {
            var value = sourceValue.GetByteValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return default(byte);
        }

        /// <summary>
        /// GetIntegerValueNullable Converts the sourceValue to Nullable<int>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static int? GetUnsignedIntegerValueNullable(this object sourceValue)
        {
            int? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is int)
                {
                    value = (int)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    value = Convert.ToInt32(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetUnsignedShortValueNullable Converts the sourceValue to Nullable<int>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static int? GetUnsignedShortValueNullable(this object sourceValue)
        {
            int? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is int)
                {
                    value = (int)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    value = Convert.ToInt16(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetByteValueNullable Converts the sourceValue to Nullable<byte/>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static byte? GetByteValueNullable(this object sourceValue)
        {
            byte? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is byte)
                {
                    value = (byte)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    value = Convert.ToByte(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetIntegerValue Converts the sourceValue to int.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetIntegerValue(this object sourceValue, int defaultValue = default(int))
        {
            var value = sourceValue.GetIntegerValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return defaultValue;
        }

        /// <summary>
        /// GetIntegerValueNullable Converts the sourceValue to Nullable<int>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static int? GetIntegerValueNullable(this object sourceValue)
        {
            int? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is int)
                {
                    value = (int)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    value = Convert.ToInt32(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetLongValue Converts the sourceValue to long.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static long GetLongValue(this object sourceValue)
        {
            var value = sourceValue.GetLongValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return default(long);
        }

        /// <summary>
        /// GetLongValueNullable Converts the sourceValue to Nullable<long>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static long? GetLongValueNullable(this object sourceValue)
        {
            long? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is int)
                {
                    value = (int)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    value = Convert.ToInt64(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetDoubleValue Converts the sourceValue to double.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static double GetDoubleValue(this object sourceValue)
        {
            var value = sourceValue.GetDoubleValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return default(double);
        }

        /// <summary>
        /// GetDoubleValueNullable Converts the sourceValue to Nullable<double>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static double? GetDoubleValueNullable(this object sourceValue)
        {
            double? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is int)
                {
                    value = (int)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    value = Convert.ToDouble(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetDecimalValue Converts the sourceValue to decimal.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static decimal GetDecimalValue(this object sourceValue)
        {
            var value = sourceValue.GetDecimalValueNullable();

            if (value.HasValue)
            {
                //return value.Value;
                return Convert.ToDecimal(string.Format("{0:0.00}", value.Value));
            }

            return default(decimal);
        }

        /// <summary>
        /// GetDecimalValueNullable Converts the sourceValue to Nullable<decimal>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static decimal? GetDecimalValueNullable(this object sourceValue)
        {
            decimal? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is int)
                {
                    value = (int)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    //value = Convert.ToDecimal(sourceValue);
                    value = Convert.ToDecimal(string.Format("{0:0.00}", sourceValue));
                }
            }

            return value;
        }

        /// <summary>
        /// GetStringValue Converts the sourceValue to string.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static string GetStringValue(this object sourceValue)
        {
            var value = string.Empty;

            if (sourceValue != null)
            {
                if (sourceValue is string)
                {
                    value = (string)sourceValue;
                }
                else
                {
                    value = Convert.ToString(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetBooleanValue Converts the sourceValue to bool.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static bool GetBooleanValue(this object sourceValue)
        {
            var value = sourceValue.GetBooleanValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return default(bool);
        }

        /// <summary>
        /// GetBooleanValueNullable Converts the sourceValue to Nullable<bool>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static bool? GetBooleanValueNullable(this object sourceValue)
        {
            bool? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is bool)
                {
                    value = (bool)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    value = Convert.ToBoolean(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetDateTimeValue Converts the sourceValue to DateTime.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeValue(this object sourceValue)
        {
            var value = sourceValue.GetDateTimeValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// GetDateTimeValueNullable Converts the sourceValue to Nullable<DateTime>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeValueNullable(this object sourceValue)
        {
            DateTime? value = null;

            if (sourceValue != null)
            {
                if (sourceValue is DateTime)
                {
                    value = (DateTime)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()))
                {
                    value = Convert.ToDateTime(sourceValue);
                }
            }

            return value;
        }

        /// <summary>
        /// GetTimeSpanValue Converts the sourceValue to TimeSpan.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpanValue(this object sourceValue)
        {
            var value = sourceValue.GetTimeSpanValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return TimeSpan.MinValue;
        }

        /// <summary>
        /// GetTimeSpanValueNullable Converts the sourceValue to Nullable<TimeSpan>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static TimeSpan? GetTimeSpanValueNullable(this object sourceValue)
        {
            TimeSpan? value = null;
            TimeSpan tryValue;

            if (sourceValue != null)
            {
                if (sourceValue is TimeSpan)
                {
                    value = (TimeSpan)sourceValue;
                }
                else if (!string.IsNullOrWhiteSpace(sourceValue.ToString()) && TimeSpan.TryParse(sourceValue.ToString(), out tryValue))
                {
                    value = tryValue;
                }
            }

            return value;
        }

        /// <summary>
        /// GetDateTimeValue Converts the sourceValue to DateTime.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeValue(this object sourceValue, string format)
        {
            var value = sourceValue.GetDateTimeValueNullable(format);

            if (value.HasValue)
            {
                return value.Value;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// GetDateTimeValueNullable Converts the sourceValue to Nullable<DateTime/>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeValueNullable(this object sourceValue, string format)
        {
            DateTime? value = null;

            if (sourceValue != null &&
                !string.IsNullOrWhiteSpace(sourceValue.ToString()))
            {
                value = DateTime.ParseExact(sourceValue.ToString(), format, null);
            }

            return value;
        }

        /// <summary>
        /// GetGuidValue Converts the sourceValue to Guid.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static Guid GetGuidValue(this object sourceValue)
        {
            var value = sourceValue.GetGuidValueNullable();

            if (value.HasValue)
            {
                return value.Value;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// GetGuidValueNullable Converts the sourceValue to Nullable<Guid>.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static Guid? GetGuidValueNullable(this object sourceValue)
        {
            Guid? value = null;

            if (sourceValue != null &&
                !string.IsNullOrWhiteSpace(sourceValue.ToString()))
            {
                value = new Guid(sourceValue.ToString());
            }

            return value;
        }

        /// <summary>
        /// GetIntegerString Converts the sourceValue to string.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static string GetIntegerString(this object sourceValue)
        {
            var value = ((int)sourceValue).ToString();

            return value;
        }

        /// <summary>
        /// GetUnsignedIntegerValue Converts the sourceValue of DayOfWeek type to int.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static int GetUnsignedIntegerValue(this DayOfWeek sourceValue)
        {
            int value = sourceValue != DayOfWeek.Sunday ? Convert.ToInt32(sourceValue) : 7;

            return value;
        }

        /// <summary>
        /// GetUnsignedIntegerValue Converts the sourceValue of DayOfWeek type to int.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static int GetUnsignedIntegerValue(this DateTime sourceValue)
        {
            return sourceValue.DayOfWeek.GetUnsignedIntegerValue();
        }

        /// <summary>
        /// GetLocalTime take System DateTime and returns local time for configured time zone.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static TimeSpan GetLocalTime(this IConfiguration configuration)
        {
            return configuration.GetLocalDateTime().TimeOfDay;

            //Web App does not allow to get TimeZone settings
            //DateTime systemDateTime = DateTime.Now;
            //TimeSpan localTimeSpan;
            //var targetTimeZone = Constant.UTCOffset;

            //if (!string.IsNullOrWhiteSpace(targetTimeZone))
            //{
            //    localTimeSpan = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(systemDateTime, TimeZoneInfo.Local.StandardName, targetTimeZone).TimeOfDay;
            //}
            //else
            //{
            //    localTimeSpan = systemDateTime.TimeOfDay;
            //}
            //return localTimeSpan;
        }

        /// <summary>
        /// GetLocalDateTime take System DateTime and returns local Date Time for configured time zone.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DateTime GetLocalDateTime(this IConfiguration configuration)
        {
            return DateTime.UtcNow.AddMinutes(configuration["General:UTCOffsetInMinutes"].GetDoubleValue());
        }


        /// <summary>
        /// GetStringValue Converts the sourceValues array to string.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>


        /// <summary>
        /// GetBytes converter from stream into Bytes.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// RoundValue rounds the value to provided precision. If precision is positve then, default method is used and if it is negative then custom implementation is used.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static decimal RoundValue(this decimal value, int precision)
        {
            if (precision >= 0)
            {
                return Math.Round(value, precision);
            }
            else
            {
                var factor = Math.Pow(10, precision).GetDecimalValue();


                return Math.Round(value * factor, 0) / factor;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertTimeSpan(this TimeSpan value)
        {
            string displayTime = "";
            if (value != null)
            {
                TimeSpan timespan = new TimeSpan(value.Ticks);
                DateTime time = DateTime.Today.Add(timespan);
                displayTime = time.ToString("hh:mm tt");
            }
            return displayTime;
        }

        /// <summary>
        /// RandomPassword creates and returns random password.
        /// </summary>
        /// <param name="passwordSeed"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomPassword(this string passwordSeed, int length)
        {
            var chars = passwordSeed.ToCharArray();
            var rand = new Random();
            var password = string.Empty;

            for (int i = 0; i < length; i++)
            {
                password += chars[rand.Next() % chars.Length];
            }

            return password;
        }

        public static bool IsValidJWT(this string token)
        {
            // Create a Regex  
            Regex rg = new Regex(@"^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$");

            if (!rg.IsMatch(token))
                return false;

            return true;
        }

    }
}

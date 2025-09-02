using System.Data.Common;

namespace AuctionPortal.Common.Infrastructure
{
    public static class DbDataReaderExtensions
    {
        /// <summary>
        /// GetIntegerValue get value from DataReader for provided ColumnName and converts it to .
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetIntegerValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetIntegerValue();
        }

        /// <summary>
        /// GetIntegerValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<int/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int? GetIntegerValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetIntegerValueNullable();
        }
        // <summary>
        /// GetIntegerValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<int/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int? GetIntegerValueNullableIfColumnExist(this DbDataReader dataReader, string columnName)
        {
            foreach (var column in dataReader.GetColumnSchema())
            {
                if (column.ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var objVal = dataReader.GetValue(columnName);
                    return objVal.GetIntegerValueNullable();
                }
            }
            return null;
        }

        /// <summary>
        /// GetUnsignedIntegerValue get value from DataReader for provided ColumnName and converts it to int.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetUnsignedIntegerValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetUnsignedIntegerValue();
        }




        /// <summary>
        /// GetUnsignedIntegerValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<int/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int? GetUnsignedIntegerValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetUnsignedIntegerValueNullable();
        }



        /// <summary>
        /// GetUnsignedIntegerValue get value from DataReader for provided ColumnName and converts it to int.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int GetUnsignedShortValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetUnsignedIntegerValue();
        }



        /// <summary>
        /// GetUnsignedShortValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<int/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int? GetUnsignedShortValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetUnsignedShortValueNullable();
        }



        /// <summary>
        /// GetByteValue get value from DataReader for provided ColumnName and converts it to Nullable<byte/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static byte GetByteValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetByteValue();
        }

        /// <summary>
        /// GetByteValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<byte/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static byte? GetByteValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetByteValueNullable();
        }

        /// <summary>
        /// GetLongValue get value from DataReader for provided ColumnName and converts it to long.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static long GetLongValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetLongValue();
        }

        /// <summary>
        /// get value from DataReader for provided ColumnName and converts it to .
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static long? GetLongValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetLongValueNullable();
        }

        /// <summary>
        /// GetDoubleValue get value from DataReader for provided ColumnName and converts it to double.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static double GetDoubleValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetDoubleValue();
        }

        /// <summary>
        /// GetDoubleValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<double/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static double? GetDoubleValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetDoubleValueNullable();
        }

        /// <summary>
        /// GetDecimalValue get value from DataReader for provided ColumnName and converts it to decimal.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static decimal GetDecimalValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetDecimalValue();
        }

        /// <summary>
        /// get value from DataReader for provided ColumnName and converts it to .
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static decimal? GetDecimalValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetDecimalValueNullable();
        }

        /// <summary>
        /// GetStringValue get value from DataReader for provided ColumnName and converts it to string.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string GetStringValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetStringValue();
        }

        /// <summary>
        /// GetBooleanValue get value from DataReader for provided ColumnName and converts it to bool.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool GetBooleanValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetBooleanValue();
        }

        /// <summary>
        /// GetBooleanValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<bool/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool? GetBooleanValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetBooleanValueNullable();
        }

        /// <summary>
        /// GetTimeSpanValue get value from DataReader for provided ColumnName and converts it to TimeSpan.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static TimeSpan GetTimeSpanValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetTimeSpanValue();
        }

        /// <summary>
        /// GetTimeSpanValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<TimeSpan/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static TimeSpan? GetTimeSpanValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetTimeSpanValueNullable();
        }

        /// <summary>
        /// GetDateTimeValue get value from DataReader for provided ColumnName and converts it to DateTime.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetDateTimeValue();
        }

        /// <summary>
        /// GetDateTimeValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<DateTime/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetDateTimeValueNullable();
        }

        /// <summary>
        /// GetGuidValue get value from DataReader for provided ColumnName and converts it to Guid.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static Guid GetGuidValue(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetGuidValue();
        }

        /// <summary>
        /// GetGuidValueNullable get value from DataReader for provided ColumnName and converts it to Nullable<Guid/>.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static Guid? GetGuidValueNullable(this DbDataReader dataReader, string columnName)
        {
            var objVal = dataReader.GetValue(columnName);

            return objVal.GetGuidValueNullable();
        }

        /// <summary>
        /// GetValue get value from DataReader for provided ColumnName and returns as object.
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static object GetValue(this DbDataReader dataReader, string columnName)
        {
            object val = null;

            if (dataReader[columnName] != DBNull.Value)
            {
                val = dataReader[columnName];
            }

            return val;
        }
    }
}

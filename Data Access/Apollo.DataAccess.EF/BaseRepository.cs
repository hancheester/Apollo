using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Apollo.DataAccess
{
    public abstract class BaseRepository
    {
        public SqlParameter GetParameterIntegerOutput(string name)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.DbType = DbType.Int32;
            param.Direction = ParameterDirection.Output;

            return param;
        }

        public SqlParameter GetParameteDecimalOutput(string name)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.DbType = DbType.Decimal;
            param.Direction = ParameterDirection.Output;

            return param;
        }

        public SqlParameter GetParameter(string name, int value, bool includeZero = false)
        {
            if (includeZero == false && value == 0) return GetParameterWithNull(name);            
            return GetParameterWithValue(name, value, DbType.Int32);
        }

        public SqlParameter GetParameter(string name, int? value)
        {
            if (value == null || value.Value == 0) return GetParameterWithNull(name);
            return GetParameterWithValue(name, value.Value, DbType.Int32);
        }

        public SqlParameter GetParameter(string name, bool? value)
        {
            if (value == null || !value.HasValue) return GetParameterWithNull(name);
            return GetParameterWithValue(name, value.Value, DbType.Boolean);
        }

        public SqlParameter GetParameter(string name, decimal? value)
        {
            if (value == null || !value.HasValue) return GetParameterWithNull(name);
            return GetParameterWithValue(name, value.Value, DbType.Decimal);
        }

        public SqlParameter GetParameter(string name, decimal value, bool includeZero = false)
        {
            if (includeZero == false && value == 0M) return GetParameterWithNull(name);
            return GetParameterWithValue(name, value, DbType.Decimal);
        }

        public SqlParameter GetParameter(string name, DateTime? value)
        {
            if (value == null || !value.HasValue) return GetParameterWithNull(name);
            return GetParameterWithValue(name, value.Value, DbType.DateTime);
        }

        public SqlParameter GetParameter(string name, string value)
        {
            if (value == null) return GetParameterWithNull(name);
            return GetParameterWithValue(name, value, DbType.String);
        }

        public SqlParameter GetParameter(string name, object value)
        {
            if (value == null) return GetParameterWithNull(name);
            return GetParameterWithValue(name, value, DbType.Object);
        }
        
        static Dictionary<string, SqlParameter> _cachedParams = new Dictionary<string, SqlParameter>();
        static readonly object _object = new object();

        private SqlParameter GetParameterWithNull(string name)
        {
            lock (_object)
            {
                if (!_cachedParams.ContainsKey(name))
                {
                    SqlParameter param = new SqlParameter(name, DBNull.Value);
                    _cachedParams.Add(name, param);
                }

                return (SqlParameter)((ICloneable)_cachedParams[name]).Clone();
            }
        }

        private SqlParameter GetParameterWithValue(string name, object value, DbType type)
        {
            SqlParameter param = GetParameterWithNull(name);
            param.Value = value;
            param.DbType = type;

            return param;
        }
    }
}

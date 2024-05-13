using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace SoccerLeague.Repository.Factory
{
    public class DbFactoryBase
    {
        private readonly string DbConnectionString;
        public DbFactoryBase(string dbConnectionString, int timeout = 100)
        {
            DbConnectionString = dbConnectionString;
            Settings.CommandTimeout = timeout;
        }
        public IDbConnection DbConnection => new NpgsqlConnection(DbConnectionString);


        public virtual async Task<IEnumerable<T>> DbQueryAsync<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbCon = DbConnection;
            if (parameters == null)
                return await dbCon.QueryAsync<T>(sql);

            return await dbCon.QueryAsync<T>(sql, parameters);
        }

        public virtual IEnumerable<T> DbQuery<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbCon = DbConnection;
            if (parameters == null)
                return dbCon.Query<T>(sql);

            return dbCon.Query<T>(sql, parameters);
        }
        public virtual IEnumerable<T> DbQuery<T>(CommandDefinition commandDefinition)
        {
            using IDbConnection dbCon = DbConnection;

            return dbCon.Query<T>(commandDefinition);
        }

        public virtual async Task<T> DbQuerySingleAsync<T>(string sql, object parameters)
        {
            using IDbConnection dbCon = DbConnection;
            return await dbCon.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }

        public virtual T DbQuerySingle<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbCon = DbConnection;
            return dbCon.QueryFirstOrDefault<T>(sql, parameters);
        }

        public virtual async Task<bool> DbExecuteAsync<T>(string sql, object parameters)
        {
            using IDbConnection dbCon = DbConnection;
            return await dbCon.ExecuteAsync(sql, parameters) > 0;
        }

        public virtual bool DbExecute<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbCon = DbConnection;
            return dbCon.Execute(sql, parameters) > 0;
        }

        public virtual async Task<bool> DbExecuteScalarAsync(string sql, object parameters)
        {
            using IDbConnection dbCon = DbConnection;
            return await dbCon.ExecuteScalarAsync<bool>(sql, parameters);
        }

        public virtual bool DbExecuteScalar(string sql, object parameters)
        {
            using IDbConnection dbCon = DbConnection;
            return dbCon.ExecuteScalar<bool>(sql, parameters);
        }

        public virtual async Task<T> DbExecuteScalarDynamicAsync<T>(string sql, object? parameters = null)
        {
            using IDbConnection dbCon = DbConnection;
            if (parameters == null)
                return await dbCon.ExecuteScalarAsync<T>(sql);

            return await dbCon.ExecuteScalarAsync<T>(sql, parameters);
        }

        public virtual async Task<(IEnumerable<T> Data, int RecordCount)> DbQueryMultipleAsync<T>(string sql, object? parameters = null)
        {
            IEnumerable<T> data = null;
            int totalRecords = 0;

            using (IDbConnection dbCon = DbConnection)
            {
                using GridReader results = await dbCon.QueryMultipleAsync(sql, parameters);
                data = await results.ReadAsync<T>();
                totalRecords = await results.ReadSingleAsync<int>();
            }

            return (data, totalRecords);
        }

        public virtual async Task<IEnumerable<T>> DbQueryMultiAsync<T, TP>(string sql, string _splitOn, string property)
        {
            using IDbConnection dbCon = DbConnection;
            dbCon.Open();
            return (await dbCon.QueryAsync<T, TP, T>(sql,
                (t, tp) =>
                {
                    TrySetProperty(t, property, tp);
                    return t;
                },
                splitOn: _splitOn))
                .Distinct()
                .ToList();
        }
        private static void TrySetProperty(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value, null);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;


namespace Server.DAL.Services
{
    public class SqLiteService<T>
    {
        private readonly SqliteConnection _db;
        public SqLiteService(string connectionstr = "Data Source = Online_Cafe.db;")
        {
            if (string.IsNullOrEmpty(connectionstr))
            {
                throw new ArgumentException($"Ошибка строки подключения {connectionstr}");
            }
            else { _db = new SqliteConnection(connectionstr); }
        }
        public IEnumerable<T> GetValues(string table_name)
        {
            _db.Open();
            var sql = $"Select * from {table_name}";
            IEnumerable<T> values = _db.Query<T>(sql);
            _db.Close();
            return values;
        }

        public IEnumerable<T> GetValuesById(string table_name, string column_name, int key)
        {
            _db.Open();
            var sql = $"Select * from {table_name} where {column_name} = {key}";
            IEnumerable<T> values = _db.Query<T>(sql);
            _db.Close();
            return values;
        }

        public T GetById(string table_name, string column_name, int key)
        {
            _db.Open();
            var sql = $"Select * from {table_name} where {column_name} = {key}";
            T value = _db.QuerySingle<T>(sql);
            _db.Close();
            return value;

        }
        public T GetByStr(string table_name, string column_name, string str)
        {
            _db.Open();
            var sql = $"Select * from {table_name} where {column_name} = '{str}'";
            T value = _db.QuerySingle<T>(sql);
            _db.Close();
            return value;

        }
        public void UpdateAndInsert(string sql)
        {
            _db.Open();
            _db.Execute(sql);
            _db.Close();
        }

    }
}

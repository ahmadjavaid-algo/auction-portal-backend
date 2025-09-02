//using AuctionPortal.Infrastructure.Interfaces;
//using Microsoft.Data.SqlClient;
//using System.Data;

//namespace AuctionPortal.Infrastructure
//{
//    public class DbContext : IDbContext
//    {
//        #region Ctor
//        private readonly string _connString;
//        public DbContext(string connectionString) => _connString = connectionString;
//        #endregion

//        #region Factory
//        public IDbConnection CreateConnection()
//            => new SqlConnection(_connString);
//        #endregion
//    }
//}

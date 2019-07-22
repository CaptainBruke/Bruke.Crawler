using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bruke.Crawler.Model;

namespace Bruke.DAL
{
  public class DalContext : DbContext
     {
         private static DalContext _instance;
 
         public static DalContext Instance
         {
             get
             {
                 if (_instance == null)
                 {
                     _instance = new DalContext();
                 }
                 return _instance;
             }
         }
 
         private string _connectionString;
 
         public string ConnectionString
         {
             get
             {
                 if (string.IsNullOrWhiteSpace(_connectionString))
                 {
                     _connectionString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
                 }
                 return _connectionString;
             }
             set
             {
                 _connectionString = value;
             }
         }
 
         public DalContext()
             : base("name=connString")
         {
             _connectionString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;
        }
        public DalContext(string connectionString)
            : base(connectionString)
        {

        }

        /// <summary>
        /// 定义的实体
        /// </summary>
        //public DbSet<Category> Categorys { get; set; }
        public DbSet<MussicDownLoad> MussicDownLoads { get; set; }
  }

}

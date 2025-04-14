using CENZURAZO_CQW1QQ_MESTER.Models;
using Microsoft.EntityFrameworkCore;

namespace CENZURAZO_CQW1QQ_MESTER.Data
{
    public class CensorDbContext : DbContext
    {

        public DbSet<ReplacementData> ReplacementDatas { get; set; }
        public CensorDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=developerdb;Integrated Security=True;MultipleActiveResultSets=true";
            object value = optionsBuilder.UseSqlServer(connString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MoneyTracker.Models;
using System.IO;


namespace MoneyTracker.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

           
                //Local SQLite file in the same folder as the application
                //optionsBuilder.UseSqlite("Data Source=moneytracker.db");
                var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "moneytracker.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
           
         

            
        }
    }
}

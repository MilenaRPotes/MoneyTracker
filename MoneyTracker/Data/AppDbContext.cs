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
            optionsBuilder.UseSqlite("Data Source=moneytracker.db");

            string path = Path.Combine(Directory.GetCurrentDirectory(), "moneytracker.db");
            Console.WriteLine($"Using DB at: {path}"); // O usa Debug.WriteLine para WPF
            optionsBuilder.UseSqlite($"Data Source={path}");
        }
    }
}

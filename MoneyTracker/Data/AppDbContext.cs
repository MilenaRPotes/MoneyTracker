using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyTracerk.Models;


namespace MoneyTracker.Data
{
    class AppDbContext: DbContext
    {
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           //Local SQLite file in the same folder as the application
            optionsBuilder.UseSqlite("Data Source=moneytracker.db");
        }
    }
}

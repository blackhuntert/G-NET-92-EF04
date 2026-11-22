using Microsoft.EntityFrameworkCore;
using NationalBankSystem.enums;
using NationalBankSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NationalBankSystem
{
    internal class NationalBankDbContext :DbContext
    {
        public NationalBankDbContext() : base()
        {
        }

        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Manager> Managers => Set<Manager>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<CustomerAccount> CustomerAccounts => Set<CustomerAccount>();
        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Database=NationalBankDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manager>().HasData(
                new Manager
                {
                    Id = 1,
                    FullName = "Tariq Hassan",
                    Email = "tariq.hassan@nationalbank.eg",
                    PhoneNumber = "01001112233",
                    HireDate = new DateTime(2020, 3, 15)
                },
            new Manager
            {
                Id = 2,
                FullName = "Salma Mansour",
                Email = "salma.mansour@nationalbank.eg",
                PhoneNumber = "01204445566",
                HireDate = new DateTime(2021, 7, 1)
            }
            );

            modelBuilder.Entity<Branch>().HasData(
            new Branch
            {
                Code = "CAI-01",
                Name = "Cairo Main Branch",
                Address = "10 Tahrir Square, Cairo",
                PhoneNumber = "0227940001",
                ManagerId = 1
            },
            new Branch
            {
                Code = "ALX-01",
                Name = "Alexandria Branch",
                Address = "25 Saad Zaghloul St, Alexandria",
                PhoneNumber = "034860002",
                ManagerId = 2
            }
            );

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

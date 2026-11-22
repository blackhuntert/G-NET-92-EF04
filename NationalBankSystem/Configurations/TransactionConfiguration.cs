using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NationalBankSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalBankSystem.Configurations
{
    internal class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.TransactionNumber);
            builder.Property(t => t.TransactionNumber).HasMaxLength(40);
            builder.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            builder.Property(t => t.TransactionType).HasConversion<string>().HasMaxLength(20);
            builder.Property(t => t.Note).HasMaxLength(250);

            builder.HasOne(t => t.Account)
                   .WithMany(a => a.Transactions)
                   .HasForeignKey(t => t.AccountNumber)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

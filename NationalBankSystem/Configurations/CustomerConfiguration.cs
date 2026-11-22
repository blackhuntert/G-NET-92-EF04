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
    internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.FullName).HasMaxLength(120).IsRequired();
            builder.Property(c => c.NationalId).HasMaxLength(30).IsRequired();
            builder.HasIndex(c => c.NationalId).IsUnique();
            builder.Property(c => c.Email).HasMaxLength(150).IsRequired();
            builder.Property(c => c.PhoneNumber).HasMaxLength(30).IsRequired();
            builder.Property(c => c.Address).HasMaxLength(250).IsRequired();
            builder.Property(c => c.CustomerType).HasConversion<string>().HasMaxLength(20);
        }
    }
}

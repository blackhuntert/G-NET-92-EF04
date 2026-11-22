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
    internal class ManagerConfiguration :IEntityTypeConfiguration<Manager>
    {
        public void Configure(EntityTypeBuilder<Manager> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.FullName).HasMaxLength(120).IsRequired();
            builder.Property(m => m.Email).HasMaxLength(150).IsRequired();
            builder.Property(m => m.PhoneNumber).HasMaxLength(30).IsRequired();
        }
    }
}

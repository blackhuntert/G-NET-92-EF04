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
    internal class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(b => b.Code);
            builder.Property(b => b.Code).HasMaxLength(20);
            builder.Property(b => b.Name).HasMaxLength(120).IsRequired();
            builder.Property(b => b.Address).HasMaxLength(250).IsRequired();
            builder.Property(b => b.PhoneNumber).HasMaxLength(30).IsRequired();

            builder.HasOne(b => b.Manager)
                   .WithOne(m => m.Branch)
                   .HasForeignKey<Branch>(b => b.ManagerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

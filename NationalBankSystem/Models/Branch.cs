using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalBankSystem.Models
{
    internal class Branch
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int ManagerId { get; set; }

        public Manager manager { get; set; } = null!;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}

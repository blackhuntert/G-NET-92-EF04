using Microsoft.EntityFrameworkCore;
using NationalBankSystem;
using NationalBankSystem.enums;
using NationalBankSystem.Models;
using System.Globalization;
using System.Net;

namespace NationalBankSystem
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            await using var context = new NationalBankDbContext();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("         National Bank Management        ");
                Console.WriteLine("========================================");
                Console.WriteLine("1) Add a new Customer");
                Console.WriteLine("2) Open a new Account for a Customer");
                Console.WriteLine("3) Update Account Status (Active / Closed)");
                Console.WriteLine("4) Remove an Account from a Customer");
                Console.WriteLine("5) List all Customers (with accounts)");
                Console.WriteLine("0) Exit");
                Console.WriteLine("========================================");
                Console.Write("Enter choice: ");

                string? input = Console.ReadLine()?.Trim();
                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number from the menu.");
                    Pause();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        await AddCustomerAsync(context);
                        break;
                    case 2:
                        await OpenAccountAsync(context);
                        break;
                    case 3:
                        await UpdateAccountStatusAsync(context);
                        break;
                    case 4:
                        await RemoveAccountFromCustomerAsync(context);
                        break;
                    case 5:
                        await ListCustomersAsync(context);
                        break;
                    case 0:
                        await context.DisposeAsync();
                        Console.WriteLine("Database connection closed. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please select a valid option (0-5).");
                        break;
                }

                Pause();
            }
        }

        private static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press any key to return to the menu...");
            Console.ReadKey(intercept: true);
        }

        private static string ReadNonEmpty(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? value = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;

                Console.WriteLine("Value cannot be empty. Please try again.");
            }
        }

        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? value = Console.ReadLine()?.Trim();
                if (DateTime.TryParseExact(
                    value,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime date))
                {
                    return date;
                }

                Console.WriteLine("Invalid date format. Please use yyyy-MM-dd (e.g., 2000-01-25).");
            }
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? value = Console.ReadLine()?.Trim();
                if (int.TryParse(value, out int number))
                    return number;

                Console.WriteLine("Invalid number. Please enter a valid integer.");
            }
        }

        private static int ReadChoiceInRange(string header, (int Option, string Label)[] options)
        {
            while (true)
            {
                Console.WriteLine(header);
                foreach (var (opt, label) in options)
                    Console.WriteLine($"{opt}) {label}");

                Console.Write("Choice: ");
                string? input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out int choice) && options.Any(o => o.Option == choice))
                    return choice;

                Console.WriteLine("Invalid choice. Please select one of the listed numbers.");
            }
        }

        private static async Task AddCustomerAsync(NationalBankDbContext context)
        {
            Console.WriteLine("\n--- Add New Customer ---"); 

            string fullName = ReadNonEmpty("Full Name: "); 
            string nationalId = ReadNonEmpty("National ID: "); 
            DateTime dob = ReadDate("Date of Birth (yyyy-MM-dd): "); 
            string email = ReadNonEmpty("Email: "); 
            string phone = ReadNonEmpty("Phone: "); 
            string address = ReadNonEmpty("Address: "); 

            int typeChoice = ReadChoiceInRange("Customer Type:", new[]
            {
                (1, "Individual"),
                (2, "Business")
            });

            var customer = new Customer
            {
                FullName = fullName,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                CustomerType = (CustomerType)typeChoice
            };

        context.Customers.Add(customer);
            await context.SaveChangesAsync();

        Console.WriteLine($"Customer created successfully. CustomerId = {customer.Id}");
        }

        private static async Task OpenAccountAsync(NationalBankDbContext context)
        {
            Console.WriteLine("\n--- Open New Account ---"); 

            string accountNumber = ReadNonEmpty("Account Number: "); 
            int accountTypeChoice = ReadChoiceInRange("Account Type:", new[]
            {
                (1, "Savings"),
                (2, "Current"),
                (3, "Business")
            });
            string branchCode = ReadNonEmpty("Branch Code: ").ToUpperInvariant(); 
        int customerId = ReadInt("Customer Id: "); 
        int roleChoice = ReadChoiceInRange("Ownership Role:", new[]
        {
                (1, "Primary"),
                (2, "CoHolder")
            });

            Console.WriteLine($"Validating branch '{branchCode}' and customer #{customerId}...");

    bool branchExists = await context.Branches.AnyAsync(b => b.Code == branchCode);
    var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);

            if (!branchExists)
            {
                Console.WriteLine($"Error: Branch '{branchCode}' does not exist.");
                return;
            }

if (customer is null)
{
    Console.WriteLine($"Error: Customer #{customerId} does not exist.");
    return;
}

var account = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
if (account is null)
{
    account = new Account
    {
        AccountNumber = accountNumber,
        AccountType = (AccountType)accountTypeChoice,
        CurrentBalance = 0m,
        OpeningDate = DateTime.UtcNow,
        BranchCode = branchCode
    };
    context.Accounts.Add(account);
}

bool linkExists = await context.CustomerAccounts
    .AnyAsync(ca => ca.CustomerId == customerId && ca.AccountNumber == accountNumber);

if (linkExists)
{
    Console.WriteLine($"Customer {customerId} is already linked to account '{accountNumber}'.");
    return;
}

var role = (OwnershipRole)roleChoice;
context.CustomerAccounts.Add(new CustomerAccount
{
    CustomerId = customerId,
    AccountNumber = accountNumber,
    OwnershipType = role,
    OwnershipStartDate = DateTime.UtcNow,
    AccountStatus = AccountStatus.Active
});

await context.SaveChangesAsync();
Console.WriteLine($"Account '{accountNumber}' created and linked to customer {customerId} as {role} owner."); 
        }

        private static async Task UpdateAccountStatusAsync(NationalBankDbContext context)
{
    Console.WriteLine("\n--- Update Account Status ---"); 

            string accountNumber = ReadNonEmpty("Account Number: "); 
            int customerId = ReadInt("Customer Id: "); 

            var link = await context.CustomerAccounts
                .FirstOrDefaultAsync(ca => ca.AccountNumber == accountNumber && ca.CustomerId == customerId);

            if (link is null)
            {
                Console.WriteLine($"No ownership record found for Account '{accountNumber}' and Customer #{customerId}.");
                return;
             }

            int statusChoice = ReadChoiceInRange("New Status:", new[]
            {
                (1, "Active"),
                (2, "Closed")
            });

            link.AccountStatus = (AccountStatus)statusChoice;
            await context.SaveChangesAsync();

            Console.WriteLine($"Status updated to {link.AccountStatus}."); 
        }

        private static async Task RemoveAccountFromCustomerAsync(NationalBankDbContext context)
        {
             Console.WriteLine("\n--- Remove Account From Customer ---"); 

            string accountNumber = ReadNonEmpty("Account Number: "); 
            int customerId = ReadInt("Customer Id: "); 

            var link = await context.CustomerAccounts
                .FirstOrDefaultAsync(ca => ca.AccountNumber == accountNumber && ca.CustomerId == customerId);

            if (link is null)
            {
                 Console.WriteLine($"No ownership record found for Account '{accountNumber}' and Customer #{customerId}.");
                return;
            }

            context.CustomerAccounts.Remove(link);
            await context.SaveChangesAsync();
            Console.WriteLine("Ownership link deleted."); 

            bool hasRemainingOwners = await context.CustomerAccounts
                .AnyAsync(ca => ca.AccountNumber == accountNumber);

            if (!hasRemainingOwners)
            {
                var account = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
                if (account is not null)
                {
                    context.Accounts.Remove(account);
                    await context.SaveChangesAsync();
                    Console.WriteLine("That was the last owner"); 
                    Console.WriteLine($"account '{accountNumber}' was also removed."); 
                }
            }
        }

        private static async Task ListCustomersAsync(NationalBankDbContext context)
        {
            Console.WriteLine("\n--- All Customers ---");

            var customers = await context.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                .OrderBy(c => c.Id)
                .ToListAsync();

            if (customers.Count == 0)
            {
                Console.WriteLine("No customers found.");
                return;
            }

            foreach (var customer in customers)
            {
                Console.WriteLine($"#{customer.Id} {customer.FullName} ({customer.CustomerType})");

                if (customer.CustomerAccounts.Count == 0)
                {
                    Console.WriteLine("  (no accounts)");
                    continue;
                }

                foreach (var ca in customer.CustomerAccounts)
                {
                    string typeCode = ca.Account.AccountType switch
                    {
                        AccountType.Savings => "SAV",
                        AccountType.Current => "CUR",
                        AccountType.Business => "BUS",
                        _ => "ACC"
                    }
                        ;

                    Console.WriteLine(
                        $"  {ca.AccountNumber}-{typeCode} {ca.Account.AccountType} " +
                        $"Balance: {ca.Account.CurrentBalance:N2} {ca.OwnershipType} {ca.AccountStatus}");
                }
            }
        }
    }
}

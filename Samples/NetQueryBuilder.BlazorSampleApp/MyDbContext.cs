using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.BlazorSampleApp.Models;

namespace NetQueryBuilder.BlazorSampleApp;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Utility> Utility { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Person>()
            .ToTable(nameof(Person))
            .HasMany(person => person.Addresses)
            .WithOne(address => address.Person)
            .HasForeignKey(address => address.PersonId);
    }
}

public static class MyDbContextExtensions
{
    public static async Task SeedDatabase(this MyDbContext context)
    {
        if (!context.Persons.Any())
        {
            var persons = new List<Person>
            {
                new()
                {
                    FirstName = "Alice",
                    LastName = "Jones",
                    PersonId = "1",
                    Created = DateTime.Now
                },
                new()
                {
                    FirstName = "Bob",
                    LastName = "Smith",
                    PersonId = "2",
                    Created = DateTime.Parse("2021-01-01")
                }
            };

            context.Persons.AddRange(persons);
            await context.SaveChangesAsync();

            var addresses = new List<Address>
            {
                new()
                {
                    AddressId = 1,
                    PersonId = "1",
                    IsPrimary = true,
                    City = "Paris",
                    Utilities = new List<Utility>
                    {
                        new()
                        {
                            UtilityId = 1,
                            AccountNumber = "123456",
                            AddressId = 1,
                            Provider = "ConEd",
                            Type = "Electricity"
                        }
                    }
                },
                new()
                {
                    AddressId = 2,
                    PersonId = "2",
                    IsPrimary = false,
                    City = "New York",
                    Utilities = new List<Utility>
                    {
                        new()
                        {
                            UtilityId = 2,
                            AccountNumber = "654321",
                            AddressId = 2,
                            Provider = "ConEd",
                            Type = "Electricity"
                        }
                    }
                }
            };

            context.Addresses.AddRange(addresses);
            await context.SaveChangesAsync();
        }
    }
}
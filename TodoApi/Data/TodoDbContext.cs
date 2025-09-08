using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the Todo entity
            modelBuilder.Entity<Todo>()
                .Property(t => t.Title)
                .IsRequired();

            // Add some sample data
            modelBuilder.Entity<Todo>().HasData(
                new Todo
                {
                    Id = 1,
                    Title = "Learn .NET Core",
                    Description = "Complete the tutorial and build a sample application",
                    Priority = Priority.High,
                    CreatedDate = DateTime.Now
                },
                new Todo
                {
                    Id = 2,
                    Title = "Build a REST API",
                    Description = "Create a RESTful API using .NET Core",
                    Priority = Priority.Medium,
                    CreatedDate = DateTime.Now
                }
            );
        }
    }
} 
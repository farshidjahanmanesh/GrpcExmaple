using Microsoft.EntityFrameworkCore;

namespace GrpcExample.Server.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public DateTime CreateAt { get; set; }
    }
    public class BookDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {
        }
    }
}

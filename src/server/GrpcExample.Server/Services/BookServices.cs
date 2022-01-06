using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcExample.BookServices;
using GrpcExample.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcExample.Server.Services
{
    public class BookServices : BookService.BookServiceBase
    {
        private readonly BookDbContext _context;

        public BookServices(BookDbContext context)
        {
            this._context = context;
        }
        public override async Task<RemoveBookResponse> RemoveBook(RemoveBookRequest request, ServerCallContext context)
        {
            if (await _context.Books.AsNoTracking().AnyAsync(c => c.Id == request.Id) == false)
                throw new BadHttpRequestException("id is invalid");
            _context.Books.Remove(new Book()
            {
                Id = request.Id,
            });
            await _context.SaveChangesAsync();
            return new RemoveBookResponse();
        }
        public override async Task GetAllBooks(GetAllBooksRequest request, IServerStreamWriter<BookResponse> responseStream, ServerCallContext context)
        {
            var books = await _context.Books.AsNoTracking().ToListAsync();
            foreach (var book in books)
                await responseStream.WriteAsync(new BookResponse()
                {
                    AuthorName = book.AuthorName,
                    CreateAt = Timestamp.FromDateTime(book.CreateAt),
                    Id = book.Id,
                    Title = book.Title,
                });

            await Task.CompletedTask;
        }
        public override async Task<AddBookResponse> AddBook(AddBookRequest request, ServerCallContext context)
        {
            var result = await _context.Books.AddAsync(new Book()
            {
                AuthorName = request.AuthorName,
                CreateAt = request.CreateAt.ToDateTime(),
                Title = request.Title
            });
            await _context.SaveChangesAsync();
            return new AddBookResponse()
            {
                BookId = result.Entity.Id
            };
        }
    }
}

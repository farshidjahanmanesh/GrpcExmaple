// See https://aka.ms/new-console-template for more information

using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcExample.BookServices;

var httpHandler = new HttpClientHandler();
// Return `true` to allow certificates that are untrusted/invalid
httpHandler.ServerCertificateCustomValidationCallback =
    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

var channel = GrpcChannel.ForAddress("https://localhost:7288",
    new GrpcChannelOptions { HttpHandler = httpHandler });
var client = new BookService.BookServiceClient(channel);
AddBooks();
await GetAllBooks();
RemoveBook(1);
await GetAllBooks();

Console.ReadKey();


void AddBooks()
{
    var tasks = new List<Task<AddBookResponse>>();
    Console.WriteLine("------------------------------------ AddBook service Call ------------------------------------");
    foreach (var i in Enumerable.Range(0, 3))
        tasks.Add(client.AddBookAsync(new AddBookRequest()
        {
            AuthorName = "farshid",
            Title = "blablabla",
            CreateAt = Timestamp.FromDateTime(DateTime.UtcNow.AddYears(-2))

        }).ResponseAsync);
    Task.WaitAll(tasks.ToArray());
    foreach (var item in tasks)
        Console.WriteLine($"id is : {item.Result.BookId}");
    Console.WriteLine("------------------------------------ AddBook service finish ------------------------------------");
}

async Task GetAllBooks()
{
    Console.WriteLine("------------------------------------ GetAllBooks service Call ------------------------------------");
    await foreach (var book in client.GetAllBooks(new GetAllBooksRequest()).ResponseStream.ReadAllAsync())
        Console.WriteLine($"book id :{book.Id} & title : {book.Title} & authorName : {book.AuthorName}");
    Console.WriteLine("------------------------------------ GetAllBooks finish ------------------------------------");
}

void RemoveBook(int id)
{
    Console.WriteLine("------------------------------------ RemoveBook service Call ------------------------------------");
    client.RemoveBook(new RemoveBookRequest()
    {
        Id = id
    });
    Console.WriteLine("------------------------------------ RemoveBook finish ------------------------------------");
}
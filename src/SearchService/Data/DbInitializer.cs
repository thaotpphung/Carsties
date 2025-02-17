using System.Text.Json;
using AuctionService.Services;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
  public static async Task InitDb(WebApplication app)
  {
    await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

    await DB.Index<Item>()
        .Key(a => a.Make, KeyType.Text)
        .Key(a => a.Model, KeyType.Text)
        .Key(a => a.Color, KeyType.Text)
        .CreateAsync();

    var count = await DB.CountAsync<Item>();
    Console.WriteLine($"Current item count: {count}");

    using var scope = app.Services.CreateScope();

    var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();
    var items = await httpClient.GetItemsForSearchDb();
    Console.WriteLine($"Got {items.Count} items from AuctionService");
  }
}

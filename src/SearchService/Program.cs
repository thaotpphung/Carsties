using AuctionService.Services;
using Polly;
using Polly.Extensions.Http;
using SearchService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseAuthorization();

app.UseRouting();  // ✅ Enables routing

app.MapControllers();  // ✅ Map controller endpoints

app.Lifetime.ApplicationStarted.Register(async () =>
{
  try
  {
    await DbInitializer.InitDb(app);
  }
  catch (Exception e)
  {
    Console.WriteLine(e);
  }
});

app.Run();

static IAsyncPolicy<HttpResponseMessage> GetPolicy()
=> HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
    .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));

using AuctionService.Services;
using MassTransit;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(x =>
{
  x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
  x.UsingRabbitMq((context, cfg) =>
  {
    cfg.ConfigureEndpoints(context);
  });
});

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

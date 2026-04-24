using GoldenBread.Api.Configuration;
using GoldenBread.Application;
using GoldenBread.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseApi();
app.Run();

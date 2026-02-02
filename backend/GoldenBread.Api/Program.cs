using GoldenBread.Api.Extensions;
using GoldenBread.Application;
using GoldenBread.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiServices();

var app = builder.Build();

app.UseApi();
app.Run();

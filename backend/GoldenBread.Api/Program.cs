using GoldenBread.Api.Extensions;
using GoldenBread.Application;
using GoldenBread.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi();

var app = builder.Build();

app.UseApiPipeline();
app.Run();

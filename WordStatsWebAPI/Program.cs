using System.Text;

using RandomWords;
using WordStats.Interfaces;
using WordStats.Implements;
using WordStats.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Word Stats Backend API", Description = "Word Stats Backend API", Version = "v1" });
});

builder.Services.AddSingleton<Stream, RandomWordStream>();
builder.Services.AddSingleton<Encoding, UTF8Encoding>();
builder.Services.AddSingleton<IWordStats, WordStatsDictionaryImplement>();
builder.Services.AddSingleton<IWordStatsWriter, WordStatsWriterConsoleImplement>();
builder.Services.Configure<WordStatsServiceOptions>(builder.Configuration.GetSection("WordStatsService"));
builder.Services.AddHostedService<WordStatsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

/*
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Word Stats Backend API v1"));
*/

app.UseCors("AllowAll");

app.MapGet("/", () => "Hello WordStats App!");
app.MapGet("/stats", () => app.Services.GetService<IWordStats>()?.ToJsonString());

app.Run();

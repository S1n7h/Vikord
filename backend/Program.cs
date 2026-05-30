using backend;
using backend.Data;
using backend.Dtos;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connString = "DataSource=VikingChat.db";
builder.Services.AddSqlite<ChatContext>(connString); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapChatEndpoints();

app.MigrateDb();

app.UseHttpsRedirection();

app.Run();
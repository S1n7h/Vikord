using backend;
using backend.Data;
using backend.Dtos;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connString = "DataSource=VikingChat.db";
builder.Services.AddSqlite<ChatContext>(connString); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()   // Allows your Vite frontend origin
              .AllowAnyMethod()   // Allows GET, POST, DELETE, etc.
              .AllowAnyHeader();  // Allows "Content-Type" headers
    });
});


var app = builder.Build();

app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapChatEndpoints();

app.MigrateDb();

app.UseHttpsRedirection();

app.Run();
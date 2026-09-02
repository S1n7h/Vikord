using backend;
using backend.Data;
using backend.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connString = "DataSource=VikingChat.db";
builder.Services.AddSqlite<ChatContext>(connString); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:6969")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Configuration.AddEnvironmentVariables();

builder.Configuration.AddUserSecrets<Program>();

//AddAuthorisation is a IService, so it becomes method in builder.Serivices
builder.Services.AddAuthorisation(builder.Configuration);

builder.Services.AddSignalR();

var app = builder.Build();

app.MigrateDb();

app.UseCors("AllowReactApp");

//app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapChatEndpoints();

app.MapAuthEndpoints();

//declare routes using which clients can connect to the hub
app.MapHub<MessageNotificationHub>("/messageHub");

app.Run();
using System.Security.Claims;
using backend.Data;
using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend;

public static class ChatEndpoints
{
    public static void MapChatEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/chatlog");

        //GET /chatlog - Fetch all messages from DB
        group.MapGet("/", async (ChatContext db) =>
        {
            // 1. Start with the base query (unexecuted)
            IQueryable<Message> query = db.Messages;

            // 3. Transform the data into MessageDto and execute the query safely
            var result = await query
            .Select(message => new MessageDto(
                message.Id,
                message.Content,
                message.User.GlobalName,
                message.User.UserId)
            )
            .ToListAsync();

            // 4. Return the results to the client
            return Results.Ok(result);                        
        });

        // PUT /chatlog/{id} - Edit an existing message
        group.MapPut("/{id}", async (int id, UpdateMessageDto updateDto, ChatContext db) =>
        {
            // 1. Find the message by its id using db.Messages.FindAsync(id)
            var messages = await db.Messages.FindAsync(id);

            // 2. If it's null, return a NotFound result            
            if (messages is null) return Results.NotFound();

            // 3. Update the message's content with the incoming new message text
            messages.Content = updateDto.NewMessage;

            // 4. Save the changes to the database using SaveChangesAsync()
            await db.SaveChangesAsync();

            // 5. Return a Results.NoContent() or Results.Ok() to show success
            return Results.NoContent();
        });

        // POST /chatlog - Save a new message to DB
        group.MapPost("/", async (CreateMessageDto newMessage, ChatContext db, ClaimsPrincipal user, ILogger<Program> logger) =>
        {
            //if user is trying to post a message but isn't authenticated, this redirects them to the authentication server
            if (user.Identity?.IsAuthenticated != true)
            {
                logger.LogInformation("Get rekt buddy, u ain't autehnticated.");
                return Results.Unauthorized();
            }
            var globalName = user.FindFirst("urn:discord:global_name")?.Value;
            var discordUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Note: For now, we're assuming a default User exists or mapping to a dummy UserId (e.g., 1)
            // until you add authentication/user registration logic.
            if (discordUserId == null)
            {
                return Results.Unauthorized();
            }
            var messageEntity = new Message
            {
                Content = newMessage.Message,
                /* UserId = newMessage.UserId */ // Hardcoded for now until user context is available
                UserId = discordUserId
            };

            db.Messages.Add(messageEntity);
            await db.SaveChangesAsync();

            var responseDto = new MessageDto(
                messageEntity.Id, 
                messageEntity.Content, 
                globalName,
                discordUserId /* messageEntity.UserId */
            );

            return Results.Created($"/chatlog/{responseDto.Id}", responseDto);
        });

        // DELETE /chatlog/{id} - Remove a message
        //the user is authenticated on the frontend, so you only require the message id, not the userId
        group.MapDelete("/{id}", async (int id, ChatContext db, ClaimsPrincipal user,  ILogger<Program> logger) =>
        {            
            var message = await db.Messages.FindAsync(id);
            
            if (message is null) return Results.NotFound();

            var messageUserId = message.UserId;
            var currentAuthenticatedUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentAuthenticatedUserId is null) 
                return Results.NotFound("Trying to pull a fast one, aren't ya?");

            if (currentAuthenticatedUserId != messageUserId)
            {
                logger.LogInformation("Stap the cap, you ain't HIM.");
                return Results.Forbid();
            }

            if (currentAuthenticatedUserId == messageUserId)
            {
                db.Messages.Remove(message);
                await db.SaveChangesAsync();
            }

            return Results.NoContent();
        });
    }
}
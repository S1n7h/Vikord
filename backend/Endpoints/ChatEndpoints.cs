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

        // GET /chatlog - Fetch all messages from DB
        group.MapGet("/", async (int? userId, ChatContext db) =>
        {
            // 1. Start with the base query (unexecuted)
            IQueryable<Message> query = db.Messages;

            // 2. If a userId was provided, layer on the filter
            if (userId is not null)
            {
                query = query.Where(m => m.UserId == userId);
            }

            // 3. Transform the data into MessageDto and execute the query safely
            var result = await query
                .Select(m => new MessageDto(m.Id, m.Content))
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
        group.MapPost("/", async (CreateMessageDto newMessage, ChatContext db) =>
        {
            // Note: For now, we're assuming a default User exists or mapping to a dummy UserId (e.g., 1)
            // until you add authentication/user registration logic.
            var messageEntity = new Message
            {
                Content = newMessage.Message,
                UserId = newMessage.UserId // Hardcoded for now until user context is available
            };

            db.Messages.Add(messageEntity);
            await db.SaveChangesAsync();

            var responseDto = new MessageDto(messageEntity.Id, messageEntity.Content);

            return Results.Created($"/chatlog/{responseDto.Id}", responseDto);
        });

        // DELETE /chatlog/{id} - Remove a message
        group.MapDelete("/{id}/{userId}", async (int id, int userId, ChatContext db) =>
        {
            var message = await db.Messages.FindAsync(id);
            if (message is null) return Results.NotFound();

            // Check ownership: Is this message owned by someone other than User 1?
            if (message.UserId != userId) 
            {
                return Results.Forbid(); // Returns a 403 Forbidden status code
            }

            db.Messages.Remove(message);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
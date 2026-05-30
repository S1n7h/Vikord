namespace backend.Dtos;

public record CreateMessageDto(
    string Message,
    int UserId
);
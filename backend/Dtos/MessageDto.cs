namespace backend.Dtos;

//records are just convenient classes which store just data
//so, 
// public record MessageDto(
//     int Id,
//     string Message,
//     string GlobalName
// );
//you can easily do
// new MessageDto(1, "Hello", "Soumya")

public record MessageDto(
    int Id, 
    string Message, 
    /* int UserId  string UserId, */ 
    string GlobalName,
    string UserId
);
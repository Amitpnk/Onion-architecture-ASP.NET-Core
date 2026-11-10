namespace OA.Test.Integration;

// Lightweight DTOs used to read API responses without coupling tests to server-side types.

public record ApiResponse<T>(bool Succeeded, string? Message, List<string>? Errors, T? Data);

public record AuthenticationData(string Id, string UserName, string Email, List<string> Roles, bool IsVerified, string JWToken);

public record ErrorResponse(int StatusCode, string ErrorMessage);

public record CustomerDto(
    int Id,
    string CustomerName,
    string ContactName,
    string ContactTitle,
    string Address,
    string City,
    string Region,
    string PostalCode,
    string Country,
    string Phone,
    string Fax);

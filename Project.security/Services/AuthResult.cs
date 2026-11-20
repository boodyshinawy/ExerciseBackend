namespace Project.Security;

public class AuthResult
{
    public bool Succeeded { get; init; }

    public static AuthResult Success => new() { Succeeded = true };

    public static AuthResult Failed => new() { Succeeded = false };
}

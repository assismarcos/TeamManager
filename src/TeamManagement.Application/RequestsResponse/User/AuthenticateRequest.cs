namespace TeamManagement.Application.RequestsResponse.User;

public class AuthenticateRequest
{
    public string UserName { get; set; }

    public string Password { get; set; }
}
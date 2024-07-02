namespace TeamManagement.Application.RequestsResponse.User;

public class AuthenticateResponse
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Token { get; set; }
}
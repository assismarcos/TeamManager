namespace TeamManagement.Application.RequestsResponse.User;

public class AddUserRequest
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
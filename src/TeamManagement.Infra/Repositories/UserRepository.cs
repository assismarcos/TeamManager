using Microsoft.Data.SqlClient;
using TeamManagement.Core.Abstractions;
using TeamManagement.Core.Entities;
using TeamManagement.Infra.DataContext;

namespace TeamManagement.Infra.Repositories;

public class UserRepository(IDataContext context) : IUserRepository
{
    public async Task<User?> GetByUserNameAsync(string userName)
    {
        var query = "select Id, UserName, Password from [User] where UserName = @UserName";

        var command = new SqlCommand(query, context.Connection);
        command.Parameters.AddWithValue("@UserName", userName);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            Id = reader.GetValue<int>(nameof(User.Id)),
            UserName = reader.GetValue<string>(nameof(User.UserName))!,
            Password = reader.GetValue<string>(nameof(User.Password))!
        };
    }

    public async Task<int> AddAsync(User user)
    {
        var query = """
                    insert into [User] (
                      UserName, Password
                    )
                    output inserted.Id
                    values (
                      @UserName, @Password
                    )
                    """;
        var command = new SqlCommand(query, context.Connection);
        command.Parameters.AddWithValue("@UserName", user.UserName);
        command.Parameters.AddWithValue("@Password", user.Password);

        var result = (int) (await command.ExecuteScalarAsync())!;

        return result;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var query = """
                    update [User] set
                     UserName = @UserName,
                     Password = @Password
                    where Id = @UserId
                    """;

        var command = new SqlCommand(query, context.Connection);
        command.Parameters.AddWithValue("@UserName", user.UserName);
        command.Parameters.AddWithValue("@Password", user.UserName);
        command.Parameters.AddWithValue("@UserId", user.Id);

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }
}
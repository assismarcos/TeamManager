using Microsoft.Data.SqlClient;
using TeamManagement.Core.Abstractions;
using TeamManagement.Core.Entities;
using TeamManagement.Infra.DataContext;

namespace TeamManagement.Infra.Repositories;

public class MemberRepository(IDataContext context) : IMemberRepository
{
    public async Task<IEnumerable<Member>> GetAllAsync(string? tags)
    {
        var query = "select Id, SalaryPerYear, Name, Type, ContractDuration, Role, Tags, CountryName, CurrencyCode, CurrencySymbol, CurrencyName from Member ";
        var command = new SqlCommand(query, context.Connection);
        if (string.IsNullOrEmpty(tags))
        {
            return await ReadMemberAsync(command);
        }

        var tagsToSearch = $"'{tags.Replace(",", "','")}'";
        query += $"""
                  where exists (select * from string_split(Tags, ',')
                                where trim(Value) in ({tagsToSearch}));
                  """;

        command = new SqlCommand(query, context.Connection);
        return await ReadMemberAsync(command);
    }

    private async Task<IEnumerable<Member>> ReadMemberAsync(SqlCommand command)
    {
        var result = new List<Member>();

        await using var reader = await command.ExecuteReaderAsync();
        while (reader.Read())
        {
            result.Add(new Member
            {
                Id = reader.GetValue<int>("Id"),
                Name = reader.GetValue<string>("Name")!,
                SalaryPerYear = reader.GetValue<decimal>("SalaryPerYear"),
                Type = (MemberType)reader.GetValue<short>("Type"),
                ContractDuration = reader.GetValue<short>("ContractDuration"),
                Role = reader.GetValue<string>("Role"),
                Tags = reader.GetValue<string>("Tags")!,
                CountryName = reader.GetValue<string>("CountryName")!,
                CurrencyCode = reader.GetValue<string>("CurrencyCode")!,
                CurrencySymbol = reader.GetValue<string>("CurrencySymbol")!,
                CurrencyName = reader.GetValue<string>("CurrencyName")!
            });
        }

        return result;
    }

    public async Task<Member?> GetAsync(int id)
    {
        var query = """
                    select Id, SalaryPerYear, Name, Type, ContractDuration, Role, Tags, CountryName, CurrencyCode, CurrencySymbol, CurrencyName
                    from Member
                    where Id = @MemberId
                    """;
        var command = new SqlCommand(query, context.Connection);
        command.Parameters.AddWithValue("@MemberId", id);
        return (await ReadMemberAsync(command)).FirstOrDefault();
    }

    public async Task<int> AddAsync(Member member)
    {
        var query = """
                    insert into [Member] (
                      Name, SalaryPerYear, Type, ContractDuration, Role, Tags, CountryName, CurrencyCode, CurrencySymbol, CurrencyName
                    )
                    output inserted.Id
                    values (
                      @Name, @SalaryPerYear, @Type, @ContractDuration, @Role, @Tags, @CountryName, @CurrencyCode, @CurrencySymbol, @CurrencyName
                    )
                    """;
        var command = new SqlCommand(query, context.Connection);
        command.Parameters.AddWithValue("@Name", member.Name);
        command.Parameters.AddWithValue("@SalaryPerYear", member.SalaryPerYear);
        command.Parameters.AddWithValue("@Type", member.Type.AsDbValue());
        command.Parameters.AddWithValue("@ContractDuration", member.ContractDuration!.AsDbValue());
        command.Parameters.AddWithValue("@Role", member.Role!.AsDbValue());
        command.Parameters.AddWithValue("@Tags", member.Tags.AsDbValue());
        command.Parameters.AddWithValue("@CountryName", member.CountryName.AsDbValue());
        command.Parameters.AddWithValue("@CurrencyCode", member.CurrencyCode.AsDbValue());
        command.Parameters.AddWithValue("@CurrencySymbol", member.CurrencySymbol.AsDbValue());
        command.Parameters.AddWithValue("@CurrencyName", member.CurrencyName.AsDbValue());

        var result = (int)(await command.ExecuteScalarAsync())!;

        return result;
    }

    public async Task<bool> UpdateAsync(Member member)
    {
        var query = """
                    update [Member] set
                     Name = @Name,
                     SalaryPerYear = @SalaryPerYear,
                     Type = @Type,
                     ContractDuration = @ContractDuration, 
                     Role = @Role, 
                     Tags = @Tags, 
                     CountryName = @CountryName,
                     CurrencyCode = @CurrencyCode,
                     CurrencySymbol = @CurrencySymbol, 
                     CurrencyName = @CurrencyName
                     where Id = @Id
                    """;

        var command = new SqlCommand(query, context.Connection);
        command.Parameters.AddWithValue("@Name", member.Name);
        command.Parameters.AddWithValue("@SalaryPerYear", member.SalaryPerYear);
        command.Parameters.AddWithValue("@Type", member.Type.AsDbValue());
        command.Parameters.AddWithValue("@ContractDuration", member.ContractDuration!.AsDbValue());
        command.Parameters.AddWithValue("@Role", member.Role!.AsDbValue());
        command.Parameters.AddWithValue("@Tags", member.Tags.AsDbValue());
        command.Parameters.AddWithValue("@CountryName", member.CountryName.AsDbValue());
        command.Parameters.AddWithValue("@CurrencyCode", member.CurrencyCode.AsDbValue());
        command.Parameters.AddWithValue("@CurrencySymbol", member.CurrencySymbol.AsDbValue());
        command.Parameters.AddWithValue("@CurrencyName", member.CurrencyName.AsDbValue());
        command.Parameters.AddWithValue("@Id", member.Id);

        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = "delete from [Member] where Id = @MemberId";
        var command = new SqlCommand(query, context.Connection);
        command.Parameters.AddWithValue("@MemberId", id);
        var affectedRows = await command.ExecuteNonQueryAsync();

        return affectedRows > 0;
    }
}
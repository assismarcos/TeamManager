using Microsoft.Data.SqlClient;

namespace TeamManagement.Infra.DataContext;

public class TeamManagerContext : IDataContext
{
    public SqlConnection Connection { get; }
    
    public TeamManagerContext(SqlConnection connection)
    {
        Connection = connection;
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Close();
        Connection.Dispose();
    }
}
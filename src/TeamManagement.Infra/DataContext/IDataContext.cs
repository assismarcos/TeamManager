using System.Data;
using Microsoft.Data.SqlClient;

namespace TeamManagement.Infra.DataContext;

public interface IDataContext
{
    SqlConnection Connection { get; }
    //IDbConnection Connection { get; }
    //IDbTransaction Transaction { get; }
}
using Microsoft.Data.SqlClient;

namespace TeamManagement.Infra;

public static class SqlDataExtensions
{
    public static T? GetValue<T>(this SqlDataReader reader, string columnName)
    {
        var index = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(index))
            return default(T);

        var value = reader.GetValue(index);

        return (T)Convert.ChangeType(value, typeof(T));
    }

    public static object AsDbValue(this object source)
    {
        return source ?? DBNull.Value;
    }
}
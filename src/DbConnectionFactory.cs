using System.Data;
using Npgsql;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _config;
    public DbConnectionFactory(IConfiguration config)
    {
        _config = config;
    }
    public IDbConnection CreateConnection()
    {
        var connStr = _config.GetConnectionString("Postgres");
        return new NpgsqlConnection(connStr);
    }
}

using Whizbang.Core.Data.FluentSqlClient;

namespace Whizbang.EventStore.Sqlite
{
    public static class Sqlite
    {
        public static IExecutor With(string connectionString)
        {
            return new SqliteExecutor(connectionString);
        }
    }
}
using System.Data.SQLite;
using Whizbang.Core.Data.FluentSqlClient;

namespace Whizbang.EventStore.Sqlite
{
    public class SqliteExecutor : Executor
    {
        public SqliteExecutor(string connectionString)
            : base(
                () => new SQLiteConnection(connectionString),
                () => new SQLiteCommand(),
                (cmd, paramName, value) => ((SQLiteCommand) cmd).Parameters.AddWithValue(paramName, value))
        {
        }
    }
}
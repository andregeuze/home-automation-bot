using LiteDB.Async;
using Microsoft.Extensions.Options;

namespace Bot.Services
{
    public class LiteDbOptions
    {
        public string ConnectionString { get; set; }
    }

    public class DatabaseService
    {
        public LiteDatabaseAsync Database { get; }

        public DatabaseService(IOptions<LiteDbOptions> options)
        {
            Database = new LiteDatabaseAsync(options.Value.ConnectionString);
        }
    }
}

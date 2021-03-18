using Bot.Services;
using LiteDB;
using LiteDB.Async;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Data
{
    public abstract class LiteDbEntity
    {
        public ulong Id { get; set; }
    }

    public class EntityContext<TEntity> where TEntity : LiteDbEntity
    {
        private readonly LiteDatabaseAsync liteDatabase;

        ILiteCollectionAsync<TEntity> MyCollection => liteDatabase.GetCollection<TEntity>(typeof(TEntity).Name);

        public EntityContext(DatabaseService databaseService)
        {
            liteDatabase = databaseService.Database;
        }

        public async Task<List<TEntity>> FindAllAsync() => (await MyCollection.FindAllAsync()).ToList();

        public Task<bool> Delete(TEntity entity) => MyCollection.DeleteAsync(entity.Id);

        public Task<bool> Upsert(TEntity entity) => MyCollection.UpsertAsync(entity);
    }
}

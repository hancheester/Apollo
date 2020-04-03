using Apollo.Core.Model.Entity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Apollo.DataAccess
{
    public interface IDbContext
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;
        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new();
        IEnumerable<TEntity> SqlQuery<TEntity>(string sql, params object[] parameters);
        DbEntityEntry Entry(object entity);
        int SaveChanges();
        int ExecuteSqlCommand(string sql, params object[] parameters);        
        void Detach(object entity);        
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Apollo.DataAccess.Interfaces
{
    public interface IRepository<T>
    {
        T Return(object id);
        int Create(T entity);
        void Create(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void Delete(object id);
        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }
    }
}

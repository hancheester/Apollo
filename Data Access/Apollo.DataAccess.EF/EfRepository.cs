using Apollo.Core.Model.Entity;
using Apollo.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Apollo.DataAccess.EF
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity, new()
    {
        private readonly IDbContext _context;        
        private IDbSet<T> _entities;

        protected virtual IDbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities;
            }
        }

        public EfRepository(IDbContext context)
        {
            this._context = context;
        }
        
        public T Return(object id)
        {
            return this.Entities.Find(id);
        }

        public int Create(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            this.Entities.Add(entity);
            this._context.SaveChanges();
            return entity.Id;
        }

        public void Create(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");

            foreach (var entity in entities)
                this.Entities.Add(entity);

            this._context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            var foundEntity = this.Entities.Find(entity.Id);
            if (foundEntity != null)
            {
                _context.Detach(foundEntity);            
            }

            this.Entities.Attach(entity);
            this._context.Entry(entity).State = EntityState.Modified;
            this._context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            this.Entities.Remove(entity);
            this._context.SaveChanges();            
        }

        public void Delete(object id)
        {
            var foundEntity = this.Entities.Find((int)id);
            if (foundEntity != null)
            {
                _context.Detach(foundEntity);
            }

            this._context.Entry(new T() {Id = (int)id}).State = EntityState.Deleted;
            this._context.SaveChanges();
        }

        public virtual IQueryable<T> Table
        {
            get { return this.Entities; }
        }

        public virtual IQueryable<T> TableNoTracking
        {
            get { return this.Entities.AsNoTracking(); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Data.Interface
{
    public interface IRepository<TEntity>
    {
        void Delete(TEntity entityToDelete);
        void Delete(object id);
        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
    }
}

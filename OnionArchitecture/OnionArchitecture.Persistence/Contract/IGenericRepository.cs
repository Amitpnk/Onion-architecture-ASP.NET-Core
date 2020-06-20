using System.Collections.Generic;

namespace OnionArchitecture.Persistence.Contract
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T obj);
        void Update(T obj);
        void Delete(object id);
        bool SaveChanges();
    }
}

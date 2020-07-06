using Microsoft.EntityFrameworkCore;
using OA.Data;
using OA.Persistence.Contract;
using System.Collections.Generic;
using System.Linq;

namespace OA.Persistence.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> entities;
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            entities = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return entities.ToList();
        }
        public T GetById(int id)
        {
            return entities.Find(id);
        }
        public void Add(T obj)
        {
            entities.Add(obj);
        }
        public void Update(T obj)
        {
            entities.Update(obj);
        }
        public void Delete(object id)
        {
            T existing = entities.Find(id);
            entities.Remove(existing);
        }
        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

    }
}

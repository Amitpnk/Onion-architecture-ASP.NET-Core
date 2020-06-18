using Microsoft.EntityFrameworkCore;
using OnionArchitecture.Data;
using OnionArchitecture.Persistence.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Persistence.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> entities;
        private readonly CustomerContext _context;
      
        public GenericRepository(CustomerContext context)
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
            return _context.SaveChanges()>0;
        }

    }
}

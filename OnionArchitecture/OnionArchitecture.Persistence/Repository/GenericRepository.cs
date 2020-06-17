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
        private DbSet<T> table = null;

        private readonly CustomerContext _context;
        public GenericRepository()
        {
            _context = new CustomerContext();
            table = _context.Set<T>();
        }
        public GenericRepository(CustomerContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }
        public T GetById(int id)
        {
            return table.Find(id);
        }
        public void Add(T obj)
        {
            table.Add(obj);
        }
        public void Update(T obj)
        {
            table.Update(obj);
        }
        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}

﻿using Gym.DataAccess.Data;
using Gym.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Gym.DataAccess.Repository
{
        public class Repository<T> : IRepository<T> where T : class
        {
            private readonly ApplicationDbContext _context;
            private DbSet<T> dbSet;
            public Repository(ApplicationDbContext context)
            {
                _context = context;
               // _context.Products.Include(u => u.Category);
                this.dbSet = _context.Set<T>();
            }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        //includeProp -"category"
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null,string? includeProperties = null)
        {
                IQueryable<T> query = dbSet;
                if(filter != null)
            {
                query = query.Where(filter);
            }
                
            if (includeProperties != null)
            {
                foreach(var includeProp in includeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
                return query.ToList();
            }
            public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
            {
                IQueryable<T> query = dbSet;
                query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
            }
            public void Remove(T entity)
            {
                dbSet.Remove(entity);
            }
            public void RemoveRange(IEnumerable<T> entity)
            {
                dbSet.RemoveRange(entity);
            }
        }
}

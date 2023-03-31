using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PetMerchandise.core.db.entity;

namespace PetMerchandise.core.db.repository;

public class EFGenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private DbContext _dbContext;

    public DbContext DbContext
    {
        get { return _dbContext; }
        private set {}
    }

    public EFGenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 新增一筆資料到資料庫
    /// </summary>
    /// <param name="entity"></param>
    public void Create(TEntity entity)
    {
        _dbContext.Set<TEntity>().Add(entity);
    }

    public TEntity? Read(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbContext.Set<TEntity>().Where(predicate).FirstOrDefault();
    }

    public IQueryable<TEntity> Reads()
    {
        return _dbContext.Set<TEntity>().AsQueryable();
    }

    public void Update(TEntity entity)
    {
        _dbContext.Entry<TEntity>(entity).State = EntityState.Modified;
    }

    public void Update(TEntity entity, Expression<Func<TEntity, object>>[]? updateProperties)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        if (null == updateProperties)
        {
            return;
        }

        foreach (var property in updateProperties)
        {
            _dbContext.Entry(entity).Property(property).IsModified = true;
        }
    }

    public void Delete(TEntity entity)
    {
        _dbContext.Entry(entity).State = EntityState.Deleted;
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
}
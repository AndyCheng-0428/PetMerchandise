using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetMerchandise.core.db.repository;

namespace PetMerchandise.core.db.unit_work;

public class EFUnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;
    private bool _disposed;
    private Hashtable _repositories;
    
    public EFUnitOfWork(DbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 清除此類別的資源
    /// </summary>
    /// <param name="disposing">是否正在清理中</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        _disposed = true;
    }
    
    /// <summary>
    /// DBContext的SaveChanges()方法實際上是對數據的所有變更提交到內存，並且將此類"變更"保存在內存中的一個事務中
    /// 調用該方法後，事務會被提交到資料庫，亦即進行Commit操作，該方法可以看作將變更提交到內存事務的觸發器，而非直接將變更提交到資料庫
    /// </summary>
    public void Commit()
    {
        _dbContext.SaveChanges();
    }

    /// <summary>
    /// 不會阻塞當前執行緒，立即返回一個表達異步操作Task對象，讓其執行緒可以執行其他操作，操作完成後返回表達操作結果的值，如保存的紀錄數
    /// </summary>
    /// <returns></returns>
    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public async Task RollbackAsync()
    {
        await _dbContext.DisposeAsync();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        if (null == _repositories)
        {
            _repositories = new();
        }

        var type = typeof(T).Name;
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(EFGenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);
            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<T>)_repositories[type];
    }
}
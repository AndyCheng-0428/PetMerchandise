using System;
using System.Threading.Tasks;
using PetMerchandise.core.db.repository;

namespace PetMerchandise.core.db.unit_work;

/// <summary>
/// 實作Unit of work的介面
/// Unit of work : 因為Repository相對應於一個表，若需要一步驟改動多表，若其中有一錯誤則整體Rollback 僅憑藉EFGenericRepository無法達成
///                故需要使用此種模式當事件做完時方才SaveChanges();
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 儲存所有異動
    /// </summary>
    void Commit();

    Task CommitAsync();

    Task RollbackAsync();

    IRepository<T> Repository<T>() where T : class;
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SqlSugar;

/// <summary>
/// 非泛型 SqlSugar reposiroty
/// </summary>
public partial interface ISqlSugarRepository
{
    /// <summary>
    /// change reposiroty
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    ISqlSugarRepository<TEntity> Change<TEntity>()
        where TEntity : class, new();

    /// <summary>
    /// Context
    /// </summary>
    SqlSugarClient Context { get; }

    /// <summary>
    /// DynamicContext
    /// </summary>
    dynamic DynamicContext { get; }

    /// <summary>
    /// 原生 Ado 
    /// </summary>
    IAdo Ado { get; }
}

/// <summary>
/// SqlSugar reposiroty interface
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public partial interface ISqlSugarRepository<TEntity>
    where TEntity : class, new()
{
    /// <summary>
    /// entity collections
    /// </summary>
    ISugarQueryable<TEntity> Entities { get; }

    /// <summary>
    /// context
    /// </summary>
    SqlSugarClient Context { get; }

    /// <summary>
    /// dynamiccontext
    /// </summary>
    dynamic DynamicContext { get; }

    /// <summary>
    /// 原生 Ado 
    /// </summary>
    IAdo Ado { get; }

    /// <summary>
    /// get 總數
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    int Count(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// get 總數
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    bool Any(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// get a enittiy by PK
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    TEntity Single(dynamic Id, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// get a enittiy
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    TEntity Single(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// get a enittiy
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// get a enittiy
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    TEntity FirstOrDefault(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// get a enittiy
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// get many enitties
    /// </summary>
    /// <returns></returns>
    List<TEntity> ToList();

    /// <summary>
    /// get many enitties
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// get many enitties
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="orderByExpression"></param>
    /// <param name="orderByType"></param>
    /// <returns></returns>
    List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);

    /// <summary>
    /// get many enitties
    /// </summary>
    /// <returns></returns>
    Task<List<TEntity>> ToListAsync();

    /// <summary>
    /// get many enitties
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// get many enitties
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="orderByExpression"></param>
    /// <param name="orderByType"></param>
    /// <returns></returns>
    Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc);

    /// <summary>
    /// insert single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    int Insert(TEntity entity, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// insert many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    int Insert(Expression<Func<TEntity, object>> ignoreColumns = null, params TEntity[] entities);

    /// <summary>
    /// insert many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    int Insert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// insert single return id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    int InsertReturnIdentity(TEntity entity, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// insert single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> InsertAsync(TEntity entity, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// insert many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> InsertAsync(Expression<Func<TEntity, object>> ignoreColumns = null, params TEntity[] entities);

    /// <summary>
    /// insert many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> InsertAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// insert single retrun id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<long> InsertReturnIdentityAsync(TEntity entity, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// update single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    int Update(TEntity entity, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// update many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    int Update(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null, params TEntity[] entities);

    /// <summary>
    /// update many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    int Update(IEnumerable<TEntity> entities, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// update single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// update many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null, params TEntity[] entities);

    /// <summary>
    /// update many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    Task<int> UpdateAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null);

    /// <summary>
    /// delete single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    int Delete(TEntity entity, Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// delete many
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    int Delete(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// delete single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    Task<int> DeleteAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate);


    /// <summary>
    /// delete many
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// query by condiction
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// query by condiction
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISugarQueryable<TEntity> Where(bool condition, Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// AsQueryable
    /// </summary>
    /// <returns></returns>
    ISugarQueryable<TEntity> AsQueryable();

    /// <summary>
    /// AsQueryable
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISugarQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// return table result
    /// </summary>
    /// <returns></returns>
    List<TEntity> AsEnumerable();

    /// <summary>
    /// return table result
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<TEntity> AsEnumerable(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// return table result
    /// </summary>
    /// <returns></returns>
    Task<List<TEntity>> AsAsyncEnumerable();

    /// <summary>
    /// return table result
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<List<TEntity>> AsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// change
    /// </summary>
    /// <typeparam name="TChangeEntity"></typeparam>
    /// <returns></returns>
    ISqlSugarRepository<TChangeEntity> Change<TChangeEntity>() where TChangeEntity : class, new();
}


/// <summary>
/// 非泛型 SqlSugar implement
/// </summary>
public partial class SqlSugarRepository : ISqlSugarRepository
{
    private readonly IServiceProvider _serviceProvider;

    public SqlSugarRepository(IServiceProvider serviceProvider
        , ISqlSugarClient db)
    {
        _serviceProvider = serviceProvider;
        DynamicContext = Context = (SqlSugarClient)db;
        Ado = db.Ado;
    }

    public virtual SqlSugarClient Context { get; }


    public virtual dynamic DynamicContext { get; }

    /// <summary>
    /// 原生 Ado 
    /// </summary>
    public virtual IAdo Ado { get; }

    /// <summary>
    /// 切換
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    public virtual ISqlSugarRepository<TEntity> Change<TEntity>()
        where TEntity : class, new()
    {
        return _serviceProvider.GetService<ISqlSugarRepository<TEntity>>();
    }
}

/// <summary>
/// SqlSugar implement
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public partial class SqlSugarRepository<TEntity> : ISqlSugarRepository<TEntity>
where TEntity : class, new()
{
    /// <summary>
    /// 非泛型 SqlSugar repository
    /// </summary>
    private readonly ISqlSugarRepository _sqlSugarRepository;

    /// <summary>
    /// construct
    /// </summary>
    /// <param name="sqlSugarRepository"></param>
    public SqlSugarRepository(ISqlSugarRepository sqlSugarRepository)
    {
        _sqlSugarRepository = sqlSugarRepository;

        DynamicContext = Context = sqlSugarRepository.Context;

        // 設定租戶
        var entityType = typeof(TEntity);
        if (entityType.IsDefined(typeof(TenantAttribute), false))
        {
            var tenantAttribute = entityType.GetCustomAttribute<TenantAttribute>(false)!;
            Context.ChangeDatabase(tenantAttribute.configId);
        }

        Ado = sqlSugarRepository.Ado;
    }

    /// <summary>
    /// entity collections
    /// </summary>
    public virtual ISugarQueryable<TEntity> Entities => _sqlSugarRepository.Context.Queryable<TEntity>();

    /// <summary>
    /// DB Context 
    /// </summary>
    public virtual SqlSugarClient Context { get; }

    /// <summary>
    /// DB DynamicContext 
    /// </summary>
    public virtual dynamic DynamicContext { get; }

    /// <summary>
    /// 原生 Ado 
    /// </summary>
    public virtual IAdo Ado { get; }

    /// <summary>
    /// get 總數
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public int Count(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Count(whereExpression);
    }

    /// <summary>
    /// get 總數
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.CountAsync(whereExpression);
    }

    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public bool Any(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Any(whereExpression);
    }

    /// <summary>
    /// 是否存在
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await Entities.AnyAsync(whereExpression);
    }

    /// <summary>
    /// get a entity by PK
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public TEntity Single(dynamic Id, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
        {
            return Entities.InSingle(Id);
        }
        else
        {
            return Entities.IgnoreColumns(ignoreColumns).InSingle(Id);
        }
    }

    /// <summary>
    /// get a entity
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public TEntity Single(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
        {
            return Entities.Single(whereExpression);
        }
        else
        {
            return Entities.IgnoreColumns(ignoreColumns).Single(whereExpression);
        }
    }

    /// <summary>
    /// get a entity
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
        {
            return Entities.SingleAsync(whereExpression);
        }
        else
        {
            return Entities.IgnoreColumns(ignoreColumns).SingleAsync(whereExpression);
        }
    }

    /// <summary>
    /// get a entity
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
        {
            return Entities.First(whereExpression);
        }
        else
        {
            return Entities.IgnoreColumns(ignoreColumns).First(whereExpression);
        }

    }

    /// <summary>
    /// get a entity
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
        {
            return await Entities.FirstAsync(whereExpression);
        }
        else
        {
            return await Entities.IgnoreColumns(ignoreColumns).FirstAsync(whereExpression);
        }

    }

    /// <summary>
    /// get many entities
    /// </summary>
    /// <returns></returns>
    public List<TEntity> ToList()
    {
        return Entities.ToList();
    }

    /// <summary>
    /// get many entities
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Where(whereExpression).ToList();
    }

    /// <summary>
    /// get many entities
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="orderByExpression"></param>
    /// <param name="orderByType"></param>
    /// <returns></returns>
    public List<TEntity> ToList(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
    {
        return Entities.OrderByIF(orderByExpression != null, orderByExpression, orderByType)
            .Where(whereExpression).ToList();
    }

    /// <summary>
    /// get many entities
    /// </summary>
    /// <returns></returns>
    public Task<List<TEntity>> ToListAsync()
    {
        return Entities.ToListAsync();
    }

    /// <summary>
    /// get many entities
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Entities.Where(whereExpression).ToListAsync();
    }

    /// <summary>
    /// get many entities
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <param name="orderByExpression"></param>
    /// <param name="orderByType"></param>
    /// <returns></returns>
    public Task<List<TEntity>> ToListAsync(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
    {
        return Entities.OrderByIF(orderByExpression != null, orderByExpression, orderByType).Where(whereExpression).ToListAsync();
    }

    /// <summary>
    /// insert single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual int Insert(TEntity entity, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Insertable(entity)
                .ExecuteCommand();
        else
            return _sqlSugarRepository.Context.Insertable(entity)
                .IgnoreColumns(ignoreColumns).ExecuteCommand();
    }

    /// <summary>
    /// insert many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual int Insert(Expression<Func<TEntity, object>> ignoreColumns = null, params TEntity[] entities)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Insertable(entities).ExecuteCommand();
        else
            return _sqlSugarRepository.Context.Insertable(entities)
                .IgnoreColumns(ignoreColumns).ExecuteCommand();
    }

    /// <summary>
    /// insert many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual int Insert(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Insertable(entities.ToArray()).ExecuteCommand();
        else
            return _sqlSugarRepository.Context.Insertable(entities.ToArray()).IgnoreColumns(ignoreColumns).ExecuteCommand();
    }

    /// <summary>
    /// insert single retrun id
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public int InsertReturnIdentity(TEntity insertObj, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Insertable(insertObj).ExecuteReturnIdentity();
        else
            return _sqlSugarRepository.Context.Insertable(insertObj).IgnoreColumns(ignoreColumns).ExecuteReturnIdentity();
    }

    /// <summary>
    /// insert single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual Task<int> InsertAsync(TEntity entity, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Insertable(entity).ExecuteCommandAsync();
        else
            return _sqlSugarRepository.Context.Insertable(entity).IgnoreColumns(ignoreColumns).ExecuteCommandAsync();
    }

    /// <summary>
    /// insert many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual Task<int> InsertAsync(Expression<Func<TEntity, object>> ignoreColumns = null, params TEntity[] entities)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Insertable(entities).ExecuteCommandAsync();
        else
            return _sqlSugarRepository.Context.Insertable(entities).IgnoreColumns(ignoreColumns).ExecuteCommandAsync();
    }

    /// <summary>
    /// insert many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual Task<int> InsertAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Insertable(entities.ToArray()).ExecuteCommandAsync();
        else
            return _sqlSugarRepository.Context.Insertable(entities.ToArray()).IgnoreColumns(ignoreColumns).ExecuteCommandAsync();
    }

    /// <summary>
    /// insert single return id
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<long> InsertReturnIdentityAsync(TEntity entity, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return await _sqlSugarRepository.Context.Insertable(entity).ExecuteReturnBigIdentityAsync();
        else
            return await _sqlSugarRepository.Context.Insertable(entity).IgnoreColumns(ignoreColumns).ExecuteReturnBigIdentityAsync();
    }

    /// <summary>
    /// update single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual int Update(TEntity entity, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Updateable(entity)
                .Where(predicate)
                .ExecuteCommand();
        else
            return _sqlSugarRepository.Context.Updateable(entity)
                .IgnoreColumns(ignoreColumns)
               .Where(predicate)
               .ExecuteCommand();
    }

    /// <summary>
    /// update many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual int Update(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null, params TEntity[] entities)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Updateable(entities)
             .Where(predicate)
             .ExecuteCommand();
        else
            return _sqlSugarRepository.Context.Updateable(entities)
                 .IgnoreColumns(ignoreColumns)
          .Where(predicate)
          .ExecuteCommand();
    }

    /// <summary>
    /// update many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual int Update(IEnumerable<TEntity> entities, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Updateable(entities.ToArray())
              .Where(predicate)
            .ExecuteCommand();
        else
            return _sqlSugarRepository.Context.Updateable(entities.ToArray())
                  .IgnoreColumns(ignoreColumns)
             .Where(predicate)
           .ExecuteCommand();
    }

    /// <summary>
    /// update single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Updateable(entity)
             .Where(predicate)
            .ExecuteCommandAsync();
        else
            return _sqlSugarRepository.Context.Updateable(entity)
                 .IgnoreColumns(ignoreColumns)
             .Where(predicate)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// update many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual Task<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> ignoreColumns = null, params TEntity[] entities)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Updateable(entities).Where(predicate).ExecuteCommandAsync();
        else
            return _sqlSugarRepository.Context.Updateable(entities)
                  .IgnoreColumns(ignoreColumns).Where(predicate).ExecuteCommandAsync();
    }

    /// <summary>
    /// update many
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public virtual Task<int> UpdateAsync(IEnumerable<TEntity> entities, Expression<Func<TEntity, bool>> predicate
        , Expression<Func<TEntity, object>> ignoreColumns = null)
    {
        if (ignoreColumns == null)
            return _sqlSugarRepository.Context.Updateable(entities.ToArray())
            .Where(predicate).ExecuteCommandAsync();
        else
            return _sqlSugarRepository.Context.Updateable(entities.ToArray())
                 .IgnoreColumns(ignoreColumns)
          .Where(predicate).ExecuteCommandAsync();
    }

    /// <summary>
    /// delete single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual int Delete(TEntity entity, Expression<Func<TEntity, bool>> predicate)
    {
        return _sqlSugarRepository.Context.Deleteable(entity)
              .Where(predicate).ExecuteCommand();
    }


    /// <summary>
    /// delete by condiction
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public int Delete(Expression<Func<TEntity, bool>> whereExpression)
    {
        return _sqlSugarRepository.Context.Deleteable<TEntity>().Where(whereExpression).ExecuteCommand();
    }

    /// <summary>
    /// delete single
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual Task<int> DeleteAsync(TEntity entity, Expression<Func<TEntity, bool>> predicate)
    {
        return _sqlSugarRepository.Context.Deleteable(entity).Where(predicate).ExecuteCommandAsync();
    }

    /// <summary>
    /// delete by condiction
    /// </summary>
    /// <param name="whereExpression"></param>
    /// <returns></returns>
    public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await _sqlSugarRepository.Context.Deleteable<TEntity>().Where(whereExpression).ExecuteCommandAsync();
    }

    /// <summary>
    /// query by condiction
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual ISugarQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable(predicate);
    }

    /// <summary>
    /// query by condiction
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual ISugarQueryable<TEntity> Where(bool condition, Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable().WhereIF(condition, predicate);
    }

    /// <summary>
    /// AsQueryable
    /// </summary>
    /// <returns></returns>
    public virtual ISugarQueryable<TEntity> AsQueryable()
    {
        return Entities;
    }

    /// <summary>
    /// AsQueryable
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual ISugarQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> predicate)
    {
        return Entities.Where(predicate);
    }

    /// <summary>
    /// return db result
    /// </summary>
    /// <returns></returns>
    public virtual List<TEntity> AsEnumerable()
    {
        return AsQueryable().ToList();
    }

    /// <summary>
    /// return db result
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual List<TEntity> AsEnumerable(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable(predicate).ToList();
    }

    /// <summary>
    /// return db result
    /// </summary>
    /// <returns></returns>
    public virtual Task<List<TEntity>> AsAsyncEnumerable()
    {
        return AsQueryable().ToListAsync();
    }

    /// <summary>
    /// return db result
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual Task<List<TEntity>> AsAsyncEnumerable(Expression<Func<TEntity, bool>> predicate)
    {
        return AsQueryable(predicate).ToListAsync();
    }

    /// <summary>
    /// chage repository
    /// </summary>
    /// <typeparam name="TChangeEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    public virtual ISqlSugarRepository<TChangeEntity> Change<TChangeEntity>()
        where TChangeEntity : class, new()
    {
        return _sqlSugarRepository.Change<TChangeEntity>();
    }
}
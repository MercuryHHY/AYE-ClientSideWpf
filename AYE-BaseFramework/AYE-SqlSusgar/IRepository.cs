using Newtonsoft.Json.Converters;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AYE.BaseFramework.SqlSusgarCore;



public interface IRepository<T> : ISimpleClient<T> where T : class, new()
{
    public Task<bool> UseTranAsync(Func<Task> fuc);
    public ISugarQueryable<T> _DbQueryable { get; set; }
    public ISqlSugarClient _Db { get; set; }
    public Task<List<S>> UseSqlAsync<S>(string sql);
    public Task<List<S>> StoreAsync<S>(string storeName, object para);
    //public Task<PageModel<List<T>>> CommonPageAsync(QueryPageCondition pars);
    //public Task<List<T>> GetListAsync(QueryCondition pars);
    //public Task<bool> UpdateIgnoreNullAsync(T entity);
    //ISugarQueryable<T> QueryConditionHandler(QueryCondition pars);
    Task<bool> UpdateSuperSaveAsync(T data, Expression<Func<T, object>> columns);
    Task<bool> InsertSuperSaveAsync(T data, Expression<Func<T, object>> columns);
    Task<bool> UpdateListColumnsAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> expression);
    Task<bool> UpdateListColumnsAsync(List<T> entitys, Expression<Func<T, object>> columns);
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> order, OrderByType orderByType = OrderByType.Desc);
    Task<T> GetFirstAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> order, OrderByType orderByType = OrderByType.Desc);
}


/// <summary>
/// 仓储模式
/// </summary>
/// <typeparam name="T"></typeparam>
public class Repository<T> : SimpleClient<T>, IRepository<T> where T : class, new()
{
    private readonly ISqlSugarClient _sqlSugarClient;
    public Repository(ISqlSugarClient context) : base(context)//注意这里要有默认值等于null
    {
        _sqlSugarClient = context;
    }

    //public Repository(ISqlSugarClient context) 
    //{
    //    _sqlSugarClient = context;
    //}


    public ISqlSugarClient _Db { get { return base.Context; } set { } }

    public ISugarQueryable<T> _DbQueryable { get { return base.Context.Queryable<T>(); } set { } }

    /// <summary>
    /// 使用异步事务,自动回滚
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task<bool> UseTranAsync(Func<Task> fuc)
    {
        var res = await _Db.AsTenant().UseTranAsync(fuc);
        return res.IsSuccess;
    }
    /// <summary>
    /// 使用ado
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <param name="sql"></param>
    /// <returns></returns>
    public async Task<List<S>> UseSqlAsync<S>(string sql)
    {
        return await _Db.Ado.SqlQueryAsync<S>(sql);
    }

    ///// <summary>
    ///// 添加返回实体
    ///// </summary>
    ///// <param name="entity"></param>
    ///// <returns></returns>
    //public async Task<T> InsertReturnEntityAsync(T entity)
    //{
    //    //entity.Id = Guid.NewGuid();
    //    return await _Db.Insertable(entity).ExecuteReturnEntityAsync();
    //}

    /// <summary>
    /// 更新忽略空值
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public async Task<bool> UpdateIgnoreNullAsync(T entity)
    {
        return await _Db.Updateable(entity).IgnoreColumns(true).ExecuteCommandAsync() > 0;
    }


    /// <summary>
    /// 列表更新指定列，全表更新+条件
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public async Task<bool> UpdateListColumnsAsync(Expression<Func<T, T>> columns, Expression<Func<T, bool>> expression)
    {
        return await _Db.Updateable<T>().SetColumns(columns).Where(expression).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 列表更新指定列，指定对象列表
    /// </summary>
    /// <param name="entitys"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public async Task<bool> UpdateListColumnsAsync(List<T> entitys, Expression<Func<T, object>> columns)
    {
        return await _Db.Updateable(entitys).UpdateColumns(columns).ExecuteCommandAsync() > 0;
    }

    ///// <summary>
    ///// 逻辑多删除
    ///// </summary>
    ///// <returns></returns>
    //public async Task<bool> DeleteByLogicAsync(List<Guid> ids)
    //{
    //    var entitys = await _Db.Queryable<T>().Where(u => ids.Contains(u.Id)).ToListAsync();
    //    entitys.ForEach(u => u.IsDeleted = true);
    //    return await _Db.Updateable(entitys).ExecuteCommandAsync() > 0;
    //}


    /// <summary>
    /// 调用存储过程
    /// </summary>
    /// <typeparam name="S"></typeparam>
    /// <param name="storeName"></param>
    /// <param name="para"></param>
    /// <returns></returns>
    public async Task<List<S>> StoreAsync<S>(string storeName, object para)
    {
        return await _Db.Ado.UseStoredProcedure().SqlQueryAsync<S>(storeName, para);
    }



    /// <summary>
    /// 更新高级保存，验证重复
    /// </summary>
    /// <param name="list"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public async Task<bool> UpdateSuperSaveAsync(T data, Expression<Func<T, object>> columns)
    {
        var x = _Db.Storageable(data)
                       .SplitError(it => it.Any())
                       .SplitUpdate(it => true)
                       .WhereColumns(columns)//这里用name作为数据库查找条件
                       .ToStorage();
        return await x.AsUpdateable.ExecuteCommandAsync() > 0;//插入可更新部分
    }

    /// <summary>
    /// 添加高级保存，验证重复
    /// </summary>
    /// <param name="list"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public async Task<bool> InsertSuperSaveAsync(T data, Expression<Func<T, object>> columns)
    {
        var x = _Db.Storageable(data)
                       .SplitError(it => it.Any())
                       .SplitInsert(it => true)
                       .WhereColumns(columns)//这里用name作为数据库查找条件
                       .ToStorage();
        return await x.AsInsertable.ExecuteCommandAsync() > 0;//插入可插入部分
    }

    /// <summary>
    /// 方法重载，多条件获取第一个值
    /// </summary>
    /// <returns></returns>
    public async Task<T> GetFirstAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> order, OrderByType orderByType = OrderByType.Desc)
    {
        return await _Db.Queryable<T>().Where(where).OrderBy(order, orderByType).FirstAsync();
    }

    /// <summary>
    /// 方法重载，多条件获取范围
    /// </summary>
    /// <returns></returns>
    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where, Expression<Func<T, object>> order, OrderByType orderByType = OrderByType.Desc)
    {
        return await _Db.Queryable<T>().Where(where).OrderBy(order, orderByType).ToListAsync();
    }
}

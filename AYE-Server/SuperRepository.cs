using AYE.BaseFramework.SqlSusgarCore;
using AYE_Interface;
using Prism.Ioc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Service;


/// <summary>
/// 我的初衷是在依赖注入时 给它参数以完成构造，
/// 可是  我发现不管怎么写，始终都会注册失败
/// 除非 封装工厂，或者直接用！！！！！！
/// </summary>
/// <typeparam name="T"></typeparam>
public class SuperRepository<T> : SimpleClient<T>, ISuperRepository<T> where T : class, new()
{
    // 在SimpleClient 内部 IOC找到并注入了，所以这里不用再重复 注入实例
    //private readonly ISqlSugarClient _sqlSugarClient;
    public SuperRepository(IContainerProvider containerProvider, string dbType) 
        : base(containerProvider.Resolve<ISqlSugarClient>(dbType))//注意这里要有默认值  不能等于null
    {
        //_sqlSugarClient = context;
        
        _Db = containerProvider.Resolve<ISqlSugarClient>(dbType);
    }



    public ISqlSugarClient _Db { get; set; }

    public ISugarQueryable<T> _DbQueryable { get { return _Db.Queryable<T>(); } set { } }   

    
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
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AYE_Interface
{
   
    public interface ISuperRepository<T> : ISimpleClient<T> where T : class, new()
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
}

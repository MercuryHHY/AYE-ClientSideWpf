using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AYE_BaseShare;


//
// 摘要:
//     定义一个实体。 它的主键可能不是“Id”，也可能有一个复合主键。
public interface IEntity
{
    //
    // 摘要:
    //     返回此实体的有序键的数组。
    object[] GetIds();
}



public interface IRepository<TEntity> : ISimpleClient<TEntity> where TEntity : class, IEntity, new()
{

}







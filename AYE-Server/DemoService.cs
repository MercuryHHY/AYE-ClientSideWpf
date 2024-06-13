using AYE_BaseShare;
using AYE_Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AYE_BaseShare._1_CodeFirst;

namespace AYE_Server
{
    public class DemoService : DemoInterface1
    {
        private readonly IRepository<UserInfo001> _repository;

        public DemoService(IRepository<UserInfo001> repository)
        {
            _repository = repository;
        }

        public Task<UserInfo001> Test()
        {
           return _repository.GetFirstAsync(it => it.UserName == "admin");
        }
    }
}

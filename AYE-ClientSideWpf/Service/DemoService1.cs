using AYE_BaseShare;
using AYE_Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AYE_BaseShare._1_CodeFirst;

namespace AYE_ClientSideWpf.Service
{
    public class DemoService1 : IDemoInterface12
    {
        private readonly IRepository<UserInfo001> _repository;

        public DemoService1(IRepository<UserInfo001> repository)
        {
            _repository = repository;
        }

        public async Task<UserInfo001> Test()
        {
            var v1 = await _repository.GetFirstAsync(it => it.UserName == "admin");
            return v1;
        }
    }
}

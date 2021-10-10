using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Mock
{
    /// <summary>
    /// Null返回降级
    /// </summary>
    public class NullReturnMock : IMock
    {
        private readonly IConfiguration Configuration;

        public NullReturnMock(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void DoMock(string serviceName)
        {
            string flag = Configuration[serviceName];
            if (flag.Equals("true"))
            {
            }

        }
    }
}

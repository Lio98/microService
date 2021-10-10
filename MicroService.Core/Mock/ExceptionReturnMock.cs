using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Mock
{
    /// <summary>
    /// 直接异常返回降级
    /// </summary>
    public class ExceptionReturnMock : IMock
    {
        private readonly IConfiguration Configuration;

        public ExceptionReturnMock(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void DoMock(string serviceName)
        {
            string flag = Configuration[serviceName];
            if (string.Equals(flag, "true"))
            {
                throw new Exception($"{serviceName}进行降级了");
            }

        }
    }
}

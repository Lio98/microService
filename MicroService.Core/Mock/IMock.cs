using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.Mock
{
    /// <summary>
    /// 降级(主要指服务降级)
    /// </summary>
    public interface IMock
    {
        /// <summary>
        /// 降级方法
        /// </summary>
        /// <param name="serviceName"></param>
        void DoMock(string serviceName);
    }
}

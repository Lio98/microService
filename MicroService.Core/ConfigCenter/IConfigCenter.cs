using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroService.Core.ConfigCenter
{
    public interface IConfigCenter
    {
        void LoadConfigCenter(WebHostBuilderContext context, IConfigurationBuilder config);
    }
}

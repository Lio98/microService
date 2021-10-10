using MicroService.AggregationService.Models;
using MicroService.AggregationService.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Services
{
    public interface IContentServiceClient
    {
        /// <summary>
        /// 根据用户ID查询内容
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IList<Content>> GetContents(int userId, ContentSelectType selectType, int selectTypeId);

        Content InsertContents(Content content);
    }
}

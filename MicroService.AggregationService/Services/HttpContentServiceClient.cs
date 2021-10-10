using MicroService.AggregationService.Models;
using MicroService.AggregationService.Models.Enum;
using MicroService.Core.HttpConsulClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Services
{
    public class HttpContentServiceClient : IContentServiceClient
    {
        private readonly string ServiceSchme = "https";
        private readonly string ServiceName = "ContentService"; //服务名称

        // httpclient consul请求
        private readonly ConsulHttpClient consulHttpClient;

        public HttpContentServiceClient(ConsulHttpClient consulHttpClient)
        {
            this.consulHttpClient = consulHttpClient;
        }

        public async Task<IList<Content>> GetContents(int userId, ContentSelectType selectType, int selectTypeId)
        {
            string ServiceLink = "/api/Content/GetContents"; //服务名称
                                                             // 1、设置参数连接
            string urlLink = $"{ServiceLink}?userId ={userId}&selectType={selectType}&selectTypeId={selectTypeId}";

            // 2、查询内容
            List<Content> contents = await consulHttpClient.GetAsync<List<Content>>(ServiceSchme, ServiceName, urlLink);
            return contents;
        }

        public Content InsertContents(Content content)
        {
            string ServiceLink = "/api/Content";
            return consulHttpClient.PostAsync<Content>(ServiceSchme, ServiceName, ServiceLink, content);
        }
    }
}

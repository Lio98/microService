using MicroService.ContentService.Models;
using MicroService.ContentService.Models.Enum;
using MicroService.ContentService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Services
{
    public class ContentServiceImpl : IContentService
    {
        private readonly IContentRepository contentRepository;

        public ContentServiceImpl(IContentRepository contentRepository)
        {
            this.contentRepository = contentRepository;
        }
        public IEnumerable<Content> GetContents()
        {
            return contentRepository.GetContents();
        }

        public IEnumerable<Content> GetContents(int id, ContentSelectType selectType, int selectTypeId)
        {
            return contentRepository.GetContents(id, selectType, selectTypeId);
        }

        public Content GetContentById(int id)
        {
            return contentRepository.GetContentById(id);
        }

        public void Create(Content content)
        {
            contentRepository.Create(content);
        }

        public void Update(Content content)
        {
            contentRepository.Update(content);
        }

        public void Delete(Content content)
        {
            contentRepository.Delete(content);
        }

        public bool ContentExists(int id)
        {
            return contentRepository.ContentExists(id);
        }

    }
}

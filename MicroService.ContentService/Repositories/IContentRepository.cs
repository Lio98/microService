using MicroService.ContentService.Models;
using MicroService.ContentService.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Repositories
{
    public interface IContentRepository
    {
        IEnumerable<Content> GetContents();

        IEnumerable<Content> GetContents(int id, ContentSelectType selectType, int selectTypeId);

        Content GetContentById(int id);

        void Create(Content content);

        void Update(Content content);

        void Delete(Content content);

        bool ContentExists(int id);
    }
}

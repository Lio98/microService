using MicroService.ContentService.Context;
using MicroService.ContentService.Models;
using MicroService.ContentService.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private ContentContext contentContext;

        public ContentRepository(ContentContext contentContext)
        {
            this.contentContext = contentContext;
        }

        public IEnumerable<Content> GetContents()
        {
            return contentContext.Content.ToList();
        }

        public IEnumerable<Content> GetContents(int id, ContentSelectType selectType, int selectTypeId)
        {
            if (id == 0)
            {
                if (selectType == ContentSelectType.UserId)
                {
                    return contentContext.Content.Where(i => i.FUserId == selectTypeId);
                }
                else if (selectType == ContentSelectType.CategoryId)
                {
                    return contentContext.Content.Where(i => i.FCategoryId == selectTypeId);
                }
            }
            else
            {
                if (selectType == ContentSelectType.UserId)
                {
                    return contentContext.Content.Where(i => i.FId == id && i.FUserId == selectTypeId);
                }
                else if (selectType == ContentSelectType.CategoryId)
                {
                    return contentContext.Content.Where(i => i.FId == id && i.FCategoryId == selectTypeId);
                }
            }

            return new List<Content>();
        }

        public Content GetContentById(int id)
        {
            return contentContext.Content.Find(id);
        }

        public void Create(Content content)
        {
            contentContext.Content.Add(content);
            contentContext.SaveChanges();
        }

        public void Update(Content content)
        {
            contentContext.Content.Update(content);
            contentContext.SaveChanges();
        }

        public void Delete(Content content)
        {
            contentContext.Content.Remove(content);
            contentContext.SaveChanges();
        }
        public bool ContentExists(int id)
        {
            return contentContext.Content.Any(i => i.FId == id);
        }

    }
}

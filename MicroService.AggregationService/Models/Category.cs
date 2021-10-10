using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Models
{
    public class Category
    {
        public int Fid { get; set; }

        public string FName { get; set; }

        public string FNumber { get; set; }

        public List<Content> FContents { get; set; }
    }
}

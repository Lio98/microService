using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Models
{
    public class Content
    {
        public int FId { get; set; }

        public string FTitle { get; set; }

        public string FContent { get; set; }

        public int FUserId { get; set; }

        public int FCategoryId { get; set; }
    }
}

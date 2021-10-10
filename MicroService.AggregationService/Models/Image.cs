using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Models
{
    public class Image
    {
        public int FId { get; set; }

        public string FImageUrl { get; set; }

        public int FContentId { get; set; }
    }
}

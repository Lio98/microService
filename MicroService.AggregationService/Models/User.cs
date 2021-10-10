using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.AggregationService.Models
{
    public class User
    {
        public int FId { get; set; }

        public string FName { get; set; }

        public int FAge { get; set; }

        public int FRoleId { get; set; }

        public IList<Content> FContents { get; set; }
    }
}

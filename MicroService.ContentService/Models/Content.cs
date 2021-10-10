using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Models
{
    public class Content
    {
        [Key]
        public int FId { get; set; }

        public string FTitle { get; set; }

        public string FContent { get; set; }

        public int FUserId { get; set; }

        public int FCategoryId { get; set; }
    }
}

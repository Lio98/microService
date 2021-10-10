using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ContentService.Models
{
    public class Category
    {
        [Key]
        public int FId { get; set; }

        public string FName { get; set; }

        public string FNumber { get; set; }
    }
}

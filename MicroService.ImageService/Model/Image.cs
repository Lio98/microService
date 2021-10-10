using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.ImageService.Model
{
    public class Image
    {
        [Key]
        public int FId { get; set; }

        public string FImageUrl { get; set; }

        public int FContentId { get; set; }
    }
}

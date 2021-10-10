using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MicroService.UserService.Models
{
    public class User
    {
        [Key]
        public int FId { get; set; }

        public string FName { get; set; }

        public  int FAge { get; set; }

        public int FRoleId { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(900)]
        public string FullName { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
    }
}

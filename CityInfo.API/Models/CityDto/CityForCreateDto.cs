using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class CityForCreateDto
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}

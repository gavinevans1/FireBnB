﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public String? CityName { get; set; } // Name of city

        [Required]
        public float TaxRate { get; set; } // City's tax rate
    }
}

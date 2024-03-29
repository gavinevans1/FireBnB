﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class PropertyDiscount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Property")]
        public int PropertyId { get; set; } // References the property

        [ForeignKey("PropertyId")]
        public Property? Property { get; set; }

        [Required]
        [Display(Name = "Discount")]
        public int DiscountId { get; set; } // References the discount

        [ForeignKey("DiscountId")]
        public Discount? Discount { get; set; }
    }
}

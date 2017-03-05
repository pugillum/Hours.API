﻿using System.ComponentModel.DataAnnotations;

namespace HoursApi.Models
{
    public class ProjectForUpdateDto
    {
        [Required(ErrorMessage = "You should provide a name value.")]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
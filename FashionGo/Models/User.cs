﻿using System.ComponentModel.DataAnnotations;

namespace FashionGo.Models
{
    public class User
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace SerwisPogodowy.Models.ViewModels
{
    public class UserLoginVM
    {
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format emaila")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków")]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
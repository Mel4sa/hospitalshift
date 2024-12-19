using System;
using System.ComponentModel.DataAnnotations;
using hospitalautomation.Models.Enum;

namespace hospitalautomation.Models.Dtos
{
    public class UserDto
    {

        public int Id { get; set; }
        
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [StringLength(50, ErrorMessage = "Email en fazla 50 karakter olabilir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 50 karakter olmalıdır.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Rol alanı zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir rol seçiniz.")]
        public UserRole Role { get; set; }

        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessage = "Adres en fazla 100 karakter olabilir.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [StringLength(15, ErrorMessage = "Telefon numarası en fazla 15 karakter olabilir.")]
        public string TelNo { get; set; }
        public int? DepartmentId { get; set; }
    }
}

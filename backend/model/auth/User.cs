using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bot.Api.Model.Auth
{
    [Table("users")]
    [Index(nameof(PhoneNumber), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(150)]
        public string? Name { get; set; }

        [MaxLength(200)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }

        [Required]
        public UserStatus Status { get; set; } = UserStatus.PendingEmail;
    }
}
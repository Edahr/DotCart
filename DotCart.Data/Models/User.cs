using System.ComponentModel.DataAnnotations;

namespace DotCart.Data.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string? ProfilePictureURL { get; set; }
        public string? Token { get; set; } //This is the token that will be used for the user to reset the password / confirm the email


        // Relationships
        public ICollection<Store> Stores { get; set; } = new List<Store>();
    }
}

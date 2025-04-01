namespace DotCart.Dto.Dtos.Users.Ouput
{
    public record UserOutputDto
    {
        public int UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string? ProfilePictureURL { get; set; }
    }
}

namespace DotCart.Dto.Dtos.Users.Input
{
    public record UpdateUserProfileInputDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfilePictureURL { get; set; }
    }
}

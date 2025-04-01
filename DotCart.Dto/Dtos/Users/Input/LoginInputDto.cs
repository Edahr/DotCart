namespace DotCart.Dto.Dtos.Users.Input
{
    public record LoginInputDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}

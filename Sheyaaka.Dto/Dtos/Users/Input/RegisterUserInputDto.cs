namespace SheyaakaDto.Dtos.Users.Input
{
    public record RegisterUserInputDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}

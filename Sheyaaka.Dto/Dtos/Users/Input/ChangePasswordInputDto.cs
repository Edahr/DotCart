namespace Sheyaaka.Dto.Dtos.Users.Input
{
    public record ChangePasswordInputDto
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}

using Sheyaaka.Data.Models;
using Sheyaaka.Dto.Dtos.Users.Ouput;

namespace Sheyaaka.Dto.MappingProfiles
{
    public static class UserMappingProfile
    {
        //user entity to user output dto
        public static UserOutputDto ToUserOutputDto(this User user)
        {
            return new UserOutputDto
            {
                UserID = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsEmailConfirmed = user.IsEmailConfirmed,
                ProfilePictureURL = user.ProfilePictureURL,
            };
        }
    }
}

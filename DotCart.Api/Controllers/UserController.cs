using DotCart.BLL.Interfaces;
using DotCart.Common.Helpers.JWTHelper;
using DotCart.Dto.Dtos.Users.Input;
using DotCart.Dto.Dtos.Users.Ouput;
using DotCart.Dto.MappingProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DotCart.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtHelper _jwtHelper;

        public UserController(IUserService userService, IJwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        #region Authentication and authorization methods
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerUserInputDto">User registration details.</param>
        /// <returns>Created user details.</returns>
        /// <remarks>
        /// This endpoint registers a new user into the system.
        /// </remarks>
        /// <response code="201">Returns the created user with their details.</response>
        /// <response code="400">If registration fails due to validation errors or other issues.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserOutputDto), 201)]
        [ProducesResponseType(typeof(object), 400)]
        public async Task<IActionResult> Register([FromBody, Required] RegisterUserInputDto registerUserInputDto)
        {
            try
            {
                var newUser = await _userService.RegisterUserAsync(registerUserInputDto);

                if (newUser == null)
                {
                    return BadRequest(new { error = "User registration failed." });
                }

                return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserID }, newUser.ToUserOutputDto());
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginInputDto">User login credentials.</param>
        /// <returns>Success message and user details if authentication is successful.</returns>
        /// <remarks>
        /// This endpoint authenticates a user and provides a JWT token to access protected resources.
        /// </remarks>
        /// <response code="200">Returns a JWT token for the authenticated user.</response>
        /// <response code="401">If credentials are incorrect or authentication fails.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody, Required] LoginInputDto loginInputDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginInputDto);
            if (user == null) return Unauthorized(new { error = "Invalid credentials." });

            // User is authenticated, generate a token
            var token = _jwtHelper.GenerateToken(user);

            return Ok(token);
        }
        #endregion

        #region Recovery Methods
        /// <summary>
        /// Confirms the email address of the user.
        /// </summary>
        /// <param name="email">User's email address.</param>
        /// <param name="token">Token sent to the user's email for confirmation.</param>
        /// <returns>A success message if email confirmation is successful.</returns>
        /// <response code="200">Returns a success message indicating email confirmation was successful.</response>
        /// <response code="404">If the user is not found or email confirmation fails.</response>
        [HttpGet("confirm-email")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var user = await _userService.ConfirmEmailAsync(email, token);
            if (user == null) return NotFound("User not found.");

            return Ok(new { message = "Email confirmed successfully" });
        }

        /// <summary>
        /// Initiates a password reset request by sending a link to the user's email.
        /// </summary>
        /// <param name="email">User's email address to send the reset link to.</param>
        /// <returns>A message indicating the password reset request has been initiated.</returns>
        /// <response code="200">Returns a success message indicating the password reset request is being processed.</response>
        /// <response code="404">If the user is not found or email reset fails.</response>
        [HttpGet("reset-password-request")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetPasswordRequest([FromQuery] string email)
        {
            var result = await _userService.ResetPasswordRequestAsync(email);
            if (result == null) return NotFound("User not found.");

            return Ok(new { message = "You'll soon receive an email with a key to reset your password!" });
        }

        /// <summary>
        /// Resets the user's password using the provided token and new password.
        /// </summary>
        /// <param name="email">User's email address associated with the password reset.</param>
        /// <param name="token">Token received in the reset password link.</param>
        /// <param name="newPassword">The new password to set for the user.</param>
        /// <returns>A message indicating the password has been reset successfully.</returns>
        /// <response code="200">Returns a success message indicating the password has been reset.</response>
        /// <response code="404">If the user is not found or the reset token is invalid.</response>
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResetPassword([FromQuery] string email, [FromQuery] string token, [FromBody] string newPassword)
        {
            var result = await _userService.ResetPasswordAsync(email, token, newPassword);
            if (result == null) return NotFound("User not found");

            return Ok(new { message = "Password reset successfully" });
        }
        #endregion

        #region Authorized User Methods
        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="changePasswordInputDto">New and current password details.</param>
        /// <returns>A success message if the password is successfully changed.</returns>
        /// <response code="200">Returns a success message if the password was changed.</response>
        /// <response code="400">If the email is invalid or other validation fails.</response>
        /// <response code="404">If the current password is incorrect.</response>
        [HttpPut("change-password")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInputDto changePasswordInputDto)
        {
            // Get the user email from the current authenticated user's claims
            var userClaims = User.Claims;
            var email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Invalid user Email.");

            // This will ensure the user won't be able to change someone else's password
            var result = await _userService.ChangePasswordAsync(email!, changePasswordInputDto.CurrentPassword, changePasswordInputDto.NewPassword);

            if (!result)
            {
                return NotFound("Invalid current password.");
            }

            return Ok("Password successfully changed.");
        }

        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        /// <param name="updateUserProfileInputDto">User profile update details.</param>
        /// <returns>The updated user details.</returns>
        /// <response code="200">Returns the updated user profile details.</response>
        /// <response code="400">If the email is invalid or other validation fails.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpPut("update-profile")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileInputDto updateUserProfileInputDto)
        {
            // Get the user email from the current authenticated user's claims
            var userClaims = User.Claims;
            var email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Invalid user Email.");

            var user = await _userService.UpdateUserProfileAsync(email!, updateUserProfileInputDto);

            if (user == null)
                return NotFound("User was not found.");

            return Ok(user?.ToUserOutputDto());
        }
        #endregion

        /// <summary>
        /// Retrieves a user by ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>User details.</returns>
        /// <response code="200">Returns the user details.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserOutputDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(user.ToUserOutputDto());
        }
    }
}

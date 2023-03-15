using Microsoft.AspNetCore.Mvc;
using SharepointPermission.Interfaces;

namespace SharepointPermission.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private readonly IUserProfileService _userProfileService;

    public UserProfileController(IUserProfileService userProfileService)
    {
        _userProfileService = userProfileService;
    }

    [HttpGet]
    public IActionResult ListSomePropertiesOfAllUsers()
    {
        try
        {
            var result = _userProfileService.ListSomePropertiesOfAllUsers();
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpPut("User-Profile/Single-Value")]
    public IActionResult UpdateUserProperty(string accountName, [FromBody] Dictionary<string, string> valuesUpdated)
    {
        try
        {
            _userProfileService.UpdateUserPropertySingleValue(accountName, valuesUpdated);
            return Ok("Updated successfully");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPut("User-Profile/Multi-Value")]
    public IActionResult UpdateUserProperty(string accountName,
        [FromBody] Dictionary<string, List<string>> valuesUpdated)
    {
        _userProfileService.UpdateUserPropertyMultiValue(accountName, valuesUpdated);
        return Ok("Fine");
    }
}
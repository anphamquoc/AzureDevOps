using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SharepointPermission.Interfaces;

namespace SharepointPermission.Controllers;

[ApiController]
[Route("api/[controller]")]
public class Group : ControllerBase
{
    private readonly ISiteService _siteService;

    public Group(ISiteService siteService)
    {
        _siteService = siteService;
    }


    [HttpPost]
    public IActionResult CreateNewGroupPermission([Required] string groupName)
    {
        try
        {
            _siteService.CreateNewGroupPermission(groupName);
            return Ok($"Created new group permission {groupName}");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpPut("Add/User")]
    public IActionResult AddUserToGroupPermission([Required] string username, [Required] string groupName)
    {
        try
        {
            _siteService.AddUserToGroupPermission(username, groupName);
            return Ok($"Added user {username} to group {groupName}");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpPut("Add/Permission-Level")]
    public IActionResult AddPermissionLevelToGroup([Required] string groupName, [Required] string permissionLevelName)
    {
        try
        {
            _siteService.AddPermissionLevelToGroup(groupName, permissionLevelName);
            return Ok($"Added permission level {permissionLevelName} to group {groupName}");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
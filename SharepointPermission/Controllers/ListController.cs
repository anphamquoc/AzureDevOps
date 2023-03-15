using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SharepointPermission.Interfaces;
using SharepointPermission.Models.List;

namespace SharepointPermission.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListController : ControllerBase
{
    private readonly IListService _listService;

    public ListController(IListService listService)
    {
        _listService = listService;
    }


    [HttpPost("Add-User/Permissions")]
    public IActionResult AddUserToPermissionList([Required] string listName, [Required] string username,
        [Required] string permissionLevelName)
    {
        try
        {
            _listService.AddUserToPermissionList(new AddUserToPermissionListInput
            {
                listName = listName,
                username = username,
                permissionLevelName = permissionLevelName
            });
            return Ok($"Added user {username} to list {listName} with permission level {permissionLevelName}");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpDelete("Stop-Inheriting/Permissions")]
    public IActionResult StopInheritingPermissions([Required] string listName)
    {
        try
        {
            _listService.StopInheritingPermissions(listName);
            return Ok($"Stopped inheriting permissions for list {listName}");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpDelete("Delete/Unique-Permission")]
    public IActionResult DeleteUniquePermission([Required] string listName)
    {
        try
        {
            _listService.DeleteUniquePermission(listName);
            return Ok($"Deleted unique permissions for list {listName}");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
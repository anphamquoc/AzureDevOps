using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SharePoint.Client;
using SharepointPermission.Interfaces;

namespace SharepointPermission.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionLevelController : ControllerBase
{
    private readonly ISiteService _siteService;
    public PermissionLevelController(ISiteService siteRepository)
    {
        _siteService = siteRepository;
    }

    [HttpPost]
    public IActionResult CreateNewPermissionLevel([Required] string permissionLevelName,
        IEnumerable<PermissionKind> roles)
    {
        try
        {
            _siteService.CreateNewPermissionLevel(permissionLevelName, roles);
            return Ok($"Created new permission level {permissionLevelName}");
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
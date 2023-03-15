using Microsoft.AspNetCore.Mvc;
using SharepointPermission.Interfaces;

namespace SharepointPermission.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SiteController : ControllerBase
{
    private readonly ISiteService _siteService;

    public SiteController(ISiteService siteRepository)
    {
        _siteService = siteRepository;
    }

    [HttpGet("Permission-Kind")]
    public IActionResult GetAllPermissionKind()
    {
        try
        {
            return Ok(_siteService.GetAllPermissionKind());
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
}
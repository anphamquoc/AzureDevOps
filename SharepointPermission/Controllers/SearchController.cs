using Microsoft.AspNetCore.Mvc;
using SharepointPermission.Interfaces;

namespace SharepointPermission.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("List")]
    public IActionResult SearchItemsUnderList(string listName, string searchProperty, string searchValue)
    {
        var result = _searchService.SearchItemsUnderList(listName, searchProperty, searchValue);
        return Ok(result);
    }

    [HttpGet("User")]
    public IActionResult SearchUsers([FromQuery] Dictionary<string, string> userProperties)
    {
        var result = _searchService.SearchUsers(userProperties);
        return Ok(result);
    }
}
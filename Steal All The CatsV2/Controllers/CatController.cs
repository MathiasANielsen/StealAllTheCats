using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Steal_All_The_CatsV2.Data;
using Steal_All_The_CatsV2.Models;
using Steal_All_The_CatsV2.Services;
using System.ComponentModel.DataAnnotations;



namespace Steal_All_The_CatsV2.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CatsController(CatService catService, CatDbContext context) : ControllerBase
{
    private readonly CatService _catService = catService;
    private readonly CatDbContext _context = context;


    /// <summary>
    /// Fetches 25 cat images from the CaaS API and stores them in the database.
    /// </summary>
    /// <returns>A message indicating the operation was successful.</returns>
    /// <response code="200">Returns a success message.</response>
    [HttpPost("fetch")]
    public async Task<IActionResult> FetchCats()
    {
        await _catService.FetchAndStoreCatsAsync();
        return Ok("Successfully fetched and stored 25 cat images.");
    }



    /// <summary>
    /// Retrieves a cat by its ID.
    /// </summary>
    /// <param name="id">The ID of the cat to retrieve.</param>
    /// <returns>The cat with the specified ID, including its tags.</returns>
    /// <response code="200">Returns the cat with the specified ID.</response>
    /// <response code="404">If the cat with the specified ID is not found.</response>
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCatById(int id)
    {
        var cat = await _context.Cats
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat == null)
        {
            return NotFound($"Cat with ID {id} not found.");
        }

        return Ok(cat);
    }



    /// <summary>
    /// Retrieves a paginated list of cats.
    /// </summary>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of cats, including their tags.</returns>
    /// <response code="200">Returns the paginated list of cats.</response>
    /// <response code="400">If the page or pageSize is invalid.</response>
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet]
    public async Task<IActionResult> GetCats([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Page and pageSize must be positive integers.");
        }

        var totalCats = await _context.Cats.CountAsync();
        var cats = await _context.Cats
            .Include(c => c.Tags)
            .OrderBy(c => c.Created)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new PagedCatResponse
        {
            TotalItems = totalCats,
            Page = page,
            PageSize = pageSize,
            Items = cats
        };

        return Ok(response);
    }


    /// <summary>
    /// Retrieves a paginated list of cats with a specific tag.
    /// </summary>
    /// <param name="tag">The tag to filter by (e.g., "playful").</param>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <returns>A paginated list of cats with the specified tag.</returns>
    /// <response code="200">Returns the paginated list of cats.</response>
    /// <response code="400">If the tag, page, or pageSize is invalid.</response>
    [HttpGet("tag")]
    public async Task<IActionResult> GetCatsByTag([FromQuery] string tag, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return BadRequest("Tag is required.");
        }

        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Page and pageSize must be positive integers."); 
        }

        var normalizedTag = tag.Trim().ToLower();

        // Count the total number of cats matching the tag
        var totalCats = await _context.Cats
            .Where(c => c.Tags.Any(t => t.Name.Equals(normalizedTag, StringComparison.OrdinalIgnoreCase)))
            .CountAsync();

        // Fetch the paginated list of cats
        var cats = await _context.Cats
            .Include(c => c.Tags)
            .Where(c => c.Tags.Any(t => t.Name == tag))
            .OrderBy(c => c.Created)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Create the response
        var response = new
        {
            TotalItems = totalCats,
            Page = page,
            PageSize = pageSize,
            Items = cats
        };

        return Ok(response);
    }
}



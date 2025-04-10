
using Steal_All_The_CatsV2.Data;
using Steal_All_The_CatsV2.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;


namespace Steal_All_The_CatsV2.Services;
//HttpClient httpClient, CatDbContext context
public class CatService(HttpClient httpClient, CatDbContext context)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly CatDbContext _context = context;

    public async Task<ServiceResult> FetchAndStoreCatsAsync()
    {
        // Fetching data from the Cat API
        var response = await _httpClient.GetStringAsync("https://api.thecatapi.com/v1/images/search?limit=25&has_breeds=true");
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var catData = JsonSerializer.Deserialize<List<CatApiResponse>>(response, options);

        // Check if data is null or empty
        if (catData == null || catData.Count == 0)
        {
            return new ServiceResult { Success = false, 
                Message = "No cat data found.",
                ValidationErrors = new List<ValidationResult>()
            };
        }
         
        // Fetch all tags from the database upfront
        var allTags = await _context.Tags.ToListAsync();

        foreach (var cat in catData)
        {
            // Download the image for the cat
            var imageData = await DownloadImageAsync(cat.Url);
            if (imageData == null || imageData.Length == 0)
            {
                continue; 
            }

            // Create CatEntity object
            var catEntity = new CatEntity
            {
                CatId = cat.Id,
                Image = cat.Url,          
                Width = cat.Width,        
                Height = cat.Height,    
                Created = DateTime.UtcNow, 
                Tags = new List<TagEntity>() 
            };

            // Validate CatEntity object
            var validationContext = new ValidationContext(catEntity);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(catEntity, validationContext, validationResults, true);

            if (!isValid)
            {
                return new ServiceResult { Success = false, Message = "Cat validation failed.", ValidationErrors = validationResults };
            }

            // Process temperaments (tags) for the cat
            var temperaments = cat.Breeds
                .SelectMany(b => b.Temperament.Split(',', StringSplitOptions.TrimEntries))
                .Distinct();

            foreach (var temperament in temperaments)
            {
                // Check if the tag exists in the pre-fetched list of tags
                var tag = allTags.FirstOrDefault(t => t.Name.Equals(temperament, StringComparison.OrdinalIgnoreCase));

                if (tag == null)
                {
                    // Create new TagEntity if not found
                    tag = new TagEntity
                    {
                        Id = tag.Id,
                        Name = temperament,
                        Created = DateTime.UtcNow,
                        Cats = new List<CatEntity>()
                    };

                    // Validate TagEntity
                    var tagValidationContext = new ValidationContext(tag);
                    var tagValidationResults = new List<ValidationResult>();
                    bool tagIsValid = Validator.TryValidateObject(tag, tagValidationContext, tagValidationResults, true);

                    if (!tagIsValid)
                    {
                        return new ServiceResult { Success = false, 
                            Message = "Tag validation failed.", 
                            ValidationErrors = tagValidationResults };
                    }

                    // Add the new tag to the context
                    _context.Tags.Add(tag);
                    allTags.Add(tag);  // Add to the in-memory list as well
                }

                // Add the tag to the cat's Tags list
                catEntity.Tags.Add(tag);
            }

            // Add the cat entity to the database
            _context.Cats.Add(catEntity);
        }
        // Save all changes to the database
        await _context.SaveChangesAsync();

        // Return success result
        return new ServiceResult
        {
            Success = true,
            Message = "Successfully fetched and stored cat images.",
            ValidationErrors = new List<ValidationResult>()
        };

        
    }




    private async Task<byte[]?> DownloadImageAsync(string url)
    {
        try
        {
            return await _httpClient.GetByteArrayAsync(url);
        }
        catch
        {
            return null;
        }
    }
}
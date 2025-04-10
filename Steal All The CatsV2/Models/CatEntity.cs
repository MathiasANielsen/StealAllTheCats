
using Steal_All_The_CatsV2.Models;
using System.ComponentModel.DataAnnotations;

namespace Steal_All_The_CatsV2.Models;
public class CatEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "CatId is required.")]
    public string CatId { get; set; } = string.Empty;

     
    [Required(ErrorMessage = "Width is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Width must be a positive integer.")]
    public int Width { get; set; }


    [Required(ErrorMessage = "Height is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Height must be a positive integer.")]
    public int Height { get; set; }

    [Required(ErrorMessage = "Url is required.")]
    public string Image { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public List<TagEntity> Tags { get; set; } = new List<TagEntity>();
}
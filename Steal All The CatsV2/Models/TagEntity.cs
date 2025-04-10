using Steal_All_The_CatsV2.Models;
using System.ComponentModel.DataAnnotations;

namespace Steal_All_The_CatsV2.Models;

public class TagEntity
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tag name is required.")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Tag name must be between 1 and 50 characters.")]
    public string Name { get; set; } = string.Empty;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public List<CatEntity> Cats { get; set; } = new List<CatEntity>();
} 
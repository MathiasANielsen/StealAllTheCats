using System.ComponentModel.DataAnnotations;

namespace Steal_All_The_CatsV2.Services
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public required string Message { get; set; }
        public required List<ValidationResult> ValidationErrors { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;

namespace Critical_Events_Finder_Api.Models
{
    public class CreateFolderRequest
    {
        [Required(ErrorMessage = "Folder name is required.")]
        [MinLength(3, ErrorMessage = "Folder name must be at least 3 characters long.")]
        [MaxLength(50, ErrorMessage = "Folder name cannot exceed 50 characters.")]
        public string folder_name { get; set; } = string.Empty;
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.ValidationConstants;

namespace SeminarHub.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(CategoryNameMaxLength)]
        [Comment("Represents the category's name.")]
        public string Name { get; set; }

        public ICollection<Seminar> Seminars { get; set; } = new HashSet<Seminar>();
    }
}

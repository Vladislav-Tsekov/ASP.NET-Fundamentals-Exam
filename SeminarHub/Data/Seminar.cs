using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SeminarHub.Data.ValidationConstants;

namespace SeminarHub.Data
{
    public class Seminar
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TopicMaxLength)]
        [Comment("Seminar's theme")]
        public string Topic { get; set; }

        [Required]
        [MaxLength(LecturerMaxLength)]
        [Comment("Seminar's lecturer - speaker")]
        public string Lecturer { get; set; }

        [Required]
        [MaxLength(DetailsMaxLength)]
        [Comment("Additional details regarding the seminar")]
        public string Details { get; set; }

        [Required]
        public string OrganizerId { get; set; }

        [Required]
        public IdentityUser Organizer { get; set; }

        [Required]
        [Comment("The date when the seminar is supposed to take place")]
        public DateTime DateAndTime { get; set; }

        [Range(DurationMinRange, DurationMaxRange)]
        [Comment("Estimated duration of the seminar")]
        public int Duration { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        public ICollection<SeminarParticipant> SeminarsParticipants { get; set; } = new HashSet<SeminarParticipant>();
    }
}

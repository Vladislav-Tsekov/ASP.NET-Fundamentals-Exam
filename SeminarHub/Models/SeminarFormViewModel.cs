using SeminarHub.Data;
using System.ComponentModel.DataAnnotations;
using static SeminarHub.Data.ValidationConstants;

namespace SeminarHub.Models
{
    public class SeminarFormViewModel
    {
        [Required(ErrorMessage = FieldIsRequired)]
        [StringLength(TopicMaxLength, MinimumLength = TopicMinLength, ErrorMessage = TextLengthRange)]
        public string Topic { get; set; }

        [Required(ErrorMessage = FieldIsRequired)]
        [StringLength(LecturerMaxLength, MinimumLength = LecturerMinLength, ErrorMessage = TextLengthRange)]
        public string Lecturer { get; set; }

        [Required(ErrorMessage = FieldIsRequired)]
        [StringLength(DetailsMaxLength, MinimumLength = DetailsMinLength, ErrorMessage = TextLengthRange)]
        public string Details { get; set; }

        [Required(ErrorMessage = FieldIsRequired)]
        public string DateAndTime { get; set; }

        [Range(DurationMinRange, DurationMaxRange, ErrorMessage = TimeLimitRange)]
        public int Duration { get; set; }

        [Required(ErrorMessage = FieldIsRequired)]
        public string Organizer { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}

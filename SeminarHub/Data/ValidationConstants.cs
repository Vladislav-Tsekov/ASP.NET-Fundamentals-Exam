namespace SeminarHub.Data
{
    public class ValidationConstants
    {
        //Seminar
        public const int TopicMinLength = 3;
        public const int TopicMaxLength = 100;

        public const int LecturerMinLength = 5;
        public const int LecturerMaxLength = 60;

        public const int DetailsMinLength = 10;
        public const int DetailsMaxLength = 500;

        public const string SeminarDateFormat = "dd/MM/yyyy HH:mm";

        public const int DurationMinRange = 30;
        public const int DurationMaxRange = 180;

        //Category
        public const int CategoryNameMinLength = 3;
        public const int CategoryNameMaxLength = 50;

        //Error Messages
        public const string FieldIsRequired = "The field {0} is required.";
        public const string TextLengthRange = "The field {0} must be between {2} and {1} characters long.";
        public const string TimeLimitRange = "The {0} must be between {1} and {2} minutes long.";
    }
}

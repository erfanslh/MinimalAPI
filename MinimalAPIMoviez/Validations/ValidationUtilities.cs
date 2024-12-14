using FluentValidation;

namespace MinimalAPIMoviez.Validations
{
    public class ValidationUtilities 
    {
        public static string MaxLengthMessage = "The {PropertyName} should has maximum {MaxLength} Character";
        public static string UpperCaseMessage = "First letter of {PropertyName} should be UpperCase";
        public static string NotEmptyMessage = "The {PropertyName} should not be empty";
        public static string GreaterThanDate(DateTime value) => "The Birthday of the Actor should be greater than " + value.ToString("yyyy-MM-dd");
        public static string ExistsActor(string name, DateTime date) => "The Actor with the " + name + " and Birthdate " + date.ToString("yyyy-MMMM-dd") + " already exists";

        public static bool FirstLetterUpperCase(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }
            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }

    }
}

namespace MinimalAPIsWithASPNetEF.Validations
{
    public static class ValidationUtils
    {
        public static string NOT_EMPTY_MESSAGE = "The field {PropertyName} is required!";
        public static string MAX_LENGTH_MESSAGE = "The field {PropertyName} must be less than {MaxLength} characters!";

        public static string EMAIL_ADDRESS_MESSAGE = "The field {PropertyName} is not a valid email address!";

        public static bool IsFirstLetterUppercase(string val)
        {
            if (val is null || val.Length < 1)
            {
                return true; // not to validation for empty or null because somewhere else does
            }

            var firstLetter = val[0].ToString();

            return firstLetter == firstLetter.ToUpper();
        }
    }
}

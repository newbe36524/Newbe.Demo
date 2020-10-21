namespace Newbe.ExpressionsTests.Model
{
    public struct ValidateResult
    {
        public bool IsOk { get; set; }
        public string ErrorMessage { get; set; }

        public void Deconstruct(out bool isOk, out string errorMessage)
        {
            isOk = IsOk;
            errorMessage = ErrorMessage;
        }

        public static ValidateResult Ok()
        {
            return new ValidateResult
            {
                IsOk = true
            };
        }

        public static ValidateResult Error(string errorMessage)
        {
            return new ValidateResult
            {
                IsOk = false,
                ErrorMessage = errorMessage
            };
        }
    }
}
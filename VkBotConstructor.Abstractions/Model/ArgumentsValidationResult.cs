namespace VkBotConstructor.Abstractions.Model
{
    public class ArgumentsValidationResult
    {
        public static ArgumentsValidationResult Success => new() { IsValid = true };

        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

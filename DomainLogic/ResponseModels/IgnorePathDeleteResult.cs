namespace DomainLogic.ResponseModels
{
    public record IgnorePathDeleteResult(
        bool Success = false,
        string ErrorMessage = ""
    );
}
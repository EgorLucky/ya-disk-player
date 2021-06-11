namespace DomainLogic.ResponseModels
{
    public record IgnorePathAddResult(
        bool Success = false,
        string ErrorMessage = ""
    );
}
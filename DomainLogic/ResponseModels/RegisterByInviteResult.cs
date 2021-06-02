using DomainLogic.Entities;

namespace DomainLogic.ResponseModels
{
    public record RegisterByInviteResult(
        bool Success = false,
        string ErrorMessage = "",
        User User = null
    );
}
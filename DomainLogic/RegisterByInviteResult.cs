using DomainLogic.Entities;

namespace DomainLogic
{
    public record RegisterByInviteResult(
        bool Success = false,
        string ErrorMessage = "",
        User User = null
    );
}
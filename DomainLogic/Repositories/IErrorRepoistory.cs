using System;
using System.Threading.Tasks;

namespace DomainLogic.Repositories
{
    public interface IErrorRepoistory
    {
        Task Add(Exception ex, Guid processId);
    }
}
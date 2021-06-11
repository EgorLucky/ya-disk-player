using System;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface IErrorRepoistory
    {
        Task Add(Exception ex, Guid processId);
    }
}
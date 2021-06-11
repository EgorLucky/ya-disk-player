using DomainLogic;
using Implementations.EFModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Implementations
{
    public class ErrorRepository : IErrorRepoistory
    {
        private YaDiskPlayerDbContext _context;

        public ErrorRepository(YaDiskPlayerDbContext context)
        {
            _context = context;
        }

        public async Task Add(Exception ex, Guid processId)
        {
            var error = new SynchronizationProcessError
            {
                Id = Guid.NewGuid(),
                MessageText = ex.ToString(),
                CreateDateTime = DateTimeOffset.Now,
                SynchronizationProcessId = processId
            };

            await _context.AddAsync(error);
            await _context.SaveChangesAsync();
        }
    }
}

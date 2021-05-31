using AutoMapper;
using DomainLogic;
using DomainLogic.Entities;
using Implementations.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

using DomainUser = DomainLogic.Entities.User;
using DBUser = Implementations.EFModels.User;

namespace Implementations
{
    public class UserRepository : IUserRepository
    {
        private YaDiskPlayerDbContext _context;
        private IMapper _mapper;

        public UserRepository(YaDiskPlayerDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DomainUser> GetUserByEmail(string email)
        {
            email = email.ToLower();

            var user = await _context
                                .Users
                                .Where(u => u.Email.ToLower() == email)
                                .FirstOrDefaultAsync();

            var result = _mapper.Map<DomainUser>(user);

            return result;
        }

        public async Task<DomainUser> GetUserByInviteId(Guid inviteId)
        {
            var user = await _context
                                .Users
                                .Where(u => u.InviteId == inviteId)
                                .FirstOrDefaultAsync();

            var result = _mapper.Map<DomainUser>(user);

            return result;
        }

        public async Task Add(DomainUser user)
        {
            var userToSave = _mapper.Map<DBUser>(user);
            userToSave.Id = Guid.NewGuid();

            await _context.AddAsync(userToSave);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserByInviteId(DomainUser newUserData)
        {
            var user = _context.ChangeTracker
                                .Entries<DBUser>()
                                .Where(u => u.Entity.InviteId == newUserData.InviteId)
                                .Select(u => u.Entity)
                                .FirstOrDefault();
            if(user == null)
                user = await _context.Users
                                .Where(u => u.InviteId == newUserData.InviteId)
                                .FirstOrDefaultAsync();

            if (user == null)
                return;
            _mapper.Map(newUserData, user);

            await _context.SaveChangesAsync();
        }

        public Task<int> GetCount()
        {
            return _context.Users
                                .Where(u => u.ActivateDateTime.HasValue)
                                .Where(u => u.DeactivateDateTime.HasValue == false)
                                .CountAsync();
        }
    }
}

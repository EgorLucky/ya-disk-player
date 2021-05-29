using DomainLogic;
using DomainLogic.Entities;
using Implementations.EFModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Implementations
{
    public class UserRepository : IUserRepository
    {
        private YaDiskPlayerDbContext _context;

        public UserRepository(YaDiskPlayerDbContext context)
        {
            _context = context;
        }

        public async Task<DomainLogic.Entities.User> GetUserByEmail(string email)
        {
            email = email.ToLower();

            var user = await _context
                                .Users
                                .Where(u => u.Email.ToLower() == email)
                                .FirstOrDefaultAsync();

            var result = new DomainLogic.Entities.User(
                YandexId: user.YandexId,
                Firstname: user.Firstname,
                Lastname: user.Lastname,
                Email: user.Email,
                Login: user.Login,
                Sex: user.Sex,
                CreateDateTime: user.CreateDateTime,
                InviteId: user.InviteId
                )
            { 
                ActivateDateTime  = user.ActivateDateTime,
                IsAdmin = user.IsAdmin
            };

            return result;
        }

        public async Task<DomainLogic.Entities.User> GetUserByInviteId(Guid inviteId)
        {
            var user = await _context
                                .Users
                                .Where(u => u.InviteId == inviteId)
                                .FirstOrDefaultAsync();

            var result = new DomainLogic.Entities.User(
                YandexId: user.YandexId,
                Firstname: user.Firstname,
                Lastname: user.Lastname,
                Email: user.Email,
                Login: user.Login,
                Sex: user.Sex,
                CreateDateTime: user.CreateDateTime,
                InviteId: user.InviteId
                )
            {
                ActivateDateTime = user.ActivateDateTime,
                IsAdmin = user.IsAdmin
            };

            return result;
        }

        public async Task Add(DomainLogic.Entities.User user)
        {
            var userToSave = new EFModels.User
            {
                Id = Guid.NewGuid(),
                YandexId = user.YandexId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Login = user.Login,
                Sex = user.Sex,
                CreateDateTime = user.CreateDateTime,
                InviteId = user.InviteId,
                ActivateDateTime = user.ActivateDateTime,
                IsAdmin = user.IsAdmin
            };

            await _context.AddAsync(userToSave);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserByInviteId(DomainLogic.Entities.User newUserData)
        {
            var user = _context.ChangeTracker
                                .Entries<EFModels.User>()
                                .Where(u => u.Entity.InviteId == newUserData.InviteId)
                                .Select(u => u.Entity)
                                .FirstOrDefault();
            if(user == null)
                user = await _context.Users
                                .Where(u => u.InviteId == newUserData.InviteId)
                                .FirstOrDefaultAsync();

            if (user == null)
                return;

            user.YandexId = newUserData.YandexId;
            user.Firstname = newUserData.Firstname;
            user.Lastname = newUserData.Lastname;
            user.Email = newUserData.Email;
            user.Login = newUserData.Login;
            user.Sex = newUserData.Sex;
            user.CreateDateTime = newUserData.CreateDateTime;
            user.InviteId = newUserData.InviteId;
            user.ActivateDateTime = newUserData.ActivateDateTime;
            user.IsAdmin = newUserData.IsAdmin;

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

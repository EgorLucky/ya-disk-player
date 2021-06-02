using DomainLogic.Entities;
using DomainLogic.RequestModels;
using DomainLogic.ResponseModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DomainLogic
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> TryCreateFirstUser(string yandexId, string email)
        {
            var userCount = await _userRepository.GetCount();

            if (userCount != 0)
                return false;

            var user = new User(
                CreateDateTime: DateTimeOffset.Now,
                Email: email,
                YandexId: yandexId
            )
            {
                IsAdmin = true,
                ActivateDateTime = DateTimeOffset.Now
            };

            await _userRepository.Add(user);

            return true;

        }

        public Task<User> GetUserByEmail(string email)
        {
            return _userRepository.GetUserByEmail(email);
        }

        public async Task<CreateUserInviteResult> CreateUserInvite(CreateUserInviteRequest request)
        {
            var user = default(User);
            var emailValidator = new EmailAddressAttribute();

            var isEmailValid = emailValidator.IsValid(request?.Email);

            if (isEmailValid == false && !string.IsNullOrEmpty(request?.Email))
            {
                return new CreateUserInviteResult(
                        ErrorMessage: $"Invalid email"
                    );
            }

            if (!string.IsNullOrEmpty(request?.Email))
            {
                user = await _userRepository.GetUserByEmail(request?.Email);

                if (user != null)
                {
                    return new CreateUserInviteResult(
                        ErrorMessage: $"User {request.Email} already exists"
                    );
                } 
            }

            user = new User(
                InviteId: Guid.NewGuid(),
                CreateDateTime: DateTimeOffset.Now,
                Email: request?.Email,
                Firstname: request?.Firstname,
                Lastname: request?.Lastname
            );

            await _userRepository.Add(user);

            return new CreateUserInviteResult(
                Success: true,
                InviteId: user.InviteId);
        }

        public async Task<RegisterByInviteResult> RegisterByInvite(Guid inviteId, string yandexId, string email)
        {
            var user = await _userRepository.GetUserByInviteId(inviteId);

            if(user == null || user.ActivateDateTime.HasValue)
            {
                return new RegisterByInviteResult(
                    ErrorMessage: "Wrong inviteId"
                );
            }

            if (!string.IsNullOrEmpty(user.Email) &&
                user.Email.ToLower() != email.ToLower())
            {
                return new RegisterByInviteResult(
                    ErrorMessage: "Wrong email"
                );
            }

            user.ActivateDateTime = DateTimeOffset.Now;
            if (string.IsNullOrEmpty(user.Email))
                user = user with { Email = email };

            user = user with { YandexId = yandexId };

            await _userRepository.UpdateUserByInviteId(user);

            return new RegisterByInviteResult(
                Success: true,
                User: user
            );
        }


    }
}

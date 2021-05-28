﻿using DomainLogic.Entities;
using System;
using System.Threading.Tasks;

namespace DomainLogic
{
    public interface IUserRepository
    {
        Task<User> GetUserByInviteId(Guid inviteId);
        Task UpdateUserByInviteId(User newUserData);
        Task Add(User user);
        Task<User> GetUserByEmail(string email);
    }
}
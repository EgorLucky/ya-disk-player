using System;
using System.ComponentModel.DataAnnotations;

namespace Implementations.EFModels
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string YandexId { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Login { get; set; }
        public string Sex { get; set; }
        public Guid? InviteId { get; set; }
        public DateTimeOffset? CreateDateTime { get; set; }
        public DateTimeOffset? ActivateDateTime { get; set; }
        public bool IsAdmin { get; set; }
    }
}
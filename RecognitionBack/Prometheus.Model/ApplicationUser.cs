using Prometheus.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Prometheus.Model
{
    /// <summary>
    /// Пользователь системы
    /// </summary>
    public class ApplicationUser : IEntity<Guid>
    {
        /// <summary>
        /// Ид пользователя
        /// </summary>
        [Key]
        public Guid UserId { get; set; }  

        /// <summary>
        /// Логин пользователя
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Login { get; set; }

        /// <summary>
        /// Хеш пароля
        /// </summary>
        [Required]
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Соль пароля
        /// </summary>
        [Required]
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        [Required]
        [MaxLength(250)]
        public string UserName { get; set; }


        public string Phone { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// Код роли
        /// </summary>        
        public int RoleId { get; set; }
       

        public Guid Key => UserId;

        /// <summary>
        /// Отметка об удалении (не отображается в справочниках)
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}

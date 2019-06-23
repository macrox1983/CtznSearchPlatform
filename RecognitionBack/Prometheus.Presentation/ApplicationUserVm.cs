using System;
using System.Collections.Generic;
using System.Text;

namespace Prometheus.Presentation
{
    // TODO: Пользователи хранятся на сервере, для каждого аэропорта индивидуально или в общей базе, но отображаться будут под каждый аэропорт???
    public class ApplicationUserVm
    {       
        public Guid UserId { get; set; }
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Хеш пароля
        /// </summary>
        public byte[] PasswordHash { get; set; }

        /// <summary>
        /// Соль пароля
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Код роли
        /// </summary>        
        public Guid RoleId { get; set; }

        /// <summary>
        /// Роль пользователя
        /// </summary>
        public virtual RoleVm Role { get; set; }

    }
}

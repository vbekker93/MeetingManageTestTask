using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MeetingService.Models
{
    /// <summary>
    /// Модель сущности "Участник встречи"
    /// </summary>
    public class Participant
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Имя участника
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Фамилия участника
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Отчество участника
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Дата рождения участника
        /// </summary>
        public DateTime BirthDay { get; set; }

        /// <summary>
        /// Email участника
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Телефон участника
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Связь с сущностью "Встреча"
        /// </summary>
        public ICollection<MeetParticipant> MeetParticipants { get; set; }
    }
}





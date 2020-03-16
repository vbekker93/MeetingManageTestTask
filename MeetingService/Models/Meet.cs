using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MeetingService.Models
{
    /// <summary>
    /// Модель сущности "Встреча"
    /// </summary>
    public class Meet
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Название встречи
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Описание встречи
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Место встречи
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Куратор встречи
        /// </summary>
        public string ContactPersonFullName { get; set; }

        /// <summary>
        /// Телефон куратора
        /// </summary>
        public string ContactPersonPhone { get; set; }

        /// <summary>
        /// Время начала встречи
        /// </summary>
        public DateTime MeetingStartDateTime { get; set; }

        /// <summary>
        /// Время завершения встречи
        /// </summary>
        public DateTime MeetingEndDateTime { get; set; }

        /// <summary>
        /// Связь с сущностью "Участник встречи"
        /// </summary>
        public ICollection<MeetParticipant> MeetParticipants { get; set; }
    }
}

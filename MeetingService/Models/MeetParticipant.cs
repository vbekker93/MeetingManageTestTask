using System.ComponentModel.DataAnnotations;

namespace MeetingService.Models
{
    /// <summary>
    /// Модель связи (многие-ко-многим) сущностей "Участник встречи" и "Встреча"
    /// </summary>
    public class MeetParticipant
    {
        [Key]
        public int MeetId { get; set; }

        /// <summary>
        /// Объект сущности "Встреча"
        /// </summary>
        public Meet Meet { get; set; }

        [Key]
        public int ParticipantId { get; set; }

        /// <summary>
        /// Объект сущности "Участник встречи"
        /// </summary>
        public Participant Participant { get; set; }
    }
}

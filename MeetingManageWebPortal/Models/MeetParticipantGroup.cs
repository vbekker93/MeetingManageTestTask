
namespace MeetingService.Models
{
    /// <summary>
    /// Модель вычисляемой сущности "Участники встречи"
    /// </summary>
    public class MeetParticipantGroup
    {
        /// <summary>
        /// Название встречи
        /// </summary>
        public string MeetTitle { set; get; }

        /// <summary>
        /// Список участников
        /// </summary>
        public string ParticipantList { set; get; }
    }
}

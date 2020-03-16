using MeetingService.Models;
using System;
using System.Linq;
using System.Text;

namespace MeetingService.Services
{
    /// <summary>
    /// Сервис валидации данных
    /// </summary>
    public static class CheckService
    {
        /// <summary>
        /// Проверка объекта Встреча
        /// </summary>
        /// <param name="_context">Контекст</param>
        /// <param name="meet">Объект Встреча</param>
        /// <param name="_errMessage">Ошибки во время проверки</param>
        /// <returns>Результат проверки объекта Встреча</returns>
        public static bool CheckIsMeetValid(MeetingBaseContext _context, Meet meet, out string _errMessage)
        {
            StringBuilder _errors = new StringBuilder();

            if (meet.MeetingStartDateTime > meet.MeetingEndDateTime)
                _errors.AppendLine("End date exceeds start date");

            if (meet.MeetingStartDateTime <= DateTime.Now)
                _errors.AppendLine("Start date must be current in time");

            var meetInDB = _context.Meets
                  .FirstOrDefault(a => a.Title == meet.Title && meet.Id == 0);

            if (meetInDB != null)
                _errors.AppendLine($"Meet with title {meet.Title} exist");

            _errMessage = _errors.ToString();

            return string.IsNullOrEmpty(_errMessage);

        }

        /// <summary>
        /// Проверка объекта Участник
        /// </summary>
        /// <param name="_context">Контекст</param>
        /// <param name="participant">Объект Участник</param>
        /// <param name="_errMessage">Ошибки во время проверки</param>
        /// <returns>Результат проверки объекта Участник</returns>
        internal static bool CheckIsParticipantValid(MeetingBaseContext _context, Participant participant, out string _errMessage)
        {
            StringBuilder _errors = new StringBuilder();

            if (participant.BirthDay >= DateTime.Now)
                _errors.AppendLine("Birth date exceeds current date");

            if (!IsValidEmail(participant.Email))
                _errors.AppendLine("Email is no valid");

            _errMessage = _errors.ToString();

            return string.IsNullOrEmpty(_errMessage);
        }

        /// <summary>
        /// Валидация Email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static bool IsValidEmail(string email)
        {
            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}

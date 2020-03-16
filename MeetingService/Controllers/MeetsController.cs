using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingService.Models;
using Newtonsoft.Json.Linq;
using MeetingService.Services;

namespace MeetingService.Controllers
{
    /// <summary>
    /// Контроллер "Встречи"
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MeetsController : ControllerBase
    {
        private readonly MeetingBaseContext _context;

        public MeetsController(MeetingBaseContext context)
        {
            _context = context;
        }

        // GET: api/Meets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Meet>>> GetMeets()
        {
            return await _context.Meets.ToListAsync();
        }

        // GET: api/Meets/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Meet>> GetMeet(int id)
        {
            Meet meet = await _context.Meets.FindAsync(id);

            if (meet == null)
            {
                return NotFound();
            }

            return meet;
        }

        // PUT: api/Meets/id
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeet(int id, Meet meet)
        {
            if (id != meet.Id)
            {
                return BadRequest();
            }

            _context.Entry(meet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeetExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Meets
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<string> PostMeet(Meet meet)
        {
            if (CheckService.CheckIsMeetValid(_context, meet, out string _infoMessage))
            {
                Meet meetInDB = _context.Meets.FirstOrDefault(a => (a.Id == meet.Id));

                if (meetInDB == null)
                {
                    _context.Meets.Add(meet);
                    _infoMessage = $"Meet {meet.Title} is added!";
                }
                else
                {
                    // Если встреча индентифицирована - обновляем поля
                    meetInDB.Address = meet.Address;
                    meetInDB.ContactPersonFullName = meet.ContactPersonFullName;
                    meetInDB.ContactPersonPhone = meet.ContactPersonPhone;
                    meetInDB.Description = meet.Description;
                    meetInDB.MeetingEndDateTime = meet.MeetingEndDateTime;
                    meetInDB.MeetingStartDateTime = meet.MeetingStartDateTime;
                    meetInDB.Title = meet.Title;
                    meet = meetInDB;
                    _infoMessage = $"Meet {meet.Title} is updated!";
                }

                await _context.SaveChangesAsync();
            }

            return _infoMessage;
        }


        // DELETE: api/Meets/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<Meet>> DeleteMeet(int id)
        {
            Meet meet = await _context.Meets.FindAsync(id);
            if (meet == null)
            {
                return NotFound();
            }

            _context.Meets.Remove(meet);
            await _context.SaveChangesAsync();

            return meet;
        }

        private bool MeetExists(int id)
        {
            return _context.Meets.Any(e => e.Id == id);
        }

        #region Custom

        [HttpGet("MeetsWithParticipantsList")]
        public async Task<ActionResult<object>> GetMeetsWithParticipantsList()
        {
            var linqResult = from Participant in _context.Participants
                             join MeetParticipants in _context.MeetParticipants on Participant.Id equals MeetParticipants.ParticipantId
                             join Meet in _context.Meets on MeetParticipants.MeetId equals Meet.Id
                             select new { Meet.Id, Meet.Title, Participant.Name };

            var listResult = await linqResult.ToListAsync();

            var groupedMeet = listResult.Select(x => new { x.Id, x.Title }).Distinct();

            List<MeetParticipantGroup> resultList = new List<MeetParticipantGroup>();

            foreach (var meet in groupedMeet)
            {
                string groupList = string.Join("; ", listResult.Where(s => s.Id == meet.Id).Select(x => x.Name));
                resultList.Add(new MeetParticipantGroup() { MeetTitle = meet.Title, ParticipantList = groupList });
            }

            return resultList;
        }


        [HttpPost("AddParticipantInMeet")]
        public async Task<string> PostAddParticipantInMeet([FromBody]JObject data)
        {
            Participant participant = data["participant"].ToObject<Participant>();
            Meet meet = data["meet"].ToObject<Meet>();

            Meet meetInDB = _context.Meets
                  .FirstOrDefault(a => a.Id == meet.Id);

            if (meetInDB == null)
                return $"Meet {meet.Title} with ID {meet.Id} is not exist in DB";

            meetInDB = _context.Meets
                  .Include(r => r.MeetParticipants)
                  .FirstOrDefault(a => a.Id == meet.Id);

          
            Participant participantInDB = _context.Participants
                .FirstOrDefault(a => (a.Surname == participant.Surname && a.Email == participant.Email)
                                     || a.Id == participant.Id);

            if (participantInDB == null)
            {
                _context.Participants.Add(participant);
            }
            else
            {
                var meetCurrentParticipant = _context.MeetParticipants
                   .Include(r => r.Meet)
                   .Where(a => a.ParticipantId == participantInDB.Id && a.Meet != null);

                foreach (var curMeet in meetCurrentParticipant)
                {
                    if (meetInDB.MeetingStartDateTime <= curMeet.Meet.MeetingStartDateTime && curMeet.Meet.MeetingStartDateTime <= meetInDB.MeetingEndDateTime)
                        return $"The participant is already participating in the meeting  {curMeet.Meet.Title} in the same period!";
                }

                // Если участник индентифицирован - обновляем поля
                participantInDB.BirthDay = participant.BirthDay;
                participantInDB.Name = participant.Name;
                participantInDB.Patronymic = participant.Patronymic;
                participantInDB.Phone = participant.Phone;

                participant = participantInDB;
            }

            if (_context.MeetParticipants.FirstOrDefault(a => a.MeetId == meetInDB.Id && a.ParticipantId == participant.Id) != null)
                return $"The participant {participant.Surname} {participant.Name} is already participating in the meeting {meetInDB.Title}";

            if (!CheckService.CheckIsParticipantValid(_context, participant, out string _errMessage))
                return _errMessage;

            MeetParticipant meetParticipant = new MeetParticipant { Participant = participant, Meet = meetInDB };
            _context.MeetParticipants.Add(meetParticipant);

            await _context.SaveChangesAsync();

            return $"Participant {participant.Surname} {participant.Name} added in {meetInDB.Title} meet!";
        }
        #endregion
    }
}

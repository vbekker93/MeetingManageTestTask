using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingService.Models;
using MeetingService.Services;

namespace MeetingService.Controllers
{
    /// <summary>
    /// Контроллер "Участник"
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly MeetingBaseContext _context;

        public ParticipantsController(MeetingBaseContext context)
        {
            _context = context;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Participant>>> GetParticipants()
        {
            return await _context.Participants.ToListAsync();
        }

        // GET: api/Participants/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Participant>> GetParticipant(int id)
        {
            Participant participant = await _context.Participants.FindAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            return participant;
        }

        // PUT: api/Participants/id
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipant(int id, Participant participant)
        {
            if (id != participant.Id)
            {
                return BadRequest();
            }

            _context.Entry(participant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(id))
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

        // POST: api/Participants
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Participant>> PostParticipant(Participant participant)
        {
            if (!CheckService.CheckIsParticipantValid(_context, participant, out string _errMessage))
                return new ObjectResult(_errMessage) { StatusCode = StatusCodes.Status500InternalServerError };

            Participant participantInDB = _context.Participants
               .FirstOrDefault(a => (a.Surname == participant.Surname && a.Email == participant.Email)
                                    || a.Id == participant.Id);


            if (participantInDB == null)
            {
                _context.Participants.Add(participant);
            }
            else
            {
                // Если участник индентифицирован - обновляем поля
                participantInDB.BirthDay = participant.BirthDay;
                participantInDB.Name = participant.Name;
                participantInDB.Patronymic = participant.Patronymic;
                participantInDB.Phone = participant.Phone;
                participantInDB.Surname = participant.Surname;
                participantInDB.Email = participant.Email;

                participant = participantInDB;
            }

            await _context.SaveChangesAsync();

            return participant;
        }

        // DELETE: api/Participants/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<Participant>> DeleteParticipant(int id)
        {
            Participant participant = await _context.Participants.FindAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            return participant;
        }

        private bool ParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.Id == id);
        }
    }
}

using MeetingManageWebPortal.Helpers;
using MeetingService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MeetingManageWebPortal.Controllers
{
    /// <summary>
    /// Контроллер представления "Участники встречи"
    /// </summary>
    public class MeetParticipantGroupController : Controller
    {
        /// <summary>
        /// Сформировать отчет участников встречи
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            return View(await WebApiHelper.ExecuteWebApiRequest<List<MeetParticipantGroup>>($"api/Meets/MeetsWithParticipantsList", WebApiHelper.HttpMethod.GET));
        }
    }
}
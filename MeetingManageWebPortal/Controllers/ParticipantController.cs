using MeetingManageWebPortal.Helpers;
using MeetingService.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MeetingManageWebPortal.Controllers
{
    /// <summary>
    /// Контроллер представления "Участники встречи"
    /// </summary>
    public class ParticipantController : Controller
    {
        public async Task<ActionResult> Index()
        {
            return View(await WebApiHelper.ExecuteWebApiRequest<List<Participant>>("api/Participants", WebApiHelper.HttpMethod.GET));
        }

        #region Edit Methods
        public async Task<ActionResult> Edit(int id = 0)
        {
            Participant resultRequest = await WebApiHelper.ExecuteWebApiRequest<Participant>($"api/Participants/{id}", WebApiHelper.HttpMethod.GET);
            return View(resultRequest);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Participant participant)
        {
            string participantJson = JsonConvert.SerializeObject(participant);
            StringContent bodyPostData = new StringContent(participantJson, Encoding.UTF8, WebApiHelper.BaseMediaType);
            Participant resultRequest = await WebApiHelper.ExecuteWebApiRequest<Participant>($"api/Participants", WebApiHelper.HttpMethod.POST, bodyPostData);
            ViewData["message"] = "Participant update completed successfully!";

            return View(resultRequest);
        }
        #endregion

        #region Create Methods
        public async Task<ActionResult> Create()
        {
            List<Meet> resultRequest = await WebApiHelper.ExecuteWebApiRequest<List<Meet>>("api/Meets", WebApiHelper.HttpMethod.GET);
            SelectList meets = new SelectList(resultRequest, "Id", "Title");
            ViewBag.Meets = meets;
 
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Participant participant)
        {
            try
            {
                /// Сбор данных Встреча-Участник
                Dictionary<string, object> participantInMeetDict = new Dictionary<string, object>();
                participantInMeetDict.Add("participant", participant);
                participantInMeetDict.Add("meet", new Meet(){Id= participant.CurrentMeet });

                /// Конвертация в Json-тело запроса
                string jsonParticipantInMeet = JsonConvert.SerializeObject(participantInMeetDict);
                StringContent bodyPostData = new StringContent(jsonParticipantInMeet, Encoding.UTF8, WebApiHelper.BaseMediaType);
                string resultRequest = await WebApiHelper.ExecuteWebApiRequest<string>($"api/Meets/AddParticipantInMeet", WebApiHelper.HttpMethod.POST, bodyPostData);

                ViewData["message"] = resultRequest;
                return await Create();
            }
            catch(Exception ex)
            {
                ViewData["message"] = ex.Message;

                return await Create();
            }
        }
        #endregion

        #region Delete Methods
        public async Task<ActionResult> Delete(int? id)
        {
            Participant resultRequest = await WebApiHelper.ExecuteWebApiRequest<Participant>($"api/Participants/{id}", WebApiHelper.HttpMethod.GET);
            return View(resultRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await WebApiHelper.ExecuteWebApiRequest<Participant>($"api/Participants/{id}", WebApiHelper.HttpMethod.DELETE);
            return RedirectToAction("Index");
        }
        #endregion
    }
}
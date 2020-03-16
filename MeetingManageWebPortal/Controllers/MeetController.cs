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
    /// Контроллер представления "Встречи"
    /// </summary>
    public class MeetController : Controller
    {
        
        public async Task<ActionResult> Index()
        {
            return View(await WebApiHelper.ExecuteWebApiRequest<List<Meet>>("api/Meets", WebApiHelper.HttpMethod.GET));
        }

        #region Create-Methods
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Meet meet)
        {
            try
            {
                /// Конвертация в Json-тело запроса
                string jsonMeet = JsonConvert.SerializeObject(meet);
                StringContent bodyPostData = new StringContent(jsonMeet, Encoding.UTF8, WebApiHelper.BaseMediaType);
                string resultRequest = await WebApiHelper.ExecuteWebApiRequest<string>($"api/Meets", WebApiHelper.HttpMethod.POST, bodyPostData);

                ViewData["message"] = resultRequest;
                return Create();
            }
            catch (Exception ex)
            {
                ViewData["message"] = ex.Message;
                return Create();
            }
        }
        #endregion

        #region Edit Methods
        public async Task<ActionResult> Edit(int id)
        {
            Meet resultRequest = await WebApiHelper.ExecuteWebApiRequest<Meet>($"api/Meets/{id}", WebApiHelper.HttpMethod.GET);
            return View(resultRequest);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Meet meet)
        {
            try
            {
                string meetJson = JsonConvert.SerializeObject(meet);
                StringContent bodyPostData = new StringContent(meetJson, Encoding.UTF8, WebApiHelper.BaseMediaType);
                string resultRequest = await WebApiHelper.ExecuteWebApiRequest<string>($"api/Meets", WebApiHelper.HttpMethod.POST, bodyPostData);
                ViewData["message"] = resultRequest;

                return View();
            }
            catch
            {
                return View();
            }
        }
        #endregion

        #region Delete-Methods
        public async Task<ActionResult> Delete(int id)
        {
            Meet resultRequest = await WebApiHelper.ExecuteWebApiRequest<Meet>($"api/Meets/{id}", WebApiHelper.HttpMethod.GET);
            return View(resultRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await WebApiHelper.ExecuteWebApiRequest<Meet>($"api/Meets/{id}", WebApiHelper.HttpMethod.DELETE);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion
    }
}

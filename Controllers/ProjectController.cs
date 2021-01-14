using Base.Models;
using Base.Services;
using BaseWeb.Services;
using DbAdm.Services;
using Microsoft.AspNetCore.Mvc;

namespace DbAdm.Controllers
{
    //[Permission(Prog = _Prog.Course)]
    public class ProjectController : Controller
    {
        #region Read View
        public ActionResult Read()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetPage(DtDto dt)
        {
            return Content(new ProjectRead().GetPage(dt).ToString(), _Web.AppJson);
        }

        [HttpPost]
        public JsonResult SetStatus(string key, bool status)
        {
            return Json(_Db.SetRowStatus("dbo.Project", "Id", key, status));
        }

        [HttpPost]
        public JsonResult Delete(string key)
        {
            return Json(new ProjectEdit().Delete(key));
        }
        #endregion

        #region Edit View
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public ContentResult GetJson(string key)
        {
            return Content(new ProjectEdit().GetJson(key).ToString(), _Web.AppJson);
        }

        [HttpPost]
        public JsonResult SaveCreate(string json)
        {
            return Json(new ProjectEdit().SaveCreate(_Json.StrToJson(json)));
        }
        public JsonResult SaveUpdate(string key, string json)
        {
            return Json(new ProjectEdit().SaveUpdate(key, _Json.StrToJson(json)));
        }
        #endregion

        //id: project Id
        public JsonResult Import(string id)
        {
            return Json(new ProjectService().Import(id));
        }

        //id: project Id
        public void GenWord(string id)
        {
            new GenDocuService().Run(id);
        }

    }//class
}
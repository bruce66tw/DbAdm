using Base.Models;
using Base.Services;
using BaseWeb.Services;
using DbAdm.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DbAdm.Controllers
{
    //[Permission(Prog = _Prog.Course)]
    public class ColumnController : Controller
    {
        //read
        public ActionResult Read()
        {
            //Ū���M�����(�U�Ԧ�)�ӷ����
            ViewBag.Projects = _Code.GetProjects();
            return View();
        }

        [HttpPost]
        public ContentResult GetPage(DtDto dt)
        {
            return Content(new ColumnRead().GetPage(dt).ToString(), _Web.AppJson, Encoding.UTF8);
        }

        public JsonResult SetStatus(string key, bool status)
        {
            return Json(_Db.SetRowStatus("dbo.[Column]", "Id", key, status));
        }

        //edit
        public ActionResult Edit()
        {
            return View();
        }

        public ContentResult GetJson(string key)
        {
            return Content(new ColumnEdit().GetJson(key).ToString(), _Web.AppJson, Encoding.UTF8);
        }

        public JsonResult SaveCreate(string json)
        {
            return Json(new ColumnEdit().SaveCreate(_Json.StrToJson(json)));
        }

        public JsonResult SaveUpdate(string key, string json)
        {
            return Json(new ColumnEdit().SaveUpdate(key, _Json.StrToJson(json)));
        }

        public JsonResult Delete(string key)
        {
            return Json(new ColumnEdit().Delete(key));
        }

    }//class
}
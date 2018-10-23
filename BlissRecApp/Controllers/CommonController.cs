using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlissBusiness.Models;
using BlissBusiness;

namespace BlissRecApp.Controllers
{
    public class CommonController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        
     
        public ActionResult CheckConnectivity()
        {
            Business business = new Business();
            int result = 0;

            bool connectivity = business.CheckConnectivity();
            if (connectivity == true)
                result = 1;
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }


    }
}
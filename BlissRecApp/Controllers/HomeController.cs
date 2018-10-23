using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlissBusiness;
using BlissRecApp.ViewModels;

namespace BlissRecApp.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index(string page, string navigate)
        {
            Business business = new Business();

            business.CheckConnectivity();

            QuestionModel model = new QuestionModel();

            if (!string.IsNullOrEmpty(page) && !string.IsNullOrEmpty(navigate))
            {
                BuildListQuestionModel(model, page, navigate, null);
            }
            else 
            {
                BuildListQuestionModel(model, null, null, null);
            }

            return View("QuestionList", model);
        }

        public ActionResult Navigate(string page, string navigate)
        {
            Business business = new Business();

            business.CheckConnectivity();

            return Index(page, navigate);
        }

        public void BuildListQuestionModel(QuestionModel model, string page, string navigate, string search)
        {

            Business business = new Business();


            int getPage = 1;


            if (!string.IsNullOrEmpty(page) && navigate.Equals("Next"))
            {
                getPage = int.Parse(page) + 1;
            }
            else if (!string.IsNullOrEmpty(page) && navigate.Equals("Back"))
            {
                getPage = int.Parse(page) - 1;
            }

            if (string.IsNullOrEmpty(search))
            {
                model.questionSearchResult = business.GetQuestion(getPage);
            }
            else
            {
                //get all records
                model.questionSearchResult = business.GetQuestion(getPage, search);
            }


            model.totalPages = business.GetTotalPages(business.totalResult, Constants.RESULTPERPAGE);
            model.page = getPage;
        }

        public ActionResult Search(string page, string navigate, string search) {

            if (string.IsNullOrEmpty(search))
            {
                search = " ";
            }

            Business business = new Business();
            QuestionModel model = new QuestionModel();
            business.CheckConnectivity();
            int getPage = 1;
            if (!string.IsNullOrEmpty(page) && navigate.Equals("Next"))
            {
                getPage = int.Parse(page) + 1;
            }
            else if (!string.IsNullOrEmpty(page) && navigate.Equals("Back"))
            {
                getPage = int.Parse(page) - 1;
            }

            model.questionSearchResult = business.GetQuestion(getPage, search);


            model.totalPages = business.GetTotalPages(business.totalResult, Constants.RESULTPERPAGE);
            model.page = getPage;
            model.search = search;



            return PartialView("_SearchResult", model);
        }

        public ActionResult NavigateSearch(string page, string navigate, string search)
        {
            Business business = new Business();

            business.CheckConnectivity();

            return Search(page, navigate, search);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlissBusiness;
using BlissRecApp.ViewModels;
using BlissBusiness.Models;
//using RazorEngine.Templating;
using System.Net.Mail;
using System.Configuration;


namespace BlissRecApp.Controllers
{
    public class QuestionController : Controller
    {

        public ActionResult ShowDetails(string id) {

            QuestionModel questionModel = new QuestionModel();

            Business business = new Business();

            Question question = business.GetQuestionByID(int.Parse(id));


            questionModel.description = question.Description;
            questionModel.ID = question.ID.ToString();
            questionModel.img_url = question.Image_Url;
            questionModel.thumb_url = question.Thumb_Url;
            questionModel.choices = question.Choices;
            

            return View("QuestionDetails", questionModel);
        }

        public ActionResult Vote(string id)
        {
            Business business = new Business();
            business.CheckConnectivity();
         

            int Result = business.Vote(int.Parse(id));
            return Json(new { result = Result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateUpdateQuestion(string id) {

            QuestionModel questionModel = new QuestionModel();
            if (!string.IsNullOrEmpty(id))
            {
                Business business = new Business();
                Question question = business.GetQuestionByID(int.Parse(id));
                questionModel.description = question.Description;
                questionModel.ID = question.ID.ToString();
                questionModel.img_url = question.Image_Url;
                questionModel.thumb_url = question.Thumb_Url;
                questionModel.choices = question.Choices;

            }

            return View("CreateUpdateQuestion", questionModel);
        }

        public ActionResult SaveRecord(string id, string description, string img_url, string thumb_url, string[] choices)
        {
            Business business = new Business();
            business.CheckConnectivity();


            if (string.IsNullOrEmpty(id))
            {
                Question question = BuildQuestion(description, img_url, thumb_url, choices);

                int result = business.CreateNewRecord(question);

                if (result >= 1)
                    return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                Question question = BuildQuestion(description, img_url, thumb_url, choices);
                question.ID = int.Parse(id);

                int result = business.UpdateRecord(question);

                if (result >= 1)
                    return Json(new { data = 1 }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { data = 0 }, JsonRequestBehavior.AllowGet);
        }

        public Question BuildQuestion(string description, string img_url, string thumb_url, string[] choices)
        {


            List<Choice> choicesList = new List<Choice>();

            for (int i = 0; i < choices.Count(); i++)
            {
                if (!string.IsNullOrEmpty(choices[i]))
                {
                    Choice choice = new Choice();
                    choice.Description = choices[i];
                    choicesList.Add(choice);
                }
            }

            Question question = new Question();
            question.Choices = choicesList;
            question.Description = description;
            question.Image_Url = img_url;
            question.Thumb_Url = thumb_url;

            return question;

        }



        public ActionResult Share(string page,  string infoType,  string search, string id) {

            QuestionModel questionModel = new QuestionModel();

            int getPage = 0 ;
            if (!string.IsNullOrEmpty(page)) {
                getPage = int.Parse(page);
            }
         
              


            if (string.IsNullOrEmpty(infoType))
                infoType = "";



            if (string.IsNullOrEmpty(search))
                search = "";


            if (string.IsNullOrEmpty(id))
                id = "";



            questionModel.page = getPage;
            questionModel.type = infoType;
            questionModel.search = search;
            questionModel.ID = id;


            return View("ShareInfo", questionModel);
        }

        public ActionResult RenderSearchScreen(string page, string infoType, string search, string id) {
            
            QuestionModel questionModel = BuildModelShare(page, infoType, search, id);
            return PartialView("_ShareDetailsScreen", questionModel);
        }

        public ActionResult RenderDetailsScreen(string page, string infoType, string search, string id)
        {

            QuestionModel questionModel = BuildModelShare(page, infoType, search, id);
            return PartialView("_ShareSearchScreen", questionModel);
        }

        public QuestionModel BuildModelShare(string page, string infoType, string search, string id) {

            QuestionModel questionModel = new QuestionModel();
            Business business = new Business();

            if (infoType.Equals("details"))
            {
                Question question = new Question();
                question = business.GetQuestionByID(int.Parse(id));

                questionModel.description = question.Description;
                questionModel.ID = question.ID.ToString();
                questionModel.img_url = question.Image_Url;
                questionModel.thumb_url = question.Thumb_Url;
                questionModel.choices = question.Choices;
                List<Question> questions = new List<Question>();
                questionModel.questionSearchResult = questions;
                questionModel.totalPages = 0;
                questionModel.page = 0;
                questionModel.search = "";
            }
            else if (infoType.Equals("search"))
            {
                if (string.IsNullOrEmpty(search))
                {
                    search = " ";
                }

                business.CheckConnectivity();


                questionModel.questionSearchResult = business.GetQuestion(int.Parse(page), search);

                questionModel.totalPages = business.GetTotalPages(business.totalResult, Constants.RESULTPERPAGE);
                questionModel.page = int.Parse(page);
                questionModel.search = search;

                questionModel.description = "";
                questionModel.ID = id;
                questionModel.img_url = "";
                questionModel.thumb_url = "";
                List<Choice> choices = new List<Choice>();
                questionModel.choices = choices;
            }

            return questionModel;
        }

        public ActionResult SendEmail(string page, string infoType, string search, string id, string address) {

            QuestionModel email = BuildModelShare(page, infoType, search, id);
            bool send = false;

            if (infoType.Equals("details"))
            {
               // send = SendEmail(email, address, "DetailsTemplate.cshtml");

            }
            else if (infoType.Equals("search"))
            {
                //send = SendEmail(email, address, "TemplateSearch.cshtml");

            }

            return Json(new { data = send });
        }

        /*
        public static string CreateEmailBody(object email, string template)
        {


            string path = System.Web.HttpContext.Current.Server.MapPath("/Views/Templates/" + template);
            string body = RazorEngine.Engine.Razor.RunCompile(System.IO.File.ReadAllText(path), template, model: email);

            return body;
        }

        public static bool SendEmail(object email, string  address, string template, string body = null, string subject = null)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                //string Title = "Password";

                string templates = template;

                if (string.IsNullOrEmpty(body))
                    body = CreateEmailBody(email, template);

                mailMessage.From = new MailAddress("gabrielpereira479@hotmail.com");

                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                if (string.IsNullOrEmpty(subject))
                    mailMessage.Subject = "BlissAppGabrielPereira";
                else
                    mailMessage.Subject = "BlissAppGabrielPereira";

               
                mailMessage.To.Add(new MailAddress(address));



                SmtpClient smtp = new SmtpClient();

                smtp.Port = 25;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "Smtp.Server.Gabriel";
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;

                smtp.Credentials = new System.Net.NetworkCredential("GabrielBlissAppSMTPServer", "BlissApp");
                smtp.EnableSsl = true;

                smtp.Send(mailMessage);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        */



    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlissBusiness.Models;
using Repository;
using DBBliss;




namespace BlissBusiness
{
    public class Business
    {
        UnitOfWork unitOfWork = null;
        public int totalResult = 0;

        public Business()
        {
            if (unitOfWork == null)
                unitOfWork = new UnitOfWork();
        }

        public bool CheckConnectivity()
        {
            try
            {
                Uri uri = new Uri("http://www.sapo.pt");
                System.Net.NetworkInformation.Ping pingSender = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingReply reply = pingSender.Send(uri.Host);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                return false;
            }

        }

        public List<Question> GetQuestion(int page)
        {
            page--;



            List<DT_QUESTION> queryQuestionResult = unitOfWork.QuestionRepository.GetAll().ToList();

            totalResult = queryQuestionResult.Count;


            return PrepareResults(queryQuestionResult).Skip(Constants.RESULTPERPAGE * page).Take(Constants.RESULTPERPAGE).ToList();
        }

        public List<Question> GetQuestion(int page, string search)
        {
            page--;

            if (string.IsNullOrEmpty(search))
            {
                return null;
            }

            List<DT_QUESTION> searchQuestionResults = unitOfWork.QuestionRepository.GetAll().Where(s => s.QUESTION_DESCRIPTION.Contains(search)).ToList();
            List<DT_CHOICE> searchChoiceResults = unitOfWork.ChoiceRepository.GetAll().Where(s => s.CHOICE_DESCRIPTION.Contains(search)).ToList();



            foreach (var choice in searchChoiceResults)
            {
                DT_QUESTION q = unitOfWork.QuestionRepository.GetAll().Where(s => s.QUESTION_ID == choice.CHOICE_QUESTION_ID).FirstOrDefault();
                if (!searchQuestionResults.Contains(q))
                    searchQuestionResults.Add(q);
            }

            totalResult = searchQuestionResults.Count;


            return PrepareResults(searchQuestionResults).Skip(Constants.RESULTPERPAGE * page).Take(Constants.RESULTPERPAGE).ToList();
            //return PrepareResults(searchQuestionResults).Skip(size * page).Take(size).ToList();
        }

        public int GetTotalPages(int totalRows, int size)
        {
            return (int)Math.Ceiling(totalRows / (double)size);
        }

        public List<Question> PrepareResults(List<DT_QUESTION> dT_QUESTIONs)
        {

            List<Question> questions = new List<Question>();

            foreach (var rowQuestion in dT_QUESTIONs)
            {


                List<DT_CHOICE> queryChoiceResult = unitOfWork.ChoiceRepository.GetAllWithCondition(c => c.CHOICE_QUESTION_ID == rowQuestion.QUESTION_ID).ToList();
                Question question = new Question();
                List<Choice> choices = new List<Choice>();

                foreach (var rowChoices in queryChoiceResult)
                {
                    Choice choice = new Choice();
                    choice.ID = rowChoices.CHOICE_ID;
                    choice.Description = rowChoices.CHOICE_DESCRIPTION;
                    choice.ID_Question = rowChoices.CHOICE_QUESTION_ID;
                    choice.Votes = rowChoices.CHOICE_VOTES;
                    choices.Add(choice);
                }

                question.ID = rowQuestion.QUESTION_ID;
                question.Description = rowQuestion.QUESTION_DESCRIPTION;
                question.Image_Url = rowQuestion.QUESTION_IMG_URL;
                question.Thumb_Url = rowQuestion.QUESTION_THUMB_URL;
                question.Published = rowQuestion.QUESTION_PUBLISHED;
                question.Choices = choices;
                questions.Add(question);
            }

            return questions;
        }

        public Question GetQuestionByID(int id)
        {

            DT_QUESTION dT_QUESTION = unitOfWork.QuestionRepository.GetByID(id);
            Question question = new Question();
            List<Choice> choices = new List<Choice>();

            foreach (var rowChoices in dT_QUESTION.DT_CHOICE)
            {
                Choice choice = new Choice();
                choice.ID = rowChoices.CHOICE_ID;
                choice.Description = rowChoices.CHOICE_DESCRIPTION;
                choice.ID_Question = rowChoices.CHOICE_QUESTION_ID;
                choice.Votes = rowChoices.CHOICE_VOTES;
                choices.Add(choice);
            }

            question.ID = dT_QUESTION.QUESTION_ID;
            question.Description = dT_QUESTION.QUESTION_DESCRIPTION;
            question.Image_Url = dT_QUESTION.QUESTION_IMG_URL;
            question.Thumb_Url = dT_QUESTION.QUESTION_THUMB_URL;
            question.Published = dT_QUESTION.QUESTION_PUBLISHED;
            question.Choices = choices;

            return question;
        }

        public int Vote(int id)
        {

            try
            {
                DT_CHOICE dT_CHOICE = unitOfWork.ChoiceRepository.GetByID(id);
                dT_CHOICE.CHOICE_VOTES++;
                int result = unitOfWork.Save();
                return result;
            }
            catch (Exception e)
            {

                throw e.InnerException;
            }

        }

        public int CreateNewRecord(Question question)
        {

            try
            {
                DT_QUESTION dT_QUESTION = new DT_QUESTION();

                dT_QUESTION.QUESTION_DESCRIPTION = question.Description;
                dT_QUESTION.QUESTION_IMG_URL = question.Image_Url;
                dT_QUESTION.QUESTION_THUMB_URL = question.Thumb_Url;
                dT_QUESTION.QUESTION_PUBLISHED = DateTime.Now;

                unitOfWork.QuestionRepository.Insert(dT_QUESTION);
                int result = unitOfWork.Save();

                if (result != 1)
                {
                    Exception e = new Exception();
                    throw e.InnerException;
                }

                int newRecordID = unitOfWork.QuestionRepository.GetAll().Last().QUESTION_ID;

                foreach (var choice in question.Choices)
                {
                    DT_CHOICE dT_CHOICE = new DT_CHOICE();

                    dT_CHOICE.CHOICE_DESCRIPTION = choice.Description;
                    dT_CHOICE.CHOICE_QUESTION_ID = newRecordID;
                    dT_CHOICE.CHOICE_VOTES = 0;
                    unitOfWork.ChoiceRepository.Insert(dT_CHOICE);

                }


                return unitOfWork.Save();
            }
            catch (Exception e)
            {

                throw e.InnerException;
            }

        }

        public int UpdateRecord(Question question)
        {

            try
            {
                List<DT_CHOICE> listToRemove = unitOfWork.ChoiceRepository.GetAll().Where(c => c.CHOICE_QUESTION_ID == question.ID).ToList();
                foreach (var dT_CHOICE in listToRemove)
                {
                    unitOfWork.ChoiceRepository.Delete(dT_CHOICE);
                }

                unitOfWork.Save();

                DT_QUESTION dT_QUESTION = unitOfWork.QuestionRepository.GetByID(question.ID);

                dT_QUESTION.QUESTION_DESCRIPTION = question.Description;
                dT_QUESTION.QUESTION_IMG_URL = question.Image_Url;
                dT_QUESTION.QUESTION_THUMB_URL = question.Thumb_Url;

                foreach (var choice in question.Choices)
                {
                    DT_CHOICE dT_CHOICE = new DT_CHOICE();
                    dT_CHOICE.CHOICE_DESCRIPTION = choice.Description;
                    dT_CHOICE.CHOICE_QUESTION_ID = dT_QUESTION.QUESTION_ID;
                    dT_CHOICE.CHOICE_VOTES = 0;
                    unitOfWork.ChoiceRepository.Insert(dT_CHOICE);
                }

                return unitOfWork.Save();
            }
            catch (Exception e)
            {

                throw e.InnerException;
            }

        }

 

    }
}

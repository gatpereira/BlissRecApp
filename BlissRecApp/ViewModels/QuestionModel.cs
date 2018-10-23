using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BlissBusiness.Models;

namespace BlissRecApp.ViewModels
{
    public class QuestionModel
    {
        public List<Question> questionSearchResult { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public string search { get; set; }
        public string description { get; set; }
        public string img_url { get; set; }
        public string thumb_url { get; set; }
        public string choice { get; set; }
        public string ID { get; set; }
        public List<Choice> choices { get; set; }
        public string type { get; set; }

    }
}
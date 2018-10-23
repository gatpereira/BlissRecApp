using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlissBusiness.Models
{
    public class Question
    {
        public int ID;
        public string Description;
        public string Image_Url;
        public string Thumb_Url;
        public DateTime Published;
        public List<Choice> Choices;
    }
}

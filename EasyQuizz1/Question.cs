using System.Collections.Generic;

namespace EazyQuizz
{
    public class Question
    {
        public string text { get; set; } = "";
        public string imagePath { get; set; } = "";
        public Domain domain { get; set; }
        public Difficulty difficulty { get; set; }
        public QuestionType type { get; set; }

        public List<Answer> answers { get; set; } = new List<Answer>();
    }
}
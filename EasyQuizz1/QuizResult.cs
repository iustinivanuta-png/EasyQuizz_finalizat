namespace EazyQuizz
{
    public class QuizResult
    {
        public string studentName { get; set; } = "";
        public int score { get; set; }
        public int total { get; set; }

        public override string ToString()
        {
            return studentName + " - Scor: " + score + "/" + total;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace EazyQuizz
{
    public class QuizManager
    {
        public List<Question> CurrentQuiz { get; private set; } = new List<Question>();
        public int CurrentIndex { get; private set; }
        public int Score { get; private set; }

        public void StartQuiz(List<Question> allQuestions, Domain domain)
        {
            CurrentQuiz = allQuestions
                .Where(q => q.domain == domain)
                .ToList();

            CurrentIndex = 0;
            Score = 0;
        }

        public Question? GetCurrentQuestion()
        {
            if (CurrentIndex >= CurrentQuiz.Count)
                return null;

            return CurrentQuiz[CurrentIndex];
        }

        public void AnswerQuestion(int selectedIndex)
        {
            if (CurrentIndex >= CurrentQuiz.Count)
                return;

            if (CurrentQuiz[CurrentIndex].answers[selectedIndex].correct)
                Score++;

            CurrentIndex++;
        }
    }
}
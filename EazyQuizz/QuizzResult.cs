using System;

namespace EazyQuizz
{
    class QuizResult
    {
        public int score;
        public int total;

        public void Show()
        {
            Console.WriteLine("Scor final: " + score + " din " + total);
        }
    }
}
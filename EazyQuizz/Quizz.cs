using System;
using System.Collections.Generic;

namespace EazyQuizz
{
    class Quiz
    {
        public List<Question> questions = new List<Question>();

        public void Start(string studentName)
        {
            int scor = 0;

            foreach (var q in questions)
            {
                Console.Clear();
                Console.WriteLine("Student: " + studentName);
                Console.WriteLine("\n" + q.text);

                for (int i = 0; i < q.answers.Length; i++)
                {
                    Console.WriteLine((i + 1) + ". " + q.answers[i].text);
                }

                Console.Write("Raspuns: ");
                int r = int.Parse(Console.ReadLine());

                if (q.answers[r - 1].correct)
                {
                    Console.WriteLine("Corect!");
                    scor++;
                }
                else
                {
                    Console.WriteLine("Gresit!");
                }

                Console.ReadKey();
            }

            QuizResult result = new QuizResult();
            result.score = scor;
            result.total = questions.Count;

            Console.Clear();
            Console.WriteLine("Quiz terminat!");
            result.Show();

            Student.SaveScore(studentName, scor, questions.Count);
        }
    }
}
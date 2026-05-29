using System;

namespace EazyQuizz
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== EazyQuizz ===\n");
                Console.WriteLine("1. Student nou");
                Console.WriteLine("2. Student existent");
                Console.WriteLine("3. Conectare + Start Quiz");
                Console.WriteLine("4. Afisare scoruri");
                Console.WriteLine("5. Iesire");

                Console.Write("\nAlege optiune: ");
                string c = Console.ReadLine();

                if (c == "1")
                {
                    bool ok = Student.Register();

                    if (ok)
                    {
                        string nume = Student.LoginAfterRegister();
                        if (nume != null)
                        {
                            RunQuiz(nume);
                        }
                    }

                    Pause();
                }
                else if (c == "2")
                {
                    Student.ShowLoginOnly();
                    Pause();
                }
                else if (c == "3")
                {
                    string nume = Student.Login();
                    if (nume != null)
                    {
                        RunQuiz(nume);
                    }

                    Pause();
                }
                else if (c == "4")
                {
                    Student.ShowScores();
                    Pause();
                }
                else if (c == "5")
                {
                    Console.WriteLine("La revedere!");
                    break;
                }
                else
                {
                    Console.WriteLine("Optiune invalida!");
                    Pause();
                }
            }
        }

        static void RunQuiz(string nume)
        {
            Quiz quiz = new Quiz();

            Question q1 = new Question();
            q1.text = "Capitala Frantei?";
            q1.domain = Domain.Geografie;
            q1.difficulty = Difficulty.Usoara;
            q1.imagePath = "";
            q1.answers = new Answer[]
            {
                new Answer { text = "Madrid", correct = false },
                new Answer { text = "Paris", correct = true },
                new Answer { text = "Roma", correct = false },
                new Answer { text = "Berlin", correct = false }
            };

            Question q2 = new Question();
            q2.text = "Cate continente exista?";
            q2.domain = Domain.Geografie;
            q2.difficulty = Difficulty.Usoara;
            q2.imagePath = "";
            q2.answers = new Answer[]
            {
                new Answer { text = "5", correct = false },
                new Answer { text = "6", correct = false },
                new Answer { text = "7", correct = true },
                new Answer { text = "8", correct = false }
            };

            Question q3 = new Question();
            q3.text = "Cine a fost Alexandru Ioan Cuza?";
            q3.domain = Domain.Istorie;
            q3.difficulty = Difficulty.Medie;
            q3.imagePath = "";
            q3.answers = new Answer[]
            {
                new Answer { text = "Poet", correct = false },
                new Answer { text = "Domnitor", correct = true },
                new Answer { text = "Medic", correct = false },
                new Answer { text = "Profesor", correct = false }
            };

            quiz.questions.Add(q1);
            quiz.questions.Add(q2);
            quiz.questions.Add(q3);

            quiz.Start(nume);
        }

        static void Pause()
        {
            Console.WriteLine("\nApasa o tasta pentru meniu...");
            Console.ReadKey();
        }
    }
}
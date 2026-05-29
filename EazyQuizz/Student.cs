using System;
using System.IO;

namespace EazyQuizz
{
    class Student
    {
        static string filePath = "students.txt";
        static string scorePath = "scores.txt";

        public static bool Register()
        {
            Console.Write("Nume: ");
            string nume = Console.ReadLine();

            Console.Write("Parola: ");
            string parola = Console.ReadLine();

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    if (line.StartsWith(nume + "_"))
                    {
                        Console.WriteLine("Studentul exista deja!");
                        return false;
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                sw.WriteLine(nume + "_" + parola);
            }

            Console.WriteLine("Inregistrare reusita!");
            return true;
        }

        public static string Login()
        {
            Console.Write("Nume: ");
            string nume = Console.ReadLine();

            Console.Write("Parola: ");
            string parola = Console.ReadLine();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Nu exista studenti.");
                return null;
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (line == nume + "_" + parola)
                {
                    Console.WriteLine("Autentificare reusita!");
                    return nume;
                }
            }

            Console.WriteLine("Date gresite.");
            return null;
        }

        public static string LoginAfterRegister()
        {
            string[] lines = File.ReadAllLines(filePath);
            string last = lines[lines.Length - 1];
            return last.Split('_')[0];
        }

        public static void ShowLoginOnly()
        {
            string nume = Login();
            if (nume != null)
            {
                Console.WriteLine("Bine ai venit, " + nume);
            }
        }

        public static void SaveScore(string nume, int scor, int total)
        {
            using (StreamWriter sw = new StreamWriter(scorePath, true))
            {
                sw.WriteLine(nume + "_" + scor + "_" + total);
            }
        }

        public static void ShowScores()
        {
            Console.WriteLine("=== Scoruri ===\n");

            if (!File.Exists(scorePath))
            {
                Console.WriteLine("Nu exista scoruri.");
                return;
            }

            string[] lines = File.ReadAllLines(scorePath);

            foreach (string line in lines)
            {
                string[] p = line.Split('_');
                Console.WriteLine("Student: " + p[0] + " | Scor: " + p[1] + "/" + p[2]);
            }
        }
    }
}
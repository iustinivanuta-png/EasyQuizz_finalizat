using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EazyQuizz
{
    public static class DataStorage
    {
        static string questionsFile = "questions.txt";
        static string studentsFile = "students.txt";
        static string scoresFile = "scores.txt";

        public static List<Question> LoadQuestions()
        {
            List<Question> questions = new List<Question>();

            if (!File.Exists(questionsFile))
            {
                return questions;
            }

            foreach (string line in File.ReadAllLines(questionsFile))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] p = line.Split('|');

                if (p.Length >= 9)
                {
                    Question q = new Question();

                    q.text = p[0];
                    q.domain = Enum.Parse<Domain>(p[1]);
                    q.difficulty = Enum.Parse<Difficulty>(p[2]);
                    q.type = QuestionType.Text | QuestionType.RaspunsMultiplu;
                    q.imagePath = "";

                    for (int i = 5; i < p.Length; i++)
                    {
                        string[] a = p[i].Split(';');

                        if (a.Length == 2)
                        {
                            q.answers.Add(new Answer
                            {
                                text = a[0],
                                correct = bool.Parse(a[1])
                            });
                        }
                    }

                    questions.Add(q);
                }
            }

            return questions;
        }

        public static void SaveQuestions(List<Question> questions)
        {
            using StreamWriter sw = new StreamWriter(questionsFile);

            foreach (Question q in questions)
            {
                string line =
                    q.text + "|" +
                    q.domain + "|" +
                    q.difficulty + "|" +
                    q.type + "|" +
                    q.imagePath;

                foreach (Answer a in q.answers)
                {
                    line += "|" + a.text + ";" + a.correct;
                }

                sw.WriteLine(line);
            }
        }

        public static bool RegisterStudent(string name, string password)
        {
            name = name.Trim();
            password = password.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (File.Exists(studentsFile))
            {
                foreach (string line in File.ReadAllLines(studentsFile))
                {
                    string[] parts = line.Split('_');

                    if (parts.Length >= 1)
                    {
                        string existingName = parts[0].Trim();

                        if (existingName.Equals(
                            name,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
            }

            File.AppendAllText(
                studentsFile,
                name + "_" + password + Environment.NewLine
            );

            return true;
        }

        public static bool LoginStudent(string name, string password)
        {
            if (!File.Exists(studentsFile))
                return false;

            return File.ReadAllLines(studentsFile)
                .Any(line => line == name + "_" + password);
        }

        public static void DeleteStudent(string name)
        {
            if (!File.Exists(studentsFile))
                return;

            var lines = File.ReadAllLines(studentsFile)
                .Where(line => !line.StartsWith(name + "_"))
                .ToList();

            File.WriteAllLines(studentsFile, lines);
        }

        public static void SaveScore(string name, int score, int total)
        {
            File.AppendAllText(
                "scores.txt",
                $"{name}|{score}|{total}\n"
            );
        }

        public static List<QuizResult> LoadScores()
        {
            List<QuizResult> results = new List<QuizResult>();

            if (!File.Exists(scoresFile))
                return results;

            foreach (string line in File.ReadAllLines(scoresFile))
            {
                string[] p = line.Split('_');

                if (p.Length == 3)
                {
                    results.Add(new QuizResult
                    {
                        studentName = p[0],
                        score = int.Parse(p[1]),
                        total = int.Parse(p[2])
                    });
                }
            }

            return results;
        }
    }
}
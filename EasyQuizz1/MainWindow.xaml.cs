using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EazyQuizz
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Question> Questions { get; set; }
        public Array DomainItems { get; set; }
        public Array DifficultyItems { get; set; }

        private string loggedStudent = "";
        private QuizManager quizManager = new QuizManager();

        private List<UserScore> allScores = new List<UserScore>();

        public MainWindow()
        {
            InitializeComponent();

            Questions = new ObservableCollection<Question>(DataStorage.LoadQuestions());
            DomainItems = Enum.GetValues(typeof(Domain));
            DifficultyItems = Enum.GetValues(typeof(Difficulty));

            DataContext = this;

            cbQuizDomain.SelectedIndex = 0;
            cbDomain.SelectedIndex = 0;
            cbDifficulty.SelectedIndex = 0;

            LoadScores();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string name = txtStudentName.Text.Trim();
            string pass = txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pass))
            {
                txtLoginStatus.Text = "Completeaza numele si parola.";
                return;
            }

            if (DataStorage.RegisterStudent(name, pass))
            {
                loggedStudent = name;
                txtLoginStatus.Text = "Student inregistrat cu succes.";
                txtLoggedUser.Text = "Utilizator: " + loggedStudent;
            }
            else
            {
                txtLoginStatus.Text = "Studentul exista deja.";
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string name = txtStudentName.Text.Trim();
            string pass = txtPassword.Password.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pass))
            {
                txtLoginStatus.Text = "Completeaza numele si parola.";
                return;
            }

            if (DataStorage.LoginStudent(name, pass))
            {
                loggedStudent = name;
                txtLoginStatus.Text = "Autentificare reusita.";
                txtLoggedUser.Text = "Utilizator: " + loggedStudent;
            }
            else
            {
                txtLoginStatus.Text = "Date gresite.";
            }
        }

        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (loggedStudent == "")
            {
                txtQuizStatus.Text = "Trebuie sa fii autentificat.";
                return;
            }

            Domain selectedDomain = (Domain)cbQuizDomain.SelectedItem;

            quizManager.StartQuiz(Questions.ToList(), selectedDomain);

            if (quizManager.CurrentQuiz.Count == 0)
            {
                txtQuizStatus.Text = "Nu exista intrebari pentru acest domeniu.";
                return;
            }

            btnAnswer.Visibility = Visibility.Visible;
            panelStartQuiz.Visibility = Visibility.Collapsed;
            panelQuestion.Visibility = Visibility.Visible;
            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            Question? q = quizManager.GetCurrentQuestion();

            if (q == null)
            {
                txtQuestion.Text = "Quiz terminat!";
                txtQuizStatus.Text = "Scor final: " + quizManager.Score + "/" + quizManager.CurrentQuiz.Count;

                rb1.Visibility = Visibility.Collapsed;
                rb2.Visibility = Visibility.Collapsed;
                rb3.Visibility = Visibility.Collapsed;
                rb4.Visibility = Visibility.Collapsed;
                btnAnswer.Visibility = Visibility.Collapsed;

                DataStorage.SaveScore(loggedStudent, quizManager.Score, quizManager.CurrentQuiz.Count);

                LoadScores();

                return;
            }

            txtQuestion.Text = q.text;
            txtQuizStatus.Text = "Intrebarea " + (quizManager.CurrentIndex + 1) + " din " + quizManager.CurrentQuiz.Count;

            rb1.Content = q.answers[0].text;
            rb2.Content = q.answers[1].text;
            rb3.Content = q.answers[2].text;
            rb4.Content = q.answers[3].text;

            rb1.IsChecked = false;
            rb2.IsChecked = false;
            rb3.IsChecked = false;
            rb4.IsChecked = false;

            rb1.Visibility = Visibility.Visible;
            rb2.Visibility = Visibility.Visible;
            rb3.Visibility = Visibility.Visible;
            rb4.Visibility = Visibility.Visible;
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            int selected = -1;

            if (rb1.IsChecked == true) selected = 0;
            if (rb2.IsChecked == true) selected = 1;
            if (rb3.IsChecked == true) selected = 2;
            if (rb4.IsChecked == true) selected = 3;

            if (selected == -1)
            {
                txtQuizStatus.Text = "Alege un raspuns.";
                return;
            }

            quizManager.AnswerQuestion(selected);
            ShowCurrentQuestion();
        }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (!IsQuestionFormValid())
                return;

            Question q = CreateQuestionFromForm();
            Questions.Add(q);
            lstQuestions.ItemsSource = Questions;
            ClearForm();
        }

        private void UpdateQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuestions.SelectedItem is Question q)
            {
                if (!IsQuestionFormValid())
                    return;

                q.text = txtQuestionAdmin.Text;
                q.domain = (Domain)cbDomain.SelectedItem;
                q.difficulty = (Difficulty)cbDifficulty.SelectedItem;
                q.type = QuestionType.Text | QuestionType.RaspunsMultiplu;
                q.imagePath = "";
                q.answers = GetAnswersFromForm();

                lstQuestions.Items.Refresh();
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (lstQuestions.SelectedItem is Question q)
            {
                Questions.Remove(q);
                lstQuestions.ItemsSource = Questions;
                ClearForm();
            }
        }

        private void SaveQuestions_Click(object sender, RoutedEventArgs e)
        {
            DataStorage.SaveQuestions(Questions.ToList());
            MessageBox.Show("Intrebarile au fost salvate.");
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string text = txtSearch.Text.ToLower();

            var results = Questions
                .Where(q => q.text.ToLower().Contains(text))
                .ToList();

            lstQuestions.ItemsSource = results;
        }

        private void ShowAllQuestions_Click(object sender, RoutedEventArgs e)
        {
            lstQuestions.ItemsSource = Questions;
            txtSearch.Text = "";
        }

        private void lstQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstQuestions.SelectedItem is Question q)
            {
                txtQuestionAdmin.Text = q.text;
                cbDomain.SelectedItem = q.domain;
                cbDifficulty.SelectedItem = q.difficulty;

                if (q.answers.Count >= 4)
                {
                    txtA1.Text = q.answers[0].text;
                    txtA2.Text = q.answers[1].text;
                    txtA3.Text = q.answers[2].text;
                    txtA4.Text = q.answers[3].text;

                    correct1.IsChecked = q.answers[0].correct;
                    correct2.IsChecked = q.answers[1].correct;
                    correct3.IsChecked = q.answers[2].correct;
                    correct4.IsChecked = q.answers[3].correct;
                }
            }
        }

        private Question CreateQuestionFromForm()
        {
            return new Question
            {
                text = txtQuestionAdmin.Text,
                domain = (Domain)cbDomain.SelectedItem,
                difficulty = (Difficulty)cbDifficulty.SelectedItem,
                imagePath = "",
                type = QuestionType.Text | QuestionType.RaspunsMultiplu,
                answers = GetAnswersFromForm()
            };
        }

        private List<Answer> GetAnswersFromForm()
        {
            return new List<Answer>
            {
                new Answer { text = txtA1.Text, correct = correct1.IsChecked == true },
                new Answer { text = txtA2.Text, correct = correct2.IsChecked == true },
                new Answer { text = txtA3.Text, correct = correct3.IsChecked == true },
                new Answer { text = txtA4.Text, correct = correct4.IsChecked == true }
            };
        }

        private bool IsQuestionFormValid()
        {
            if (string.IsNullOrWhiteSpace(txtQuestionAdmin.Text) ||
                string.IsNullOrWhiteSpace(txtA1.Text) ||
                string.IsNullOrWhiteSpace(txtA2.Text) ||
                string.IsNullOrWhiteSpace(txtA3.Text) ||
                string.IsNullOrWhiteSpace(txtA4.Text))
            {
                MessageBox.Show("Completeaza intrebarea si toate raspunsurile.");
                return false;
            }

            if (correct1.IsChecked != true &&
                correct2.IsChecked != true &&
                correct3.IsChecked != true &&
                correct4.IsChecked != true)
            {
                MessageBox.Show("Alege raspunsul corect.");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtQuestionAdmin.Text = "";

            txtA1.Text = "";
            txtA2.Text = "";
            txtA3.Text = "";
            txtA4.Text = "";

            correct1.IsChecked = false;
            correct2.IsChecked = false;
            correct3.IsChecked = false;
            correct4.IsChecked = false;
        }

        private void LoadScores_Click(object sender, RoutedEventArgs e)
        {
            LoadScores();
        }

        private void LoadScores()
        {
            lstScores.Items.Clear();
            allScores.Clear();

            if (!File.Exists("scores.txt"))
                return;

            string[] lines = File.ReadAllLines("scores.txt");

            var users = new Dictionary<string, UserScore>();

            foreach (string line in lines)
            {
                string[] parts = line.Split('|');

                if (parts.Length != 3)
                    continue;

                string name = parts[0];
                int correct = int.Parse(parts[1]);
                int total = int.Parse(parts[2]);

                if (!users.ContainsKey(name))
                {
                    users[name] = new UserScore
                    {
                        Name = name,
                        Correct = 0,
                        Total = 0,
                        Quizzes = 0
                    };
                }

                users[name].Correct += correct;
                users[name].Total += total;
                users[name].Quizzes++;
            }

            allScores = users.Values.ToList();

            DisplayScores(allScores);
        }

        private void DisplayScores(List<UserScore> scores)
        {
            lstScores.Items.Clear();

            foreach (UserScore user in scores)
            {
                Grid row = new Grid();

                row.ColumnDefinitions.Add(new ColumnDefinition());
                row.ColumnDefinitions.Add(new ColumnDefinition());
                row.ColumnDefinitions.Add(new ColumnDefinition());
                row.ColumnDefinitions.Add(new ColumnDefinition());

                TextBlock t1 = new TextBlock
                {
                    Text = user.Name,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock t2 = new TextBlock
                {
                    Text = $"{user.Correct}/{user.Total}",
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock t3 = new TextBlock
                {
                    Text = $"{user.Percent:F0}%",
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock t4 = new TextBlock
                {
                    Text = user.Quizzes.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                Grid.SetColumn(t1, 0);
                Grid.SetColumn(t2, 1);
                Grid.SetColumn(t3, 2);
                Grid.SetColumn(t4, 3);

                row.Children.Add(t1);
                row.Children.Add(t2);
                row.Children.Add(t3);
                row.Children.Add(t4);

                lstScores.Items.Add(row);
            }
        }

        private void txtSearchUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearchUser.Text.ToLower();

            var results = allScores
                .Where(u => u.Name.ToLower().Contains(search))
                .ToList();

            DisplayScores(results);
        }

        private void SortDesc_Click(object sender, RoutedEventArgs e)
        {
            var sorted = allScores
                .OrderByDescending(u => u.Percent)
                .ToList();

            DisplayScores(sorted);
        }

        private void SortAsc_Click(object sender, RoutedEventArgs e)
        {
            var sorted = allScores
                .OrderBy(u => u.Percent)
                .ToList();

            DisplayScores(sorted);
        }

        public class UserScore
        {
            public string Name { get; set; } = "";
            public int Correct { get; set; }
            public int Total { get; set; }
            public int Quizzes { get; set; }

            public double Percent
            {
                get
                {
                    if (Total == 0)
                        return 0;

                    return (double)Correct * 100 / Total;
                }
            }
        }
    }
}
namespace EazyQuizz
{
    enum Domain
    {
        Geografie,
        Istorie,
        Informatica,
        Biologie
    }

    enum Difficulty
    {
        Usoara,
        Medie,
        Grea
    }

    class Question
    {
        public string text;
        public string imagePath;
        public Domain domain;
        public Difficulty difficulty;
        public Answer[] answers;
    }
}
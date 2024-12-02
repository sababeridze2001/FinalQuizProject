using Newtonsoft.Json;
using QuizModels;
using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace QuizRepository
{

    public class QuizLogics
    {
        public readonly static string _filePathforusers = "C:\\Users\\User\\Desktop\\12.1\\QuizRepository\\Data\\UserData.json";
        public readonly static string _filepathforquiz = "C:\\Users\\User\\Desktop\\12.1\\QuizRepository\\Data\\QuizData.json";


        public static User Registration(string id, string name, string password)
        {
            List<User> users = LoadFromJson<User>(_filePathforusers);


            if (users.Any(u => u.Id == id))
            {

                throw new Exception("such id already exists!!!");
            }

            User user = new User
            {
                Id = id,
                Name = name,
                Password = password,
                Score = 0,
                Record = 0

            };
            users.Add(user);


            SaveToJson(users, _filePathforusers);



            return user;

        }

        //public static string SignIn(string id, string password)
        //{
        //    List<User> users = LoadFromJson<User>(_filePathforusers);
        //    if (users.Any(u => u.Id == id && u.Password == password))
        //    {
        //        return id;
        //    }

        //    return "null";
        //}


        public static string SignIn(string id, string password)
        {
            List<User> users = LoadFromJson<User>(_filePathforusers);

            
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                throw new Exception("You have entered Invalid ID!!!");
            }

           
            if (user.Password != password)
            {
                throw new Exception("You have entered Incorrect password!!!.");
            }

            return id; 
        }




        //public static List<T> LoadFromJson<T>(string filePath)
        //{
        //    string json = File.ReadAllText(filePath);
        //    var resultList = System.Text.Json.JsonSerializer.Deserialize<List<T>>(json);
        //    return resultList;

        //}

        //public static void SaveToJson<T>(List<T> data, string filePath)
        //{
        //    var options = new JsonSerializerOptions
        //    {
        //        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        //        WriteIndented = true,
        //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        //    };
        //    string json = System.Text.Json.JsonSerializer.Serialize(data, options);
        //    File.WriteAllText(filePath, json);
        //}

        public static List<T> LoadFromJson<T>(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception($"File not found at {filePath}");

            string json = File.ReadAllText(filePath);
           
            var resultList = System.Text.Json.JsonSerializer.Deserialize<List<T>>(json);
            return resultList;
        }


        public static void SaveToJson<T>(List<T> data, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = System.Text.Json.JsonSerializer.Serialize(data, options);

           
            File.WriteAllText(filePath, json);
        }


        public static List<string> FirstTen()
        {

            List<User> users = LoadFromJson<User>(_filePathforusers);

            List<string> topTenUserNames = users
                .OrderByDescending(user => user.Record)
                .Take(10)
                .Select(user => $"{user.Name} --- {user.Record}")
                .ToList();

            return topTenUserNames;
        }


        private static List<Quiz> _remainingQuizzes = null;

        //ამას არ ვიყენებ.
        public static Quiz PlayQuiz(string author)
        {
            if (_remainingQuizzes == null || _remainingQuizzes.Count == 0)
            {
                
                _remainingQuizzes = LoadFromJson<Quiz>(_filepathforquiz);
            }

            
            var availableQuizzes = _remainingQuizzes.Where(q => q.Author != author).ToList();

            
            if (availableQuizzes.Count == 0)
            {
                return null;
            }

            
            Random rand = new Random();
            Quiz randomQuiz = availableQuizzes[rand.Next(availableQuizzes.Count)];

            
            _remainingQuizzes.Remove(randomQuiz);

            randomQuiz.ShuffleAnswers();

            return randomQuiz;
        }

        //public static List<User> CountScores(Quiz quiz, string author, string answer, List<User> users)
        //{
        //    var currentUser = users.FirstOrDefault(u => u.Id == author);
        //    if (currentUser == null)
        //        throw new Exception("User not found.");

        //    Console.WriteLine($"User found: {currentUser.Name}, Score: {currentUser.Score}, Record: {currentUser.Record}"); // Debug log

        //    foreach (var question in quiz.Questions)
        //    {
        //        if (!question.ShuffledAnswers.Any())
        //            throw new Exception("Answers not shuffled. Call ShuffleAnswers before counting scores.");

        //        if (!int.TryParse(answer, out int answerIndex) || answerIndex < 1 || answerIndex > question.ShuffledAnswers.Count)
        //            throw new Exception("Invalid answer. Please enter a number corresponding to one of the options.");

        //        string selectedAnswer = question.ShuffledAnswers[answerIndex - 1];
        //        if (selectedAnswer == question.RightAnswer)
        //        {
        //            currentUser.Score += 20;  // Add 20 points for correct answer
        //        }
        //        else
        //        {
        //            currentUser.Score -= 20;  // Subtract 20 points for incorrect answer, or consider no penalty for wrong answers
        //        }
        //    }

        //    // Update record if necessary (only if the current score is greater than the previous record)
        //    if (currentUser.Score > currentUser.Record)
        //    {
        //        currentUser.Record = currentUser.Score;
        //    }

        //    Console.WriteLine($"After update: {currentUser.Name}, Score: {currentUser.Score}, Record: {currentUser.Record}");

        //    // Save updated user data to JSON file
        //    SaveToJson(users, _filePathforusers);  // Save updated user data to JSON

        //    return users;
        //}

        public static List<User> CountScores(Quiz.Question question, string userId, int answerIndex, List<User> users)
        {
            
            var currentUser = users.FirstOrDefault(u => u.Id == userId);
            if (currentUser == null)
                throw new Exception("User not found.");

            
            string selectedAnswer = question.ShuffledAnswers[answerIndex - 1];

           
            if (selectedAnswer == question.RightAnswer)
            {
                currentUser.Score += 20; 
            }
            else
            {
                currentUser.Score -= 20; 
            }

            return users;
        }


        public static Quiz CreateQuiz(string author, string name, List<Quiz.Question> questions)
        {
            var quizzes = LoadFromJson<Quiz>(_filepathforquiz);
            var quiz = new Quiz
            {
                Author = author,
                Name = name,
                Questions = questions
            };
            quizzes.Add(quiz);
            SaveToJson(quizzes, _filepathforquiz);
            return quiz;


        }

        //0 რეფერენსი
        public static void DeleteQuiz(string id, string name)
        {
            var quizList = LoadFromJson<Quiz>(_filepathforquiz);
            var quizToDelete = quizList.FirstOrDefault(q => q.Name == name);

            if (quizToDelete == null)
                throw new Exception("Quiz not found.");

            if (quizToDelete.Author != id)
                throw new Exception("You are not authorized to delete this quiz.");

            quizList.Remove(quizToDelete);
            SaveToJson(quizList, _filepathforquiz);
        }

        //0 რეფერენსი
        public static Quiz EditQuiz(string name, string userId, string updatedQuestion, string updatedRightAnswer,
            string answer2, string answer3, string answer4)
        {
            var quizList = LoadFromJson<Quiz>(_filepathforquiz);
            var updatableQuiz = quizList.FirstOrDefault(q => q.Name == name);

            if (updatableQuiz == null)
                throw new Exception("Quiz not found.");

            if (updatableQuiz.Author != userId)
                throw new Exception("You are not authorized to edit this quiz.");

            
            if (updatableQuiz.Questions.Count > 0)
            {
                updatableQuiz.Questions[0].Text = updatedQuestion;
                updatableQuiz.Questions[0].RightAnswer = updatedRightAnswer;
                updatableQuiz.Questions[0].Answer2 = answer2;
                updatableQuiz.Questions[0].Answer3 = answer3;
                updatableQuiz.Questions[0].Answer4 = answer4;
            }

            SaveToJson(quizList, _filepathforquiz);
            return updatableQuiz;

        }



    }

}

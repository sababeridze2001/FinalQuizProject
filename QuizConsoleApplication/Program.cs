using QuizModels;
using QuizRepository;



namespace QuizConsoleApplication
{
    internal class Program
    {



        static void Main(string[] args)
        {
            bool loop = true;

            Console.WriteLine("***** Welcome!!! You have entered in a QuizUpper  *****");

            while (loop)
            {
                try
                {

                    Console.WriteLine();
                    Console.WriteLine("For registration press 'R', To sign in press 'S' or 'E' to exit");
                    string inputText = Console.ReadLine().ToUpper();

                    if (inputText == "R")
                    {
                        Console.Write("Please enter your ID :");
                        string id = Console.ReadLine();
                        Console.Write("Please enter your Name :");
                        string name = Console.ReadLine();
                        Console.Write("Please enter Password :");
                        string password = Console.ReadLine();

                        User user = QuizLogics.Registration(id, name, password);

                        if (user.Id.Length != 0)
                        {

                            Console.WriteLine("Your registration was succesfull!!!");

                        }

                    }

                    else if (inputText == "S")
                    {
                        Console.Write("Please enter your ID :");
                        string id = Console.ReadLine();
                        Console.Write("Please enter Password :");
                        string password = Console.ReadLine();

                        string result = QuizLogics.SignIn(id, password);
                        if (result.Length != 0 && result != "null")
                        {
                            Console.WriteLine("You signed in successfully!!!");
                            Console.WriteLine("-------------------------------");
                            Console.WriteLine();
                            Console.WriteLine("Here is the list of top gamers:");

                            List<string> topUsers = QuizLogics.FirstTen();
                            foreach (string user in topUsers)
                            {
                                Console.WriteLine(user);
                            }

                            Console.WriteLine();

                            Console.WriteLine("We offer you to play quizz 'P' or create your own one 'C', to delete quiz presss 'D', to update 'U' ");
                            string inputText2 = Console.ReadLine().ToUpper();
                            List<User> updatedUsers = new List<User>();
                            List<User> users = QuizLogics.LoadFromJson<User>("C:\\Users\\User\\Desktop\\12.1\\QuizRepository\\Data\\UserData.json");


                            if (inputText2 == "P")
                            {
                                
                                Console.WriteLine("Enter the name of the quiz you want to play: ");
                                string quizName = Console.ReadLine();

                                
                                var allQuizzes = QuizLogics.LoadFromJson<Quiz>("C:\\Users\\User\\Desktop\\12.1\\QuizRepository\\Data\\QuizData.json");
                                Quiz selectedQuiz = allQuizzes.FirstOrDefault(q => q.Name == quizName && q.Author != result); 

                                if (selectedQuiz == null)
                                {
                                    
                                    throw new Exception("something went wrong. Either it's your own quiz or you have entered wrong credentials!!!");
                                }


                                var currentUser = users.FirstOrDefault(u => u.Id == result);
                                currentUser.Score = 0; 

                                
                                const int totalQuizTimeLimitInSeconds = 120;
                                long totalElapsedTime = 0;
                                selectedQuiz.ShuffleAnswers();
                                

                                foreach (var question in selectedQuiz.Questions)
                                {
                                    long startTime = DateTime.Now.Ticks;


                                    Console.WriteLine($"Question: {question.Text}");

                                    for (int j = 0; j < question.ShuffledAnswers.Count; j++)
                                    {
                                        Console.WriteLine($"{j + 1}. {question.ShuffledAnswers[j]}");
                                    }

                                    Console.Write("Enter the number of your answer: ");
                                    string userInput = Console.ReadLine();
                                    int answerIndex = int.Parse(userInput);

                                    long endTime = DateTime.Now.Ticks;
                                    long elapsedTime = (endTime - startTime) / TimeSpan.TicksPerSecond;
                                    totalElapsedTime += elapsedTime;

                                    if (totalElapsedTime <= totalQuizTimeLimitInSeconds)
                                    {
                                        
                                        users = QuizLogics.CountScores(question, result, answerIndex, users); 

                                       
                                        currentUser = users.FirstOrDefault(u => u.Id == result); 
                                    }
                                    else
                                    {
                                       
                                        
                                        throw new Exception("Time limit exceeded!!!");
                                    }

                                    Console.WriteLine($"Total time passed: {totalElapsedTime} seconds");
                                }

                                // Check if total time is within the limit before saving
                                if (totalElapsedTime <= totalQuizTimeLimitInSeconds)
                                {
                                    // Save updated users with updated scores and records
                                    try
                                    {
                                        if (currentUser.Score > currentUser.Record)
                                        {
                                            currentUser.Record = currentUser.Score;
                                        }
                                        QuizLogics.SaveToJson(users, QuizLogics._filePathforusers);  // Save the updated list of users
                                        Console.WriteLine("Quiz completed successfully within time.");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Failed to save results: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Quiz ended due to time limit.");
                                }

                            }


                            else if (inputText2 == "C")
                            {
                                if (string.IsNullOrEmpty(result))
                                {
                                    throw new Exception("Please sign in before creating a quiz.");
                                   
                                }

                                List<Quiz.Question> questions = new List<Quiz.Question>();
                                Console.WriteLine("Enter name of quiz: ");
                                string name = Console.ReadLine();


                                for (int i = 0; i < 5; i++)
                                {

                                    Console.WriteLine($"Enter question{i + 1}: ");
                                    string questionText = Console.ReadLine();

                                    Console.WriteLine("Enter the correct answer (answer1): ");
                                    string rightAnswer = Console.ReadLine();

                                    Console.WriteLine("Enter answer2: ");
                                    string answer2 = Console.ReadLine();

                                    Console.WriteLine("Enter answer3: ");
                                    string answer3 = Console.ReadLine();

                                    Console.WriteLine("Enter answer4: ");
                                    string answer4 = Console.ReadLine();

                                    var newQuestion = new Quiz.Question
                                    {
                                        Text = questionText,
                                        RightAnswer = rightAnswer,
                                        Answer2 = answer2,
                                        Answer3 = answer3,
                                        Answer4 = answer4
                                    };

                                    questions.Add(newQuestion);



                                }
                                Quiz quiz = QuizLogics.CreateQuiz(result, name, questions);

                                Console.WriteLine("Your quiz with 5 questions has been created successfully!");
                            }




                            else if (inputText2 == "D")
                            {
                                Console.WriteLine("Enter name to delete: ");
                                string quizName = Console.ReadLine();

                                var allQuizzes = QuizLogics.LoadFromJson<Quiz>("C:\\Users\\User\\Desktop\\12.1\\QuizRepository\\Data\\QuizData.json");
                                var quizToDelete = allQuizzes.FirstOrDefault(q => q.Name == quizName);

                                if (quizToDelete == null)
                                {
                                   
                                    throw new Exception("Quiz not found!!!");
                                }

                                if (quizToDelete.Author != result) 
                                {
                                   
                                    throw new Exception("something went wrong. Either you are not the author of this quiz or you have entered wrong credentials!!!");
                                }

                                
                                allQuizzes.Remove(quizToDelete);
                                QuizLogics.SaveToJson(allQuizzes, QuizLogics._filepathforquiz);
                                Console.WriteLine("Quiz deleted successfully.");

                            }
                            else if (inputText2 == "U")
                            {

                                Console.WriteLine("Enter name to update: ");
                                string name = Console.ReadLine();

                                var allQuizzes = QuizLogics.LoadFromJson<Quiz>("C:\\Users\\User\\Desktop\\12.1\\QuizRepository\\Data\\QuizData.json");
                                var quizToUpdate = allQuizzes.FirstOrDefault(q => q.Name == name);

                                if (quizToUpdate == null)
                                {
                                   
                                    throw new Exception("Quiz not found!!!");
                                }

                                if (quizToUpdate.Author != result) 
                                {
                                    
                                    throw new Exception("something went wrong. Either you are not the author of this quiz or you have entered wrong credentials!!!");
                                }

                                
                                for (int i = 0; i < quizToUpdate.Questions.Count; i++)
                                {
                                    var question = quizToUpdate.Questions[i];

                                    Console.WriteLine($"Old question {i + 1}: {question.Text}");
                                    Console.WriteLine("Enter new one:");
                                    string newQuestion = Console.ReadLine();

                                    Console.WriteLine($"Old right answer: {question.RightAnswer}");
                                    Console.WriteLine("Enter new one:");
                                    string newRightAnswer = Console.ReadLine();

                                    Console.WriteLine($"Old Answer 2: {question.Answer2}");
                                    Console.WriteLine("Enter new one:");
                                    string newAnswer2 = Console.ReadLine();

                                    Console.WriteLine($"Old Answer 3: {question.Answer3}");
                                    Console.WriteLine("Enter new one:");
                                    string newAnswer3 = Console.ReadLine();

                                    Console.WriteLine($"Old Answer 4: {question.Answer4}");
                                    Console.WriteLine("Enter new one:");
                                    string newAnswer4 = Console.ReadLine();

                                    quizToUpdate.Questions[i].Text = newQuestion;
                                    quizToUpdate.Questions[i].RightAnswer = newRightAnswer;
                                    quizToUpdate.Questions[i].Answer2 = newAnswer2;
                                    quizToUpdate.Questions[i].Answer3 = newAnswer3;
                                    quizToUpdate.Questions[i].Answer4 = newAnswer4;
                                }

                                QuizLogics.SaveToJson(allQuizzes, QuizLogics._filepathforquiz);
                                Console.WriteLine("Quiz updated successfully.");
                            }

                            else { Console.WriteLine("You have entered wrong credentials!!!"); };

                        }




                    }


                    else if (inputText == "E")
                    {

                        Console.WriteLine("You are living the application, goodbye!!!");
                        loop = false;
                    }

                    else
                    {

                        Console.WriteLine("You have entered wrong credentials!!!");
                    }

                }

                catch (Exception ex)
                {

                    Console.WriteLine($"Error:{ex.Message}");
                }
            }


        }


    }


}








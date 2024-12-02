using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizModels
{

    public class Quiz   
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();

        public void ShuffleAnswers()
        {
            foreach (var question in Questions)
            {
                question.ShuffleAnswers();
            }

        }




        public class Question
        {
            public string Text { get; set; }
            public string RightAnswer { get; set; }
            public string Answer2 { get; set; }
            public string Answer3 { get; set; }
            public string Answer4 { get; set; }

            public List<string> ShuffledAnswers { get; private set; }
            public int CorrectAnswerIndex { get; private set; } 

            public void ShuffleAnswers()
            {
                
                var answers = new List<string> { RightAnswer, Answer2, Answer3, Answer4 };

                
                Random rand = new Random();
                ShuffledAnswers = answers
                    .Select((answer, index) => new { answer, index })
                    .OrderBy(x => rand.Next())
                    .ToList()
                    .Select((x, newIndex) =>
                    {
                        if (x.answer == RightAnswer)
                            CorrectAnswerIndex = newIndex;
                        return x.answer;
                    })
                    .ToList();
            }
        }

    }


}
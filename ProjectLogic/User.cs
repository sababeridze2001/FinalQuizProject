using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizModels
{
    public class User
    {

        //public string Id { get; set; }
        //public string Name { get; set; }
        //public string Password { get; set; }
        //public int Score {  get; set; }
        //public int Record {  get; set; }


        private string id { get; set; }
        private string name { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }

        public int Record { get; set; }



        public string Id


        {
            get
            {
                return id;
            }
            set
            {
                if (value.Length > 5)
                {
                    throw new Exception("ID must not be more than 5 digits!!!");
                }
                else if (!value.All(char.IsDigit))
                {
                    throw new Exception("ID must only be digits!!!");
                }
                else
                {
                    id = value;
                }
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                if (value.Any(char.IsDigit))
                {
                    throw new Exception("You have entered wrong symbol!!!");
                }
                else
                {
                    name = value;
                }
            }
        }
        //public string Password
        //{
        //    get { return password; }
        //    set
        //    {
        //        if (value.Length > 5)
        //        {
        //            throw new Exception("Password must not be more than 5 symbols!!!");
        //        }
        //        else
        //        {
        //            password = value;
        //        }
        //    }
        //}

    }

}


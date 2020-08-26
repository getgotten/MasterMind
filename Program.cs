using System;
using System.Linq;
using System.Text;

namespace MasterMind
{
    class Program
    {
        public static int[] answer;
        public static bool success = false;
        public static short guessCount = 1;
        public const short guessMax = 10;
        public const short minDigit = 1;
        public const short maxDigit = 6;
        public const string rules = "Guess a number, 4 digits in length, with each digit between (and including) the numbers 1 and 6.";
        public const string scoring = "After each attempt, correctly positioned digits will receive a '+'\r\nand other correct digits, not found in the correct position, will receive a '-'.\r\nIncorrect digits will not be printed.";

        public class Result
        {
            public int Correct { get; set; }
            public int Present { get; set; }
        }

        public static void Main(string[] args)
        {
            PlayMastermind();

            Console.WriteLine("===========================");
            Console.WriteLine("Thank you for playing Mastermind.");
            Console.WriteLine("Goodbye.");

            System.Threading.Thread.Sleep(3000);

            Environment.Exit(0);
        }

        private static void PlayMastermind()
        {
            success = false;
            guessCount = 1;

            Console.WriteLine("MASTERMIND");

            Console.WriteLine(rules);
            Console.WriteLine(scoring);
            Console.WriteLine(string.Format("You have {0} attempts to guess the correct combination.", guessMax));
            Console.WriteLine("Ready to Play.");

            //todo: implement Duplicate digit functionality (ability to turn on/off)
            var random = new Random();
            answer = new int[4];
            answer.SetValue(random.Next(minDigit, maxDigit), 0);
            answer.SetValue(random.Next(minDigit, maxDigit), 1);
            answer.SetValue(random.Next(minDigit, maxDigit), 2);
            answer.SetValue(random.Next(minDigit, maxDigit), 3);

            while (success == false && guessCount <= guessMax)
            {
                ProcessAttempt();

                guessCount += 1;

                if (success == false && guessCount <= guessMax)
                {
                    Console.WriteLine("Please, try again.");
                }
            }

            Console.WriteLine("===========================");
            Console.WriteLine(string.Format("The answer was: {0}", string.Join("", answer)));
            Console.WriteLine("===========================");

            if (success)
            {
                Console.WriteLine("You have solved Mastermind! Congratulations!");
            }
            else
            {
                Console.WriteLine("You have not solved Mastermind.");
            }

            Console.WriteLine("===========================");

            Console.WriteLine("Would you like to play Mastermind again (Y or N)?");
            var playAgain = Console.ReadLine();

            if (playAgain.ToUpper() == "Y")
            {
                Console.WriteLine("===========================");

                PlayMastermind();
            }
        }

        private static bool ProcessAttempt()
        {
            Console.WriteLine("===========================");
            Console.WriteLine(string.Format("Enter guess #{0}:", guessCount));
            
            string guess = Console.ReadLine();
            int[] guessArray = guess.ToString().ToCharArray().Select(s => (int)Char.GetNumericValue(s)).ToArray();

            var validGuess = ValidateGuess(guessArray);

            if (validGuess)
            {
                Console.WriteLine(string.Format("Your guess is: {0}", guess));
                Console.WriteLine("Checking results...");

                Result result = GetResult(guessArray);

                WriteResults(result);

                if (result.Correct == 4)
                {
                    success = true;
                }
            }
            else
            {
                Console.WriteLine(string.Format("Your guess of: {0} is invalid", guess));
                Console.WriteLine(rules);
            }

            return success;
        }

        private static bool ValidateGuess(int[] guessArray)
        {
            if (guessArray.Length == 4)
            {
                foreach (var n in guessArray)
                {
                    if (n > 6 || n < 1)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private static Result GetResult(int[] guessArray)
        {
            var i = 0;
            var correct = 0;
            var present = 0;
            //don't include positionally correct digits in the 'present' check, if already accounted for
            bool[] positionallyCorrect = new bool[4];
            int[] localAnswer = new int[4];

            //check for positionally correct digits first
            while (i < 4)
            {
                foreach (var n in guessArray)
                {
                    if (n == answer[i])
                    {
                        correct += 1;
                        positionallyCorrect.SetValue(true, i);
                        localAnswer.SetValue(-1, i); //this will not be used for the present check
                        guessArray.SetValue(-2, i);
                    }
                    else
                    {
                        positionallyCorrect.SetValue(false, i);
                        localAnswer.SetValue(answer[i], i);
                    }
                    i += 1;
                }
            }

            //check for digits that are present, but not already positionally correct
            for (int ii = 0; ii < 4; ii++) 
            {
                if (localAnswer.Contains(guessArray[ii]))
                {
                    present += 1;

                    for (int iii = 0; iii < 4; iii++)
                    {
                        if (localAnswer[iii] == guessArray[ii])
                        {
                            //exclude this answer from future checks
                            localAnswer.SetValue(-4, iii);
                            break;
                        }
                    }
                }
            }

            return new Result { Correct = correct, Present = present };
        }

        private static void WriteResults(Result result)
        {
            var sb = new StringBuilder();
            var correct = result.Correct;
            var present = result.Present;

            while (correct > 0)
            {
                sb.Append(" +");
                correct -= 1;
            }

            while (present > 0)
            {
                sb.Append(" -");
                present -= 1;
            }

            Console.WriteLine(string.Format("Current result: {0}", sb.ToString()));
        }
    }
}
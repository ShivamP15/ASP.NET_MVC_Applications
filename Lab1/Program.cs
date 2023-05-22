using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Linq;

namespace Lab1
{
    internal class Program
    {
        static String choice = "";
        static void Menu() 
        {
            Console.WriteLine("\n1 - Import words from File");
            Console.WriteLine("2 - Bubble Sort words");
            Console.WriteLine("3 - LINQ/Lambda Sort words");
            Console.WriteLine("4 - Count the distinct words");
            Console.WriteLine("5 - Take the last 50 words");
            Console.WriteLine("6 - Reverse print the words");
            Console.WriteLine("7 - Get and display words that end with 'd' and display the count");
            Console.WriteLine("8 - Get and display words that start with 'r' and display the count");
            Console.WriteLine("9 - Get and display words that are more than 3 characters long and start with the letter 'a' and display the count");
            Console.WriteLine("x - Exit");
            Console.Write("Enter choice: ");
            choice = Console.ReadLine();
        }
        static void Main(string[] args)
        {
            IList<string> words = new List<string>();
            bool finish = false;
            while (!finish)
            {
                Menu();
                switch (choice)
                {
                    case "1":
                        ImportWordsFromFile(words);
                        break;
                    case "2":
                        if (words.Count == 0)
                        {
                            Console.WriteLine("Please load words first!");
                            break;
                        }
                        BubbleSortWords(words);
                        break;
                    case "3":
                        if (words.Count == 0)
                        {
                            Console.WriteLine("Please load words first!");
                            break;
                        }
                        LambdaSortWords(words);
                        break;
                    case "4":
                        if (words.Count == 0)
                        {
                            Console.WriteLine("Please load words first!");
                            break;
                        }
                        CountDistinctWords(words);
                        break;
                    case "5":
                        if (words.Count == 0)
                        {
                            Console.WriteLine("Please load words first!");
                            break;
                        }
                        Last50Words(words);
                        break;
                    case "6":
                        if (words.Count == 0)
                        {
                            Console.WriteLine("Please load words first!");
                            break;
                        }
                        WordsInReverse(words);
                        break;
                    case "7":
                        if (words.Count == 0)
                        {
                            Console.WriteLine("Please load words first!");
                            break;
                        }
                        WordsEndsWithD(words);
                        break;
                    case "8":
                        if (words.Count == 0)
                        {
                            Console.WriteLine("Please load words first!");
                            break;
                        }
                        WordsStartsWithR(words);
                        break;
                    case "9":
                        if (words.Count == 0)
                        {
                            Console.WriteLine("Please load words first!");
                            break;
                        }
                        WordsStartWithAAnd3CharLong(words);
                        break;
                    case "x":
                        finish = true;
                        break;
                    default: 
                        Console.WriteLine("Invalid Choice. Please choose number between 1-9 or 'x' to exit");
                        break;
                }
            }
        }

        static IList<string> ImportWordsFromFile(IList<string> words)
        {
            try
            {
                using (StreamReader file = new StreamReader("..\\..\\..\\Words.txt"))
                {
                    if (words.Count != 0)
                    {
                        words.Clear();
                    }
                    String wordInFile;
                    while ((wordInFile = file.ReadLine()) != null)
                    {
                        words.Add(wordInFile);
                    }
                    Console.WriteLine("The number of words is "+words.Count);
                }
                /*
                StreamReader file = new StreamReader("..\\..\\..\\Words.txt");
                String wordInFile;
                while ((wordInFile = file.ReadLine()) != null)
                {
                    words.Add(wordInFile);
                }
                Console.WriteLine("The number of words is " + words.Count);
                file.Dispose();
                Console.WriteLine(file.ReadToEnd());
                 */
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return words;
        }

        static IList<string> BubbleSortWords(IList<string> wordsList)
        {
            int n = wordsList.Count - 1;
            bool swapped = true;
            DateTime startTime = DateTime.Now;

            while (swapped)
            {
                swapped = false;
                for (int i = 1; i < n; i++)
                {
                    if (string.Compare(wordsList[i], wordsList[i+1]) > 0)
                    {
                        string temp = wordsList[i+1];
                        wordsList[i+1] = wordsList[i];
                        wordsList[i] = temp;
                        swapped = true;
                    }
                }
            }
            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;
            /*
            foreach (string w in wordsList)
            {
                Console.WriteLine(w);
            }
            */
            Console.Write("Sorting Time: {0:0.0} ms\n", duration.TotalMilliseconds);
            Console.WriteLine("The number of words is " + wordsList.Count);
            return wordsList;
        }

        static IList<string> LambdaSortWords(IList<string> wordsList)
        {
            DateTime startTime = DateTime.Now;
            
            IList<string> linqSortedList = wordsList.OrderBy(w => w).ToList();

            DateTime endTime = DateTime.Now;
            TimeSpan duration = endTime - startTime;

            Console.Write("Sorting Time: {0:0.0} ms\n", duration.TotalMilliseconds);
            /*
            foreach (string w in linqSortedList)
            {
                Console.WriteLine(w);
            }
            */
            return linqSortedList;
        }

        static int CountDistinctWords(IList<string> wordsList)
        {
            int distinctCount = wordsList.Distinct().Count();
            Console.WriteLine("Distinct Count: " + distinctCount);
            return distinctCount;
        }
        
        static IList<String> Last50Words(IList<string> wordsList)
        {
            //IList<string> last50WordsList = wordsList.Skip(Math.Max(0,wordsList.Count()-50)).ToList();
            IList<string> last50WordsList = wordsList.TakeLast(50).ToList();
            Console.WriteLine("The Last 50 words are:");
            foreach (string w in last50WordsList)
            {
                Console.WriteLine(w);
            }
            return last50WordsList;
        }

        static IList<string> WordsInReverse(IList<string> wordsList)
        {
            IList<string> reverseList = (wordsList.Reverse()).ToList();
            Console.WriteLine("The words printed from the end to beginning are:");
            foreach (string w in reverseList)
            {
                Console.WriteLine(w);
            }
            return reverseList;
        }

        static IList<string> WordsEndsWithD(IList<string> wordsList)
        {
            IList<string> endWithDList = (from w in wordsList where w.EndsWith("d") select w).ToList();
            Console.WriteLine("The "+ endWithDList.Count() +" words that end with 'd' are:");
            foreach (string w in endWithDList)
            {
                Console.WriteLine(w);
            }
            return endWithDList;
        }

        static IList<string> WordsStartsWithR(IList<string> wordsList)
        {
            IList<string> startWithRList = (from w in wordsList where w.StartsWith("r") select w).ToList();
            Console.WriteLine("The " + startWithRList.Count() + " words that starts with the letter 'r' are:");
            foreach (string w in startWithRList)
            {
                Console.WriteLine(w);
            }
            return startWithRList;
        }

        static IList<string> WordsStartWithAAnd3CharLong (IList<string> wordsList)
        {
            IList<string> startWithAand3CharList = (from w in wordsList where w.Contains("a") && w.Length > 3 select w).ToList();
            Console.WriteLine("The " + startWithAand3CharList.Count() + " words that have more than 3 characters and include the letter 'a' are:");
            foreach (string w in startWithAand3CharList)
            {
                Console.WriteLine(w);
            }
            return startWithAand3CharList;
        }
    }
}
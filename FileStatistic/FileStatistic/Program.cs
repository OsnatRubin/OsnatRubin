using System;
using System.IO;
using System.Collections.Generic;


namespace FileStatistic
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputPath = MyProgram("C:/Users/osnat/OneDrive/Desktp/Hadasim/file.txt");
            Console.WriteLine("OUTPUT FILE LOCATION: {0}", outputPath);
        }
        public static string MyProgram(string filePath)
        {
            #region Params
            
            FileStream file = null;
            string nonsense=" ",kSec=null, kFullSequence=null, text = null, outputFile = "C:/Users/osnat/OneDrive/Desktop/Hadasim/output.txt";
            string[] lines = null, line = null;
            int countWords = 0, maxLine = 0, maxUniq = 0;
            List<string> uniqWords = new List<string>(),
            // link of the source https://www.wallstreet-english.co.il/%D7%9E%D7%99%D7%9C%D7%95%D7%AA-%D7%A7%D7%99%D7%A9%D7%95%D7%A8-%D7%91%D7%90%D7%A0%D7%92%D7%9C%D7%99%D7%AA/
                connectionWords = new List<string>(new string[] { "with", "additionally", "also", "too", "and", "but", "except", "besides", "morever", "furthermore", "including", "as", "not", "only", "the", "that", "without", "another", "about", "just" });
            StringWriter newFile = new StringWriter();
            //1. two parallel lists: first contains uniq words, second contains amount in text
            List<string> uniqWord = new List<string>();
            List<int> uniqWordCount = new List<int>();
            //2.
            int countWithoutK = 0, startSecRow = 0, startSecLine = 0, maxRowIndex = 0, maxLineIndex = 0, maxSec = 0;
            bool isInSec = false;
            //4.
            string[] allColors = { "blue", "black","white","red","green","orange","grey","pink","purple" },
            //in case we should saperate colors by shades-
            //string[] shades = {"dark", "light"};
            //then we check shades first, and find it out as another color like blue and light blue
                shades = {"dark", "light"};

            int[] colorCounter = new int[allColors.Length];
            bool isAnyColor = false;
            //3.
            string[] digits = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" },
                     teens = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" },
                     dozens = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" },
                     bigNumbers = { "hundred", "thousand", "million", "billion" };
            int buildNumber=0;

            #endregion

            //Opening the file into reading access
            try
            {
                lines = File.ReadAllLines(filePath);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "Error "+e.HResult;
            }
            catch (FileLoadException e)
            {
                Console.WriteLine(e.Message);
                return "Error " + e.HResult;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return "Error " + e.HResult;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Message);
                return "Error " + e.HResult;
            }
            finally
            {
                if (file != null)
                    file.Close();
            }

            for (int i = 0; i < lines.Length; i++) //every row
            {
                int cnt = 0; //count words in sentence- in order to avoide null words

                text = lines[i].Trim();
                line = text.Split();
                nonsense += line.ToString();
                for (int j = 0; j < line.Length && line[j] != ""; j++, cnt++) //every word
                {
                    //cleaning the word from ./,
                    char[] c = new char[] { ',', '/', '!', '.', '-' };
                    string word = line[j].ToLower().Trim();
                    while (word.IndexOfAny(c) > -1)
                        word = word.Remove(word.IndexOfAny(c));
                    //checks if this is a uniq word (A/a)
                    if (!uniqWords.Contains(word))
                        uniqWords.Add(word);
                    //1. connection word
                    if (!connectionWords.Contains(word))
                    {
                        if (uniqWord.Contains(word))
                        {
                            int index = uniqWord.IndexOf(word);
                            uniqWordCount[index]++;
                            if (uniqWordCount[index] > maxUniq)
                                maxUniq = uniqWordCount[index];
                        }
                        else
                        {
                            uniqWord.Add(word);
                            uniqWordCount.Add(1);
                        }
                    }
                    //.2
                    if (!word.Contains('k'))
                    {
                        if (!isInSec)
                        {
                            startSecRow = i;
                            startSecLine = j;
                            isInSec = true;
                            countWithoutK++;
                            kSec += word+" ";
                        }
                        else
                        {
                            countWithoutK++;
                            kSec += word+" ";
                        }
                        }
                    else if (isInSec)
                    {
                        isInSec = false;
                        if (countWithoutK > maxSec)
                        {
                            maxSec = countWithoutK;
                            maxRowIndex = startSecRow;
                            maxLineIndex = startSecLine;
                            kFullSequence = kSec;
                        }
                        countWithoutK = 0;
                        kSec = "";
                    }
                    //4.
                    int indx = Array.IndexOf(allColors, word);
                    if(indx>=0)
                        colorCounter[indx]++;

                }
                //checks if the current row is the maximum
                maxLine = cnt > maxLine ? cnt : maxLine;
                countWords += cnt;
            }

            if (isInSec && countWithoutK > maxSec)
            {
                maxSec = countWithoutK;
                maxRowIndex = startSecRow;
                maxLineIndex = startSecLine;
                kFullSequence = kSec;
            }
            Console.WriteLine(nonsense);
            newFile.WriteLine("number of rows: {0}", lines.Length);
            newFile.WriteLine("number of words: {0}", countWords);
            newFile.WriteLine("number of uniq words: {0}", uniqWords.Count);
            newFile.WriteLine("length avarage of sentance: {0}", (0.0 + countWords) / lines.Length);
            newFile.WriteLine("max lenght of line: {0}", maxLine);
            newFile.WriteLine("most popular word in text: {0}", uniqWord[uniqWordCount.IndexOf(maxUniq)]);
            newFile.WriteLine("longest sentance without 'k': '{0}'", kFullSequence);
            newFile.WriteLine("colors in the text: ");
            for (int i = 0; i < allColors.Length; i++)
            {
                if (colorCounter[i] > 0)
                {
                    isAnyColor = true;
                    newFile.WriteLine("{0}: {1} times.", allColors[i], colorCounter[i]);
                }
            }
            if(!isAnyColor)
                newFile.WriteLine("NO COLOR WAS MANTION IN THE TEXT");

            Console.WriteLine(newFile.ToString());
            File.WriteAllText(outputFile, newFile.ToString());
            return outputFile;
        }
    }
}
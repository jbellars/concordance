using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace consoleConcordance
{
    /// <summary>
    /// The Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Generic main method required by a console application. This is the main entry point for the program.
        /// </summary>
        /// <param name="args">Additional arguments fed to the program.</param>
        public static void Main(string[] args)
        {
            Dictionary<string, WordEntry> concordance = new Dictionary<string, WordEntry>();

            try
            {
                Console.Write("Enter full path of arbitrary text file to process: ");
                var path = Console.ReadLine();

                //Pass the file path and file name to the StreamReader constructor
                using StreamReader sr = new StreamReader(path);
                var inputFile = sr.ReadToEnd();

                BuildConcordanceFromStreamReader(inputFile, concordance);

                // sort and print concordance
                List<string>? sortedKeys = SortConcordanceKeys(concordance);
                PrintSortedConcordance(concordance, sortedKeys);

                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {

            }
        }

        /// <summary>
        /// Generates a concordance from all of the words in a file.
        /// </summary>
        /// <param name="inputFile">The contents of the input file as a string.</param>
        /// <param name="concordance">The collection of word data.</param>
        public static Dictionary<string, WordEntry> BuildConcordanceFromStreamReader(string inputFile, Dictionary<string, WordEntry> concordance)
        {
            //Read the first line of text
            var sentenceNum = 1;

            var normalizedWords = inputFile.ToLower();
            List<string> words = normalizedWords.Split(new char[] { ' ', '\r', '\n' }).ToList();

            foreach (var word in words)
            {
                var newSentenceNum = AddEdgeCases(word, concordance, sentenceNum, out var added);

                if (!added)
                {
                    var keyword = Regex.Replace(word.Trim(), @"[\,\(\)\!\@\#\$\%\^\&\*\[\]\{\}\~\`\.]", string.Empty);
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        if (concordance.ContainsKey(keyword))
                        {
                            UpdateExistingWordEntry(concordance, keyword, sentenceNum);
                        }
                        else
                        {
                            AddNewWordEntry(concordance, keyword, sentenceNum);
                        }
                    }
                }

                sentenceNum = newSentenceNum;
            }

            return concordance;
        }

        /// <summary>
        /// Adds a new entry and word data to the concordance.
        /// </summary>
        /// <param name="concordance">The collection of word data.</param>
        /// <param name="keyword">The word being used for a key in the concordance.</param>
        /// <param name="sentenceNum">The sentence number in which the entry is found.</param>
        public static void AddNewWordEntry(IDictionary<string, WordEntry> concordance, string? keyword, int sentenceNum)
        {
            concordance.Add(keyword, new WordEntry(frequency: 1, location: sentenceNum.ToString()));
        }

        /// <summary>
        /// Updates the data for an existing word entry in the concordance.
        /// </summary>
        /// <param name="concordance">The collection of word data.</param>
        /// <param name="keyword">The word being used for a key in the concordance.</param>
        /// <param name="sentenceNum">The sentence number in which the entry is found.</param>
        public static void UpdateExistingWordEntry(Dictionary<string, WordEntry> concordance, string? keyword, int sentenceNum)
        {
            concordance[keyword].IncrementFrequency();
            concordance[keyword].AddLocation(sentenceNum);
        }

        /// <summary>
        /// Add words containing punctuation that would otherwise be stripped out.
        /// </summary>
        /// <param name="word">The entry being interrogated.</param>
        /// <param name="concordance">The collection of word data.</param>
        /// <param name="sentenceNum">The current sentence number.</param>
        /// <param name="added">Indicates whether or not an edge case (exception) entry was added.</param>
        /// <returns>Number indicating current sentence number.</returns>
        public static int AddEdgeCases(string? word, Dictionary<string, WordEntry> concordance, int sentenceNum, out bool added)
        {
            var keyword = Regex.Replace(word.Trim(), @"[\,\(\)\!\@\#\$\%\^\&\*\[\]\{\}\~\`]", string.Empty);
            added = false;

            // Check for exceptions
            if (keyword == "i.e.")
            {
                if (!concordance.ContainsKey(keyword))
                {
                    AddNewWordEntry(concordance, keyword, sentenceNum);
                    added = true;
                }
                else
                {
                    UpdateExistingWordEntry(concordance, keyword, sentenceNum);
                }
            }
            else if (keyword.Contains('.'))
            {
                return sentenceNum + 1;
            }

            return sentenceNum;
        }

        /// <summary>
        /// Returns a sorted list of concordance keys.
        /// </summary>
        /// <param name="concordance">The collection of word data.</param>
        /// <returns></returns>
        public static List<string> SortConcordanceKeys(Dictionary<string, WordEntry> concordance)
        {
            List<string>? list = concordance.Keys.ToList();
            list.Sort();
            return list;
        }

        /// <summary>
        /// Print formatted concordance contents in order.
        /// </summary>
        /// <param name="concordance">The collection of word data.</param>
        /// <param name="sortedKeys">The sorted list of keys in the concordance.</param>
        public static void PrintSortedConcordance(Dictionary<string, WordEntry> concordance, List<string> sortedKeys)
        {
            foreach (var key in sortedKeys)
            {
                Console.WriteLine($"{key.PadRight(25)}{{{concordance[key].GetFrequency()}:{concordance[key].PrintLocations()}}}");
            }
        }

        /// <summary>
        /// Creates a sorted concordance from an unsorted one.
        /// </summary>
        /// <param name="concordance">The initial concordance.</param>
        /// <param name="sortedKeys">The list of sorted concordance keys.</param>
        /// <returns></returns>
        public static Dictionary<string, WordEntry> CreateSortedConcordance(Dictionary<string, WordEntry> concordance, List<string> sortedKeys)
        {
            var sorted = new Dictionary<string, WordEntry>();
            foreach (var key in sortedKeys)
            {
                sorted.Add(key, new WordEntry(concordance[key].GetFrequency(), concordance[key].GetLocations()));
            }

            return sorted;
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static consoleConcordance.Program;

namespace consoleConcordance.Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void AddNewWordEntry_Succeeds()
        {
            Dictionary<string, WordEntry> concordance = new Dictionary<string, WordEntry>();
            AddNewWordEntry(concordance, "blue", 1);
            Assert.IsTrue(concordance.ContainsKey("blue"));
        }

        [TestMethod]
        public void UpdateExistingWordEntry_Succeeds()
        {
            Dictionary<string, WordEntry> concordance = new Dictionary<string, WordEntry>();
            AddNewWordEntry(concordance, "blue", 1);
            UpdateExistingWordEntry(concordance, "blue", 2);
            Assert.IsTrue(concordance.ContainsKey("blue"));
            Assert.IsTrue(concordance["blue"].GetFrequency() == 2);
            Assert.AreEqual(concordance["blue"].PrintLocations(), "1,2");
        }

        [TestMethod]
        public void BuildConcordanceFromStreamReader_Succeeds()
        {
            Dictionary<string, WordEntry> concordance = new Dictionary<string, WordEntry>();
            var inputFile = ExtractTextFromEmbeddedResource("consoleConcordance.Tests.arbitrary.txt");
            Dictionary<string, WordEntry>? result = BuildConcordanceFromStreamReader(inputFile, concordance);
            Assert.AreEqual(result.Count, 34);
        }

        [TestMethod]
        public void SortConcordanceKeys_Succeeds()
        {
            var inputFile = ExtractTextFromEmbeddedResource("consoleConcordance.Tests.arbitrary.txt");
            Dictionary<string, WordEntry>? concordance = BuildConcordanceFromStreamReader(inputFile, new Dictionary<string, WordEntry>());
            var result = SortConcordanceKeys(concordance);
            Assert.AreEqual("a", result.First());
            Assert.AreEqual("written", result.Last());
        }

        [TestMethod]
        public void CreateSortedConcordance_Succeeds()
        {
            var inputFile = ExtractTextFromEmbeddedResource("consoleConcordance.Tests.arbitrary.txt");
            Dictionary<string, WordEntry>? concordance = BuildConcordanceFromStreamReader(inputFile, new Dictionary<string, WordEntry>());
            var sortedList = SortConcordanceKeys(concordance);
            var result = CreateSortedConcordance(concordance, sortedList);
            Assert.AreEqual("a", result.First().Key);
            Assert.AreEqual("written", result.Last().Key);
        }

        /// <summary>
        /// Extracts text from an embedded resource for processing.
        /// </summary>
        /// <param name="resourceName">The name of the resource file.</param>
        /// <returns>A string that contains the contents of the resource file.</returns>
        private static string ExtractTextFromEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            return result;
        }
    }
}
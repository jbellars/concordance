using System.Collections.Generic;

namespace consoleConcordance
{
    /// <summary>
    /// A container for a word entry in the concordance.
    /// </summary>
    public class WordEntry
    {
        // Private properties
        private int _frequency;
        private List<string> _locations = new List<string>();

        // Constructors
        public WordEntry(int frequency, string location)
        {
            _frequency = frequency;
            _locations.Add(location);
        }

        public WordEntry(int frequency, List<string> locations)
        {
            _frequency = frequency;
            _locations = locations;
        }

        // Member Methods

        /// <summary>
        /// Increments the _frequency of the word.
        /// </summary>
        public void IncrementFrequency()
        {
            _frequency++;
        }

        /// <summary>
        /// Adds a sentence location for the word.
        /// </summary>
        /// <param name="sentenceNumber"></param>
        public void AddLocation(int sentenceNumber)
        {
            _locations.Add(sentenceNumber.ToString());
        }

        public int GetFrequency()
        {
            return _frequency;
        }

        public string PrintLocations()
        {
            return string.Join(",", _locations);
        }

        public List<string> GetLocations()
        {
            return _locations;
        }
    }
}
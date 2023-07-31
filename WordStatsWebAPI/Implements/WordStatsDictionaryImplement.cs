using System.Text.Json;

using WordStats.Interfaces;

namespace WordStats.Implements
{
    public sealed class WordStatsDictionaryImplement : IWordStats
    {
        private static readonly object _lock = new object();

        private IDictionary<string, int> Words = new Dictionary<string, int>();
        private IDictionary<char, int> Characters = new Dictionary<char, int>();

        private void AddWord(string word, int count)
        {
            lock (_lock)
            {
                if (Words.ContainsKey(word))
                {
                    Words[word] += count;
                }
                else
                {
                    Words.Add(word, count);
                }
            }
        }

        public void AddWords(IDictionary<string, int> words)
        {
            foreach (var word in words)
            {
                AddWord(word.Key, word.Value);
            }
        }

        private void AddCharacter(char character, int count)
        {
            lock (_lock)
            {
                if (Characters.ContainsKey(character))
                {
                    Characters[character] += count;
                }
                else
                {
                    Characters.Add(character, count);
                }
            }
        }

        public void AddCharacters(IDictionary<char, int> characters)
        {
            foreach (var character in characters)
            {
                AddCharacter(character.Key, character.Value);
            }
        }

        public IEnumerable<string> GetLargestFiveWords()
        {
            lock (_lock)
            {
                return Words.OrderByDescending(w => w.Key).OrderByDescending(w => w.Key.Length).Take(5).Select(w => w.Key);
            }
        }

        public IEnumerable<string> GetSmallestFiveWords()
        {
            lock (_lock)
            {
                return Words.OrderBy(w => w.Key).OrderBy(w => w.Key.Length).Take(5).Select(w => w.Key);
            }
        }

        public IEnumerable<KeyValuePair<string, int>> GetMostFrequentTenWords()
        {
            lock (_lock)
            {
                return Words.OrderByDescending(w => w.Value).Take(10);
            }
        }

        public IEnumerable<KeyValuePair<string, int>> GetWordsOrderByFrequencyDescending()
        {
            lock (_lock)
            {
                return Words.OrderByDescending(w => w.Value);
            }
        }

        public IEnumerable<KeyValuePair<char, int>> GetCharactersOrderByFrequencyDescending()
        {
            lock (_lock)
            {
                return Characters.OrderByDescending(c => c.Value);
            }
        }

        public string ToJsonString()
        {
            lock (_lock)
            {
                var result = new
                {
                    TotalWords = Words.Sum(x => x.Value),
                    TotalCharacters = Characters.Sum(x => x.Value),
                    LargestFiveWords = GetLargestFiveWords(),
                    SmallestFiveWords = GetSmallestFiveWords(),
                    MostFrequentTenWords = GetMostFrequentTenWords(),
                    Characters = GetCharactersOrderByFrequencyDescending()
                };
                return JsonSerializer.Serialize(result);
            }
        }
    }
}
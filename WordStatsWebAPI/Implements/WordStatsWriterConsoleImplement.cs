using WordStats.Interfaces;

namespace WordStats.Implements
{
    public class WordStatsWriterConsoleImplement : IWordStatsWriter
    {
        public void WriteStats(IWordStats? stats)
        {
            if (stats == null)
                throw new ArgumentNullException(nameof(stats));

            Console.Clear();
            Console.SetCursorPosition(0, 0);

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"| Total Words: {stats.GetWordsOrderByFrequencyDescending().Sum(x => x.Value)}, Total Characters: {stats.GetCharactersOrderByFrequencyDescending().Sum(x => x.Value)}");
            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"| Largest 5 words: {string.Join(", ", stats.GetLargestFiveWords())}");
            Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"| Smallest 5 words: {string.Join(", ", stats.GetSmallestFiveWords())}");
            Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Most frequent 10 words:");
            Console.WriteLine("| " + string.Join(", ", stats.GetMostFrequentTenWords()));
            Console.WriteLine("|----------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("  Characters:");
            Console.WriteLine(string.Join(", ", stats.GetCharactersOrderByFrequencyDescending()));
        }
    }
}
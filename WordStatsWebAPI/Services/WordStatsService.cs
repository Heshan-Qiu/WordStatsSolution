using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

using WordStats.Interfaces;

namespace WordStats.Services
{
    public class WordStatsService : BackgroundService
    {
        private readonly ILogger<WordStatsService> _logger;

        private readonly Stream _stream;

        private readonly Encoding _encoding;

        private readonly IWordStats _stats;

        private readonly IWordStatsWriter _writer;

        private readonly IOptions<WordStatsServiceOptions> _options;

        public WordStatsService(Stream stream, Encoding encoding, IWordStats stats, IWordStatsWriter writer,
            IOptions<WordStatsServiceOptions> options, ILogger<WordStatsService> logger)
        {
            _stream = stream;
            _encoding = encoding;
            _stats = stats;
            _writer = writer;
            _options = options;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoReadAsyncTask();
                await DoWriteAsyncTask();
            }
        }

        private async Task DoReadAsyncTask()
        {
            var buffer = ReadStream();
            var text = ConvertStreamToString(buffer);

            GetWordStats(text);
            GetCharacterStats(text);

            await Task.Delay(_options.Value.ReadDelay);
        }

        private async Task DoWriteAsyncTask()
        {
            _writer.WriteStats(_stats);
            await Task.Delay(_options.Value.WriteDelay);
        }

        private byte[] ReadStream()
        {
            if (_stream == null || !_stream.CanRead)
                return new byte[0];

            var bufferSize = 1024;
            var buffer = new byte[bufferSize];
            int bytesRead = 0;
            bool continueReading = true;
            List<(byte[] buffer, int bytesRead)> buffers = new List<(byte[] buffer, int bytesRead)>();

            while (continueReading && (bytesRead = _stream.Read(buffer)) > 0)
            {
                var tempBuffer = new byte[bytesRead];
                Array.Copy(buffer, tempBuffer, bytesRead);
                buffers.Add((tempBuffer, bytesRead));

                // Check if the last character is not a space
                if (buffer[bytesRead - 1] != ' ')
                    continueReading = true;
                else
                    continueReading = false;
            }

            // Create a new byte array with the actual number of bytes read
            int totalBytes = buffers.Sum(b => b.bytesRead);
            byte[] result = new byte[totalBytes];
            int offset = 0;

            foreach (var b in buffers)
            {
                Array.Copy(b.buffer, 0, result, offset, b.bytesRead);
                offset += b.bytesRead;
            }

            return result;
        }

        private string ConvertStreamToString(byte[] buffer)
        {
            return _encoding.GetString(buffer);
        }

        private void GetWordStats(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            // Define the regular expression pattern to match words
            string pattern = @"\b\w+\b";

            var words = Regex.Matches(text, pattern)
                .Cast<Match>()
                .Select(m => m.Value)
                .GroupBy(w => w)
                .ToDictionary(g => g.Key, g => g.Count());

            _stats.AddWords(words);
        }

        private void GetCharacterStats(string text)
        {
            var characters = text.Replace(" ", "").ToCharArray()
                .GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());
            _stats.AddCharacters(characters);
        }
    }
}
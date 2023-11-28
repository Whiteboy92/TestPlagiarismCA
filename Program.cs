using TestPlagiarismCA.Shared;
using System.Diagnostics; // Add this namespace for Stopwatch

namespace TestPlagiarismCA
{
    internal static class Program
    {
        private static void Main()
        {
            string testPath = @"C:\Users\Admin\Desktop\Plagiarisms";
            List<string> filePaths = Directory.GetFiles(testPath, "*.cs").ToList();

            string newestFilePath = Utility.FindNewestFile(filePaths);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            PlagiarismDetection.CompareFilesWithNewest(newestFilePath, filePaths);
            stopwatch.Stop();

            Console.WriteLine($"Execution time: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
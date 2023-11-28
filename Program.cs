using TestPlagiarismCA.Shared;
using System.Diagnostics;
using TestPlagiarismCA.CppDetection;
using TestPlagiarismCA.CSharpDetection;

namespace TestPlagiarismCA
{
    internal static class Program
    {
        private static void Main()
        {
            string cSharpPath = @"C:\Users\Admin\DesktopPlagiarisms CSharp";
            string javaPath = @"C:\Users\Admin\DesktopPlagiarisms Java";
            string cppPath = @"C:\Users\Admin\DesktopPlagiarisms Cpp";
            
            List<string> csFiles = Directory.GetFiles(cSharpPath, "*.cs").ToList();
            List<string> javaFiles = Directory.GetFiles(javaPath, "*.java").ToList();
            List<string> cppFiles = Directory.GetFiles(cppPath, "*.cpp").ToList();

            //TestPlagiarismForLanguage("C#", csFiles);
            //TestPlagiarismForLanguage("Java", javaFiles);
            TestPlagiarismForLanguage("C++", cppFiles);
        }

        private static void TestPlagiarismForLanguage(string language, List<string> filePaths)
        {
            if (filePaths.Count < 2)
            {
                Console.WriteLine($"Insufficient {language} files for comparison.");
                return;
            }
            
            string newestFilePath = Utility.FindNewestFile(filePaths);
            Stopwatch stopwatch = Stopwatch.StartNew();

            if (language == "C#")
            {
                CSharpPlagiarismDetection cSharpPlagiarismDetection = new CSharpPlagiarismDetection();
                cSharpPlagiarismDetection.CompareFilesWithNewest(newestFilePath, filePaths);
            }
            
            else if (language == "C++")
            {
                CppPlagiarismDetection cppPlagiarismDetector = new CppPlagiarismDetection();
                cppPlagiarismDetector.CompareFilesWithNewest(newestFilePath, filePaths);
            }

            stopwatch.Stop();

            Console.WriteLine($"Execution time for {language}: {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}

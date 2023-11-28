namespace TestPlagiarismCA.Shared;

public static class Utility
{
    internal static string FindNewestFile(List<string> files)
    {
        DateTime lastWriteTime = DateTime.MinValue;
        string newestFile = null!;

        foreach (string file in files)
        {
            DateTime writeTime = File.GetLastWriteTime(file);
            if (writeTime <= lastWriteTime) continue;
            
            lastWriteTime = writeTime;
            newestFile = file;
        }

        return newestFile;
    }
}
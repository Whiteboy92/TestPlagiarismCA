namespace TestPlagiarismCA.Shared;

public static class PlagiarismDetection
{
    internal static void CompareFilesWithNewest(string newestFilePath, List<string> filePaths)
    {
        const double plagiarizedMethodThreshold = 0.8d;

        string newestFileContent = File.ReadAllText(newestFilePath);
        Dictionary<string, List<string>> methodTokens = Tokenization.TokenizeMethods(newestFileContent);

        Parallel.ForEach(filePaths.Where(path => path != newestFilePath), filePath =>
        {
            string content = File.ReadAllText(filePath);
            Dictionary<string, List<string>> otherMethodTokens = Tokenization.TokenizeMethods(content);

            double plagiarismPercentage = CalculatePlagiarismPercentage(methodTokens, otherMethodTokens, plagiarizedMethodThreshold);

            Console.WriteLine($"File {Path.GetFileName(filePath)} is {plagiarismPercentage * 100:F2}% plagiarized by {Path.GetFileName(newestFilePath)}");
        });
    }
        
    private static double CalculatePlagiarismPercentage(Dictionary<string, List<string>> newestMethodTokens,
        Dictionary<string, List<string>> otherMethodTokens,
        double plagiarizedMethodThreshold)
    {
        double highestPlagiarizedMethodAverage = 0.0;

        foreach (KeyValuePair<string, List<string>> newestMethodToken in newestMethodTokens)
        {
            double maxSimilarity = 0.0;

            foreach (KeyValuePair<string, List<string>> otherMethodToken in otherMethodTokens)
            {
                double similarity = CalculateSimilarity(newestMethodToken.Value, otherMethodToken.Value);

                if (similarity > maxSimilarity)
                {
                    maxSimilarity = similarity;
                }

                // Similarity > 90% consider it as 100%
                if (maxSimilarity >= 0.9)
                {
                    maxSimilarity = 1.0;
                    break;
                }
            }

            if (maxSimilarity >= plagiarizedMethodThreshold)
            {
                highestPlagiarizedMethodAverage += maxSimilarity;
            }
        }

        int totalMethods = newestMethodTokens.Count;
        double plagiarismPercentage = totalMethods > 0 ? highestPlagiarizedMethodAverage / totalMethods : 0.0;

        return plagiarismPercentage;
    }
    
    private static double CalculateSimilarity(List<string> methodTokens1, List<string> methodTokens2)
    {
        HashSet<int> hashSet = new HashSet<int>(methodTokens1.Select(token => token.GetHashCode()));
        int intersectionCount = methodTokens2.Count(token => hashSet.Contains(token.GetHashCode()));

        double similarity = (double)intersectionCount / methodTokens2.Count;
        return similarity;
    }
}
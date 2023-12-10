namespace JJ_API.Service.Buisneess
{
    public static class CensorshipService
    {
        static List<string> Curses = new List<string> { "word1", "word2", "word3","mouse" };
        public static (bool isClean, string curse) CheckForCurses(string[] wordsInSentence)
        {
            if (wordsInSentence == null) 
            { 
                return (true,""); 
            }
            var wordSet = new HashSet<string>(Curses, StringComparer.OrdinalIgnoreCase);

            foreach (var word in wordsInSentence)
            {
                var choosenWords = new string(word.Where(char.IsLetterOrDigit).ToArray());
                if (wordSet.Contains(choosenWords))
                {
                    return (false, choosenWords);
                }
            }
            return (true, "");
        }
    }
}

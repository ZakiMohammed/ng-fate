using System.Globalization;
using System.Text.RegularExpressions;

namespace ng_fate
{
    static class Utils
    {
        public static string GetFilePath(string projectPathFull, string fileName)
        {
            var files = Directory.GetFiles(projectPathFull, fileName, SearchOption.AllDirectories).ToList();
            var fileDefault = files.FirstOrDefault();
            var filePath = string.Empty;

            if (fileDefault != null)
            {
                var fileInfo = new FileInfo(fileDefault);
                filePath = fileInfo.FullName;
            }

            return filePath;
        }

        public static async Task<List<string>> SearchInProject(string projectPathFull, IEnumerable<string> selectors)
        {
            var files = Directory.GetFiles(projectPathFull, "*.html", SearchOption.AllDirectories);
            var found = new List<string>();

            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                if (selectors.Any(selector => content.Contains(selector)))
                    found.Add(file);
            }

            return found;
        }

        public static string GetPascalCase(string name)
        {
            var parts = name.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[i]);
            }
            return string.Join("", parts);
        }

        public static string GetKebabCase(string name)
        {
            return Regex.Replace(name, @"(\p{Ll})(\p{Lu})|(\p{Lu})(\p{Lu})(\p{Ll})", "$1$3-$2$4$5").ToLower();
        }

        public static bool IsValidPrefix(string input)
        {
            return Regex.IsMatch(input, Constants.PATTERN_NO_SPECIAL_CHAR_ONLY_COMMA);
        }
    }
}

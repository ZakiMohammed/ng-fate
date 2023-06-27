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

        public static async Task<List<string>> SearchInProject(string projectPathFull, string selector)
        {
            var files = Directory.GetFiles(projectPathFull, "*.html", SearchOption.AllDirectories);
            var found = new List<string>();

            foreach (var file in files)
            {
                var content = await File.ReadAllTextAsync(file);
                if (content.Contains(selector))
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
            return Regex.Replace(name, @"(\p{Ll})(\p{Lu})", "$1-$2").ToLower();
        }
    }
}

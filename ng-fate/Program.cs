using System.Globalization;
using System.Text.RegularExpressions;

const string EXTENSION_TS = ".ts";

// taken from user
var projectPath = @"C:\Zaki\Study\Angular\service-desk\service-desk-app";
var projectPathFull = projectPath + @"\src\app\";

var modules = new List<Module>();

Console.WriteLine("Swiming please wait...");

Console.Clear();

GetModules(projectPathFull);

foreach (var module in modules)
{
    Console.WriteLine("Name: {0}", module.Name);
    Console.WriteLine("FileName: {0}", module.FileName);
    Console.WriteLine("FullPath: {0}", module.FilePath);

    Console.WriteLine("Components:");
    foreach (var component in module.Components)
    {
        Console.WriteLine("\tName: {0}", component.Name);
        Console.WriteLine("\tFileName: {0}", component.FileName);
        Console.WriteLine("\tFullPath: {0}", component.FilePath);
        Console.WriteLine("\tRouted: {0}", component.Routed);

        Console.WriteLine();
    }

    Console.WriteLine();
}

void GetModules(string fullPath)
{
    bool isModule(string f) => f.Contains(".module.ts") && !f.Contains("-routing.module.ts");
    var files = Directory.GetFiles(fullPath).Where(isModule);

    FeedModule(files);

    var folders = Directory.GetDirectories(fullPath);

    foreach (var folder in folders)
        GetModules(folder);
}

void FeedModule(IEnumerable<string> files)
{
    foreach (var file in files)
    {
        var fileInfo = new FileInfo(file);
        var module = new Module
        {
            Name = GetModuleName(fileInfo.Name),
            FileName = fileInfo.Name,
            FilePath = fileInfo.FullName,
        };
        var components = new List<Component>();

        FeedComponents(module, components, file);

        module.Components = components;

        modules.Add(module);
    }
}

void FeedComponents(Module module, List<Component> components, string file)
{
    var declarations = new List<string>();

    FeedDeclarations(declarations, file);

    foreach (var name in declarations)
    {
        var fileName = GetFileName(name);
        var filePath = GetFilePath(fileName);
        var routed = IsRouted(module.FilePath, name, filePath);

        components.Add(new Component
        {
            Name = name,
            FileName = fileName,
            FilePath = filePath,
            Routed = routed
        });
    }
}

void FeedDeclarations(List<string> declarations, string file)
{
    var lines = File.ReadLines(file);

    var foundDeclarations = false;
    foreach (var line in lines)
    {
        if (line.ToLower().Contains("declarations") && line.Contains("]"))
        {
            var pattern = "\\[(.*?)\\]";
            var match = Regex.Match(line, pattern);
            var split = match.Groups[1].Value.Split(",").Select(i => i.Trim());
            declarations.AddRange(split);
            break;
        }
        if (line.ToLower().Contains("declarations"))
        {
            foundDeclarations = true;
            continue;
        }
        if (foundDeclarations)
        {
            if (line.Contains("]"))
                break;
            declarations.Add(line.Replace(",", "").Trim());
        }
    }
}

bool IsRouted(string modulePath, string name, string path)
{
    var routed = false;
    var routingPath = modulePath.Replace(".module.ts", "-routing.module.ts");
    if (File.Exists(routingPath))
        routed = IsComponentPathExist(routingPath, name);
    else
        routed = IsComponentPathExist(modulePath, name);

    if (!routed)
        routed = IsComponentPathExistInAppRouting(name, path);

    return routed;
}

bool IsComponentPathExistInAppRouting(string name, string path)
{
    var appRoutingFilePath = projectPathFull + "app-routing.module.ts";

    if (!File.Exists(appRoutingFilePath))
        return false;

    var content = File.ReadAllText(appRoutingFilePath);

    var routePattern1 = "component: " + name;
    var routePattern2 = "component:" + name;
    var routePattern = content.Contains(routePattern1) || content.Contains(routePattern2);

    var relativePath = path.Replace(projectPathFull, "./").Replace("\\", "/").Replace(".ts", "").Trim();
    var importPattern = content.Contains(relativePath);

    return routePattern && importPattern;
}

bool IsComponentPathExist(string routingFilePath, string name)
{
    var content = File.ReadAllText(routingFilePath);
    var routePattern1 = "component: " + name;
    var routePattern2 = "component:" + name;
    var routePattern = content.Contains(routePattern1) || content.Contains(routePattern2);
    return routePattern;
}

string GetFilePath(string fileName)
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

string GetFileName(string name)
{
    return GetKebabCase(name)
        .Replace("-component", ".component")
        .Replace("-pipe", ".pipe")
        .Replace("-directive", ".directive") + EXTENSION_TS;
}

string GetModuleName(string name)
{
    return GetPascalCase(name.Replace(".ts", ""));
}

string GetPascalCase(string name)
{
    var parts = name.Split('.');
    for (int i = 0; i < parts.Length; i++)
    {
        parts[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[i]);
    }
    return string.Join("", parts);
}

string GetKebabCase(string name)
{
    return Regex.Replace(name, @"(\p{Ll})(\p{Lu})", "$1-$2").ToLower();
}
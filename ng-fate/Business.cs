using System.Text.RegularExpressions;

namespace ng_fate
{
    static class Business
    {
        public static string ProjectPathFull { get; set; } = string.Empty;

        public static List<Module> Modules { get; set; } = new List<Module>();

        public static void Print()
        {
            Shell.Clear();
            Shell.WriteHeading();

            foreach (var module in Modules)
            {
                Shell.WriteKeyValue("Name", module.Name);
                Shell.WriteKeyValue("FileName", module.FileName);
                Shell.WriteKeyValue("FilePath", module.FilePath);

                Shell.WriteKey("Components:");
                foreach (var component in module.Components)
                {
                    Shell.WriteKeyValue("\tName", component.Name);
                    Shell.WriteKeyValue("\tFileName", component.FileName);
                    Shell.WriteKeyValue("\tFullPath", component.FilePath);
                    Shell.WriteKeyValue("\tRouted", component.Routed);
                }

                Shell.EmptyLine();
            }
        }

        public static async Task ProcessModules(string fullPath)
        {
            bool isModule(string f) => f.Contains(Constants.PATTERN_MODULE_EXTENSION) && !f.Contains(Constants.PATTERN_ROUTING_MODULE);
            var files = Directory.GetFiles(fullPath).Where(isModule);

            await FeedModule(files);

            var folders = Directory.GetDirectories(fullPath);

            foreach (var folder in folders)
                await ProcessModules(folder);
        }

        static async Task FeedModule(IEnumerable<string> files)
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

                await FeedComponents(module, components, file);

                module.Components = components;

                Modules.Add(module);
            }
        }

        static async Task FeedComponents(Module module, List<Component> components, string file)
        {
            var declarations = new List<string>();

            await FeedDeclarations(declarations, file);

            foreach (var name in declarations)
            {
                var fileName = GetMemberFileName(name);
                var filePath = Utils.GetFilePath(ProjectPathFull, fileName);
                var routed = await IsRouted(module.FilePath, name, filePath);

                components.Add(new Component
                {
                    Name = name,
                    FileName = fileName,
                    FilePath = filePath,
                    Routed = routed
                });
            }
        }

        static async Task FeedDeclarations(List<string> declarations, string file)
        {
            var lines = await File.ReadAllLinesAsync(file);

            var foundDeclarations = false;
            foreach (var line in lines)
            {
                if (line.ToLower().Contains(Constants.PATTERN_DECLARATIONS) && line.Contains(Constants.PATTERN_CLOSING_BRACKETS))
                {
                    var pattern = Constants.PATTERN_ENCLOSED_BRACKETS;
                    var match = Regex.Match(line, pattern);
                    var split = match.Groups[1].Value.Split(Constants.PATTERN_COMMA).Select(i => i.Trim());
                    declarations.AddRange(split);
                    break;
                }
                if (line.ToLower().Contains(Constants.PATTERN_DECLARATIONS))
                {
                    foundDeclarations = true;
                    continue;
                }
                if (foundDeclarations)
                {
                    if (line.Contains(Constants.PATTERN_CLOSING_BRACKETS))
                        break;
                    declarations.Add(line.Replace(Constants.PATTERN_COMMA, string.Empty).Trim());
                }
            }
        }

        static async Task<bool> IsRouted(string modulePath, string name, string path)
        {
            bool routed;
            var routingPath = modulePath.Replace(Constants.PATTERN_MODULE_EXTENSION, Constants.PATTERN_ROUTING_MODULE);
            if (File.Exists(routingPath))
                routed = await IsComponentPathExist(routingPath, name);
            else
                routed = await IsComponentPathExist(modulePath, name);

            if (!routed)
                routed = await IsComponentPathExistInAppRouting(name, path);

            return routed;
        }

        static async Task<bool> IsComponentPathExistInAppRouting(string name, string path)
        {
            var appRoutingFilePath = ProjectPathFull + Constants.FILE_APP_ROUTING_MODULE;

            if (!File.Exists(appRoutingFilePath))
                return false;

            var content = await File.ReadAllTextAsync(appRoutingFilePath);

            var routePattern1 = Constants.PATTERN_ROUTE_COMPONENT_COLON_SPACE + name;
            var routePattern2 = Constants.PATTERN_ROUTE_COMPONENT_COLON + name;
            var routePattern = content.Contains(routePattern1) || content.Contains(routePattern2);

            var relativePath = path
                .Replace(ProjectPathFull, Constants.PATTERN_ROOT)
                .Replace(Constants.PATTERN_BACKWARD_SLASH, Constants.PATTERN_FORWARD_SLASH)
                .Replace(Constants.EXTENSION_TS, string.Empty)
                .Trim();
            var importPattern = content.Contains(relativePath);

            return routePattern && importPattern;
        }

        static async Task<bool> IsComponentPathExist(string routingFilePath, string name)
        {
            var content = await File.ReadAllTextAsync(routingFilePath);
            var routePattern1 = Constants.PATTERN_ROUTE_COMPONENT_COLON_SPACE + name;
            var routePattern2 = Constants.PATTERN_ROUTE_COMPONENT_COLON + name;
            var routePattern = content.Contains(routePattern1) || content.Contains(routePattern2);
            return routePattern;
        }

        static string GetMemberFileName(string name)
        {
            return Utils.GetKebabCase(name)
                .Replace(Constants.PATTERN_COMPONENT_DASH, Constants.PATTERN_COMPONENT_DOT)
                .Replace(Constants.PATTERN_PIPE_DASH, Constants.PATTERN_PIPE_DOT)
                .Replace(Constants.PATTERN_DIRECTIVE_DASH, Constants.PATTERN_DIRECTIVE_DOT) + Constants.EXTENSION_TS;
        }

        static string GetModuleName(string name)
        {
            return Utils.GetPascalCase(name.Replace(Constants.EXTENSION_TS, string.Empty));
        }
    }
}
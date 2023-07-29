using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ng_fate
{
    static class Business
    {
        public static DateTime StartTime { get; set; }

        public static DateTime EndTime { get; set; }

        public static OutputType OutputTypeValue => (OutputType)Convert.ToInt32(OutputTypeOption);

        public static string? OutputTypeOption { get; set; } = string.Empty;

        public static string? OutputPath { get; set; } = string.Empty;

        public static string? ProjectPath { get; set; } = string.Empty;

        public static string ProjectPathFull => ProjectPath + Constants.PATH_APP;

        public static string? ProjectPrefix { get; set; } = string.Empty;

        public static List<Module> Modules { get; set; } = new List<Module>();

        public static void Print()
        {
            Shell.EmptyLine();

            Shell.WriteKey(Constants.KEY_MODULES);
            foreach (var module in Modules)
                Shell.WriteLine($"\t{module.Name}");
            Shell.EmptyLine();

            foreach (var module in Modules)
            {
                Shell.WriteKeyValue(Constants.KEY_NAME, module.Name);
                Shell.WriteKeyValue(Constants.KEY_FILE_NAME, module.FileName);

                if (module.Standalone)
                {
                    Shell.WriteKeyValue(Constants.KEY_STANDALONE, module.Standalone);
                    Shell.WriteKeyValue(Constants.KEY_SELECTOR, module.Selector);
                    Shell.WriteKeyValue(Constants.KEY_ROUTED, module.Routed);

                    if (module.Routed)
                    {
                        Shell.WriteKeyValue(Constants.KEY_ROUTE_PATH, module.RoutePath);
                    }
                    else if (module.Parents != null && module.Parents.Count > 0)
                    {
                        Shell.WriteKey(Constants.KEY_PARENTS);
                        foreach (var parent in module.Parents)
                        {
                            Shell.WriteKeyValue($"\t{Constants.KEY_NAME}", parent.Name);
                            Shell.WriteKeyValue($"\t{Constants.KEY_FILE_NAME}", parent.FileName);
                            Shell.WriteKeyValue($"\t{Constants.KEY_FILE_PATH}", parent.FilePath);
                            Shell.WriteKeyValue($"\t{Constants.KEY_SELECTOR}", parent.Selector);
                        }
                    }
                }

                if (module.Components != null)
                {
                    Shell.WriteKey(Constants.KEY_COMPONENTS);
                    foreach (var component in module.Components)
                    {
                        Shell.WriteKeyValue($"\t{Constants.KEY_NAME}", component.Name);
                        Shell.WriteKeyValue($"\t{Constants.KEY_FILE_NAME}", component.FileName);
                        Shell.WriteKeyValue($"\t{Constants.KEY_FILE_PATH}", component.FilePath);
                        Shell.WriteKeyValue($"\t{Constants.KEY_SELECTOR}", component.Selector);
                        Shell.WriteKeyValue($"\t{Constants.KEY_ROUTED}", component.Routed);

                        if (component.Routed)
                        {
                            Shell.WriteKeyValue($"\t{Constants.KEY_ROUTE_PATH}", component.RoutePath);
                        } 
                        else if (component.Parents != null && component.Parents.Count > 0)
                        {
                            Shell.WriteKey($"\t{Constants.KEY_PARENTS}");
                            foreach (var parent in component.Parents)
                            {
                                Shell.WriteKeyValue($"\t\t{Constants.KEY_NAME}", parent.Name);
                                Shell.WriteKeyValue($"\t\t{Constants.KEY_FILE_NAME}", parent.FileName);
                                Shell.WriteKeyValue($"\t\t{Constants.KEY_FILE_PATH}", parent.FilePath);
                                Shell.WriteKeyValue($"\t\t{Constants.KEY_SELECTOR}", parent.Selector);
                            }
                        }
                    }
                }

                Shell.EmptyLine();
            }
        }

        public static void PrintStats()
        {
            var format = Constants.VALUE_LOG_TIME_FORMAT;
            var startString = StartTime.ToString(format);
            var endString = EndTime.ToString(format);
            var taken = EndTime - StartTime;
            var takenString = $"{taken.TotalSeconds:F2} {Constants.MESSAGE_STATS_SECONDS}";

            if (taken.TotalSeconds > Constants.VALUE_STATS_MAX_SECOND)
                takenString = $"{taken.TotalMinutes:F2} {Constants.MESSAGE_STATS_MINUTES}";
            else if (taken.TotalSeconds < Constants.VALUE_STATS_MIN_SECOND)
                takenString = $"{taken.TotalMilliseconds:F2} {Constants.MESSAGE_STATS_MILLISECONDS}";

            Shell.WriteLine($"\n{Constants.MESSAGE_STATS_START}", ConsoleColor.Yellow);
            Shell.WriteKeyValue(Constants.KEY_STATS_START_TIME, startString);
            Shell.WriteKeyValue(Constants.KEY_STATS_END_TIME, endString);
            Shell.WriteKeyValue(Constants.KEY_STATS_TIME_TAKEN, takenString);
            Shell.WriteLine(Constants.MESSAGE_STATS_END, ConsoleColor.Yellow);
        }

        public static async Task Save()
        {
            var content = JsonConvert.SerializeObject(Modules);

            if (!Directory.Exists(OutputPath!))
                Directory.CreateDirectory(OutputPath!);

            if (OutputTypeValue == OutputType.Html || OutputTypeValue == OutputType.All)
            {
                var fileJs = $"{OutputPath!}{Constants.OUTPUT_PATH_JS}";
                var contentJs = Constants.CONTENT_JS_MODULES_VARIABLE + content;

                var htmlContent = ResourceUtils.GetHtmlResourceContent();
                var htmlFile = $"{OutputPath!}{Constants.OUTPUT_PATH_HTML}";

                await File.WriteAllTextAsync(htmlFile, htmlContent);
                await File.WriteAllTextAsync(fileJs, contentJs);
            }

            if (OutputTypeValue == OutputType.Json || OutputTypeValue == OutputType.All)
            {
                var fileJson = $"{OutputPath!}{Constants.OUTPUT_PATH_JSON}";
                await File.WriteAllTextAsync(fileJson, content);
            }
        }

        public static async Task Run()
        {
            await ProcessModules();

            await ProcessStandaloneModules();

            OrderModules();
        }

        static void OrderModules()
        {
            var standalones = Modules.Where(i => i.Standalone).ToList();
            var nonStandalones = Modules.Where(i => !i.Standalone).ToList();

            standalones = standalones.OrderBy(i => !i.Routed).ThenBy(i => i.Name).ToList();
            nonStandalones = nonStandalones.OrderBy(i => i.Name).ToList();

            Modules = nonStandalones.Concat(standalones).ToList();
        }

        static async Task ProcessModules()
        {
            var files = Directory.GetFiles(ProjectPathFull, Constants.PATTERN_MODULE_EXTENSION_WILDCARD, SearchOption.AllDirectories).ToList();
            files = files.Where(file => !file.EndsWith(Constants.PATTERN_ROUTING_MODULE, StringComparison.OrdinalIgnoreCase)).ToList();

            await FeedModule(files);
        }

        static async Task ProcessStandaloneModules()
        {
            var files = Directory.GetFiles(ProjectPathFull, Constants.PATTERN_COMPONENT_EXTENSION_WILDCARD, SearchOption.AllDirectories);
            var aloneModules = new List<string>();

            var allComponents = GetAllComponents();

            foreach (var file in files)
            {
                if (allComponents.Exists(item => item.FilePath == file))
                    continue;

                var content = await File.ReadAllTextAsync(file);
                if (content.Contains(Constants.CONTENT_STANDALONE_TRUE) || content.Contains(Constants.CONTENT_STANDALONE_TRUE_NO_SPACE))
                    aloneModules.Add(file);
            }

            await FeedStandaloneModule(aloneModules);
        }

        static async Task FeedStandaloneModule(List<string> aloneModules)
        {
            var moduleLength = aloneModules.Count;

            for (var index = 0; index < moduleLength; index++)
            {
                var aloneModule = aloneModules[index];
                var aloneModuleNo = index + 1;
                var name = Path.GetFileName(aloneModule);
                var nameNoExtension = name.Replace(Constants.EXTENSION_TS, string.Empty);
                var routeDetail = await GetRouteStandaloneDetail(nameNoExtension);
                var selector = await GetComponentSelector(aloneModule);

                Shell.Log(string.Format(Constants.MESSAGE_LOG_STANDALONE_MODULE, aloneModuleNo, moduleLength));

                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    Name = GetComponentName(name, Constants.EXTENSION_TS),
                    FileName = name,
                    FilePath = aloneModule,
                    Standalone = true,
                    Routed = routeDetail.Item1,
                    RoutePath = routeDetail.Item2,
                    Selector = selector
                };

                if (!routeDetail.Item1)
                    await GetParentDetail(module);

                Modules.Add(module);
            }
        }

        static async Task FeedModule(List<string> files)
        {
            for (var index = 0; index < files.Count; index++)
            {
                var file = files[index];
                var fileInfo = new FileInfo(file);
                var fileNo = index + 1;
                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    Name = GetModuleName(fileInfo.Name),
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                };
                var components = new List<Component>();

                Shell.Log(string.Format(Constants.MESSAGE_LOG_MODULE, fileNo, files.Count));

                await FeedComponents(module, components, file);

                module.Components = components.OrderBy(i => i.Name).ToList();

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
                var routeDetail = await GetRouteDetail(module.FilePath, name, filePath);
                var selector = await GetComponentSelector(filePath);

                var component = new Component
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    FileName = fileName,
                    FilePath = filePath,
                    Routed = routeDetail.Item1,
                    RoutePath = routeDetail.Item2,
                    Selector = selector
                };

                if (!component.Routed)
                    await GetParentDetail(component);

                components.Add(component);
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

        static async Task GetParentDetail(Component component)
        {
            var selectors = GetSelectors(component.FileName);
            var files = await Utils.SearchComponentInProject(ProjectPathFull, selectors);

            if (files.Count < 1)
                return;

            component.Parents = new List<Component>();

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                component.Parents.Add(new Component
                {
                    Id = Guid.NewGuid(),
                    Name = GetComponentName(fileInfo.Name, Constants.EXTENSION_HTML),
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                    Selector = await GetComponentSelector(fileInfo.FullName.Replace(Constants.EXTENSION_HTML, Constants.EXTENSION_TS))
                });
            }

            component.Parents = component.Parents.OrderBy(i => i.Name).ToList();
        }

        static async Task GetParentDetail(Module module)
        {
            var selectors = GetSelectors(module.FileName);
            var files = await Utils.SearchComponentInProject(ProjectPathFull, selectors);

            if (files.Count < 1)
                return;

            module.Parents = new List<Component>();

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                module.Parents.Add(new Component
                {
                    Id = Guid.NewGuid(),
                    Name = GetComponentName(fileInfo.Name, Constants.EXTENSION_HTML),
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                    Selector = await GetComponentSelector(fileInfo.FullName.Replace(Constants.EXTENSION_HTML, Constants.EXTENSION_TS))
                });
            }

            module.Parents = module.Parents.OrderBy(i => i.Name).ToList();
        }

        static async Task<string> GetComponentSelector(string filePath)
        {
            if (File.Exists(filePath))
            {
                var pattern = Constants.PATTERN_COMPONENT_SELECTOR;
                var content = await File.ReadAllTextAsync(filePath);
                var match = Regex.Match(content, pattern);

                if (match.Success)
                    return match.Groups[1].Value;
            }

            return string.Empty;
        }

        static async Task<(bool, string)> GetRouteDetail(string modulePath, string name, string path)
        {
            (bool, string) detail;
            var routingPath = modulePath.Replace(Constants.PATTERN_MODULE_EXTENSION, Constants.PATTERN_ROUTING_MODULE);
            if (File.Exists(routingPath))
                detail = await IsComponentPathExist(routingPath, name);
            else
                detail = await IsComponentPathExist(modulePath, name);

            if (!detail.Item1)
                detail = await IsComponentPathExistInAppRouting(name, path);

            return detail;
        }

        static async Task<(bool, string)> GetRouteStandaloneDetail(string name)
        {
            var files = Directory.GetFiles(ProjectPathFull, Constants.PATTERN_ROUTING_EXTENSION_WILDCARD, SearchOption.AllDirectories);
            (bool, string) detail = (false, string.Empty);

            foreach (var file in files)
            {
                detail = await IsComponentStandalonePathExist(file, name);
                if (detail.Item1)
                    break;
            }

            return detail;
        }

        static async Task<(bool, string)> IsComponentPathExistInAppRouting(string name, string path)
        {
            var appRoutingFilePath = ProjectPathFull + Constants.FILE_APP_ROUTING_MODULE;

            if (!File.Exists(appRoutingFilePath))
                return (false, string.Empty);

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

            var pathExist = routePattern && importPattern;
            var route = string.Empty;

            if (pathExist)
                route = await GetRoute(appRoutingFilePath, name);

            return (pathExist, route);
        }

        static async Task<(bool, string)> IsComponentPathExist(string routingFilePath, string name)
        {
            var content = await File.ReadAllTextAsync(routingFilePath);
            var routePattern1 = Constants.PATTERN_ROUTE_COMPONENT_COLON_SPACE + name;
            var routePattern2 = Constants.PATTERN_ROUTE_COMPONENT_COLON + name;
            var pathExist = content.Contains(routePattern1) || content.Contains(routePattern2);
            var route = string.Empty;

            if (pathExist)
                route = await GetRoute(routingFilePath, name);

            return (pathExist, route);
        }
        
        static async Task<(bool, string)> IsComponentStandalonePathExist(string routingFilePath, string fileName)
        {
            var content = await File.ReadAllTextAsync(routingFilePath);
            var routePattern = GetRoutePatterns(fileName, true);
            var pathExist = content.Contains(routePattern.Item1);
            var route = string.Empty;

            if (pathExist)
                route = await GetRoute(routingFilePath, fileName, true);

            return (pathExist, route);
        }

        static async Task<string> GetRoute(string routingFilePath, string name, bool standalone = false)
        {
            var route = string.Empty;
            var lines = await File.ReadAllLinesAsync(routingFilePath);

            var (routePattern1, routePattern2) = GetRoutePatterns(name, standalone);

            for (int index = 0; index < lines.Length; index++)
            {
                var prevIndex = index - 1;
                var current = lines[index];
                var component = current.Contains(routePattern1) || current.Contains(routePattern2);

                if (component)
                {
                    var path =
                        current.Contains(Constants.PATTERN_ROUTE_PATH) ||
                        current.Contains(Constants.PATTERN_ROUTE_PATH_SPACE);

                    if (path)
                        route = GetRouteFromPath(current);
                    else if (index != 0)
                    {
                        var previous = lines[prevIndex];
                        var prevPath =
                            previous.Contains(Constants.PATTERN_ROUTE_PATH) ||
                            previous.Contains(Constants.PATTERN_ROUTE_PATH_SPACE);

                        if (prevPath)
                            route = GetRouteFromPath(previous);
                    }
                }
            }

            return route;
        }

        static (string, string) GetRoutePatterns(string nameOrFileName, bool standalone)
        {
            string routePattern1;
            string routePattern2;

            if (!standalone)
            {
                routePattern1 = Constants.PATTERN_ROUTE_COMPONENT_COLON_SPACE + nameOrFileName;
                routePattern2 = Constants.PATTERN_ROUTE_COMPONENT_COLON + nameOrFileName;
            }
            else
            {
                routePattern1 = string.Format(Constants.PATTERN_ROUTE_COMPONENT_STANDALONE_THEN, nameOrFileName);
                routePattern2 = routePattern1;
            }

            return (routePattern1, routePattern2);
        }

        static string GetRouteFromPath(string value)
        {
            var pattern = value.Contains(Constants.PATTERN_ROUTE_PATH_SPACE) ? Constants.PATTERN_PATH_ENCLOSED_QUOTS_SPACE : Constants.PATTERN_PATH_ENCLOSED_QUOTS;
            var match = Regex.Match(value, pattern);

            return match.Groups[1].Value;
        }

        static List<Component> GetAllComponents()
        {
            return Modules
                .Where(module => module.Components != null)
                .SelectMany(module => module.Components).ToList();
        }

        static IEnumerable<string> GetSelectors(string fileName)
        {
            return ProjectPrefixes().Select(prefix => GetSelector(fileName, prefix));
        }

        static string GetSelector(string fileName, string prefix)
        {
            var selector = fileName
                .Replace(Constants.EXTENSION_TS, string.Empty)
                .Replace(Constants.PATTERN_COMPONENT_DOT, string.Empty);
            return $"{Constants.PATTERN_LESS_THAN}{prefix}{Constants.PATTERN_DASH}{selector}";
        }

        static string GetMemberFileName(string name)
        {
            return Utils.GetKebabCase(name)
                .Replace(Constants.PATTERN_COMPONENT_DASH, Constants.PATTERN_COMPONENT_DOT)
                .Replace(Constants.PATTERN_PIPE_DASH, Constants.PATTERN_PIPE_DOT)
                .Replace(Constants.PATTERN_DIRECTIVE_DASH, Constants.PATTERN_DIRECTIVE_DOT) + Constants.EXTENSION_TS;
        }

        static string GetComponentName(string name, string extension)
        {
            return Utils
                .GetPascalCase(name.Replace(extension, string.Empty))
                .Replace(Constants.PATTERN_DASH, string.Empty)
                .Replace(Constants.PATTERN_COMPONENT_TWICE, Constants.PATTERN_COMPONENT);
        }

        static string GetModuleName(string name)
        {
            return Utils
                .GetPascalCase(name.Replace(Constants.EXTENSION_TS, string.Empty))
                .Replace(Constants.PATTERN_DASH, string.Empty)
                .Replace(Constants.PATTERN_MODULE_TWICE, Constants.PATTERN_MODULE);
        }

        public static List<string> ProjectPrefixes()
        {
            return !string.IsNullOrWhiteSpace(ProjectPrefix) ? ProjectPrefix.Split(Constants.PATTERN_COMMA).Select(i => i.Trim()).ToList() : new List<string>();
        }

        public static bool IsOptionAllOrCli()
        {
            return OutputTypeValue == OutputType.All || OutputTypeValue == OutputType.Cli;
        }

        public static bool IsOptionAllOrNonCli()
        {
            return OutputTypeValue == OutputType.All || OutputTypeValue != OutputType.Cli;
        }
    }
}
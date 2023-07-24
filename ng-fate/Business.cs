﻿using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ng_fate
{
    static class Business
    {
        public static OutputType OutputTypeValue => (OutputType)Convert.ToInt32(OutputTypeOption);

        public static string? OutputTypeOption { get; set; } = string.Empty;

        public static string? OutputPath { get; set; } = string.Empty;

        public static string? ProjectPath { get; set; } = string.Empty;

        public static string ProjectPathFull => ProjectPath + Constants.PATH_APP;

        public static string? ProjectPrefix { get; set; } = string.Empty;

        public static List<Module> Modules { get; set; } = new List<Module>();

        public static void Print()
        {
            Shell.WriteKey("Modules:");
            foreach (var module in Modules)
                Shell.WriteLine($"\t{module.Name}");
            Shell.EmptyLine();

            foreach (var module in Modules)
            {
                Shell.WriteKeyValue("Name", module.Name);
                Shell.WriteKeyValue("FileName", module.FileName);
                Shell.WriteKeyValue("FilePath", module.FilePath);

                if (module.Standalone)
                {
                    Shell.WriteKeyValue("Standalone", module.Standalone);

                    if (module.Routed)
                    {
                        Shell.WriteKeyValue("Routed", module.Routed);
                        Shell.WriteKeyValue("RoutePath", module.RoutePath);
                    }

                    if (module.Parents != null && module.Parents.Count > 0)
                    {
                        Shell.WriteKey("\tParents:");
                        foreach (var parent in module.Parents)
                        {
                            Shell.WriteKeyValue("\t\tName", parent.Name);
                            Shell.WriteKeyValue("\t\tFileName", parent.FileName);
                            Shell.WriteKeyValue("\t\tFullPath", parent.FilePath);
                        }
                    }
                }

                if (module.Components != null)
                {
                    Shell.WriteKey("Components:");
                    foreach (var component in module.Components)
                    {
                        Shell.WriteKeyValue("\tName", component.Name);
                        Shell.WriteKeyValue("\tFileName", component.FileName);
                        Shell.WriteKeyValue("\tFullPath", component.FilePath);

                        if (component.Routed)
                        {
                            Shell.WriteKeyValue("\tRouted", component.Routed);
                            Shell.WriteKeyValue("\tRoutePath", component.RoutePath);
                        }

                        if (component.Parents != null && component.Parents.Count > 0)
                        {
                            Shell.WriteKey("\tParents:");
                            foreach (var parent in component.Parents)
                            {
                                Shell.WriteKeyValue("\t\tName", parent.Name);
                                Shell.WriteKeyValue("\t\tFileName", parent.FileName);
                                Shell.WriteKeyValue("\t\tFullPath", parent.FilePath);
                            }
                        }
                    }
                }

                Shell.EmptyLine();
            }
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

            Shell.SetForegroundColor(ConsoleColor.Green);
            Shell.WriteLine(Constants.MESSAGE_SUCCESS);
            Shell.ResetColor();
        }

        public static async Task Run()
        {
            await ProcessModules(ProjectPathFull);

            await ProcessStandaloneModules();

            Modules = Modules.OrderBy(i => i.Standalone).ThenBy(i => i.Name).ToList();
        }

        static async Task ProcessModules(string fullPath)
        {
            bool isModule(string f) => f.Contains(Constants.PATTERN_MODULE_EXTENSION) && !f.Contains(Constants.PATTERN_ROUTING_MODULE);
            var files = Directory.GetFiles(fullPath).Where(isModule);

            await FeedModule(files);

            var folders = Directory.GetDirectories(fullPath);

            foreach (var folder in folders)
                await ProcessModules(folder);
        }

        static async Task ProcessStandaloneModules()
        {
            var files = Directory.GetFiles(ProjectPathFull, "*.component.ts", SearchOption.AllDirectories);
            var aloneModules = new List<string>();

            var allComponents = Modules.SelectMany(module => module.Components).ToList();

            foreach (var file in files)
            {
                if (allComponents.Exists(item => item.FilePath == file))
                    continue;

                var content = await File.ReadAllTextAsync(file);
                if (content.Contains(Constants.CONTENT_STANDALONE_TRUE) || content.Contains(Constants.CONTENT_STANDALONE_TRUE_NO_SPACE))
                    aloneModules.Add(file);
            }

            foreach (var aloneModule in aloneModules)
            {
                var name = Path.GetFileName(aloneModule);
                var nameNoExtension = name.Replace(Constants.EXTENSION_TS, string.Empty);
                var routeDetail = await GetRouteStandaloneDetail(nameNoExtension);

                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    Name = GetComponentName(name, Constants.EXTENSION_TS),
                    FileName = name,
                    FilePath = aloneModule,
                    Standalone = true,
                    Routed = routeDetail.Item1,
                    RoutePath = routeDetail.Item2
                };

                if (!routeDetail.Item1)
                    await GetParentDetail(module);

                Modules.Add(module);
            }
        }

        static async Task FeedModule(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    Name = GetModuleName(fileInfo.Name),
                    FileName = fileInfo.Name,
                    FilePath = fileInfo.FullName,
                };
                var components = new List<Component>();

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

                var component = new Component
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    FileName = fileName,
                    FilePath = filePath,
                    Routed = routeDetail.Item1,
                    RoutePath = routeDetail.Item2,
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
                });
            }

            module.Parents = module.Parents.OrderBy(i => i.Name).ToList();
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
            var files = Directory.GetFiles(ProjectPathFull, "*-routing.module.ts", SearchOption.AllDirectories);
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
            var pathExist = content.Contains(Constants.PATTERN_ROUTE_COMPONENT_STANDALONE);
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
                routePattern1 = $"/{nameOrFileName}').then(";
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
            return !string.IsNullOrWhiteSpace(ProjectPrefix) ? ProjectPrefix.Split(",").Select(i => i.Trim()).ToList() : new List<string>();
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
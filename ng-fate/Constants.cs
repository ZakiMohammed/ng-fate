namespace ng_fate
{
    static class Constants
    {
        public const string EXTENSION_TS                            = ".ts";
        public const string EXTENSION_HTML                          = ".html";

        public const string PATH_APP                                = @"\src\app\";

        public const string FILE_APP_ROUTING_MODULE                 = "app-routing.module.ts";

        public const string OUTPUT_PATH                             = "\\ng-fate-output";
        public const string OUTPUT_PATH_JSON                        = "\\ng-fate-output.json";
        public const string OUTPUT_PATH_JS                          = "\\ng-fate-output.js";
        public const string OUTPUT_PATH_HTML                        = "\\ng-fate-output.html";

        public const string CONTENT_JS_MODULES_VARIABLE             = "const _modules = ";
        public const string CONTENT_STANDALONE_TRUE                 = "standalone: true";
        public const string CONTENT_STANDALONE_TRUE_NO_SPACE        = "standalone:true";

        public const int VALUE_STATS_MAX_SECOND                     = 60;
        public const int VALUE_STATS_MIN_SECOND                     = 0;
        public const string VALUE_LOG_TIME_FORMAT                   = "hh:mm:ss:fff";

        public const string KEY_MODULES                             = "Modules";
        public const string KEY_COMPONENTS                          = "Components";
        public const string KEY_PARENTS                             = "Parents";
        public const string KEY_STANDALONE                          = "Standalone";
        public const string KEY_NAME                                = "Name";
        public const string KEY_FILE_NAME                           = "FileName";
        public const string KEY_FILE_PATH                           = "FilePath";
        public const string KEY_SELECTOR                            = "Selector";
        public const string KEY_ROUTED                              = "Routed";
        public const string KEY_ROUTE_PATH                          = "RoutePath";
        public const string KEY_STATS_START_TIME                    = "Start Time";
        public const string KEY_STATS_END_TIME                      = "End Time";
        public const string KEY_STATS_TIME_TAKEN                    = "Time Taken";

        public const string MESSAGE_TITLE                           = "NgFate - Fate of Angular App";
        public const string MESSAGE_PUNCH_LINE                      = "A tool for providing reports on the Angular app structure, routes, dependencies and relations.";
        public const string MESSAGE_GITHUB_LINE                     = "Please check the GitHub repository here: https://github.com/ZakiMohammed/ng-fate";
        public const string MESSAGE_LOADING                         = "Diving please wait...";
        public const string MESSAGE_FINISH                          = "Finished!";
        public const string MESSAGE_INPUT_PATH                      = "Project Path: ";
        public const string MESSAGE_INPUT_PREFIX                    = "Project Prefix (comma separated): ";
        public const string MESSAGE_INPUT_OUTPUT_TYPE               = "Select Output Type (1 - JSON, 2 - HTML, 3 - CLI, 4 - ALL): ";
        public const string MESSAGE_INPUT_OUTPUT_PATH               = "Output Folder Path: ";
        public const string MESSAGE_EXCEPTION                       = "Drowning! Fate is dark, can't save now!";
        public const string MESSAGE_VALIDATION_ERROR                = "Please be honest with your input!";
        public const string MESSAGE_ERROR_PROJECT_PATH              = "Project path cannot be empty";
        public const string MESSAGE_ERROR_PROJECT_PATH_NOT_EXIST    = "Project path does not exist";
        public const string MESSAGE_ERROR_OUTPUT_PATH               = "Output path cannot be empty";
        public const string MESSAGE_ERROR_OUTPUT_PATH_NOT_EXIST     = "Output path does not exist";
        public const string MESSAGE_ERROR_PREFIX                    = "Project prefix cannot be empty";
        public const string MESSAGE_ERROR_PREFIX_INVALID            = "Project prefix is invalid";
        public const string MESSAGE_ERROR_OUTPUT_TYPE_INVALID       = "Output selection is invalid";
        public const string MESSAGE_STATS_START                     = "=============== Stats ===============";
        public const string MESSAGE_STATS_END                       = "=====================================";
        public const string MESSAGE_STATS_MINUTES                   = "minutes";
        public const string MESSAGE_STATS_SECONDS                   = "seconds";
        public const string MESSAGE_STATS_MILLISECONDS              = "milliseconds";
        public const string MESSAGE_LOG_MODULE                      = "Processing module {0} of {1}";
        public const string MESSAGE_LOG_STANDALONE_MODULE           = "Processing standalone module {0} of {1}";
        public const string MESSAGE_LOG_CHILDREN                    = "Processing {0} child components";

        public const string PATTERN_MODULE_EXTENSION                = ".module.ts";
        public const string PATTERN_MODULE_EXTENSION_WILDCARD       = "*.module.ts";
        public const string PATTERN_ROUTING_EXTENSION_WILDCARD      = "*-routing.module.ts";
        public const string PATTERN_COMPONENT_EXTENSION_WILDCARD    = "*.component.ts";
        public const string PATTERN_COMPONENT_DASH                  = "-component";
        public const string PATTERN_COMPONENT_DOT                   = ".component";
        public const string PATTERN_PIPE_DASH                       = "-pipe";
        public const string PATTERN_PIPE_DOT                        = ".pipe";
        public const string PATTERN_DIRECTIVE_DASH                  = "-directive";
        public const string PATTERN_DIRECTIVE_DOT                   = ".directive";
        public const string PATTERN_ROUTING_MODULE                  = "-routing.module.ts";
        public const string PATTERN_DECLARATIONS                    = "declarations";
        public const string PATTERN_CLOSING_BRACKETS                = "]";
        public const string PATTERN_COMMA                           = ",";
        public const string PATTERN_LESS_THAN                       = "<";
        public const string PATTERN_DASH                            = "-";
        public const string PATTERN_ROOT                            = "./";
        public const string PATTERN_FORWARD_SLASH                   = "/";
        public const string PATTERN_BACKWARD_SLASH                  = "\\";
        public const char   PATTERN_BACKWARD_SLASH_CHAR             = '\\';
        public const string PATTERN_ROUTE_COMPONENT_COLON_SPACE     = "component: ";
        public const string PATTERN_ROUTE_COMPONENT_COLON           = "component:";
        public const string PATTERN_ROUTE_COMPONENT_STANDALONE      = "loadComponent";
        public const string PATTERN_ROUTE_COMPONENT_STANDALONE_THEN = "/{0}').then(";
        public const string PATTERN_ROUTE_PATH_SPACE                = "path: ";
        public const string PATTERN_ROUTE_PATH                      = "path:";
        public const string PATTERN_MODULE_TWICE                    = "ModuleModule";
        public const string PATTERN_COMPONENT_TWICE                 = "ComponentComponent";
        public const string PATTERN_COMPONENT                       = "Component";
        public const string PATTERN_MODULE                          = "Module";
        public const string PATTERN_ENCLOSED_BRACKETS               = "\\[(.*?)\\]";
        public const string PATTERN_PATH_ENCLOSED_QUOTS_SPACE       = "path: '(.*?)'";
        public const string PATTERN_PATH_ENCLOSED_QUOTS             = "path:'(.*?)'";
        public const string PATTERN_DEFAULT_PREFIX                  = "app";
        public const string PATTERN_NO_SPECIAL_CHAR_ONLY_COMMA      = @"^[A-Za-z0-9,\-\s]+$";
        public const string PATTERN_COMPONENT_SELECTOR              = @"selector:\s*'([^']*)'";
    }
}
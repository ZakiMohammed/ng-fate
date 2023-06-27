using ng_fate;

try
{
    Shell.WriteHeading();

    var projectPath = Shell.AcceptLine(Constants.MESSAGE_INPUT_PATH);
    var projectPrefix = Shell.AcceptLine(Constants.MESSAGE_INPUT_PREFIX);

    if (string.IsNullOrWhiteSpace(projectPath))
    {
        Shell.Error($"\n{Constants.MESSAGE_VALIDATION_ERROR}");
        return;
    }
    if (string.IsNullOrWhiteSpace(projectPrefix))
        projectPrefix = Constants.PATTERN_DEFAULT_PREFIX;

    var projectPathFull = projectPath + Constants.PATH_APP;

    Console.WriteLine($"\n{Constants.MESSAGE_LOADING}");

    Business.ProjectPathFull = projectPathFull;
    Business.ProjectPrefix = projectPrefix;

    await Business.ProcessModules(projectPathFull);

    Business.Print();

    await Business.Save();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;

    Shell.Error($"\n{Constants.MESSAGE_EXCEPTION} - {ex.Message}");
    Shell.Error(ex.StackTrace);
}
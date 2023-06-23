using ng_fate;

try
{
    Shell.WriteHeading();

    var projectPath = string.Empty;

    Console.ForegroundColor = ConsoleColor.Yellow;

    Shell.Write(Constants.MESSAGE_INPUT_PATH);
    Shell.ResetColor();
    projectPath = Shell.ReadLine();

    if (projectPath == null || projectPath.Trim() == string.Empty)
    {
        Shell.Error($"\n{Constants.MESSAGE_VALIDATION_ERROR}");
        return;
    }

    var projectPathFull = projectPath + Constants.PATH_APP;

    Console.WriteLine(Constants.MESSAGE_LOADING);

    Business.ProjectPathFull = projectPathFull;

    await Business.ProcessModules(projectPathFull);

    Business.Print();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;

    Shell.Error($"\n{Constants.MESSAGE_EXCEPTION} - {ex.Message}");
    Shell.Error(ex.StackTrace);
}
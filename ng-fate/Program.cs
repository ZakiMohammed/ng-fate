﻿using ng_fate;

try
{
    Shell.WriteHeading();

    Business.ProjectPath = Shell.AcceptLine(Constants.MESSAGE_INPUT_PATH);
    Business.ProjectPrefix = Shell.AcceptLine(Constants.MESSAGE_INPUT_PREFIX);

    if (!Validators.IsProjectPathValid() ||
        !Validators.IsProjectPrefixValid())
    {
        Shell.Error($"\n{Constants.MESSAGE_VALIDATION_ERROR}");
        return;
    }

    Business.OutputTypeOption = Shell.AcceptLine(Constants.MESSAGE_INPUT_OUTPUT_TYPE);

    if (!Validators.IsOutputOptionValid())
    {
        Shell.Error($"\n{Constants.MESSAGE_VALIDATION_ERROR}");
        return;
    }

    if (Business.OutputTypeValue != OutputType.Cli)
    {
        Business.OutputPath = Shell.AcceptLine(Constants.MESSAGE_INPUT_OUTPUT_PATH);

        if (!Validators.IsOutputPathValid())
        {
            Shell.Error($"\n{Constants.MESSAGE_VALIDATION_ERROR}");
            return;
        }

        Business.OutputPath = Business.OutputPath!.TrimEnd(Constants.PATTERN_BACKWARD_SLASH_CHAR);
        Business.OutputPath += Constants.OUTPUT_PATH;
    }

    Business.StartTime = DateTime.Now;

    Shell.EmptyLine();
    Shell.Log(Constants.MESSAGE_LOADING);

    await Business.Run();

    if (Business.IsOptionAllOrNonCli())
    {
        Shell.Log("Files are getting saved");

        await Business.Save();
    }

    if (Business.IsOptionAllOrCli())
    {
        Business.Print();
    }

    Business.EndTime = DateTime.Now;

    Shell.Finish();

    Business.PrintStats();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;

    Shell.Error($"\n{Constants.MESSAGE_EXCEPTION} - {ex.Message}\n");
    Shell.Error(ex.StackTrace);
}
namespace ng_fate
{
    static class Validators
    {
        public static bool IsProjectPathValid()
        {
            if (string.IsNullOrWhiteSpace(Business.ProjectPath))
            {
                Shell.Error($"\n{Constants.MESSAGE_ERROR_PROJECT_PATH}");
                return false;
            }
            if (!Directory.Exists(Business.ProjectPathFull))
            {
                Shell.Error($"\n{Constants.MESSAGE_ERROR_PROJECT_PATH_NOT_EXIST}");
                return false;
            }

            return true;
        }

        public static bool IsProjectPrefixValid()
        {
            if (string.IsNullOrWhiteSpace(Business.ProjectPrefix))
            {
                Shell.Error($"\n{Constants.MESSAGE_ERROR_PREFIX}");
                return false;
            }
            if (!Utils.IsValidPrefix(Business.ProjectPrefix))
            {
                Shell.Error($"\n{Constants.MESSAGE_ERROR_PREFIX_INVALID}");
                return false;
            }

            return true;
        }

        public static bool IsOutputOptionValid()
        {
            try
            {
                var option = Convert.ToInt32(Business.OutputTypeOption);
                if (!Enum.IsDefined(typeof(OutputType), option))
                {
                    Shell.Error($"\n{Constants.MESSAGE_ERROR_OUTPUT_TYPE_INVALID}");
                    return false;
                }
            }
            catch
            {
                Shell.Error($"\n{Constants.MESSAGE_ERROR_OUTPUT_TYPE_INVALID}");
                return false;
            }

            return true;
        }
        
        public static bool IsOutputPathValid()
        {
            if (string.IsNullOrWhiteSpace(Business.OutputPath))
            {
                Shell.Error($"\n{Constants.MESSAGE_ERROR_OUTPUT_PATH}");
                return false;
            }
            if (!Directory.Exists(Business.OutputPath))
            {
                Shell.Error($"\n{Constants.MESSAGE_ERROR_OUTPUT_PATH_NOT_EXIST}");
                return false;
            }

            return true;
        }
    }
}

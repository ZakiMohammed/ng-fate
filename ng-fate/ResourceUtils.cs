using ng_fate.Properties;

namespace ng_fate
{
    static class ResourceUtils
    {
        public static string GetHtmlResourceContent()
        {
            var htmlContent = Resources.index;
            return htmlContent;
        }
    }
}

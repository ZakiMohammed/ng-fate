namespace ng_fate
{
    public class Module
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool Routed { get; set; }
        public string RoutePath { get; set; }
        public bool Standalone { get; set; } = false;
        public List<Component> Components { get; set; }
        public List<Component> Parents { get; set; }
    }

    public class Component
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool Routed { get; set; }
        public string RoutePath { get; set; }
        public List<Component> Parents { get; set; }
    }

    public enum OutputType
    {
        Json = 1,
        Html,
        Cli,
        All
    }
}
namespace ng_fate
{
    public class Module
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public List<Component> Components { get; set; }
    }

    public class Component
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool Routed { get; set; }
        public string RoutePath { get; set; }
        public List<Component> Parents { get; set; }
    }
}
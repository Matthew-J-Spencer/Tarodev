using System.IO;

namespace Tarodev.FileWatcher
{
    public abstract class WatcherBase
    {
        public bool Enabled;
        public string Arguments;
        public string OutputPath;

        protected virtual string CreateOutputPath(string assetPath)
        {
            return string.IsNullOrEmpty(OutputPath) ? assetPath : $"{Directory.GetCurrentDirectory()}/Assets/{OutputPath}/{Path.GetFileName(assetPath)}";
        }

        public abstract string AssembleCommand(string assetPath, string ext);
        public abstract string[] SupportedExtensions { get; }
        public abstract string ProgramName { get; }
        public abstract string Name { get; }
        public abstract string InstallCommand { get; }
    }
}
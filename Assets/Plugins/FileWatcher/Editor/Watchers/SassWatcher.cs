using System;

namespace Tarodev.FileWatcher
{
    [Serializable]
    public class SassWatcher : WatcherBase
    {
        public override string AssembleCommand(string assetPath, string ext) => $"\"{assetPath}.{ext}\":\"{CreateOutputPath(assetPath)}.uss\" --no-source-map {Arguments}";
        public override string[] SupportedExtensions => new[] { "sass", "scss" };
        public override string ProgramName => "sass";
        public override string Name => "Sass";
        public override string InstallCommand => "npm install -g sass";
    }
}
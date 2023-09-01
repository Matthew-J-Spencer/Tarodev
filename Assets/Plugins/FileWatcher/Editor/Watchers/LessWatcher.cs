using System;

namespace Tarodev.FileWatcher
{
    [Serializable]
    public class LessWatcher : WatcherBase
    {
        public override string AssembleCommand(string assetPath, string ext) => $"\"{assetPath}.{ext}\" \"{CreateOutputPath(assetPath)}.uss\" {Arguments}";

        public override string[] SupportedExtensions => new[] { "less" };
        public override string ProgramName => "lessc";
        public override string Name => "Less";
        public override string InstallCommand => "npm install -g less";
    }
}
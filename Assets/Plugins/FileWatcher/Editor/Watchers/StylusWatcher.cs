using System;
using System.IO;

namespace Tarodev.FileWatcher
{
    [Serializable]
    public class StylusWatcher : WatcherBase
    {
        protected override string CreateOutputPath(string assetPath)
        {
            var basePath = base.CreateOutputPath(assetPath);
            return basePath[..basePath.LastIndexOf('/')];
        }

        public override string AssembleCommand(string assetPath, string ext)
        {
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            var outputPath = CreateOutputPath(assetPath);
            return $"\"{assetPath}.{ext}\" -o \"{outputPath}\" {Arguments} && ren \"{outputPath}/{fileName}.css\" \"{fileName}.uss\"";
        }

        public override string[] SupportedExtensions => new[] { "styl" };
        public override string ProgramName => "stylus";
        public override string Name => "Stylus";
        public override string InstallCommand => "npm install -g stylus";
    }
}
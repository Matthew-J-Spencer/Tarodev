using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Tarodev.FileWatcher
{
    internal static class FileWatcher 
    {
        internal static void ProcessFiles(WatcherConfig settings, IEnumerable<string> assetPaths)
        {
            var directory = Directory.GetCurrentDirectory();

            foreach (var asset in assetPaths)
            {
                var split = asset.Split('.');
                if (split.Length < 2) continue;

                var assetPath = $"{directory}/{split[0]}";

                foreach (var watcher in settings.Watchers)
                {
                    var ext = split[1];
                    if (!watcher.Enabled || !watcher.SupportedExtensions.Contains(ext)) continue;

                    var startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = watcher.ProgramName,
                        Arguments = watcher.AssembleCommand(assetPath, ext)
                    };

                    Process.Start(startInfo);
                }
            }
        }
    }
}
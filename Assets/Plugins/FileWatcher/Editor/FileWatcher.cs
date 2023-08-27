using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Tarodev.FileWatcher
{
    internal class FileWatcher : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var directory = Directory.GetCurrentDirectory();
            var settings = FileWatcherScriptable.instance;

            foreach (var asset in importedAssets)
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
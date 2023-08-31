using UnityEditor;

namespace Tarodev.FileWatcher
{
    internal class FileWatcherProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            FileWatcher.ProcessFiles(FileWatcherScriptable.instance.Config, importedAssets);
        }
    }
}
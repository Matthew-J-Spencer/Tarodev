using System.Collections.Generic;
using UnityEditor;

namespace Tarodev.FileWatcher
{
    [FilePath("File Watcher/FileWatcherConfig", FilePathAttribute.Location.PreferencesFolder)]
    public class FileWatcherScriptable : ScriptableSingleton<FileWatcherScriptable>
    {
        public SassWatcher SassWatcher;
        public LessWatcher LessWatcher;
        public StylusWatcher StylusWatcher;

        public List<WatcherBase> Watchers => new() { SassWatcher, LessWatcher /*, StylusWatcher*/ };
        public void Save() => Save(true);
    }
}
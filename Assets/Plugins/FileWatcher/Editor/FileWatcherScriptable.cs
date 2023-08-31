using System;
using System.Collections.Generic;
using UnityEditor;

namespace Tarodev.FileWatcher
{
    [FilePath("File Watcher/FileWatcherConfig", FilePathAttribute.Location.PreferencesFolder)]
    public class FileWatcherScriptable : ScriptableSingleton<FileWatcherScriptable>
    {
        public WatcherConfig Config;
        public void Save() => Save(true);
    }

    [Serializable]
    public class WatcherConfig
    {
        public SassWatcher SassWatcher = new SassWatcher();
        public LessWatcher LessWatcher = new LessWatcher();
        public StylusWatcher StylusWatcher = new StylusWatcher();

        public List<WatcherBase> Watchers => new() { SassWatcher, LessWatcher /*, StylusWatcher*/ };
    }
}
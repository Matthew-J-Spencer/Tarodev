﻿using System.Linq;
using UnityEditor;

namespace Tarodev.StyleClassGenerator
{
    public class StyleClassGeneratorProcessor : AssetPostprocessor
    {
        private static readonly StyleClassGenerator Generator = new(StyleClassGeneratorScriptable.instance.Configs);

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!StyleClassGeneratorScriptable.instance) return;
            var combined = importedAssets.Concat(deletedAssets);
            if (!combined.Any(i => i.EndsWith(StyleClassGeneratorShared.USS_EXTENSION))) return;
            Generator.Generate(isAutoGenerating:true);
        }
    }
}
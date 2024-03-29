﻿using UnityEngine;

namespace Tarodev.StyleClassGenerator
{
    internal static class StyleClassGeneratorShared
    {
        internal const string DEFAULT_FILE_NAME = "StyleClasses";
        internal const string USS_EXTENSION = "uss";

        internal static string GeneratePathAndFileName(string directory, string fileName)
        {
            var newDirectory = $"{Application.dataPath}/{directory}";
            var newFileName = string.IsNullOrEmpty(fileName) ? DEFAULT_FILE_NAME : fileName;
            return $"{newDirectory}/{newFileName}.Gen.cs";
        }
    }
}
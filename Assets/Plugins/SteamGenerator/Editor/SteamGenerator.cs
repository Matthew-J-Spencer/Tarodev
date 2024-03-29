#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Tarodev.SteamGenerator
{
    public static class SteamGenerator
    {
        [MenuItem("Tools/Steam Generator/Generate")]
        public static async void GenerateSchema()
        {
            var config = SteamGeneratorScriptable.instance.Config;

            var url = $"https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key={config.APIKey}&appid={config.AppID}";
            var data = await GetJsonDataAsync<GameSchema>(url);

            var builder = new StringBuilder();

            builder.Append("// This file was auto-generated. DO NOT EDIT!\n");
            builder.Append("// ReSharper disable InconsistentNaming\n\n");
            builder.Append("using System.Collections.Generic;\n");

            builder.Append($"\nnamespace {config.Namespace}\n{{\n");

            builder.Append($"\tpublic static class {config.GeneratedFileName}\n\t{{\n");

            builder.Append($"\t\tpublic static readonly IReadOnlyDictionary<{config.AchievementsEnumName}, {nameof(SteamAchievement)}> Achievements = new Dictionary<{config.AchievementsEnumName}, {nameof(SteamAchievement)}>\n\t\t{{\n");
            foreach (var achievement in data.Game.AvailableGameStats.Achievements)
            {
                builder.Append(
                    $"\t\t\t{{ {config.AchievementsEnumName}.{achievement.Name}, new {nameof(SteamAchievement)} {{ {nameof(SteamAchievement.Name)} = \"{achievement.Name}\", {nameof(SteamAchievement.DefaultValue)} = {achievement.DefaultValue}, {nameof(SteamAchievement.DisplayName)} = \"{achievement.DisplayName}\", {nameof(SteamAchievement.Hidden)} = {achievement.Hidden}, {nameof(SteamAchievement.Description)} = \"{achievement.Description}\", {nameof(SteamAchievement.Icon)} = \"{achievement.Icon}\", {nameof(SteamAchievement.IconGray)} = \"{achievement.IconGray}\" }} }},\n");
            }

            builder.Append("\t\t};\n\n");

            builder.Append($"\t\tpublic static readonly IReadOnlyDictionary<{config.StatsEnumName}, {nameof(SteamStat)}> Stats = new Dictionary<{config.StatsEnumName}, {nameof(SteamStat)}>\n\t\t{{\n");
            foreach (var stat in data.Game.AvailableGameStats.Stats)
            {
                builder.Append(
                    $"\t\t\t{{ {config.StatsEnumName}.{stat.Name}, new {nameof(SteamStat)} {{ {nameof(SteamStat.Name)} = \"{stat.Name}\", {nameof(SteamStat.DefaultValue)} = {stat.DefaultValue}, {nameof(SteamStat.DisplayName)} = \"{stat.DisplayName}\" }} }},\n");
            }

            builder.Append("\t\t};\n\t}\n");

            CreateEnum(builder, config.AchievementsEnumName, data.Game.AvailableGameStats.Achievements.ConvertAll(a => a.Name));
            CreateEnum(builder, config.StatsEnumName, data.Game.AvailableGameStats.Stats.ConvertAll(a => a.Name));

            StringifyModel(builder, typeof(SteamStat));
            StringifyModel(builder, typeof(SteamAchievement));
            StringifyModel(builder, typeof(AvailableGameStats));
            StringifyModel(builder, typeof(Game));
            StringifyModel(builder, typeof(GameSchema));

            builder.Append("}");

            CreateFile(builder.ToString(), config.GeneratedOutputPath, $"{config.GeneratedFileName}.gen.cs");
        }

        #region Helpers

        private static void CreateEnum(StringBuilder builder, string enumName, IEnumerable<string> enumValues)
        {
            builder.Append("\n\tpublic enum " + enumName + "\n\t{\n");
            foreach (var value in enumValues)
            {
                builder.Append("\t\t" + value + ",\n");
            }

            builder.Append("\t}\n");
        }

        private static void StringifyModel(StringBuilder sb, Type type)
        {
            sb.AppendLine($"\n\tpublic class {type.Name}");
            sb.AppendLine("\t{");

            foreach (var prop in type.GetProperties())
            {
                sb.AppendLine($"\t\tpublic {GetTypeName(prop.PropertyType)} {prop.Name} {{ get; set; }}");
            }

            sb.AppendLine("\t}");
        }
        
        static string GetTypeName(Type type)
        {
            // Mapping of common type names to their C# aliases
            var typeAliases = new Dictionary<string, string>
            {
                { "Int32", "int" },
                { "Int32[]", "int[]" },
                { "String", "string" },
                { "Object", "object" },
                { "Single", "float" },
            };

            string typeName;
            if (type.IsGenericType)
            {
                var genericTypeName = type.Name[..type.Name.IndexOf('`')];
                var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
                typeName = $"{genericTypeName}<{genericArgs}>";


                if (typeName.StartsWith("Task<"))
                {
                    const string PATTERN = @"^Task<";
                    typeName = Regex.Replace(typeName, PATTERN, "");
                    typeName = typeName[..^1];
                }
            }
            else
            {
                typeName = typeAliases.TryGetValue(type.Name, out var alias) ? alias : type.Name;
            }

            return typeName;
        }

        private static async Task<T> GetJsonDataAsync<T>(string uri)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        private static void CreateFile(string contents, string path, string fileName)
        {
            var folderPath = Application.dataPath + path;
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var filePath = folderPath + fileName;

            File.WriteAllText(filePath, contents);
            AssetDatabase.Refresh();
        }

        #endregion
    }
}

#endif
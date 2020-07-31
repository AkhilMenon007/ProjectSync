using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FYP.Editor
{
    public static class EditorPropertySaver
    {

        public static readonly string EDITOR_PROPERTY_ROOT = GetProjectRoot();
        private const string EDITOR_PROPERTY_FOLDER_NAME = "CustomProjectPrefs";

        private static string GetProjectRoot()
        {
            var path = Path.Combine(Path.GetDirectoryName(Application.dataPath), EDITOR_PROPERTY_FOLDER_NAME);
            if (!Directory.Exists(path)) 
            {
                path = Directory.CreateDirectory(path).FullName;
            }

            return path;
        }

        public static string GetProperty(string key)
        {
            key += ".json";
            if (File.Exists(Path.Combine(EDITOR_PROPERTY_ROOT, key)))
            {
                var s = File.ReadAllText(Path.Combine(EDITOR_PROPERTY_ROOT, key));
                return s;
            }
            return default;
        }
        public static void SetProperty(string key, string val)
        {
            key += ".json";
            if(!File.Exists(Path.Combine(EDITOR_PROPERTY_ROOT, key)))
            {
                var writer = File.Create(Path.Combine(EDITOR_PROPERTY_ROOT, key));
                writer.Close();
            }
            File.WriteAllText(Path.Combine(EDITOR_PROPERTY_ROOT, key), val);
        }


        //JSON objects

        public static void SetProperty(string key, object val)
        {
            SetProperty(key, JsonUtility.ToJson(val,true));
        }
        public static T GetProperty<T>(string key)
        {
            return JsonUtility.FromJson<T>(GetProperty(key));
        }
    }
}
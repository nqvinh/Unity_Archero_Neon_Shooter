using UnityEditor;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    
    public class JsonParser : EditorWindow
    {
        static string quickTypeNamespace = "QuickType";
        static string currentClassName = "";
        static string currentJsonString = "";
        static string currentAssetpath = "";

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        [MenuItem("Assets/Create/JsonParser/GenerateScriptObject %#J")]
        private static void GenerateCode()
        {
          
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            currentJsonString = File.ReadAllText(path);

            string fileName =Path.GetFileNameWithoutExtension(path);
            currentClassName = fileName;
            currentAssetpath = @"Assets\Data\Runtime\" + fileName + ".cs";
            string outputCodePath = Path.Combine(Directory.GetCurrentDirectory(), currentAssetpath);

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), path);
            filePath = filePath.Replace("/", "\\");

            string cmd = @"quicktype " + filePath + @" -o " + outputCodePath;

            ShellHelper.ShellRequest shellRequest = ShellHelper.ProcessCommand(cmd,"");
            shellRequest.onLog += delegate (int logType, string log) {
                Debug.LogError(log);
            };

            shellRequest.onDone += ShellRequest_onDone;
            //ExecuteCommand(filePath, outputCodePath);
            //var jsonString = File.ReadAllText(path);
            //Debug.LogError(jsonString);
        }

        private static void ShellRequest_onDone()
        {
            AssetDatabase.ImportAsset(currentAssetpath);

            string typeReflection = quickTypeNamespace + "." + currentClassName;
            Type calledType = GetType(typeReflection);
            Debug.LogError(calledType);

            var res = calledType.InvokeMember(
                            "FromJson",
                            BindingFlags.InvokeMethod | BindingFlags.Public |
                                BindingFlags.Static,
                            null,
                            null,
                            new System.Object[] { currentJsonString });
            

        }

        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }



    }
}

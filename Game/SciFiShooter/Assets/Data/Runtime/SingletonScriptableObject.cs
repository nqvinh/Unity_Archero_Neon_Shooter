using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on editor, null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
///
/// See Also:
///     blog page: http://baraujo.net/unity3d-making-singletons-from-scriptableobjects-automatically/
///     gist page: https://gist.github.com/baraujo/07bb162a1f916595cad1a2d1fee5e72d
/// </summary>
/// <typeparam name="T">Type of the singleton</typeparam>

public abstract class SingletonScriptObject<T> : ScriptableObject where T : ScriptableObject
{
    private const string RESOURCE_DIR = "Assets/Resources/Config/";
    private static T _instance = null;
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = Resources.Load($"Config/{typeof(T).FullName}") as T;
                _instance.GetType().GetMethod("Init")?.Invoke(_instance, null);
            }
            return _instance;
        }
    }

    public static void Reload()
    {
        _instance = Resources.Load($"Config/{typeof(T).FullName}") as T;
        _instance.GetType().GetMethod("Init")?.Invoke(_instance, null);
    }

#if UNITY_EDITOR
    public static void CreateAsset(Type type, string fileName)
    {
        if (!Directory.Exists(RESOURCE_DIR))
        {
            Directory.CreateDirectory(RESOURCE_DIR);
        }

        var filepath = RESOURCE_DIR + fileName;
        if (File.Exists(filepath))
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = Instance;
            return;
        }

        ScriptableObject asset = CreateInstance(type);
        AssetDatabase.CreateAsset(asset, filepath);
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
#endif
}

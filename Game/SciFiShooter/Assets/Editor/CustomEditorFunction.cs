using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomEditorFunction
{
    [MenuItem("Assets/Create/PlayerConfig")]
    public static void CreatePlayerConfig()
    {
        //PlayerConfig playerConfigAsset = ScriptableObject.CreateInstance<PlayerConfig>();

        //AssetDatabase.CreateAsset(playerConfigAsset, "Assets/Resources/Config/PlayerConfig.asset");
        //AssetDatabase.SaveAssets();
        //EditorUtility.FocusProjectWindow();

        //Selection.activeObject = playerConfigAsset;
    }

    
   
    public static void RemovePlayerConfig()
    {
        AssetDatabase.RemoveObjectFromAsset(GameConfig.Instance.playerConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        GameConfig.Instance.playerConfig = null;
    }
}

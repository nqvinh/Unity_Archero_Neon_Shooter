using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(EnemyConfigRef))]
public class EnemyConfigPropertyDrawer : PropertyDrawer
{
    private EnemyConfig lastSelected = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (GameConfig.Instance.enemyConfigs.Count == 0)
            return;
        List<string> configName = new List<string>();
        GameConfig.Instance.enemyConfigs.ForEach((config) =>
        {
            configName.Add(config.configName);
        });


        int lastSeletecIndex = configName.IndexOf(property.FindPropertyRelative("enemyConfigId").stringValue);
        if (lastSeletecIndex == -1) lastSeletecIndex = 0;

        int selectedINdex = EditorGUI.Popup(position,"EnemyConfig", lastSeletecIndex, configName.ToArray());

        if (selectedINdex >= 0)
            property.FindPropertyRelative("enemyConfigId").stringValue = configName[selectedINdex];
        

    }
}

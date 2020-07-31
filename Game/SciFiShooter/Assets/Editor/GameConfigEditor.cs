using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameConfigEditor : EditorWindow
{
    TabContainer tabContainer;

    private void OnEnable()
    {
        tabContainer = new TabContainer(this.rootVisualElement);
        ListEnemyConfig listEnemyConfig = new ListEnemyConfig(tabContainer.tabContent, GameConfig.Instance.enemyConfigs);
        tabContainer.AddTab("EnemyConfig",listEnemyConfig,true);
        tabContainer.AddTab("Globalconfig", null);
    }

    [MenuItem("GameDB/GameConfig")]
    public static void ShowWindow()
    {
        SingletonScriptObject<GameConfig>.CreateAsset(typeof(GameConfig), "GameConfig.asset");
        var window = GetWindow<GameConfigEditor>();
        window.titleContent = new GUIContent("GameConfig");
    }

   
    private void OnInspectorUpdate()
    {
        tabContainer.DoRender();
    }
}

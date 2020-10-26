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
        ListEnemyConfig listEnemyConfig = new ListEnemyConfig(GameConfig.Instance.enemyConfigs);
        ListTableEnemyConfig listTableEnemyConfig = new ListTableEnemyConfig(GameConfig.Instance.enemyConfigs);
        PlayerConfigEditor playerConfigEditor = new PlayerConfigEditor();

        //ScrollView
        //Set Flex direction of container to flew-row
        //Adjust Content Height Manual
        tabContainer.AddTab("EnemyConfig_GridView",listEnemyConfig,true);

        //Adjust Content Width Manual
        tabContainer.AddTab("EnemyConfig_ListView", listTableEnemyConfig);

        tabContainer.AddTab("Player Config", playerConfigEditor);
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

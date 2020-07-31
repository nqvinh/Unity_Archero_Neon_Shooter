using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ListEnemyConfig : ListCardItem<EnemyConfig>
{
    VisualTreeAsset enemyConfigAsset = null;
    Dictionary<VisualElement, EnemyConfig> dataMap = new Dictionary<VisualElement, EnemyConfig>();
    public ListEnemyConfig(VisualElement root, List<EnemyConfig> data) : base(root, data)
    {
        enemyConfigAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Layout/EnemyConfig.uxml");
        Button addButton = rootElement.Q<Button>("Add");
        addButton.clicked += () =>
        {
            var enemyConfig = new EnemyConfig("New Enemy Config");
            
            GameConfig.Instance.enemyConfigs.Add(enemyConfig);
          
            CreateConfigIns(enemyConfig,true);
        };
        InitUIIns();
    }

    void InitUIIns()
    {
        ClearAllChild();
        int configCount = GameConfig.Instance.enemyConfigs.Count;
        for (int i = 0; i < configCount; ++i)
        {
            CreateConfigIns(GameConfig.Instance.enemyConfigs[i]);
        }
    }

    void CreateConfigIns(EnemyConfig data,bool isNewData=false)
    {
        SerializedObject serializedObject = new SerializedObject(data);
        VisualElement configUI = enemyConfigAsset.CloneTree();
        VisualElement container = configUI.Q<VisualElement>("Container");

        var property = serializedObject.GetIterator().Copy();

        while(property.NextVisible(true))
        {
            if (property.name.Equals("id") ||
                property.displayName.Equals("Script"))
                continue;
            PropertyField propertyField = new PropertyField(property, property.displayName);
            propertyField.focusable = true;
            
            container.Add(propertyField);
        }

        configUI.Bind(serializedObject);

        this.scrollViewContent.Add(configUI);
        dataMap.Add(configUI, data);

        EnemyConfig refData = data;
        Button btnDelete = configUI.Q<Button>("btnDelete");
        btnDelete.clicked += () =>
        {
            GameConfig.Instance.enemyConfigs.Remove(refData);
            AssetDatabase.RemoveObjectFromAsset(data);
            AssetDatabase.SaveAssets();
            this.scrollViewContent.Remove(configUI);
            this.dataMap.Remove(configUI);
        };

        Button btnClone = configUI.Q<Button>("btnClone");
        btnClone.clicked += () =>
        {
            var newClone = refData.Clone();
            GameConfig.Instance.enemyConfigs.Add(newClone);
            CreateConfigIns(newClone,true);
        };

        if (isNewData)
        {
            AssetDatabase.AddObjectToAsset(data, GameConfig.Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public override void DoRender()
    {
     
    }


    void ClearAllChild()
    {
        int uiIns = dataMap.Count;
        foreach(KeyValuePair<VisualElement,EnemyConfig> map in dataMap)
        {
            this.scrollViewContent.Remove(map.Key);
        }
    }

   
}

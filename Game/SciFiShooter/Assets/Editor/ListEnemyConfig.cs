using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ListEnemyConfig : ListCardItem<EnemyConfig>
{
    //Visual Element Prefab ...
    VisualTreeAsset enemyConfigAsset = null;

    Dictionary<VisualElement, EnemyConfig> dataMap = new Dictionary<VisualElement, EnemyConfig>();
    bool hasResize = false;
    VisualElement firstChild = null;

    public ListEnemyConfig( List<EnemyConfig> data) : base( data)
    {
        //Load Visual Element Prefavs
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

        //Clone to create new VisualElement from VisualTreeAssets
        VisualElement configUI = enemyConfigAsset.CloneTree();

        //Query to get Container Element By Id
        VisualElement container = configUI.Q<VisualElement>("Container");

        //Get property of EnemyCOnfig
        var property = serializedObject.GetIterator().Copy();

        while(property.NextVisible(true))
        {
            if (property.name.Equals("id") ||
                property.displayName.Equals("Script"))
                continue;

            //Create property Field and Bind Propery to property field
            PropertyField propertyField = new PropertyField(property, property.displayName);
            propertyField.focusable = true;
            
            container.Add(propertyField);
        }

        //Bind visual element to object
        configUI.Bind(serializedObject);

        this.scrollViewContent.Add(configUI);
        configUI.MarkDirtyRepaint();
        dataMap.Add(configUI, data);
        firstChild = configUI;
        hasResize = false;

        EnemyConfig refData = data;
        Button btnDelete = configUI.Q<Button>("btnDelete");
        btnDelete.clicked += () =>
        {
            GameConfig.Instance.enemyConfigs.Remove(refData);
            AssetDatabase.RemoveObjectFromAsset(data);
            AssetDatabase.SaveAssets();
            this.scrollViewContent.Remove(configUI);
            this.AdjustContentSize(configUI.layout.size);
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
        if (hasResize== false && !float.IsNaN(firstChild.contentRect.height))
        {
            hasResize = true;
            this.AdjustContentSize(firstChild.contentRect.size);
        }
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

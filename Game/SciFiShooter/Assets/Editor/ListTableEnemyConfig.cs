using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ListTableEnemyConfig : ListTableItem<EnemyConfig>
{
    VisualTreeAsset enemyConfigAsset = null;
    Dictionary<VisualElement, EnemyConfig> dataMap = new Dictionary<VisualElement, EnemyConfig>();
    bool hasResize = false;
    VisualElement firstChild = null;

    public ListTableEnemyConfig(List<EnemyConfig> data) : base(data)
    {
        enemyConfigAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Layout/EnemyConfigHorizontal.uxml");
        //Button addButton = rootElement.Q<Button>("Add");
        //addButton.clicked += () =>
        //{
        //    var enemyConfig = new EnemyConfig("New Enemy Config");
            
        //    GameConfig.Instance.enemyConfigs.Add(enemyConfig);
          
        //    CreateConfigIns(enemyConfig,true);
        //};
        InitUIIns();
    }

    protected override void InitHeader(EnemyConfig firstElement)
    {
        SerializedObject serializedObject = new SerializedObject(firstElement);
        VisualElement header = new VisualElement();
        header.style.flexDirection = FlexDirection.Row;

        var property = serializedObject.GetIterator().Copy();
        int headerCount = 0;
        while (property.NextVisible(true))
        {
            if (property.name.Equals("id") ||
               property.displayName.Equals("Script"))
                continue;
            Label label = new Label(property.displayName);
            label.style.width = 120;
            label.style.marginLeft = label.style.marginRight = 3;
            label.style.marginTop = label.style.marginBottom = 1;
            header.Add(label);
            headerCount++;
        }
        int totalWidth = headerCount * 120;
        Debug.LogError("Total With: " + totalWidth);
        this.AdjustContentSize(headerCount);

        this.tableContent.Add(header);
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
        VisualElement container = configUI.Q<VisualElement>("Content");

        var property = serializedObject.GetIterator().Copy();

        while (property.NextVisible(true))
        {
            if (property.name.Equals("id") ||
              property.displayName.Equals("Script"))
                continue;
            var type = property.type;
            var match = System.Text.RegularExpressions.Regex.Match(type, @"PPtr<\$(.*?)>");
            if (match.Success)
                type = match.Groups[1].Value;
            Debug.Log(property.displayName + " : " + type);

            VisualElement field = null;
            switch (type)
            {
                case "long":
                    field = new IntegerField("", 120);
                    var longField = field as IntegerField;
                    longField.value = property.intValue;
                    longField.BindProperty(property);

                    break;
                case "float":
                    field = new FloatField("", 120);
                    var floatField = field as FloatField;
                    floatField.value = property.floatValue;
                    floatField.BindProperty(property);
                
                    break;
                case "string":
                    field = new TextField("");
                    var textFiled = field as TextField;
                   
                    textFiled.BindProperty(property);
                    break;
                case "Enum":
                    field = new EnumField();
                    var enumField = field as EnumField;
                    enumField.BindProperty(property);
                    break;
                case "bool":
                    field = new Toggle();
                    var toggleField = field as Toggle;
                    toggleField.BindProperty(property);
                    break;
            }


            if (field != null)
            {
                container.Add(field);
                field.style.width = 120;
            }
                
        }

        //container.Bind(serializedObject);

        this.tableContent.Add(configUI);
     
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
            //this.scrollViewContent.Remove(map.Key);
        }
    }

   
}

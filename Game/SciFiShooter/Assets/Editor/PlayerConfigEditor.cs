using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerConfigEditor : BaseVisualElementContainer
{

    public PlayerConfigEditor()
    {
        //1. Load uxml 
        var playerConfigVisualElementPrefab = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Layout/PlayerConfig.uxml");
        this.rootElement = playerConfigVisualElementPrefab.CloneTree();

        var container = this.rootElement.Q<VisualElement>("Container");

        if (GameConfig.Instance.playerConfig == null)
        {
            GameConfig.Instance.playerConfig = ScriptableObject.CreateInstance<PlayerConfig>();

            AssetDatabase.AddObjectToAsset(GameConfig.Instance.playerConfig, GameConfig.Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        PlayerConfig playerConfig = GameConfig.Instance.playerConfig;
        //Config must be an Scriptable Object
        SerializedObject serializedObject = new SerializedObject(playerConfig);
        SerializedProperty property = serializedObject.GetIterator().Copy();

        while (property.NextVisible(true))
        {
            PropertyField propertyField = new PropertyField(property, property.displayName);
            propertyField.focusable = true;
            container.Add(propertyField);
        }
        container.Bind(serializedObject);


        //2. Create by code
        //this.rootElement = new VisualElement();

        //LongField longField = new LongField("Player Speed", 120);
        //longField.value = 120;
        //this.rootElement.Add(longField);

        //FloatField floatField = new FloatField("Player Attack Speed", 100);
        //floatField.value = 3.0f;
        //this.rootElement.Add(floatField);

    }

    public override void DoRender()
    {
        
    }
}

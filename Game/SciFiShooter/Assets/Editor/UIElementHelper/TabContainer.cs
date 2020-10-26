using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TabContainer:BaseVisualElementContainer
{
    public VisualElement tabNavigator;
    public VisualElement tabContent;

    BaseVisualElementContainer currentTab;

    Dictionary<string, BaseVisualElementContainer> tab = new Dictionary<string, BaseVisualElementContainer>();
    public TabContainer(VisualElement parent)
    {
        var tabVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Layout/TabContainer.uxml"); // 1
        var tabVisualElement = tabVisualTree.CloneTree();
        this.rootElement = tabVisualElement;
        parent.Add(rootElement);
        tabNavigator = rootElement.Q<VisualElement>("TabNavigator");
        tabContent = rootElement.Q<VisualElement>("TabContent");
    }

    public void AddTab(string name, BaseVisualElementContainer content,bool setAsFirstSelectTab = false)
    {
        if (!tab.ContainsKey(name))
            tab.Add(name, content);

        if (setAsFirstSelectTab)
        {
            currentTab = content;
            tabContent.Add(content.rootElement);
        }
       


        Button button = new Button();
        button.name = name;
        button.text = name;
        tabNavigator.Add(button);

        button.clicked += () =>
        {
            OnSwitchTab(button.name);
        };
    }


    public override void DoRender()
    {
        if (currentTab != null) currentTab.DoRender();
    }

    private void OnSwitchTab(string tabName)
    {
        Debug.Log("On Switch To Tab" + tabName);
        tabContent.Remove(currentTab.rootElement);
        currentTab = tab[tabName];
        tabContent.Add(currentTab.rootElement);
        
        //currentTab.rootElement.visible = true;
    }
}

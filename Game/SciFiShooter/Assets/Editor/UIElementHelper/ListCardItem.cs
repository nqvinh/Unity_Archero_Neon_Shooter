using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ListCardItem<T>:BaseVisualElementContainer
{
    protected ScrollView scrollViewContent;
    protected List<T> data;

    
    public ListCardItem(VisualElement root,List<T> data)
    {
        var tabVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Layout/ListCardContainer.uxml"); // 1
        var tabVisualElement = tabVisualTree.CloneTree();
        this.rootElement = tabVisualElement;
        scrollViewContent = rootElement.Q<ScrollView>("ScrollView");        
        this.data = data;
    }

    public abstract override void DoRender();
}

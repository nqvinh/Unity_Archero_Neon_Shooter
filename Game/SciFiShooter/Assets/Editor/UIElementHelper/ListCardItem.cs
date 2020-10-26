using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ListCardItem<T>:BaseVisualElementContainer
{
    protected ScrollView scrollViewContent;
    protected List<T> data;

    
    public ListCardItem(List<T> data)
    {
        var tabVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Layout/ListCardContainer.uxml"); // 1
        var tabVisualElement = tabVisualTree.CloneTree();
        this.rootElement = tabVisualElement;
        scrollViewContent = rootElement.Q<ScrollView>("ScrollView");
        this.scrollViewContent.contentContainer.style.flexWrap = Wrap.Wrap;
        this.scrollViewContent.contentContainer.style.justifyContent = Justify.Center;
        this.scrollViewContent.showHorizontal = true;
        this.scrollViewContent.MarkDirtyRepaint();
        this.data = data;
    }


    protected void AdjustContentSize(Vector2 childSize)
    {
        int col = Mathf.RoundToInt(this.scrollViewContent.contentContainer.layout.width / childSize.x);
        int height= (this.scrollViewContent.contentContainer.childCount / col+1) * (Mathf.RoundToInt(childSize.y));

        var bouding = this.scrollViewContent.contentContainer.contentRect.height;
        this.scrollViewContent.contentContainer.style.height = height;
        Debug.LogError(this.scrollViewContent.contentContainer.resolvedStyle.height);
        Debug.LogError(this.scrollViewContent.contentContainer.layout.size);
        

        this.scrollViewContent.MarkDirtyRepaint();

    }
    public abstract override void DoRender();
}

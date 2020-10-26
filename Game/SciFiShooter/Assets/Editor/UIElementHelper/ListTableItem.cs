using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class ListTableItem<T>:BaseVisualElementContainer
{
    protected ListView tableContent;
    protected List<T> data;

    
    public ListTableItem(List<T> data)
    {
        var tabVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Layout/ListTableContainer.uxml"); // 1
        var tabVisualElement = tabVisualTree.CloneTree();
        this.rootElement = tabVisualElement;
        tableContent = rootElement.Q<ListView>("Container");
        tableContent.Q<ScrollView>().showHorizontal = true;
        this.data = data;
        if (this.data.Count > 0)
            InitHeader(this.data[0]);
    }

    protected void AdjustContentSize(int headerCount)
    {

        this.tableContent.contentContainer.style.width = headerCount * 120 + 250;

    }

    protected abstract void InitHeader(T firstElement);

    public abstract override void DoRender();
}

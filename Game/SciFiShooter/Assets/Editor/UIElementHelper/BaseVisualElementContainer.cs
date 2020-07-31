using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseVisualElementContainer
{
    public VisualElement rootElement;
    public abstract void DoRender();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    int _sortValue = 100;

    Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();

    UI_Scene _sceneUI = null;

    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public void SetCanvas(GameObject go, bool sort = true )
    {
        Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true; // ��ø�� parent canvas�� ������ child sort���� ������ ����
        
        if(sort)
        {
            canvas.sortingOrder = _sortValue;
            _sortValue++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if(string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        string sceneName = Managers.Scene.CurrentScene.SceneName;

        GameObject go = Managers.Resource.Instantiate($"{sceneName}/UI/World/{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Utils.GetOrAddComponent<T>(go);
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if(string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        string sceneName = Managers.Scene.CurrentScene.SceneName;

        GameObject go = Managers.Resource.Instantiate($"{sceneName}/UI/SubItem/{name}");
        if (parent != null)
            go.transform.SetParent(parent);

        return Utils.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if(string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        string sceneName = Managers.Scene.CurrentScene.SceneName;

        GameObject go = Managers.Resource.Instantiate($"{sceneName}/UI/Scene/{name}");
        T sceneUI = Utils.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        string sceneName = Managers.Scene.CurrentScene.SceneName;

        GameObject go = Managers.Resource.Instantiate($"{sceneName}/UI/Popup/{name}");
        T popup = Utils.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if(_popupStack.Peek() != popup)
        {
            Debug.Log("ClosePopupUI Failed");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        Managers.Resource.Destroy(popup.gameObject);
        popup = null;
        _sortValue--;
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopupUI();
        _sceneUI = null;
    }
}
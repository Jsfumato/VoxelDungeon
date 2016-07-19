using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BlockSettingEditor : EditorWindow
{
    [MenuItem("MyMenu/Open Block List")]
    static void OpenBlockList()
    {
        BlockSettingEditor window = (BlockSettingEditor)EditorWindow.GetWindow(typeof(BlockSettingEditor));
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += SceneGUI;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= SceneGUI;
    }

    void SceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        //Handles.BeginGUI();
        //Handles.EndGUI();
        
    }

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Test Window", GUILayout.MinWidth(20), GUILayout.MinHeight(50));

        Color tmp = GUI.color;
        GUI.color = (rootBlock == null) ? Color.red : Color.green;
        rootBlock = (GameObject)EditorGUILayout.ObjectField("prefab", rootBlock, typeof(GameObject), true);
        GUI.color = tmp;

        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
        GUILayout.Space(10);
    }

    
    //  variables
    List<GameObject> blockList = new List<GameObject>();
    GameObject rootBlock;

    private void GetObjectsInLayer(GameObject root, int layer)
    {
        foreach (Transform t in root.transform.GetComponentsInChildren(typeof(GameObject), true))
        {
            if (t.gameObject.layer == layer)
            {
                blockList.Add(t.gameObject);
            }
        }
    }
}
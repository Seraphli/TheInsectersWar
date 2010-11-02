using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ZZSprite))]
public class ZZSpriteEditor : Editor
{

    //// Use this for initialization
    //void Start () {
	
    //}
	
    //// Update is called once per frame
    //void Update () {
	
    //}

    bool mBool = true;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //GUIContent co = new GUIContent("test toolbarButton");
        //Rect r = GUILayoutUtility.GetRect(co, EditorStyles.toolbarButton);
        //GUI.BeginGroup(r);
        //if( GUI.Button(new Rect(0, 0, Screen.width, 20), "test"))
        //    Debug.Log("OnInspectorGUI()");
        //GUI.EndGroup();
        if(Application.isPlaying )
        {
            ZZSprite lSprite = (ZZSprite)target;
            string[] ldisplayedOptions = new string[lSprite.getAnimationNum()];
            lSprite.getAnimNameList().CopyTo(ldisplayedOptions, 0);

            SerializedObject lSerializedObject = new SerializedObject(lSprite);
            SerializedProperty lAnimLengthProperty = lSerializedObject.FindProperty("nowAnimationData.animationLength");
            SerializedProperty lAnimLoopProperty = lSerializedObject.FindProperty("nowAnimationData.loop");

            int lSelected;

            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("选择动画:", "");
                lSelected = EditorGUILayout.Popup(
                    lSprite.getNowAnimIndex(),
                    ldisplayedOptions
                    );
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.PropertyField(lAnimLengthProperty, new GUIContent("动画长度"));
            EditorGUILayout.PropertyField(lAnimLoopProperty, new GUIContent("是否循环"));


            EditorGUILayout.EndVertical();

            lSerializedObject.ApplyModifiedProperties();

            //foreach (string lName in ldisplayedOptions)
            //{
            //    Debug.Log(lName);
            //}
            //Debug.Log(lSprite.getNowAnimIndex());
            //Debug.Log(lSprite.getAnimationNum());
            //Debug.Log(lSelected);
            lSprite.playAnimation(lSelected);
            //Debug.Log(lSprite.getNowAnimIndex());
        }
    }
}

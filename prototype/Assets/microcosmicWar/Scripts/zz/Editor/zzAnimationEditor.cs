using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Animation))]
public class zzAnimationEditor : Editor
{
    bool mUseCrossFade = true;
    float mFadeLength = 0.3f;


    bool mUseQueuedCrossFade = false;
    float mQueuedFadeLength = 0.3f;

    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        if (Application.isPlaying)
        {
            Animation animation = (Animation)target;
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Foldout(true, "播放设置");
            mUseCrossFade = EditorGUILayout.Toggle("使用渐变", mUseCrossFade);
            mFadeLength = EditorGUILayout.FloatField("渐变时间", mFadeLength);

            EditorGUILayout.Space();
            EditorGUILayout.Foldout(true, "队列播放设置");
            mUseQueuedCrossFade = EditorGUILayout.Toggle("使用渐变", mUseQueuedCrossFade);
            mQueuedFadeLength = EditorGUILayout.FloatField("渐变时间", mQueuedFadeLength);

            EditorGUILayout.Space();
            EditorGUILayout.Foldout(true,"动画");
            EditorGUILayout.Space();
            //EditorGUILayout.BeginToggleGroup("选择动画",true);
            foreach (AnimationState state in animation)
            {
                //EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                state.enabled = EditorGUILayout.Toggle(state.name, state.enabled);
                if (GUILayout.Button("play"))
                {
                    if (mUseCrossFade)
                        animation.CrossFade(state.name, mFadeLength);
                    else
                        animation.Play(state.name);
                }
                if (GUILayout.Button("play queued"))
                {
                    if (mUseQueuedCrossFade)
                        animation.CrossFadeQueued(state.name, mQueuedFadeLength);
                    else
                        animation.PlayQueued(state.name);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                //EditorGUILayout.LabelField("动画名称:", state.name);
                //state.weight = zzEditorGUIUtility.LayoutField("weight", state.weight);
                state.weight = EditorGUILayout.FloatField("Weight", state.weight);
                state.layer = EditorGUILayout.IntField("Layer", state.layer);
                state.wrapMode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode", state.wrapMode );
                //Debug.Log(state.time);
                //state.time = EditorGUILayout.Slider("Time", state.time, 0.0f, state.length);
                //Debug.Log(state.time);
                //GUILayout.Button(state.name);
                //EditorGUILayout.Space();
                //GUILayout.FlexibleSpace();
                //EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            //EditorGUILayout.EndToggleGroup();

            EditorGUILayout.Foldout(true, "");

            EditorGUILayout.EndVertical();
        }
    }

}
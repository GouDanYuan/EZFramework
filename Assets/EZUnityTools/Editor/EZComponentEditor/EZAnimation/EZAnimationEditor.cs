/*
 * Author:      熊哲
 * CreateTime:  10/31/2017 5:30:50 PM
 * Description:
 * 
*/
using EZComponent.EZAnimation;
using EZUnityEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EZComponentEditor.EZAnimation
{
    public class EZAnimationEditor : Editor
    {
        protected IEZAnimation anim;

        protected SerializedProperty m_Loop;
        protected SerializedProperty m_RestartOnEnable;
        protected SerializedProperty m_UpdateMode;
        protected SerializedProperty m_PhaseList;
        protected ReorderableList phaseList;

        protected float space = EZEditorGUIUtility.space;
        protected float headerIndent = EZEditorGUIUtility.reorderableListHeaderIndent;
        protected float lineHeight = EditorGUIUtility.singleLineHeight;

        protected virtual void OnEnable()
        {
            anim = target as IEZAnimation;

            m_Loop = serializedObject.FindProperty("m_Loop");
            m_RestartOnEnable = serializedObject.FindProperty("m_RestartOnEnable");
            m_UpdateMode = serializedObject.FindProperty("m_UpdateMode");
            m_PhaseList = serializedObject.FindProperty("m_PhaseList");
            phaseList = new ReorderableList(serializedObject, m_PhaseList, true, true, true, true);
            phaseList.drawHeaderCallback = DrawPhaseListHeader;
            phaseList.drawElementCallback = DrawPhaseListElement;
        }

        protected virtual void DrawPhaseListHeader(Rect rect)
        {
            rect.x += headerIndent; rect.y += 1; rect.width -= headerIndent;
            float width = rect.width / 6;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, width * 2 - space, lineHeight), "Start Value");
            rect.x += width * 2;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, width * 2 - space, lineHeight), "End Value");
            rect.x += width * 2;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, width - space, lineHeight), "Duration");
            rect.x += width;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, width - space, lineHeight), "Curve");
        }

        protected virtual void DrawPhaseListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            Color curveColor = anim.currentIndex == index ? Color.red : Color.green;

            SerializedProperty phase = phaseList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty startValue = phase.FindPropertyRelative("m_StartValue");
            SerializedProperty endValue = phase.FindPropertyRelative("m_EndValue");
            SerializedProperty duration = phase.FindPropertyRelative("m_Duration");
            SerializedProperty curve = phase.FindPropertyRelative("m_Curve");

            rect.y += 1; float width = rect.width / 6;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width * 2 - space, lineHeight), startValue, GUIContent.none);
            rect.x += width * 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width * 2 - space, lineHeight), endValue, GUIContent.none);
            rect.x += width * 2;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width - space, lineHeight), duration, GUIContent.none);
            if (duration.floatValue <= 0) duration.floatValue = 0;
            rect.x += width;
            curve.animationCurveValue = EditorGUI.CurveField(new Rect(rect.x, rect.y, width - space, lineHeight), curve.animationCurveValue, curveColor, new Rect(0, 0, duration.floatValue, 1));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EZEditorGUIUtility.ScriptTitle(target);
            DrawController();
            EditorGUILayout.PropertyField(m_Loop);
            EditorGUILayout.PropertyField(m_RestartOnEnable);
            EditorGUILayout.PropertyField(m_UpdateMode);
            DrawPhaseList();
            serializedObject.ApplyModifiedProperties();
        }

        public virtual void DrawController()
        {
            GUI.enabled = false;
            EditorGUILayout.EnumPopup("Status", anim.status);
            if (Application.isPlaying) GUI.enabled = true;
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Restart"))
            {
                anim.StartPhase(0);
            }
            if (GUILayout.Button("Pause"))
            {
                anim.Pause();
            }
            if (GUILayout.Button("Resume"))
            {
                anim.Resume();
            }
            if (GUILayout.Button("Stop"))
            {
                anim.Stop();
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
        }
        public virtual void DrawPhaseList()
        {
            phaseList.DoLayoutList();
        }
    }
}
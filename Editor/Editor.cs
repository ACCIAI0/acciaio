#if !USE_NAUGHTY_ATTRIBUTES
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Acciaio.Editor
{
    [CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
    public class Editor : UnityEditor.Editor
    {
        private const string SCRIPT_PROP_NAME = "m_Script";
        private static readonly BindingFlags fieldsBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private Dictionary<string, FoldoutAttribute> _cacheFolds = new Dictionary<string, FoldoutAttribute>();
		private List<SerializedProperty> _properties = new List<SerializedProperty>();

        private void OnEnable()
        {
            SerializedProperty p = serializedObject.GetIterator();
            bool first = true;
            Type type = target.GetType();
            while(p.NextVisible(first))
            {
                first = false;
                SerializedProperty property = p.Copy();

                _properties.Add(property);

                FoldoutAttribute attribute = type.GetField(property.name, fieldsBindingFlags)?
                        .GetCustomAttribute<FoldoutAttribute>();
                if (attribute == null) continue;

                _cacheFolds.Add(property.name, attribute);
            }
        }

        private int DrawFoldout(int i, SerializedProperty p, FoldoutAttribute f)
        {
            p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, f.Name);
            if (p.isExpanded && f.IsBoxed) 
                EditorGUILayout.BeginVertical("GroupBox");
            EditorGUI.BeginDisabledGroup(f.IsReadOnly);
            if (p.isExpanded) EditorGUILayout.PropertyField(p);

            int actualCount = 0;                   
            for (int j = 1; (f.Count < 0 || j < f.Count) && i + j < _properties.Count; j++)
            {
                SerializedProperty extra = _properties[i + j];
                if (_cacheFolds.ContainsKey(extra.name)) break;
                actualCount++;
                if (p.isExpanded) EditorGUILayout.PropertyField(extra);
            }
            EditorGUI.EndDisabledGroup();
            if (p.isExpanded && f.IsBoxed)
                EditorGUILayout.EndVertical();
            else if (p.isExpanded && !f.IsBoxed)
                EditorGUILayout.Space();
            return actualCount;
        }

        public override void OnInspectorGUI()
        {
            if (_cacheFolds.Count == 0) 
            {
                DrawDefaultInspector();
                return;
            }

            using (new EditorGUI.DisabledScope(SCRIPT_PROP_NAME == _properties[0].propertyPath))
                EditorGUILayout.PropertyField(_properties[0], true);

            serializedObject.Update();

            for (int i = 1; i < _properties.Count; i++)
            {
                SerializedProperty p = _properties[i];
                if (_cacheFolds.TryGetValue(p.name, out FoldoutAttribute f)) 
                    i += DrawFoldout(i, p, f);
                else EditorGUILayout.PropertyField(p);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

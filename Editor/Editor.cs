#if !USE_NAUGHTY_ATTRIBUTES
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Acciaio.Editor
{
    [CustomEditor(typeof(UnityEngine.Object), true, isFallback = true)]
    public class Editor : UnityEditor.Editor
    {
        private const string SCRIPT_PROP_NAME = "m_Script";
        private const BindingFlags FIELDS_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private readonly Dictionary<string, FoldoutAttribute> _cacheFolds = new Dictionary<string, FoldoutAttribute>();
		private readonly List<SerializedProperty> _properties = new List<SerializedProperty>();

        private void OnEnable()
        {
            var p = serializedObject.GetIterator();
            var first = true;
            var type = target.GetType();
            while(p.NextVisible(first))
            {
                first = false;
                var property = p.Copy();

                _properties.Add(property);

                var attribute = type.GetField(property.name, FIELDS_BINDING_FLAGS)?.GetCustomAttribute<FoldoutAttribute>();
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

            var actualCount = 0;                   
            for (var j = 1; (f.Count < 0 || j < f.Count) && i + j < _properties.Count; j++)
            {
                var extra = _properties[i + j];
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

            for (var i = 1; i < _properties.Count; i++)
            {
                var p = _properties[i];
                if (_cacheFolds.TryGetValue(p.name, out FoldoutAttribute f)) 
                    i += DrawFoldout(i, p, f);
                else EditorGUILayout.PropertyField(p);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif

using System;
using System.Linq;
using System.Text;
using Acciaio.Editor.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor
{
    [CustomPropertyDrawer(typeof(Id), true)]
    public class IdDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 18;

        private const string GenButtonIcon = "d_Refresh";

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => new SerializedId(property).Height;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var id = new SerializedId(property);
            
            Rect construction = new(rect);
            construction.width -= ButtonWidth;

            EditorGUI.BeginDisabledGroup(property.type.Equals(nameof(AutoId), StringComparison.Ordinal));
            EditorGUI.PropertyField(construction, id.AsSerializedProperty(), label);
            EditorGUI.EndDisabledGroup();

            construction.x += construction.width;
            construction.width = ButtonWidth;

            if (GUI.Button(construction, EditorGUIUtility.IconContent(GenButtonIcon), EditorStyles.label)) 
                id.StringValue = Guid.NewGuid().ToString();
        }
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var id = new SerializedId(property);
            
            VisualElement root = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Row
                }
            };

            TextField valueField = new(property.displayName)
            {
                value = id.StringValue,
                bindingPath = id.PropertyPath,
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1
                }
            };
            valueField.SetEnabled(property.GetPropertyType() != typeof(AutoId));

            Image genButton = new()
            {
                image = EditorGUIUtility.IconContent(GenButtonIcon).image,
                style =
                {
                    flexGrow = 0,
                    flexShrink = 0,
                    width = ButtonWidth
                }
            };
            
            genButton.RegisterCallback<ClickEvent, SerializedId>(
                (_, prop) => prop.StringValue = Guid.NewGuid().ToString(), id);
            
            root.Add(valueField);
            root.Add(genButton);
            return root;
        }
    }

    [CustomPropertyDrawer(typeof(IdReference<>))]
    public class IdReferenceDrawer : PropertyDrawer
    {
        private const int DisplayIdMaxLength = 16;
        private const int DisplayIdTrimmedHalfLength = 5;

        private static readonly StringBuilder LabelBuilder = new();

        private static UnityEngine.Object GetObject(SerializedReferenceId reference, Type refType)
        {
            static IIdentifiable Retrieve(string guid, Type refType) 
                => (IIdentifiable)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), refType);
            
            if (string.IsNullOrEmpty(reference.GuidValue))
            {
                var guid = AssetDatabase.FindAssets($"t:{refType.Name}")
                        .FirstOrDefault(guid =>  Retrieve(guid, refType).Id == reference.IdValue);
                if (!string.IsNullOrEmpty(guid)) reference.GuidValue = guid;
                else reference.StringValue = null;
            }

            if (string.IsNullOrEmpty(reference.GuidValue)) return null;
            
            var obj = Retrieve(reference.GuidValue, refType);
            if (obj.Id != reference.StringValue) reference.StringValue = obj.Id;

            return (UnityEngine.Object)obj;
        }

        private static string CalculateLabel(string defaultLabel, string id)
        {
            if (string.IsNullOrEmpty(id)) return defaultLabel;

            LabelBuilder.Append(defaultLabel)
                    .Append(" (<color=\"red\"><b>");
            
            if (id.Length > DisplayIdMaxLength)
            {
                LabelBuilder.Append(id[..DisplayIdTrimmedHalfLength])
                        .Append("...")
                        .Append(id[^DisplayIdTrimmedHalfLength..]);
            }
            else LabelBuilder.Append(id);

            var result = LabelBuilder.Append("</b></color>)").ToString();
            LabelBuilder.Clear();
            return result;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => new SerializedReferenceId(property).Height;
        
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var reference = new SerializedReferenceId(property);
            var refType = property.GetPropertyType().GetGenericArguments()[0];
            var obj = GetObject(reference, refType);
            
            var newLabel = CalculateLabel(label.text, ((IIdentifiable)obj)?.Id);
            EditorStyles.label.richText = true;
            var newObj = EditorGUI.ObjectField(rect, newLabel, obj, refType, false);
            EditorStyles.label.richText = false;

            if (ReferenceEquals(newObj, obj)) return;
            
            reference.IdValue = ((IIdentifiable)newObj)?.Id;
            reference.GuidValue = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(newObj)).ToString();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var reference = new SerializedReferenceId(property);
            var refType = property.GetPropertyType().GetGenericArguments()[0];
            var obj = GetObject(reference, refType);

            ObjectField field = new(CalculateLabel(property.displayName, ((IIdentifiable)obj)?.Id))
            {
                value = obj,
                allowSceneObjects = false,
                objectType = refType
            };

            field.RegisterValueChangedCallback(evt =>
            {
                var newObj = evt.newValue;
                if (ReferenceEquals(newObj, obj)) return;
                
                var newLabel = CalculateLabel(property.displayName, ((IIdentifiable)newObj)?.Id);
                field.label = newLabel;
                
                reference.IdValue = ((IIdentifiable)newObj)?.Id;
                reference.GuidValue = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(newObj)).ToString();
            });
            
            return field;
        }
    }
}
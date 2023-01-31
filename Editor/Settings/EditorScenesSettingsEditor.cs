using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor.Settings
{
	[CustomEditor(typeof(EditorScenesSettings))]
	public sealed class EditorScenesSettingsEditor : UnityEditor.Editor
	{
		private const string NoneValue = "(None)";
		private const int TitleMarginTop = 2;
		private const int TitleMarginLeft = 9;
		private const int TitleFontSize = 19;
		
		private const string SettingsMenuPath = "Acciaio/Scenes";

		[SettingsProvider]
		public static SettingsProvider Provide()
		{
			return new(SettingsMenuPath, SettingsScope.Project)
			{
				activateHandler = (searchContext, rootElement) =>
            	{
					var editor = CreateEditor(EditorScenesSettings.GetOrCreateSettings());
					rootElement.Add(editor.CreateInspectorGUI());
				}
			};
		}

		private SerializedProperty _startupScene;
		private SerializedProperty _isActive;

		private void OnEnable()
		{
			_startupScene = serializedObject.FindProperty("_startupScene");
			_isActive = serializedObject.FindProperty("_isActive");
		}
		
		public override VisualElement CreateInspectorGUI()
		{
			VisualElement rootElement = new();

			var isActive = serializedObject.FindProperty("_isActive");
			var startupScene = serializedObject.FindProperty("_startupScene");

			var container = new VisualElement
			{
				style =
				{
					flexDirection = FlexDirection.Row
				}
			};

			var title = new Label("Acciaio Scenes")
			{
				style =
				{
					marginLeft = TitleMarginLeft,
					marginTop = TitleMarginTop,
					fontSize = TitleFontSize,
					unityFontStyleAndWeight = FontStyle.Bold
				}
			};

			var scenes = AcciaioEditor.BuildScenesArrayForField(true, NoneValue).ToList();
			var defaultValue = startupScene.stringValue;
			if (string.IsNullOrEmpty(defaultValue) || !scenes.Contains(defaultValue)) defaultValue = NoneValue;

			PopupField<string> popup = new("Editor Startup Scene", scenes, defaultValue);
			popup.style.flexGrow = 1;
			popup.style.flexShrink = 1;
			popup.style.overflow = Overflow.Hidden;
			popup.SetEnabled(isActive.boolValue);

			popup.RegisterValueChangedCallback<string>(evt =>
            {
                var value = evt.newValue;
                if (value == NoneValue) value = "";
                startupScene.stringValue = value;
                serializedObject.ApplyModifiedProperties();
            });

			Toggle toggle = new()
			{
				value = isActive.boolValue
			};

			toggle.RegisterValueChangedCallback<bool>(evt => 
            {
                isActive.boolValue = evt.newValue; 
                popup.SetEnabled(isActive.boolValue);
                serializedObject.ApplyModifiedProperties();
            });

			container.Add(toggle);
			container.Add(popup);
			
			rootElement.Add(title);
			rootElement.Add(new Label());
			rootElement.Add(container);

			return rootElement;
		}
	}
}
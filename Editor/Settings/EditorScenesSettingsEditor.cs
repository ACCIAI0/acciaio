using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Acciaio.Editor.Settings
{
	[CustomEditor(typeof(EditorScenesSettings))]
	public sealed class EditorScenesSettingsEditor : UnityEditor.Editor
	{
		private const int TitleMarginTop = 2;
		private const int TitleMarginLeft = 9;
		private const int TitleFontSize = 19;
		
		private const string SettingsMenuPath = "Acciaio/Scenes";

		private static readonly SceneReferenceDrawer ReferenceDrawer = new();

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
		
		public override VisualElement CreateInspectorGUI()
		{
			var startupScene = serializedObject.FindProperty("_editorStartupScene");
			var isActive = serializedObject.FindProperty("_isActive");
			var consistency = serializedObject.FindProperty("<EnableReferencesConsistency>k__BackingField");
			
			VisualElement rootElement = new();

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
			
			var reference = ReferenceDrawer.CreatePropertyGUI(startupScene, null);
			reference.SetEnabled(isActive.boolValue);
			
			Toggle toggle = new()
			{
				value = isActive.boolValue
			};

			toggle.RegisterValueChangedCallback(evt =>
			{
				isActive.boolValue = evt.newValue;
				serializedObject.ApplyModifiedProperties();
				reference.SetEnabled(isActive.boolValue);
			});

			container.Add(toggle);
			container.Add(reference);

			Toggle consistencyToggle = new(consistency.displayName)
			{
				value = consistency.boolValue
			};

			consistencyToggle.RegisterValueChangedCallback(evt =>
			{
				consistency.boolValue = evt.newValue;
				serializedObject.ApplyModifiedProperties();
			});
			
			rootElement.Add(title);
			rootElement.Add(new Label());
			rootElement.Add(container);
			rootElement.Add(consistencyToggle);
			
			return rootElement;
		}
	}
}
using UnityEditor;

namespace Acciaio.Editor
{
	[CustomEditor(typeof(EditorScenesSetup))]
	public class EditorScenesSetupEditor : UnityEditor.Editor
	{
		private const string OVERRIDE_START_SCENE_KEY = "Acciaio.Editor.DoOverrideStartScene";

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			string value = EditorPrefs.GetString(EditorScenesSetup.LOCAL_START_OVERRIDE_KEY, null);
			bool @override = EditorPrefs.GetBool(OVERRIDE_START_SCENE_KEY, false);

			EditorGUILayout.Space();

			if (@override != EditorGUILayout.Toggle("Specify Local Override", @override))
			{
				EditorPrefs.SetBool(OVERRIDE_START_SCENE_KEY, !@override);
				@override = !@override;
			}

			if (@override)
			{
				value = AcciaioEditor.SceneField("Local Override", value, false, null);
				EditorPrefs.SetString(EditorScenesSetup.LOCAL_START_OVERRIDE_KEY, value);
			}
			else EditorPrefs.DeleteKey(EditorScenesSetup.LOCAL_START_OVERRIDE_KEY);
		}	
	}
}
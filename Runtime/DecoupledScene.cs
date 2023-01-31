using System;
using UnityEngine.SceneManagement;

namespace Acciaio
{
	[System.Serializable]
	public struct DecoupledScene
	{
		private const string EditorScenePrefsKey = "Acciaio.Editor.EditingScene";
		
		[UnityEngine.SerializeField]
		private string _scene;

		public string Scene
		{
			get
			{
				var scene = _scene;
#if UNITY_EDITOR
				var editorScene = UnityEditor.EditorPrefs.GetString(EditorScenePrefsKey, null);
				if (!string.IsNullOrEmpty(editorScene) && SceneManager.GetActiveScene().name != editorScene)
					scene = editorScene;
#endif
				return scene;
			}
		}

		public DecoupledScene(string scene) => _scene = scene;

		public override string ToString() => Scene;

		public bool Equals(DecoupledScene s) => s.Scene.Equals(Scene, StringComparison.Ordinal);

		public bool Equals(string s) => s.Equals(Scene, StringComparison.Ordinal);

        public override bool Equals(object obj) 
	        => obj is DecoupledScene ds && Equals(ds) || obj is string s && Equals(s);

        public override int GetHashCode() => Scene.GetHashCode();

		public static implicit operator string(DecoupledScene ds) => ds.Scene;
		public static implicit operator DecoupledScene(string s) => new(s);

        public static bool operator ==(DecoupledScene ds1, DecoupledScene ds2) => ds1.Equals(ds2);
        public static bool operator !=(DecoupledScene ds1, DecoupledScene ds2) => !ds1.Equals(ds2);
		public static bool operator ==(DecoupledScene ds1, string s2) => ds1.Equals(s2);
        public static bool operator !=(DecoupledScene ds1, string s2) => !ds1.Equals(s2);
		public static bool operator ==(string s1, DecoupledScene ds2) => ds2.Equals(s1);
        public static bool operator !=(string s1, DecoupledScene ds2) => !ds2.Equals(s1);

    }
}
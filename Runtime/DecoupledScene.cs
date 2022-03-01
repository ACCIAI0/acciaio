using UnityEngine.SceneManagement;

namespace Acciaio
{
	[System.Serializable]
	public struct DecoupledScene
	{
		[UnityEngine.SerializeField]
		private string _scene;

		public string Scene
		{
			get
			{
				string scene = _scene;
#if UNITY_EDITOR
				string editorScene = UnityEditor.EditorPrefs.GetString("Acciaio.Editor.EditingScene", null);
				if (!string.IsNullOrEmpty(editorScene) && SceneManager.GetActiveScene().name != editorScene)
					scene = editorScene;
#endif
				return scene;
			}
		}

		public DecoupledScene(string scene) => _scene = scene;

		public override string ToString() => Scene;

		public bool Equals(DecoupledScene s) => s.Scene == Scene;

		public bool Equals(string s) => s == Scene;

		public override bool Equals(object obj)
		{
			if (obj is DecoupledScene ds) return Equals(ds);
			else if (obj is string s) return Equals(s);
			return false;
		}

        // override object.GetHashCode
        public override int GetHashCode() => Scene.GetHashCode();

		public static implicit operator string(DecoupledScene ds) => ds.Scene;
		public static implicit operator DecoupledScene(string s) => new DecoupledScene(s);

        public static bool operator ==(DecoupledScene ds1, DecoupledScene ds2) => ds1.Equals(ds2);
        public static bool operator !=(DecoupledScene ds1, DecoupledScene ds2) => !ds1.Equals(ds2);
		public static bool operator ==(DecoupledScene ds1, string s2) => ds1.Equals(s2);
        public static bool operator !=(DecoupledScene ds1, string s2) => !ds1.Equals(s2);

		public static bool operator ==(string s1, DecoupledScene ds2) => ds2.Equals(s1);
        public static bool operator !=(string s1, DecoupledScene ds2) => !ds2.Equals(s1);

    }
}
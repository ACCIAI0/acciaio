using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace Acciaio.Editor
{
	[CustomPropertyDrawer(typeof(DecoupledScene))]
	public sealed class DecoupledSceneDrawer : PropertyDrawer
	{
		private readonly SceneAttributeDrawer _drawer = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
            => _drawer.GetPropertyHeight(property.FindPropertyRelative("_scene"), label);

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			var scene = property.FindPropertyRelative("_scene");
			_drawer.OnGUI(rect, scene, label, false, null);
		}

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var scene = property.FindPropertyRelative("_scene");
			return _drawer.CreatePropertyGUI(scene, false, null);
		}
	}
}
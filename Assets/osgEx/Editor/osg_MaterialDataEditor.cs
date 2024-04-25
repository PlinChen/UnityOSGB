using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering;
using __Editor = UnityEditor.Editor;

namespace osgEx.Editor
{
    [CustomEditor(typeof(osg_MaterialData))]
    public class osg_MaterialDataEditor : __Editor
    {
        public osg_MaterialData targetData { get => (osg_MaterialData)target; }
        public override void OnInspectorGUI()
        {
            osg_MaterialData data = (osg_MaterialData)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_material"));

            if (data.Material != null && data.Material.shader != null)
            {
                DrawProperty(ShaderPropertyType.Texture, serializedObject.FindProperty("m_mainTexProperty"));
                DrawProperty(ShaderPropertyType.Color, serializedObject.FindProperty("m_ambientColorProperty"));
                DrawProperty(ShaderPropertyType.Color, serializedObject.FindProperty("m_diffuseColorProperty"));
                DrawProperty(ShaderPropertyType.Color, serializedObject.FindProperty("m_specularColorProperty"));
                DrawProperty(ShaderPropertyType.Color, serializedObject.FindProperty("m_emissionColorProperty"));
            }
            serializedObject.ApplyModifiedProperties();
        }
        void DrawProperty(ShaderPropertyType propertyType, SerializedProperty serializedProperty)
        {
            var shader = targetData.Material.shader;
            var nameList = new List<string>();
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                if (shader.GetPropertyType(i) == propertyType)
                {
                    var propertyName = shader.GetPropertyName(i);
                    nameList.Add(propertyName);
                }
            }
            var nameArray = nameList.ToArray();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(serializedProperty.displayName);
            var stringValue = serializedProperty.stringValue;
            var hasMainTexProperty = !string.IsNullOrWhiteSpace(serializedProperty.stringValue);
            if (EditorGUILayout.Toggle(hasMainTexProperty))
            {
                var texturePropertyIndex = hasMainTexProperty ? Array.IndexOf(nameArray, stringValue) : 0;
                texturePropertyIndex = EditorGUILayout.Popup(texturePropertyIndex, nameArray);
                serializedProperty.stringValue = nameArray[texturePropertyIndex];
            }
            else
            {
                serializedProperty.stringValue = null;

            }
            EditorGUILayout.EndHorizontal();


            serializedObject.ApplyModifiedProperties();
          
        }
    }
}

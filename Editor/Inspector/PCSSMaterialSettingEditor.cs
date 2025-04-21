namespace Numeira;

[CustomEditor(typeof(PCSSMaterialSetting))]
internal sealed class PCSSMaterialSettingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var target = this.target as PCSSMaterialSetting;
        if (target == null)
            return;

        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(PCSSMaterialSetting.Materials)));
        var ignoreProp = serializedObject.FindProperty(nameof(PCSSMaterialSetting.Ignore));
        EditorGUILayout.PropertyField(ignoreProp, new GUIContent("Exclude from conversion"));

        EditorGUIUtil.Separator();
        EditorGUI.BeginDisabledGroup(ignoreProp.boolValue);
        ShaderSettingDrawer.Draw(target.Setting.GetType(), serializedObject.FindProperty("Setting"), true);
        EditorGUI.EndDisabledGroup();

        serializedObject.ApplyModifiedProperties();
    }
}

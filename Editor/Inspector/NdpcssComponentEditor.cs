
namespace Numeira;

[CustomEditor(typeof(NonDestructivePCSS))]
internal sealed class NdpcssComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var target = this.target as NonDestructivePCSS;
        if (target is null)
            return;

        serializedObject.Update();

        ShaderSettingDrawer.Draw(target.ShaderSetting.GetType(), serializedObject.FindProperty("ShaderSetting"));

        serializedObject.ApplyModifiedProperties();
    }
}

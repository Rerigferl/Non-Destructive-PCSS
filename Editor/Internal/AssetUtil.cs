using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numeira;

internal static class AssetUtil
{
    public static T? LoadAssetFromGUID<T>(string guid) where T : Object
    {
        return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
    }
}

internal static class EditorGUIUtil
{

    public static void Separator(float? height = default)
    {
        var h = height ?? EditorGUIUtility.singleLineHeight;
        var rect = EditorGUILayout.GetControlRect(false, h);
        rect.y += rect.height / 2;
        rect.height = 1;
        EditorGUI.DrawRect(rect, Color.gray with { a = 0.1f });
    }

    private static GUIContent[] PopupToggleDisplayContnts = { new("Off"), new("On"), };

    public static void PopupToggle(Rect position, SerializedProperty property, GUIContent? label = null)
    {
        label ??= new(property.displayName);
        EditorGUI.BeginChangeCheck();
        var idx = EditorGUI.Popup(position, label, property.boolValue ? 1 : 0, PopupToggleDisplayContnts);
        if (EditorGUI.EndChangeCheck())
        {
            property.boolValue = idx != 0;
        }
    }
}
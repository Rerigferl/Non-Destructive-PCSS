namespace Numeira;

internal static class ShaderSettingDrawer
{
    public static void Draw(Type type, SerializedProperty property, bool showOverride = false)
    {
        DynamicEditorBuilder.OnPropertyGUI(type, property, (property, type, attributes) =>
        {
            var propertyType = typeof(IShaderProperty).IsAssignableFrom(type) ? type.GetGenericArguments()[0] : type;
            var rect = EditorGUILayout.GetControlRect(true, typeof(Texture).IsAssignableFrom(propertyType) ? 96 : EditorGUIUtility.singleLineHeight);

            var propertyName = property.displayName;
            if (attributes.OfType<DisplayNameAttribute>().FirstOrDefault() is { } displayNameAttr)
                propertyName = displayNameAttr.Name;

            if (typeof(IShaderProperty).IsAssignableFrom(type))
            {
                if (showOverride)
                {
                    var flag = property.FindPropertyRelative("Override");
                    EditorGUI.BeginChangeCheck();
                    var x = EditorGUI.ToggleLeft(rect with { width = EditorGUIUtility.labelWidth }, propertyName, flag.boolValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        flag.boolValue = x;
                    }
                    rect.width -= EditorGUIUtility.labelWidth + 3;
                    rect.x += EditorGUIUtility.labelWidth + 3;
                    EditorGUI.BeginDisabledGroup(!flag.boolValue);
                    Draw(rect, property.FindPropertyRelative("Value"), GUIContent.none);
                    EditorGUI.EndDisabledGroup();
                }
                else
                {
                    Draw(rect, property.FindPropertyRelative("Value"), new(propertyName));
                }
            }
            else
            {
                Draw(rect, property, new(propertyName));
            }

            void Draw(Rect position, SerializedProperty property, GUIContent label)
            {
                if (typeof(Texture).IsAssignableFrom(propertyType))
                {
                    position = EditorGUI.PrefixLabel(position, label);
                    position.x += position.width;
                    position.width = position.height;
                    position.x -= position.width;
                    EditorGUI.ObjectField(position, property, propertyType, GUIContent.none);

                    return;
                }

                if (property.propertyType == SerializedPropertyType.Boolean)
                {
                    EditorGUIUtil.PopupToggle(position, property, label);
                    return;
                }

                if (attributes.FirstOrDefault(x => x is CustomRangeAttribute) is CustomRangeAttribute { } range)
                {
                    if (property.propertyType == SerializedPropertyType.Integer)
                        EditorGUI.IntSlider(position, property, (int)range.Min, (int)range.Max, label);
                    else
                        EditorGUI.Slider(position, property, range.Min, range.Max, label);
                    return;
                }
                EditorGUI.PropertyField(position, property, label);
            }
        });
    }
}
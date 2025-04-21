using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
namespace Numeira;

internal abstract class DynamicEditorBuilder
{
    public delegate void DrawPropertyDelegate(SerializedProperty property, Type type, Attribute[] attributes);
    protected delegate void OnGUIDelegate(SerializedObject serializedObject, DrawPropertyDelegate drawProperty);
    protected delegate void OnPropertyGUIDelegate(SerializedProperty serializedProperty, DrawPropertyDelegate drawProperty);

    public static void OnGUI(Type type, SerializedObject serializedObject, DrawPropertyDelegate? drawProperty = null)
        => Get(type).OnGUI(serializedObject, drawProperty ?? ((x, _, _) => EditorGUILayout.PropertyField(x)));

    public static void OnPropertyGUI(Type type, SerializedProperty serializedObject, DrawPropertyDelegate? drawProperty = null)
        => Get(type).OnPropertyGUI(serializedObject, drawProperty ?? ((x, _, _) => EditorGUILayout.PropertyField(x)));

    private static Dictionary<Type, (OnGUIDelegate OnGUI, OnPropertyGUIDelegate OnPropertyGUI)> cache = new();
    private static (OnGUIDelegate OnGUI, OnPropertyGUIDelegate OnPropertyGUI) Get(Type type)
    {
        if (!cache.TryGetValue(type, out var x))
        {
            var type2 = typeof(DynamicEditorBuilder<>).MakeGenericType(type);
            var m1 = type2.GetField("_OnGUI", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as OnGUIDelegate;
            var m2 = type2.GetField("_OnPropertyGUI", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as OnPropertyGUIDelegate;
            x = (m1!, m2!);
            cache.Add(type, x);
        }
        return x;
    }
}

internal abstract class DynamicEditorBuilder<T> : DynamicEditorBuilder
{
    private static readonly OnGUIDelegate _OnGUI;
    private static readonly OnPropertyGUIDelegate _OnPropertyGUI;

    public static void OnGUI(SerializedObject serializedObject, DrawPropertyDelegate? drawProperty = null) => _OnGUI(serializedObject, drawProperty ?? ((x, _, _) => EditorGUILayout.PropertyField(x)));
    public static void OnPropertyGUI(SerializedProperty serializedProperty, DrawPropertyDelegate? drawProperty = null) => _OnPropertyGUI(serializedProperty, drawProperty ?? ((x, _, _) => EditorGUILayout.PropertyField(x)));


    static DynamicEditorBuilder()
    {
        _OnGUI = Create<OnGUIDelegate>();
        _OnPropertyGUI = Create<OnPropertyGUIDelegate>();
    }

    private static TDelegate Create<TDelegate>() where TDelegate : Delegate
    {
        var method = new DynamicMethod($"{typeof(T).Name}_{typeof(TDelegate).Name}", null, new[] { typeof(TDelegate) == typeof(OnGUIDelegate) ? typeof(SerializedObject) : typeof(SerializedProperty), typeof(DrawPropertyDelegate) }, true);
        var il = method.GetILGenerator();

        var propertyFieldMethod = GetMethod((SerializedProperty x, GUILayoutOption[] options) => EditorGUILayout.PropertyField(x, options));
        var findPropertyMethod = typeof(TDelegate) == typeof(OnGUIDelegate) ? GetMethod((SerializedObject so) => so.FindProperty("")) : GetMethod((SerializedProperty sp) => sp.FindPropertyRelative(""));
        var invoke = GetMethod((DrawPropertyDelegate d) => d.Invoke(null!, null!, null!));
        var getTypeFromHandle = GetMethod((RuntimeTypeHandle x) => Type.GetTypeFromHandle(x));

        foreach (var field in GetSerializableFields())
        {
            il.Emit(OpCodes.Ldarg_1);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, field.Name);
            il.Emit(OpCodes.Call, findPropertyMethod);


            il.Emit(OpCodes.Ldtoken, field.FieldType);
            il.Emit(OpCodes.Call, getTypeFromHandle);

            var attributes = field.GetCustomAttributes(true);
            il.Emit(OpCodes.Ldc_I4, attributes.Length);
            il.Emit(OpCodes.Newarr, typeof(Attribute));
            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = (Attribute)attributes[i];

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, i);

                CopyAttribute(il, attribute);
                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Callvirt, invoke);
        }
        il.Emit(OpCodes.Ret);
        return (TDelegate)method.CreateDelegate(typeof(TDelegate));
    }

    private static OnGUIDelegate CreateOnGUI()
    {
        var method = new DynamicMethod($"{typeof(T).Name}_OnGUI", null, new[] { typeof(SerializedObject), typeof(DrawPropertyDelegate) }, true);
        var il = method.GetILGenerator();

        var propertyFieldMethod = GetMethod((SerializedProperty x, GUILayoutOption[] options) => EditorGUILayout.PropertyField(x, options));
        var findPropertyMethod = GetMethod((SerializedObject so) => so.FindProperty(""));
        var invoke = GetMethod((DrawPropertyDelegate d) => d.Invoke(null!, null!, null!));
        var getTypeFromHandle = GetMethod((RuntimeTypeHandle x) => Type.GetTypeFromHandle(x));

        foreach (var field in GetSerializableFields())
        {
            il.Emit(OpCodes.Ldarg_1);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, field.Name);
            il.Emit(OpCodes.Call, findPropertyMethod);


            il.Emit(OpCodes.Ldtoken, field.FieldType);
            il.Emit(OpCodes.Call, getTypeFromHandle);

            var attributes = field.GetCustomAttributes(true);
            il.Emit(OpCodes.Ldc_I4, attributes.Length);
            il.Emit(OpCodes.Newarr, typeof(Attribute));
            for (int i = 0; i < attributes.Length; i++)
            {
                var attribute = (Attribute)attributes[i];

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4, i);

                CopyAttribute(il, attribute);
                il.Emit(OpCodes.Stelem_Ref);
            }

            il.Emit(OpCodes.Callvirt, invoke);
        }
        il.Emit(OpCodes.Ret);
        return (OnGUIDelegate)method.CreateDelegate(typeof(OnGUIDelegate));
    }

    private static void CopyAttribute(ILGenerator il, Attribute attribute)
    {
        var type = attribute.GetType();

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        var namedValues = properties.Select(x => (x.Name, x.GetValue(attribute))).Concat(fields.Select(x => (x.Name, x.GetValue(attribute)))).ToDictionary(x => x.Name, x => x.Item2, StringComparer.OrdinalIgnoreCase);
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); 

        if (namedValues.Count == 0 && constructors.Any(x => x.GetParameters().Length == 0 && x.IsPublic))
        {
            il.Emit(OpCodes.Newobj, constructors.FirstOrDefault());
            return;
        }

        ConstructorInfo? constructor = null;
        foreach(var x in constructors)
        {
            foreach(var param in x.GetParameters())
            {
                if (!namedValues.TryGetValue(param.Name, out var value) || value.GetType() != param.ParameterType)
                    goto Break;
            }

            constructor = x;
            break;

        Break:
            continue;
        }

        if (constructor != null)
        {
            foreach (var property in constructor.GetParameters())
            {
                namedValues.TryGetValue(property.Name, out var value);
                if (value is int i32)
                {
                    il.Emit(OpCodes.Ldc_I4, i32);
                }
                else if (value is float f32)
                {
                    il.Emit(OpCodes.Ldc_R4, f32);
                }
                else if (value is double d64)
                {
                    il.Emit(OpCodes.Ldc_R8, d64);
                }
                else if (value is string str)
                {
                    il.Emit(OpCodes.Ldstr, str);
                }
                else if (value is bool boolean)
                {
                    il.Emit(boolean ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                }
            }

            foreach (var property in properties)
            {
            }

            il.Emit(OpCodes.Newobj, constructor);
        }
        else
        {
            il.Emit(OpCodes.Ldnull);
        }
    }

    private static IEnumerable<FieldInfo> GetSerializableFields()
    {
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach(var field in fields.OrderBy(GetClassDepth))
        {
            if (field.IsLiteral || field.GetCustomAttribute<HideInInspector>() != null)
                continue;

            if (field.IsPrivate && field.GetCustomAttribute<SerializeField>() == null)
                continue;

            yield return field;
        }
    }

    private static int GetClassDepth(FieldInfo field)
    {
        var type = field.DeclaringType;
        int count = 0;
        while (type != null && type != typeof(object) && type != typeof(ValueType))
        {
            count++;
            type = type.BaseType;
        }
        return count;
    }

    private static MethodInfo? GetMethod<TExpression>(TExpression selector) where TExpression : Expression
    {
        var lambda = selector as LambdaExpression;
        var body = lambda?.Body as MethodCallExpression;
        return body?.Method;
    }
}

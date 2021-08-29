// See https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/StyledText.html for an intro to rich text in Unity

/// <summary>
/// A set of extension methods for the <see cref="string"/> class
/// to make it fast and easy for you to stylize and log text.
/// </summary>
public static class StringExtensionsForUnity
{
    /// <summary>Bolds the given string by surrounding it with &lt;b&gt; tags.</summary>
    public static string Bold(this string str) => $"<b>{str}</b>";

    /// <summary>Italicizes the given string by surrounding it with &lt;i&gt; tags.</summary>
    public static string Italics(this string str) => $"<i>{str}</i>";

    /// <summary>Sets the given string of text with a specific size by surrounding it in &lt;size&gt; tags.</summary>
    /// <param name="size">The size of the text in pixels.</param>
    public static string Size(this string str, int size) => $"<size={size}>{str}</size>";

    /// <summary>
    /// Colors the given string of text by surrounding it with &lt;color&gt; tags.
    /// The color value is based on the <paramref name="colorValue"/> parameter.
    /// </summary>
    /// <param name="colorValue">The color to use for the word. Will be converted into hexadecimal for use with the string.</param>
    public static string Color(this string str, UnityEngine.Color colorValue)
    {
        // need to convert from 0..1 to 0..255 and then to hexadecimal
        int redInt      = UnityEngine.Mathf.FloorToInt(colorValue.r * 255);
        int greenInt    = UnityEngine.Mathf.FloorToInt(colorValue.g * 255);
        int blueInt     = UnityEngine.Mathf.FloorToInt(colorValue.b * 255);
        int alphaInt    = UnityEngine.Mathf.FloorToInt(colorValue.a * 255);

        string colorString = $"#{redInt:X2}{greenInt:X2}{blueInt:X2}{alphaInt:X2}";

        return $"<color={colorString}>{str}</color>";
    }

    /// <summary>
    /// Sets the material of the given string to a specific material by surrounding it with &lt;material&gt; tags.
    /// Only works on text meshes.
    /// </summary>
    /// <param name="index">The index of a material in the text mesh's array of materials.</param>
    /// <returns></returns>
    public static string Material(this string str, string index) => $"<material={index}>{str}</material>";

    /// <summary>Logs the given string to the Unity logger.</summary>
    public static void Log(this string str) => UnityEngine.Debug.Log(str);

    /// <summary>Logs the given string to the Unity logger as a warning.</summary>
    public static void LogAsWarning(this string str) => UnityEngine.Debug.LogWarning(str);

    /// <summary>Logs the given string to the Unity logger as an error.</summary>
    public static void LogAsError(this string str) => UnityEngine.Debug.LogError(str);

    /// <summary>
    /// Logs the given string to the Unity logger, with a context to which
    /// this message applies to.
    /// </summary>
    /// <param name="context">
    /// A <see cref="UnityEngine.Object"/> to which this message applies to.
    /// Refer to the Unity documentation on <a href="https://docs.unity3d.com/ScriptReference/Debug.Log.com">Debug.Log</a>
    /// for more information on using a <see cref="UnityEngine.Object"/> as context.
    /// </param>
    public static void Log(this string str, UnityEngine.Object context) => UnityEngine.Debug.Log(str, context);

    /// <summary>
    /// Logs the given string to the Unity logger as a warning, with a context to which
    /// this message applies to.
    /// </summary>
    /// <param name="context">
    /// A <see cref="UnityEngine.Object"/> to which this warning applies to.
    /// Refer to the Unity documentation on <a href="https://docs.unity3d.com/ScriptReference/Debug.Log.com">Debug.Log</a>
    /// for more information on using a <see cref="UnityEngine.Object"/> as context.
    /// </param>
    public static void LogAsWarning(this string str, UnityEngine.Object context) => UnityEngine.Debug.LogWarning(str, context);

    /// <summary>
    /// Logs the given string to the Unity logger as an error, with a context to which
    /// this message applies to.
    /// </summary>
    /// <param name="context">
    /// A <see cref="UnityEngine.Object"/> to which this error applies to.
    /// Refer to the Unity documentation on <a href="https://docs.unity3d.com/ScriptReference/Debug.Log.com">Debug.Log</a>
    /// for more information on using a <see cref="UnityEngine.Object"/> as context.
    /// </param>
    public static void LogAsError(this string str, UnityEngine.Object context) => UnityEngine.Debug.LogError(str, context);
}

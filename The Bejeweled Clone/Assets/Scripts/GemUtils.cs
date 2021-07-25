using UnityEngine;

public static class GemUtils
{
    public static Color GetColorBasedOnGemType(GemTypes gemType) =>
        gemType switch
        {
            GemTypes.Red => Color.red,
            GemTypes.Orange => new Color(1f, 0.75f, 0f),
            GemTypes.Yellow => Color.yellow,
            GemTypes.Green => Color.green,
            GemTypes.Blue => Color.blue,
            GemTypes.Purple => Color.magenta,
            GemTypes.White => Color.white,
            _ => Color.black,
        };
}
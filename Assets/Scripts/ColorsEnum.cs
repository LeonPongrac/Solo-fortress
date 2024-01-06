using UnityEngine;

public enum PlayerColors
{
    Blue,
    Brown,
    Cyan,
    Green,
    Orange,
    Purple,
    Red,
    Yellow
}

// function that accepts enum PlayerColors and returns Color data type for each color

public static class PlayerColorUtils
{
    public static Color GetColor(this PlayerColors color)
    {
        switch (color)
        {
            case PlayerColors.Blue:
                return new Color(0.0f, 0.0f, 1.0f);
            case PlayerColors.Brown:
                return new Color(0.5f, 0.25f, 0.0f);
            case PlayerColors.Cyan:
                return new Color(0.0f, 1.0f, 1.0f);
            case PlayerColors.Green:
                return new Color(0.0f, 1.0f, 0.0f);
            case PlayerColors.Orange:
                return new Color(1.0f, 0.5f, 0.0f);
            case PlayerColors.Purple:
                return new Color(0.5f, 0.0f, 0.5f);
            case PlayerColors.Red:
                return new Color(1.0f, 0.0f, 0.0f);
            case PlayerColors.Yellow:
                return new Color(1.0f, 1.0f, 0.0f);
            default:
                return new Color(0.0f, 0.0f, 0.0f);
        }
    }
}
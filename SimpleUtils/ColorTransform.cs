namespace SimpleUtils;

public class ColorTransform
{
    public static void Transform()
    {
        // ColorDistributed("ff080808", "fff4f4f4");
        ColorAdd("ff317c83", "ff080808");
    }

    private static void ColorAdd(string baseColor, string addColor)
    {
        var valueR = Convert.ToInt32(addColor[2..4], 16);
        var valueG = Convert.ToInt32(addColor[4..6], 16);
        var valueB = Convert.ToInt32(addColor[6..8], 16);
        var valueBaseR = Convert.ToInt32(baseColor[2..4], 16);
        var valueBaseG = Convert.ToInt32(baseColor[4..6], 16);
        var valueBaseB = Convert.ToInt32(baseColor[6..8], 16);
        for (var alpha = 0f; alpha <= 1f; alpha += 0.1f)
        {
            var newValueR = ((int)(valueR * alpha + valueBaseR * (1 - alpha))).ToString("x");
            var newValueG = ((int)(valueG * alpha + valueBaseG * (1 - alpha))).ToString("x");
            var newValueB = ((int)(valueB * alpha + valueBaseB * (1 - alpha))).ToString("x");

            var newColor = "ff" + newValueR + newValueG + newValueB;
            Console.WriteLine(newColor);
        }
    }

    private static void ColorDistributed(string baseColor, string baseColor2)
    {
        var valueBaseR = Convert.ToInt32(baseColor[2..4], 16);
        var valueBaseG = Convert.ToInt32(baseColor[4..6], 16);
        var valueBaseB = Convert.ToInt32(baseColor[6..8], 16);
        var valueBase2R = Convert.ToInt32(baseColor2[2..4], 16);
        var valueBase2G = Convert.ToInt32(baseColor2[4..6], 16);
        var valueBase2B = Convert.ToInt32(baseColor2[6..8], 16);

        for (var i = 0f; i <= 1f; i += 0.1f)
        {
            var newValueR = ((int)(valueBaseR * i + valueBase2R * (1 - i))).ToString("x");
            var newValueG = ((int)(valueBaseG * i + valueBase2G * (1 - i))).ToString("x");
            var newValueB = ((int)(valueBaseB * i + valueBase2B * (1 - i))).ToString("x");
            var newColor = "ff" + newValueR + newValueG + newValueB;
            Console.WriteLine(newColor);
        }
    }
}
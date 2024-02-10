namespace SimpleUtils;

public class ColorTransform
{
    public static void Transform()
    {
        var colorWhite = "fff4f4f4";
        var colorPrimarys = new List<string>
        {
            "e5317c83",
            "cc317c83",
            "b2317c83",
            "99317c83",
            "5f317c83",
            "66317c83",
            "4c317c83",
            "33317c83",
            "19317c83",
        };

        foreach (var colorPrimary in colorPrimarys)
        {
            Console.WriteLine(ColorAdd(colorWhite, colorPrimary));
        }
    }

    private static string ColorAdd(string baseColor, string addColor)
    {
        var alpha = Convert.ToInt32(addColor[0..2], 16) / 255f;
        var valueR = Convert.ToInt32(addColor[2..4], 16);
        var valueG = Convert.ToInt32(addColor[4..6], 16);
        var valueB = Convert.ToInt32(addColor[6..8], 16);
        var valueBaseR = Convert.ToInt32(baseColor[2..4], 16);
        var valueBaseG = Convert.ToInt32(baseColor[4..6], 16);
        var valueBaseB = Convert.ToInt32(baseColor[6..8], 16);

        var newValueR = ((int)(valueR * alpha + valueBaseR * (1 - alpha))).ToString("x");
        var newValueG = ((int)(valueG * alpha + valueBaseG * (1 - alpha))).ToString("x");
        var newValueB = ((int)(valueB * alpha + valueBaseB * (1 - alpha))).ToString("x");

        var newColor = "ff" + newValueR + newValueG + newValueB;
        return newColor;
    }
}
namespace XApi.Common.Extensions;

public static class StringExtensions
{
    public static string CapitalizeFirstLetter(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }

    public static string CapitalizeFirstLetterOfEachWord(this string input)
        => string.Join(' ', input.Split(' ').Select(input => input.CapitalizeFirstLetter()));
}

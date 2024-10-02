namespace XApi.Common.Extensions;

public static class EnumExtensions
{
    public static TEnum? ParseEnum<TEnum>(this int? value) where TEnum : struct, Enum
    {
        if (value == null) return null;

        if (Enum.IsDefined(typeof(TEnum), value))
        {
            return (TEnum)(object)value;
        }
        else
        {
            return default;
        }
    }
}
namespace EmulsiveStoreE2E.Core.Helpers;

public static class EnumHelper
{
    public static string GetDescription(this Enum genericEnum)
    {
        var genericEnumType = genericEnum.GetType();
        var memberInfo = genericEnumType.GetMember(genericEnum.ToString());

        if (memberInfo.Length <= 0) return genericEnum.ToString();

        var attributes = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

        return attributes.Any() ? ((System.ComponentModel.DescriptionAttribute)attributes.ElementAt(0)).Description : genericEnum.ToString();
    }
}
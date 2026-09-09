using System.Globalization;

namespace Client.Helpers;

public static class LocalizationHelper
{
    public static string ToPersianDigits(this object input)
    {
        if (input == null) return string.Empty;
        return input.ToString()!
            .Replace('0', '۰')
            .Replace('1', '۱')
            .Replace('2', '۲')
            .Replace('3', '۳')
            .Replace('4', '۴')
            .Replace('5', '۵')
            .Replace('6', '۶')
            .Replace('7', '۷')
            .Replace('8', '۸')
            .Replace('9', '۹');
    }
    public static string ToPersianDateTimeString(DateTime? gregorianDate)
    {
        if (!gregorianDate.HasValue) return "";
        
        var localDate = gregorianDate.Value.ToLocalTime();
        var pCal = new PersianCalendar();
        
        string year = pCal.GetYear(localDate).ToString();
        string month = pCal.GetMonth(localDate).ToString("00");
        string day = pCal.GetDayOfMonth(localDate).ToString("00");
        string time = localDate.ToString("HH:mm");

        string finalString = $"{time} - {year}/{month}/{day}";
        
        // Finally, convert the whole string to Persian digits
        return finalString.ToPersianDigits();
    }
}
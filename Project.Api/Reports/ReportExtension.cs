namespace Project.Api;

public static class ReportExtension
{
    public static string ConvertNumerals(this string input)
    {
        string formattedDate =
                input.Replace('0', '\u0660')
                .Replace('1', '\u0661')
                .Replace('2', '\u0662')
                .Replace('3', '\u0663')
                .Replace('4', '\u0664')
                .Replace('5', '\u0665')
                .Replace('6', '\u0666')
                .Replace('7', '\u0667')
                .Replace('8', '\u0668')
                .Replace('9', '\u0669');
        char[] charArray = formattedDate.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
    private static IContainer Cell(this IContainer container, bool dark)
    {
        return container
            .Border(1)
            .Background(dark ? Colors.Grey.Lighten2 : Colors.White)
            .Padding(10);
    }

    public static TextSpanDescriptor ArabicDate(this IContainer container, DateTime date, string format = "yyyy/MM/dd")
    {
        return
        container.Text(date.ToString(format).Replace('0', '\u0660')
                                            .Replace('1', '\u0661')
                                            .Replace('2', '\u0662')
                                            .Replace('3', '\u0663')
                                            .Replace('4', '\u0664')
                                            .Replace('5', '\u0665')
                                            .Replace('6', '\u0666')
                                            .Replace('7', '\u0667')
                                            .Replace('8', '\u0668')
                                            .Replace('9', '\u0669'))
                                            .DirectionFromLeftToRight();

    }
    public static string ArabicDateAsString(this DateTime date, string format = "yyyy/MM/dd")
    {
        string formattedDate=
        date.ToString(format).Replace('0', '\u0660')
                                            .Replace('1', '\u0661')
                                            .Replace('2', '\u0662')
                                            .Replace('3', '\u0663')
                                            .Replace('4', '\u0664')
                                            .Replace('5', '\u0665')
                                            .Replace('6', '\u0666')
                                            .Replace('7', '\u0667')
                                            .Replace('8', '\u0668')
                                            .Replace('9', '\u0669');
        char[] charArray=formattedDate.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
    public static string ArabicDateAsStringNotExtensionMethod( DateTime? date, string format = "yyyy/MM/dd")
    {
        string formattedDate =
        date?.ToString(format).Replace('0', '\u0660')
                                            .Replace('1', '\u0661')
                                            .Replace('2', '\u0662')
                                            .Replace('3', '\u0663')
                                            .Replace('4', '\u0664')
                                            .Replace('5', '\u0665')
                                            .Replace('6', '\u0666')
                                            .Replace('7', '\u0667')
                                            .Replace('8', '\u0668')
                                            .Replace('9', '\u0669');
        char[] charArray = formattedDate.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public static DateTime ToGregorian(this DateTime date)
    {
        GregorianCalendar gregorianCalendar = new GregorianCalendar();
        return new DateTime(date.Year,date.Month,date.Day,date.Hour,date.Minute,date.Second,gregorianCalendar);
    }

}

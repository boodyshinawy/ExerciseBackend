namespace Project.Shared;

static class SimpleExtension
{
    private static IContainer BaseCell(this IContainer container, bool dark)
    {
        return container
            .Border(1)
            .Background(dark ? Colors.Grey.Lighten2 : Colors.White);
    }

    private static IContainer Cell(this IContainer container, bool dark)
    {
        return BaseCell(container, dark)
            .AlignCenter()
            .AlignMiddle();
    }

    private static IContainer LeftCell(this IContainer container, bool dark)
    {
        return BaseCell(container, dark)
            .AlignMiddle()
            .PaddingHorizontal(5);
    }

    // displays only text label
    public static void LabelCell(this IContainer container, string text, bool isBold = false, bool left = false)
    {
        var selectedContainer = left ? container.LeftCell(false) : container.Cell(false);

        if (isBold)
            selectedContainer.Text(text).Bold();
        else
            selectedContainer.Text(text).Medium();
    }

    public static IContainer ValueCell(this IContainer container) => container.Cell(false);
}
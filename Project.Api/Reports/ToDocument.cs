using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Globalization;
namespace Project.Api;
public class ToDocument : IDocument
{
    public List<SeizureRequestDetailReadDto> SeizureRequestDetailReadDtos { get; set; }
    public string _imagePath { get; set; }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title = "Seizure Report",
        Subject = "Seizure",
        Author = "Msdc",
        Creator = "Shaqwieer",
        Keywords = "To",
    };

    public DocumentSettings GetSettings() => new DocumentSettings
    {
        ContentDirection = ContentDirection.RightToLeft,

    };

    public void Compose(IDocumentContainer container)
    {
        container
           .Page(page =>
           {
               page.Margin(25);
               page.Size(PageSizes.A4.Landscape());
               page.DefaultTextStyle(x => x.FontFamily(Fonts.Calibri).FontSize(16));
               page.ContentFromRightToLeft();
               page.Background().Layers(layer =>
               {
                   layer.PrimaryLayer()
                          .AlignCenter()
                          .AlignBottom()
                          .Rotate(315)
                          .PaddingBottom(90)
                          .Text("هيئة شئون مالية")
                          .FontSize(70).Bold().FontColor(Colors.Grey.Lighten2);
               });
               page.Header().BorderBottom(0).BorderLeft(4).BorderRight(4).BorderTop(4).PaddingHorizontal(20).PaddingTop(15).Element(ComposeHeader);
               page.Content().BorderBottom(4).BorderLeft(4).BorderRight(4).BorderTop(0).PaddingHorizontal(20).PaddingTop(10).Element(ComposeContent);
           });
    }

    void ComposeHeader(IContainer container)
    {
        var titleStyle = TextStyle.Default.Black().FontSize(13);
        var period = "عام";
        container.Row(row =>
        {
            row.RelativeItem().AlignRight().Column(column =>
            {

                column.Item().AlignCenter().Text("وزارة الدفاع").LineHeight(0.7f).Style(titleStyle);
                column.Item().AlignCenter().Text("هيئة شئون المالية للقوات المسلحة").LineHeight(0.8f).Style(titleStyle);
                //column.Item().AlignCenter().Text("فـــــــــــــــــــــــــــــــــــــــــــــــــــرع الادارة العسكريــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــة").LineHeight(0.7f).Underline().Style(titleStyle);
                column.Item().AlignCenter().Text(text =>
                {
                    text.Element().Text(" التـــــــــــــــــــــــــــــــــــــــــــــــــــــــــــاريــــــــــــــــــــــــــــــــــــــخ : ").Underline().Style(titleStyle);
                    text.Element().ArabicDate(DateTime.Now).Underline().Style(titleStyle);
                });


            });

            //row.RelativeItem().AlignLeft().Text("سرى").Underline().FontSize(10).ExtraBold();

            var image = Path.Combine(_imagePath, "Reports", "finance.png");
            if (!string.IsNullOrEmpty(image))
                row.ConstantItem(60).Image(image).FitArea();

            //else
            //row.ConstantItem(100).AlignCenter().AlignMiddle().Height(50).Placeholder();
        });


    }
            

    void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Element(ComposeFirstBlock);
            column.Item().Element(ComposeTable);
            //column.Item().Element(ComposeSecondBlock);
            //column.Item().Element(ComposeSignature);
            //column.Item().Element(ComposeThirdBlock);


        });



        //column.Item().PaddingBottom(15).Element(ComposeTable);
        //column.Item().Element(ComposeSignatures);


    }


    void ComposeFirstBlock(IContainer container)
    {
        var titleSize = 12;
        container.Column(column =>
        {
            column.Item().PaddingVertical(4).AlignCenter().AlignMiddle().Column(col =>
            {
                col.Item().AlignCenter().Text("تقرير مفصل بالهدايا").Underline().FontSize(14);

            });
        });
    }
    //void ComposeSecondBlock(IContainer container)
    //{
    //    var bodySize = 13;
    //    var husbandName = "محمد السيد السيد محمد شاهين ";
    //    var name = "منى فرحات عبد الوهاب محمد عفيفى ";
    //    var period = "عام ";
    //    var location = "مستشفى العسكرى بالمنوفية";
    //    container.Column(column =>
    //    {
    //        column.Item().PaddingTop(12).PaddingRight(10).Text("يرجى التكرم بالاحاطة بالأتى :").Underline().ExtraBold().FontSize(15);
    //        //column.Item().Width(60).PaddingVertical(1).LineHorizontal(1).LineColor(Colors.Grey.Darken3);
    //        column.Item().PaddingTop(8).Text($"صودق على منح المذكورة بعاليه أجازة بدون راتب لمدة {VacationDetail.VacationPeriod} لرعاية طفلها و على نفقتها الخاصة دون أن تتحمل القوات المسلحة أى نفقات أو التزامات مالية نتيجة لذلك .").ExtraBold().FontSize(bodySize);
    //        column.Item().PaddingTop(12).AlignCenter().Text("مرسل للتكرم باتخاذ اللازم و موافاتنا بيومية السير الدالة على القيام والعودة .").ExtraBold().FontSize(15);
    //        column.Item().PaddingTop(12).AlignCenter().Text("مع وافر التحية ,,,").ExtraBold().FontSize(15);

    //    });
    //}

    //void ComposeThirdBlock(IContainer container)
    //{
    //    var bodySize = 13;
    //    var name = "محمد حسين ";
    //    var location = "مستشفى العسكرى بالمنوفية";
    //    var specificDate = new DateTime(1995, 8, 2);
    //    container.Column(column =>
    //    {
    //        column.Item().Text("صورة الى :").Underline().ExtraBold().FontSize(13);
    //        column.Item().Text("-  هيئة الشئون المالية للقوات المسلحة .").ExtraBold().FontSize(bodySize);
    //        column.Item().Text("- هيئة الاستخبارات العسكرية / جهاز الأمن الحربى .").ExtraBold().FontSize(bodySize);
    //        column.Item().Text(" - إدارة السجلات العسكرية ").ExtraBold().FontSize(bodySize);
    //        column.Item().PaddingBottom(50).Text("- فرع الافراد بالهيئة ").ExtraBold().FontSize(bodySize);

    //        column.Item().Width(200).Border(1).Column(col =>
    //        {
    //            col.Item().PaddingHorizontal(4).Text($"المختص : رائد / {name}  MDC  {"73590".ConvertNumerals()} ").ExtraBold().FontSize(11);
    //            col.Item().PaddingHorizontal(4).Text("نسخها : مساعد / وليد عبد الظاهر ابرايهم ").ExtraBold().FontSize(11);

    //        });

    //    });

    //}
    //void ComposeSignature(IContainer container)
    //{
    //    var bodySize = 15;
    //    var name = "يســــــــــــرى فــــــــــــاروق حســـــــــــــــــــــانين ";
    //    var location = "مستشفى العسكرى بالمنوفية";
    //    var job = "نائب رئيس هيئة التنظيم و ألادارة للقوات المسلحة ";
    //    var specificDate = new DateTime(1995, 8, 2);
    //    container.Column(column =>
    //    {
    //        column.Item().PaddingTop(6).PaddingBottom(2).Column(
    //            handler: col =>
    //            {
    //                column.Item().PaddingBottom(2).AlignLeft().Column(handler: col =>
    //                {
    //                    col.Item().PaddingBottom(8).Text("التوقيع / ").ExtraBold().FontSize(bodySize);
    //                    col.Item().AlignCenter().Text($"لواء  / {name}").ExtraBold().FontSize(bodySize);
    //                    col.Item().PaddingBottom(2).AlignCenter().Text($"{job}").ExtraBold().FontSize(bodySize);
    //                });
    //            });
    //    });

    //}

    void ComposeTable(IContainer container)
    {
        var fontStyle = TextStyle.Default.FontSize(8);
        var seizure = new SeizureRequestDetailReadDto()
        {
            SeizureRequestId = 47,
            ItemId = 1,
            ItemName = "تلفزيون سامسونج 42 بوصة",
            CategoryName = "اجهزة كهربائية",
            FirmName = "مركز إنتاج البرمجيات الرئيسي ق.م",
            CategoryId = 1,
            CategoryColor = "success",
            StatusId = (int)SeizureStatus.Completed,
            StatusName = "مكتمل",
            Id = 2
        };


        container.PaddingTop(15).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.ConstantColumn(50);
            });

            table.Header(header =>
            {
                header.Cell().Row(1).Column(1).Element(CellHeaderStyle).Text("التصنيف").Style(fontStyle).ExtraBold();
                header.Cell().Row(1).Column(2).Element(CellHeaderStyle).Text("الصنف").Style(fontStyle).ExtraBold();
                header.Cell().Row(1).Column(3).Element(CellHeaderStyle).Text("الوحدة").Style(fontStyle).ExtraBold();
                header.Cell().Row(1).Column(4).Element(CellHeaderStyle).Text("الحالة").Style(fontStyle).ExtraBold();

                static IContainer CellHeaderStyle(IContainer container)
                {
                    return container.Border(1)
                                    .BorderColor(Colors.Grey.Darken1)
                                    .Background(Colors.Grey.Lighten4)
                                    .PaddingVertical(2)
                                    .PaddingHorizontal(2)
                                    .AlignCenter()
                                    .AlignMiddle();
                }

            });


            foreach(var s in SeizureRequestDetailReadDtos )
            {
                table.Cell().Element(CellStyle).Text(s.CategoryName).Style(fontStyle);
                table.Cell().Element(CellStyle).Text(s.ItemName).Style(fontStyle);
                table.Cell().Element(CellStyle).Text(s.FirmName).Style(fontStyle);
                table.Cell().Element(CellStyle).Text(s.StatusName).Style(fontStyle);
            }



            static IContainer CellStyle(IContainer container)
            {
                return container.Border(1)
                                .BorderColor(Colors.Grey.Darken1)
                                .PaddingVertical(2)
                                .AlignMiddle()
                                .AlignCenter()
                                .PaddingHorizontal(2);
            }
        });
    }

}

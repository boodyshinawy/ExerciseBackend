
namespace Project.Api;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

public class PdfConverter
{
    public static byte[] ConvertImageToPdf(byte[] imageData)
    {
        using (var memoryStream = new MemoryStream())
        {
            var document = new Document();
            var pdfWriter = PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Load the image
            var image = Image.GetInstance(imageData);

            // Scale the image to fit within the page while preserving the aspect ratio
            image.ScaleToFit(document.PageSize.Width - document.LeftMargin - document.RightMargin,
                             document.PageSize.Height - document.TopMargin - document.BottomMargin);

            // Center the image horizontally and vertically
            image.Alignment = Element.ALIGN_CENTER;

            // Calculate the position to center the image vertically
            float imageHeight = image.ScaledHeight;
            float imagePosition = (document.PageSize.Height - imageHeight) / 2;
            image.SetAbsolutePosition((document.PageSize.Width - image.ScaledWidth) / 2, imagePosition);

            document.Add(image);
            document.Close();

            return memoryStream.ToArray();
        }
    }
}
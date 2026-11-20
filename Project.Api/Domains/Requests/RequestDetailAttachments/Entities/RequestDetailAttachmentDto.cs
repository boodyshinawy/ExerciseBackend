using System.Reflection.Metadata;

namespace Project.Api;

public class RequestDetailAttachmentDto : BaseDto
{
    public int SeizureRequestDetailId { get; set; }
    public string? FileData { get; set; }
    public string? FileName { get; set; }
    public bool? IsPdf { get; set; }
}


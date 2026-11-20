using System.Net.Mail;
using static iTextSharp.text.pdf.events.IndexEvents;

namespace Project.Api;

public class RequestDetailAttachmentRepository : BaseRepository<RequestDetailAttachment>, IRequestDetailAttachmentRepository
{
    private readonly IConfiguration _configuration;

    public RequestDetailAttachmentRepository(ApplicationDbContext context, IConfiguration configuration) : base(context)
    {
        _configuration = configuration;
    }

    public async Task<List<RequestDetailAttachmentDto>> GetRequestDetailAttachmensts(int seizureRequestDetailId)
    {
        try
        {
            var attachments = await _dbSet.Where(e => e.SeizureRequestDetailId == seizureRequestDetailId).ToListAsync();
            var attachemntDtos = new List<RequestDetailAttachmentDto>();
            foreach (var attachment in attachments)
            {
                var attachmentDto = new RequestDetailAttachmentDto
                {
                    SeizureRequestDetailId = attachment.SeizureRequestDetailId,
                    Id = attachment.Id,
                    IsPdf = attachment.IsPdf
                };

                if (!string.IsNullOrEmpty(attachment.FilePath) && File.Exists(attachment.FilePath))
                {
                    var ext = Path.GetExtension(attachment.FilePath).TrimStart('.').ToLower();
                    var isPdf = ext == "pdf";
                    attachmentDto.IsPdf = isPdf;
                    attachmentDto.FileName = Path.GetFileName(attachment.FilePath);

                    var mimeType = isPdf 
                        ? "application/pdf" 
                        : (ext == "jpg" || ext == "jpeg" ? "image/jpeg" : (ext == "png" ? "image/png" : (ext == "webp" ? "image/webp" : $"image/{ext}")));

                    var blob = await File.ReadAllBytesAsync(attachment.FilePath);
                    attachmentDto.FileData = $"data:{mimeType};base64,{Convert.ToBase64String(blob)}";
                }

                attachemntDtos.Add(attachmentDto);
            }
            return attachemntDtos;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetRequestDetailAttachmensts", ex);
            throw new RepositoryException("An error occurred while GetRequestDetailAttachmensts.", ex);
        }
    }

    public async Task CreateRequestDetailAttachmensts(int seizureRequestDetailId, List<RequestDetailAttachmentUploading> requestDetailAttachmentUploadings, string firmCode)
    {
        var fallbackFiles = new List<string>();

        try
        {
            var firmName = await _context.Set<Firm>().Where(e => e.Id == firmCode).Select(e => e.Name).FirstOrDefaultAsync();
            var uploadPath = _configuration["PhysicalFile:FilePath"];
            var remainingPath = Path.Combine(uploadPath, firmName, seizureRequestDetailId.ToString());
            var attachmentObjects = new List<RequestDetailAttachment>();
            if (!Directory.Exists(remainingPath))
            {
                Directory.CreateDirectory(remainingPath);
            }
            foreach (var attachment in requestDetailAttachmentUploadings)
            {
                var attachmentObject = new RequestDetailAttachment
                {
                    SeizureRequestDetailId = seizureRequestDetailId,
                    IsPdf = attachment != null && Path.GetExtension(attachment.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                };
                //string filenameWithExtension = Path.ChangeExtension(attachment.FileName, Path.GetExtension(attachment.File.FileName));
                var filePath = Path.Combine(remainingPath, attachment.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await attachment.FileData.CopyToAsync(stream);
                    fallbackFiles.Add(filePath);
                }
                attachmentObject.FilePath = filePath;
                attachmentObjects.Add(attachmentObject);
            }
            await _dbSet.AddRangeAsync(attachmentObjects);
            await SaveChangesAsync();
        }
        catch (Exception ex)
        {
            foreach(var fallbackFile in fallbackFiles)
            {
                File.Delete(fallbackFile);
            }
            Log.Error(ex, "Error occurred while CreateRequestDetailAttachmensts", ex);
            throw new RepositoryException("An error occurred while CreateRequestDetailAttachmensts.", ex);
        }
    }

    public async Task DeleteRequestDetailAttachmensts(int attachemntId)
    {
        try
        {
            var attachment = await _dbSet.FirstOrDefaultAsync(e => e.Id == attachemntId);
            if (attachment == null)
            {
                throw new Exception("Not Found ");
            }
            if (File.Exists(attachment.FilePath))
            {
                File.Delete(attachment.FilePath);
            }
            _dbSet.Remove(attachment);
            await SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while DeleteRequestDetailAttachmensts", ex);
            throw new RepositoryException("An error occurred while DeleteRequestDetailAttachmensts.", ex);
        }
    }

}

namespace Project.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SaleEntitiesController : BaseController<SaleEntity, SaleEntityDto>
{
    public SaleEntitiesController(ISaleEntityService service) : base(service)
    {
    }
}

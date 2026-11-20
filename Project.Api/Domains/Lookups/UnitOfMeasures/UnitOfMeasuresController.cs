using Microsoft.AspNetCore.Mvc;
using Project.Shared;

namespace Project.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UnitOfMeasuresController : BaseController<UnitOfMeasure, UnitOfMeasureDto>
{
    private readonly IUnitOfMeasureService _service;

    public UnitOfMeasuresController(IUnitOfMeasureService service) : base(service)
    {
        _service = service;
    }
}

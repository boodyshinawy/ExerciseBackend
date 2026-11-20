using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


using System.Linq;
using System.Threading.Tasks;

namespace Project.Api
{
    public class StatusService : BaseService<Status, StatusDto>, IStatusService
    {
        private readonly IStatusRepository _statusRepository;

        public StatusService(
            IValidator<Status> validator,
            IStatusRepository repository,
            IMapper mapper,
            IStringLocalizer<SharedService> localizer)
            : base(repository, validator, mapper, localizer)
        {
            _statusRepository = repository;
        }


    }


}

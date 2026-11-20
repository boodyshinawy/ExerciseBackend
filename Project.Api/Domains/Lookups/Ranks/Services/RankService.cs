using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


using System.Linq;
using System.Threading.Tasks;

namespace Project.Api
{
    public class RankService : BaseService<Rank, RankDto>, IRankService
    {
        private readonly IRankRepository _rankRepository;

        public RankService(
            IValidator<Rank> validator,
            IRankRepository repository,
            IMapper mapper,
            IStringLocalizer<SharedService> localizer)
            : base(repository, validator, mapper, localizer)
        {
            _rankRepository = repository;
        }


    }


}

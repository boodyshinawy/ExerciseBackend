using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


using System.Linq;
using System.Threading.Tasks;

namespace Project.Api
{
    public class PageService : BaseService<Page, PageDto>, IPageService
    {
        private readonly IPageRepository _pageRepository;

        public PageService(
            IValidator<Page> validator,
            IPageRepository repository,
            IMapper mapper,
            IStringLocalizer<SharedService> localizer)
            : base(repository, validator, mapper, localizer)
        {
            _pageRepository = repository;
        }


    }


}

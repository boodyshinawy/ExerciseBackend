using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Project.Api;

public class PageRoleService : BaseService<PageRole, PageRoleDto>, IPageRoleService
{
    private readonly IPageRoleRepository _pageRoleRepository;

    private readonly IPageRepository _pageRepository;
    public PageRoleService(IValidator<PageRole> validator, IPageRoleRepository repository, IMapper mapper, IStringLocalizer<SharedService> localizer, IPageRepository pageRepository) : base(repository, validator, mapper, localizer)
    {
        _pageRoleRepository = repository;
        _pageRepository = pageRepository;
    }
    public async Task<OperationResult<List<PagesOnRole>>> GetPageNamesInRole(string roleId)
    {
        try
        {
            // Pages can be added after a role already exists, so it might not have a
            // PageRole row for every page yet. We start from the full page list and fill in
            // any missing ones as inactive, so every page always shows up for every role.
            var allPages = await _pageRepository.Get();
            var pageNameWithRoleName = await _pageRoleRepository.GetPageNamesInRole(roleId);
            var pageRoleByPageId = pageNameWithRoleName.ToDictionary(pageRole => pageRole.PageId);

            List<PagesOnRole> pageNameWithRoleNameDto = allPages
                .Where(page => !page.IsDeleted)
                .OrderBy(page => page.SortNumber)
                .Select(page => pageRoleByPageId.TryGetValue(page.Id, out var existingPageRole)
                    ? _mapper.Map<PagesOnRole>(existingPageRole)
                    : new PagesOnRole
                    {
                        PageId = page.Id.ToString(),
                        RoleId = roleId,
                        PageName = page.Label,
                        CreateAction = false,
                        ReadAction = false,
                        UpdateAction = false,
                        DeleteAction = false
                    })
                .ToList();

            return OperationResult<List<PagesOnRole>>.Succeeded(_localizer["PagesRetrievedSuccessfully"], pageNameWithRoleNameDto);

        }
        catch (RepositoryException ex)
        {
            return OperationResult<List<PagesOnRole>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
        }
        catch (Exception ex)
        {
            return OperationResult<List<PagesOnRole>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
        }

    }
    public async Task<OperationResult<List<PagesOnRole>>> SavePermissions(List<PagesOnRole> pagesOnRole)
    {
        var pagesOnRoleClone = pagesOnRole;
        using IDbContextTransaction transaction = await _pageRoleRepository.GetTransaction();
        try
        {
            List<PageRole>? entities = _mapper.Map<List<PageRole>>(pagesOnRole);
            var roleId=entities.FirstOrDefault().RoleId;
            var pageNameWithRoleName = await _pageRoleRepository.GetPageNamesInRole(roleId);
            var deletedEntities = await _pageRoleRepository.Remove(pageNameWithRoleName, true);
            await _pageRoleRepository.Add(entities);
            transaction.Commit();
            return OperationResult<List<PagesOnRole>>.Succeeded(_localizer["PagesRetrievedSuccessfully"], pagesOnRoleClone);
        }
        catch (RepositoryException ex)
        {
            await transaction.RollbackAsync();
            return OperationResult<List<PagesOnRole>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return OperationResult<List<PagesOnRole>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
        }

    }

    public async Task<OperationResult<CategorizedMenuItemsDto>> GetMenuItems(List<string> rolesId)
    {
        try
        {
            var pages = await _pageRepository.GetMenuByRoles(rolesId);
            var categorizedPages = CategorizeAndBuildPaths(pages);

            var categorizedMenuItems = new CategorizedMenuItemsDto
            {
                SearchItems = categorizedPages.Where(p => p.IsSearchable).Select(p => new MenuItemDto
                {
                    Label = p.Label,
                    To = p.To,
                    Icon = p.Icon,
                    Items = new List<MenuItemDto>()
                }).ToList(),
                NavigableItems = categorizedPages.Where(p => p.IsNavigable).Select(p => p.FullPath).ToList(),
                SidebarItems = BuildSidebarHierarchy(categorizedPages, null)
            };

            return OperationResult<CategorizedMenuItemsDto>.Succeeded(_localizer["MenuRetrievedSuccessfully"], categorizedMenuItems);

        }
        catch (RepositoryException ex)
        {
            return OperationResult<CategorizedMenuItemsDto>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
        }
        catch (Exception ex)
        {
            return OperationResult<CategorizedMenuItemsDto>.InternalServerError(_localizer["AnErrorOccurredPages"]);
        }

    }
    public async Task<OperationResult<List<PageRoleDto>>> GetActions(string path, List<string> rolesIds)
    {
        try
        {
            var actions = await _pageRoleRepository.GetActionsBasedOnRolesAndPath(path, rolesIds);
            var actionsDto=_mapper.Map<List<PageRoleDto>>(actions);
            return OperationResult<List<PageRoleDto>>.Succeeded(_localizer["MenuRetrievedSuccessfully"], actionsDto);

        }
        catch (RepositoryException ex)
        {
            return OperationResult<List<PageRoleDto>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
        }
        catch (Exception ex)
        {
            return OperationResult<List<PageRoleDto>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
        }
    }

    private List<MenuItemDto> BuildSidebarHierarchy(List<Page> pages, int? parentId)
    {
        var sidebarItems = new List<MenuItemDto>();

        var childPages = pages
            .Where(p => p.ParentId == parentId)
            .OrderBy(p => p.SortNumber)
            .ToList();

        foreach (var page in childPages)
        {
            if (page.IsSidebarItem)
            {
                sidebarItems.Add(new MenuItemDto
                {
                    Label = page.Label,
                    Icon = page.Icon,
                    To = page.To,
                    Items = BuildSidebarHierarchy(pages, page.Id) // Recursive call to build child items
                });
            }
            else
            {
                // If the page is not a sidebar item, include its children that are sidebar items
                var childSidebarItems = BuildSidebarHierarchy(pages, page.Id);
                sidebarItems.AddRange(childSidebarItems);
            }
        }

        return sidebarItems;
    }
    private List<Page> CategorizeAndBuildPaths(List<Page> pages)
    {
        var pageDictionary = pages.ToDictionary(p => p.Id, p => p);
        foreach (var page in pages)
        {
            BuildFullPath(page, pageDictionary);
        }
        return pages;
    }

    private string BuildFullPath(Page page, Dictionary<int, Page> pageDictionary)
    {
        if (page.FullPath == null)
        {
            var parentPath = page.ParentId != null && pageDictionary.ContainsKey((int)page.ParentId)
                ? BuildFullPath(pageDictionary[(int)page.ParentId], pageDictionary)
                : string.Empty;

            page.FullPath = string.IsNullOrEmpty(parentPath) ? page.To : $"{parentPath}{page.To}";
        }

        return page.FullPath;
    }


}
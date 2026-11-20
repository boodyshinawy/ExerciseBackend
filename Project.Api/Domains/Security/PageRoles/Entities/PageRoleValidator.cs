namespace Project.Api;
public class PageRoleValidator : AbstractValidator<PageRole>
{
    public PageRoleValidator()
    {
        RuleFor(x => x.PageId)
            .NotEmpty().WithMessage("PageId is required.");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required.");
    }
}
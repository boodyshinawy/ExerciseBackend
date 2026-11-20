namespace Project.Api;

public class SaleEntityRepository : BaseRepository<SaleEntity>, ISaleEntityRepository
{
    public SaleEntityRepository(ApplicationDbContext context) : base(context)
    {
    }
}

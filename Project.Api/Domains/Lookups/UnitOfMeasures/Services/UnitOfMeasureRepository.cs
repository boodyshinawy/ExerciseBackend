namespace Project.Api;

public class UnitOfMeasureRepository : BaseRepository<UnitOfMeasure>, IUnitOfMeasureRepository
{
    public UnitOfMeasureRepository(ApplicationDbContext context) : base(context)
    {
    }
}

namespace Project.Api;

public class StatusRepository : BaseRepository<Status>, IStatusRepository
{
    public StatusRepository(ApplicationDbContext context) : base(context)
    {
    }





}

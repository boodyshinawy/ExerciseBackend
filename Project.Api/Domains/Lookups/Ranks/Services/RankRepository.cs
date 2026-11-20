namespace Project.Api;

public class RankRepository : BaseRepository<Rank>, IRankRepository
{
    public RankRepository(ApplicationDbContext context) : base(context)
    {
    }





}

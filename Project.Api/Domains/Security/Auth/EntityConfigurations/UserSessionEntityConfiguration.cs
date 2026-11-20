namespace Project.Api;

public class UserSessionEntityConfiguration : BaseEntityConfiguration<UserSession>
{
    public override void Configure(EntityTypeBuilder<UserSession> builder)
    {

        builder.Property(e => e.UserId)
               .IsRequired();

        builder.Property(e => e.SessionId)
               .IsRequired();
        builder.Property(e => e.IsActive);

        base.Configure(builder);
    }

}


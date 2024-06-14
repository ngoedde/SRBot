namespace SRGame.Client.Entity;

public abstract class Entity<TPrimaryKey>
{
    public virtual TPrimaryKey Id { get; } = default!;

    public virtual bool Parse(EntityParser parser)
    {
        return false;
    }
}
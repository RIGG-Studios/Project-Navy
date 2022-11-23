public interface IDamagable
{
    int ActorID { get; }
    
    void Damage(int attackerID, float damageAmount);
}

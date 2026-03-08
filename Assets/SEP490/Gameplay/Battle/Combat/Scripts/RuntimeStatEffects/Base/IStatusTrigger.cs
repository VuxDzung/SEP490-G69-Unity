public interface IStatusTrigger
{
    void OnApply();

    void OnTurnStart();

    void OnTurnEnd();

    void OnAfterReceiveDamage(float dmg);
}

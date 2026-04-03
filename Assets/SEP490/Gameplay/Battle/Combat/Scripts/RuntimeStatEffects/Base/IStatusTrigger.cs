public interface IStatusTrigger
{
    void OnApply();

    void OnTurnStart();

    void OnTurnEnd();

    void OnAfterBeingAttacked(float dmg);
}

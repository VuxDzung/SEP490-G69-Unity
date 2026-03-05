using UnityEngine;

public interface IStatusTrigger
{
    void OnApply();

    void OnTurnStart();

    void OnTurnEnd();

    float ModifyIncomingDamage(float dmg);

    float ModifyDealDamage(float dmg);

    void OnAfterReceiveDamage(float dmg);

    void OnAction();
}

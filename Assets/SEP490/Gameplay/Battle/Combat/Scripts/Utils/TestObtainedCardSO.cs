using UnityEngine;

[CreateAssetMenu(fileName = "TestObtainedCardConfig", menuName = "Game Testing/Obtained card config")]
public class TestObtainedCardSO : ScriptableObject
{
    [SerializeField] private string[] ids;

    public string[] Ids => ids;
}

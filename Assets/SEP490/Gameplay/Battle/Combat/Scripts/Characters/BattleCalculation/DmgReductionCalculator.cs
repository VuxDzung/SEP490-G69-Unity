namespace SEP490G69.Battle.Combat
{
    public class DmgReductionCalculator : IDmgReductionCalculator
    {
        public float Calculate(float defValue)
        {
            return defValue / 400f;
        }
    }
}
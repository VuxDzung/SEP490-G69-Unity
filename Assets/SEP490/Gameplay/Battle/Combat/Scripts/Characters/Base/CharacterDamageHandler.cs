namespace SEP490G69.Battle.Combat
{
    public class CharacterDamageHandler
    {
        private readonly IDmgReductionCalculator _reduction;


        public CharacterDamageHandler(IDmgReductionCalculator reduction)
        {
            _reduction = reduction;
        }

        public float CalculateFinalDamage(float rawDamage, float def)
        {
            return rawDamage - rawDamage * _reduction.Calculate(def);
        }
    }
}
public static class ReputationHelper
{
    public static EReputationRank GetRankFromRP(int rp)
    {
        if (rp >= 4000) return EReputationRank.SS;
        if (rp >= 2500) return EReputationRank.S;
        if (rp >= 1500) return EReputationRank.A;
        if (rp >= 1000) return EReputationRank.B;
        if (rp >= 600) return EReputationRank.C;
        if (rp >= 300) return EReputationRank.D;
        if (rp >= 100) return EReputationRank.E;
        return EReputationRank.F;
    }
}
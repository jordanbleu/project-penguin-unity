using Source.Constants;
using Source.Data;
using UnityEngine;

namespace Source.Data
{
    public static class RankCalculator
    {
        // these should add up to 1.0
        private const float DeathPenaltyWeight = 0.5f;
        private const float AccuracyWeight = 0.15f;
        private const float DamageTakenWeight = 0.35f;
        
        // Super advanced algorithm to calculate the player's rank based on their stats
        public static Rank CalculateRank(int deaths, int bulletsFired, int bulletsHit, int damageTaken)
        {
            var accuracy = (float)bulletsHit / bulletsFired;

            // start with perfection <3
            var rank = 1f;
            
            // -- Player Deaths --
            var penaltyPerDeath = DeathPenaltyWeight / GameplayConstants.TotalLives;
            var deathPenalty = penaltyPerDeath * deaths;
            rank -= deathPenalty;
            
            // -- Accuracy --
            var inverseAccuracy = 1 - accuracy;
            var accuracyPenalty = inverseAccuracy * AccuracyWeight;
            rank -= accuracyPenalty;
            
            // -- Damage Taken --
            // My arbitrary max 'damage taken' is 500, which is generous cuz the player would have 
            // died 5 times if it weren't for healing.
            var damageTakenMinusDeaths = damageTaken - (deaths * 100);
            var damageTakenPercent = (float)Mathf.Min(500,damageTakenMinusDeaths) / 500;
            rank -= damageTakenPercent * DamageTakenWeight;

            return GetRank(rank);
        }

        private static Rank GetRank(float rank)
        {
            // this is just calculating based in my tiny brain understanding of how these work.
            if (rank <= 0.5f)
                return Rank.F;
            
            // D
            if (rank <= 0.63f)
                return Rank.DMinus;
            
            if (rank <= 0.66)
                return Rank.D;

            if (rank <= 0.69)
                return Rank.DPlus;
            
            // C
            if (rank <= 0.73)
                return Rank.CMinus;

            if (rank <= 0.76)
                return Rank.C;

            if (rank <= 0.79)
                return Rank.CPlus;
            
            // B
            if (rank <= 0.83)
                return Rank.BMinus;

            if (rank <= 0.86)
                return Rank.B;

            if (rank <= 0.89)
                return Rank.BPlus;
            
            // A
            if (rank <= 0.93)
                return Rank.AMinus;
            
            // S
            return Rank.S;
        }
    }
    
    public enum Rank
    {
        F,
        
        DMinus,
        D,
        DPlus,
        
        CMinus,
        C,
        CPlus,
        
        BMinus,
        B,
        BPlus,
        
        AMinus,
        
        S
    }
    
}

public static class RankExtensions
{
    public static string ToDisplayString(this Rank rank)
    {
        switch (rank)
        {
            case Rank.F:
                return "F";
            case Rank.DMinus:
                return "D-";
            case Rank.D:
                return "D";
            case Rank.DPlus:
                return "D+";
            case Rank.CMinus:
                return "C-";
            case Rank.C:
                return "C";
            case Rank.CPlus:
                return "C+";
            case Rank.BMinus:
                return "B-";
            case Rank.B:
                return "B";
            case Rank.BPlus:
                return "B+";
            case Rank.AMinus:
                return "A-";
            case Rank.S:
                return "S";
            default:
                return "???";
        }
    }
}
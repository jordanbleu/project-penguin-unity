using Source.Constants;
using Source.Data;
using UnityEngine;

namespace Source.Data
{
    
    
    /// <summary>
    /// How rank is calculated
    ///
    /// Rank is calculated based on Deaths, accuracy, and damage taken.
    /// Deaths are the most detrimental to your score, with each one dropping your grade by 17%.
    /// So, if you die twice, you'll get a D, if you die three times it's a guaranteed F.
    ///
    /// Accuracy technically would take a quarter of the grade but anything below 40% is
    /// ignored because that's already just horrendous.
    ///
    /// Damage taken will ignore penalties given from deaths.  The closer to 1200 damage you take,
    /// the worse this penalty will be, with anything over 1200 ignored.  
    /// 
    /// </summary>
    public static class RankCalculator
    {
        // these should add up to 1.0
        private const float DeathPenaltyWeight = 0.5f;
        private const float AccuracyWeight = 0.25f;
        private const float DamageTakenWeight = 0.15f;
        
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
            // how "not accurate" were you
            var inverseAccuracy =1f - accuracy;
            // bottom out at 40% accuracy so sloppy players don't get reamed on.
            inverseAccuracy = Mathf.Clamp(inverseAccuracy, 0f, 0.6f);
            var accuracyPenalty = inverseAccuracy * AccuracyWeight;
            rank -= accuracyPenalty;
            
            // -- Damage Taken --
            // player was already penalized for deaths so remove those 
            // (100 is max health).
            var damageTakenMinusDeaths = damageTaken - (deaths * 100);
            // up to 1200 damage taken is accounted for in your score, after that it maxes out.
            // 1200 damage is the equivalent of 12 deaths.
            var damageTakenPercent = Mathf.Min(1200, damageTakenMinusDeaths / 1200f);
            
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
            
            if (rank <= 0.95)
                return Rank.A;
            
            // S - anything above 95%
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
        A,
        
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
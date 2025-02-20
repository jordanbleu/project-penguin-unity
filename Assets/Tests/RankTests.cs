using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Source.Data;
using UnityEngine;
using UnityEngine.TestTools;

public class RankTests
{
    
    [Test]
    public void InsanelyGoodPerformance_ResultsInSRank()
    {
        // 90% accuracy, barely any damage taken, no deaths.
        var finalGrade = StatsCalculator.CalculateRank
            (
                deaths: 0,
                bulletsFired: 100,
                bulletsHit: 90,
                damageTaken: 30
            );
        
        Assert.AreEqual(Rank.S, finalGrade);
    }
    
    
    [Test]
    public void VeryGoodPerformance_ResultsInARank()
    {
        // No deaths but more damage taken and less accurate
        var finalGrade = StatsCalculator.CalculateRank
        (
            deaths: 0,
            bulletsFired: 100,
            bulletsHit: 80,
            damageTaken: 100
        );
        
        // Fuzzy Assertions
        Assert.Contains(finalGrade, new [] {Rank.A, Rank.AMinus});
    }
    
    [Test]
    public void VeryGoodPerformance_ResultsInBRank_WithLowAccuracy()
    {
        // Very low accuracy
        var finalGrade = StatsCalculator.CalculateRank
        (
            deaths: 0,
            bulletsFired: 100,
            bulletsHit: 40, 
            damageTaken: 100
        );
        
        // Fuzzy Assertions
        Assert.Contains(finalGrade, new [] {
            Rank.BMinus, 
            Rank.B,
            Rank.BPlus
        });
    }
    
    [Test]
    public void AveragePerformance_ResultsInCRank()
    {
        // Very low accuracy
        var finalGrade = StatsCalculator.CalculateRank
        (
            deaths: 1,
            bulletsFired: 100,
            bulletsHit: 70, 
            damageTaken: 600
        );
        
        // Fuzzy Assertions
        Assert.Contains(finalGrade, new [] {
            Rank.CMinus, 
            Rank.C,
            Rank.CPlus
        });
    }
    
    [Test]
    public void AbysmalPerformance_ResultsInF()
    {
        // Very low accuracy
        var finalGrade = StatsCalculator.CalculateRank
        (
            deaths: 2,
            bulletsFired: 100,
            bulletsHit: 20, 
            damageTaken: 900
        );
        
        // Fuzzy Assertions
        Assert.Contains(finalGrade, new [] {
            Rank.F
        });
    }
    
    [Test]
    public void BadPerformance_ResultsInD()
    {
        // Very low accuracy
        var finalGrade = StatsCalculator.CalculateRank
        (
            deaths: 2,
            bulletsFired: 100,
            bulletsHit: 40, 
            damageTaken: 200
        );
        
        // Fuzzy Assertions
        Assert.Contains(finalGrade, new [] {
            Rank.DMinus,
            Rank.D,
            Rank.DPlus
        });
    }
    
    // This is testing that accuracy bottoms out at 40%.
    [Test]
    public void BadPerformance_ResultsInD_EvenWithZeroAccuracy()
    {
        // Very low accuracy
        var finalGrade = StatsCalculator.CalculateRank
        (
            deaths: 2,
            bulletsFired: 100,
            bulletsHit: 0, 
            damageTaken: 200
        );
        
        // Fuzzy Assertions
        Assert.Contains(finalGrade, new [] {
            Rank.DMinus,
            Rank.D,
            Rank.DPlus
        });
    }
    
    [Test]
    public void NotGreatPerformance_ResultsInC_WithNoDeathsButHighDamage()
    {
        // Very low accuracy
        var finalGrade = StatsCalculator.CalculateRank
        (
            deaths: 0,
            bulletsFired: 100,
            bulletsHit: 0, 
            damageTaken: 900
        );
        
        // Fuzzy Assertions
        Assert.Contains(finalGrade, new [] {
            Rank.CMinus,
            Rank.C,
            Rank.CPlus
        });
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class AgeData
{
    private readonly ReadOnlyCollection<float> ageCounts;
    private readonly ReadOnlyCollection<float> maxMortality;
    public float TotalMaxMortality { get; }

    public AgeData(IEnumerable<float> ageCounts)
    {
        this.ageCounts = ageCounts.ToList().AsReadOnly();
        this.maxMortality = GetMaxMortailyt().ToList().AsReadOnly();
        TotalMaxMortality = maxMortality.Sum();
    }

    private IEnumerable<float> GetMaxMortailyt()
    {
        for (int i = 0; i < ageCounts.Count; i++)
        {
            float val = this.ageCounts[i];
            yield return AgeBracket.Brackets[i].CovidMortalityRate * val;
        }
    }
}

public class AgeBracket
{
    public int MinAge { get; }
    public int MaxAge { get; }
    public float CovidMortalityRate { get; }

    public AgeBracket(int minAge, int maxAge, float mortalityRate)
    {
        MinAge = minAge;
        MaxAge = maxAge;
        CovidMortalityRate = mortalityRate;
    }

    public static ReadOnlyCollection<AgeBracket> Brackets { get; } =
        new ReadOnlyCollection<AgeBracket>(new List<AgeBracket>()
        {
            new AgeBracket(0, 4, 0.002f),
            new AgeBracket(5, 9, 0.002f),
            new AgeBracket(10, 14, 0.002f),
            new AgeBracket(15, 19, 0.002f),
            new AgeBracket(20, 24, 0.002f),
            new AgeBracket(25, 29, 0.002f),
            new AgeBracket(30, 34, 0.002f),
            new AgeBracket(35, 39, 0.002f),
            new AgeBracket(40, 44, 0.004f),
            new AgeBracket(45, 49, 0.004f),
            new AgeBracket(50, 54, 0.013f),
            new AgeBracket(55, 59, 0.013f),
            new AgeBracket(60, 64, 0.036f),
            new AgeBracket(65, 69, 0.036f),
            new AgeBracket(70, 74, 0.08f),
            new AgeBracket(75, 79, 0.08f),
            new AgeBracket(80, 84, 0.148f),
            new AgeBracket(85, 150, 0.18f),
        });
}

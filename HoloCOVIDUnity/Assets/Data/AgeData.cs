using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class AgeData
{
    private readonly ReadOnlyCollection<float> ageCounts;

    public AgeData(IEnumerable<float> ageCounts)
    {
        this.ageCounts = ageCounts.ToList().AsReadOnly();
    }
}

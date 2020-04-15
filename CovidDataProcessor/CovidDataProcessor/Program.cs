using System.Collections.Generic;

namespace CovidDataProcessor
{
    partial class Program
    {
        public const string DirectoryRoot = @"C:\Users\Lisa\Documents\HoloCOVID\CovidDataProcessor\CovidDataProcessor\";
        public const string NationIdsPath = DirectoryRoot + @"SourceData\nationIds_1_degree.txt";
        public const string PopulationsPath = DirectoryRoot + @"SourceData\populationGrid_2020_1_degree.txt";

        static void Main(string[] args)
        {
            ReportLoader loader = new ReportLoader();
            GriddedMap map = new GriddedMap(360, 180, loader.DaysOfData);
            map.LoadNations(NationIdsPath, PopulationsPath);
            foreach (ReportLineItem item in loader.ReportItems)
            {
                map.AddData(item);
            }
        }
    }
}
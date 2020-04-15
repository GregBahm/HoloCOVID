using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CovidDataProcessor
{
    partial class Program
    {
        public const string DirectoryRoot = @"C:\Users\Lisa\Documents\HoloCOVID\CovidDataProcessor\CovidDataProcessor\";
        public const string ReportOutput = DirectoryRoot + @"CovidReport.txt";

        static void Main(string[] args)
        {
            ReportLoader loader = new ReportLoader();
            string[] reportLines = loader.ReportItems.Select(item => item.GetReportLine()).ToArray();
            File.WriteAllLines(ReportOutput, reportLines);
        }
    }
}
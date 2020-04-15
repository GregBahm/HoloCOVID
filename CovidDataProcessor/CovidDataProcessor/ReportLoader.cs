using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CovidDataProcessor
{
    public class ReportLoader
    {
        public const string EarlyReportsFolder = Program.DirectoryRoot + @"SourceData\EarlyReports";
        public const string LaterReportsFolder = Program.DirectoryRoot + @"SourceData\LaterReports";

        public IEnumerable<ReportLineItem> ReportItems { get; }
        public int DaysOfData { get; }

        public ReportLoader()
        {
            DaysOfData = GetDaysOfData();
            ReportItems = LoadReportItems();
        }

        private int GetDaysOfData()
        {
            return Directory.GetFiles(EarlyReportsFolder).Length + Directory.GetFiles(LaterReportsFolder).Length;
        }

        private static IEnumerable<ReportLineItem> LoadEarlyReport(string path, int day)
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines.Skip(1))
            {
                string[] components = GetEarlyReportLineComponents(line);
                if(IsValidEarlyLine(components))
                {
                    yield return LoadEarlyReportLine(components, day);
                }
            }
        }

        private static readonly Regex cmvSplitter = new Regex("(?<=^|,)(\"(?:[^\"]|\"\")*\"|[^,]*)"); //Internet...

        private static IEnumerable<ReportLineItem> LoadLateReport(string path, int day)
        {
            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines.Skip(1))
            {
                string[] components = LoadLateReportComponents(line).ToArray();
                if (IsValidLaterLine(components))
                {
                    yield return LoadLateReportLine(components, day);
                }
            }
        }

        private static IEnumerable<string> LoadLateReportComponents(string line)
        {
            foreach (Match match in cmvSplitter.Matches(line))
            {
                yield return match.Value;
            }
        }

        private static bool IsValidLaterLine(string[] components)
        {
            return !string.IsNullOrWhiteSpace(components[5]); // Just tossing lines with no coords. Mostly cruise ships.
        }

        private static bool IsValidEarlyLine(string[] components)
        {
            return components.Length == 8;
        }

        private static ReportLineItem LoadLateReportLine(string[] components, int day)
        {
            string rawLat = components[5];
            float latitude = LoadFloat(rawLat);

            string rawLong = components[6];
            float longitude = LoadFloat(rawLong);

            string rawConfirmed = components[7];
            int confirmed = LoadInt(rawConfirmed);

            string rawDeaths = components[8];
            int deaths = LoadInt(rawDeaths);

            string rawRecovered = components[9];
            int recovered = LoadInt(rawRecovered);

            LineItemType lineType = string.IsNullOrWhiteSpace(components[2]) ? LineItemType.Nation : LineItemType.Point;

            return new ReportLineItem(day, latitude, longitude, confirmed, deaths, recovered, lineType);
        }

        private static ReportLineItem LoadEarlyReportLine(string[] lineComponents, int day)
        {
            string rawConfirmed = lineComponents[3];
            int confirmed = LoadInt(rawConfirmed);

            string rawDeaths = lineComponents[4];
            int deaths = LoadInt(rawDeaths);

            string rawRecovered = lineComponents[5];
            int recovered = LoadInt(rawRecovered);

            string rawLatitude = lineComponents[6];
            float latitude = LoadFloat(rawLatitude);

            string rawLongitude = lineComponents[7];
            float longitude = LoadFloat(rawLongitude);

            LineItemType type = string.IsNullOrWhiteSpace(lineComponents[0]) ? LineItemType.Nation : LineItemType.Point;

            return new ReportLineItem(day, latitude, longitude, confirmed, deaths, recovered, type);
        }

        private static string[] GetEarlyReportLineComponents(string line)
        {
            //US cities are "City, State" obnoxiously
            string[] quoteSplit = line.Split('"');
            if (quoteSplit.Length == 1)
            {
                return line.Split(',');
            }
            else if (quoteSplit.Length == 3)
            {
                List<string> ret = new List<string>() { quoteSplit[1] };
                ret.AddRange(quoteSplit[2].Split(',').Skip(1));
                return ret.ToArray();
            }
            else
            {
                throw new Exception("This data is weird: " + line);
            }
        }

        private static int LoadInt(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return 0;
            }
            return Convert.ToInt32(val);
        }

        private static float LoadFloat(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
            {
                return 0;
            }
            return Convert.ToSingle(val);
        }

        private static IEnumerable<ReportLineItem> LoadReportItems()
        {
            List<Report> reports = GetReports().ToList();
            return LoadReportItems(reports);
        }

        private static IEnumerable<ReportLineItem> LoadReportItems(IEnumerable<Report> reports)
        {
            List<ReportLineItem> ret = new List<ReportLineItem>();
            foreach (Report report in reports)
            {
                IEnumerable<ReportLineItem> reportItems = LoadReportItems(report);
                ret.AddRange(reportItems);
            }
            return ret;
        }

        private static IEnumerable<ReportLineItem> LoadReportItems(Report report)
        {
            if (report.EarlyType)
            {
                return LoadEarlyReport(report.Path, report.Day).ToArray();
            }
            else
            {
                return LoadLateReport(report.Path, report.Day).ToArray();
            }
        }

        private static IEnumerable<Report> GetReports()
        {
            int day = -1;
            string[] earlyReportPaths = Directory.GetFiles(EarlyReportsFolder);
            foreach (string earlyReport in earlyReportPaths)
            {
                day++;
                yield return new Report(earlyReport, true, day);
            }

            string[] lateReportPaths = Directory.GetFiles(LaterReportsFolder);
            foreach (string lateReportPath in lateReportPaths)
            {
                day++;
                yield return new Report(lateReportPath, false, day);
            }
        }

        private class Report
        {
            public string Path { get; }
            public bool EarlyType { get; }
            public int Day { get; }

            public Report(string path, bool earlyType, int day)
            {
                Path = path;
                Day = day;
                EarlyType = earlyType;
            }
        }
    }
}
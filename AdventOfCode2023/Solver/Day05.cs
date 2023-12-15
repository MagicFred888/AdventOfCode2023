using System;

namespace AdventOfCode2023.Solver
{
    internal class Day05 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "If You Give A Seed A Fertilizer";
        private class MapConverter
        {
            public string From { get; private set; } = string.Empty;
            public string To { get; private set; } = string.Empty;

            private List<Tuple<long, long, long>> Ranges = new List<Tuple<long, long, long>>(); // Start - Stop - Offset

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public MapConverter(string fromToRawData)
            {
                string[] parts = fromToRawData.Replace(" map:", "").Split('-');
                From = parts[0];
                To = parts[2];
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public void AddConversionInfo(string conversionRawData)
            {
                long[] values = Array.ConvertAll(conversionRawData.Split(' ', StringSplitOptions.RemoveEmptyEntries), i => long.Parse(i));
                Ranges.Add(new Tuple<long, long, long>(values[1], values[1] + values[2] - 1, values[0] - values[1]));
                Ranges.Sort(delegate (Tuple<long, long, long> a, Tuple<long, long, long> b) { return a.Item1.CompareTo(b.Item1); });
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public long ConvertFromTo(long tmpValue)
            {
                Tuple<long, long, long>? range = Ranges.Find(r => tmpValue >= r.Item1 && tmpValue <= r.Item2);
                if (range == null) return tmpValue;
                return tmpValue + range.Item3;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public long ConvertToFrom(long tmpValue)
            {
                Tuple<long, long, long>? range = Ranges.Find(r => tmpValue >= r.Item1 + r.Item3 && tmpValue <= r.Item2 + r.Item3);
                if (range == null) return tmpValue;
                return tmpValue - range.Item3;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public List<long> GetRangeKeyValues()
            {
                List<long> values = new List<long>();
                foreach (Tuple<long, long, long> convInfos in Ranges)
                {
                    values.Add(convInfos.Item1 - 1);
                    values.Add(convInfos.Item1);
                    values.Add(convInfos.Item2);
                    values.Add(convInfos.Item2 + 1);
                }
                return values.Distinct().ToList();
            }
        }

        private List<MapConverter> allMapConverters = new List<MapConverter>();
        private List<long> seeds = new List<long>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day05(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Compute solution
            List<long> allLocations = new List<long>();
            foreach (long seed in seeds)
            {
                string current = "seed";
                long tmpValue = seed;
                while (current != "location")
                {
                    MapConverter? mc = allMapConverters.Find(mc => mc.From == current);
                    tmpValue = mc.ConvertFromTo(tmpValue);
                    current = mc.To;
                }
                allLocations.Add(tmpValue);
            }

            // Done
            return allLocations.Min().ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Compute key points from LOCATION to SEED (reversed)
            List<long> keyPoints = new List<long>();
            string current = "location";
            while (current != "seed")
            {
                // Get proper converter
                MapConverter? mc = allMapConverters.Find(mc => mc.To == current);

                // Convert previous key point 
                keyPoints = keyPoints.ConvertAll(i => mc.ConvertToFrom(i));

                // Add new key points and move next
                keyPoints.AddRange(mc.GetRangeKeyValues());
                current = mc.From;
            }
            keyPoints = keyPoints.FindAll(i => i >= 0).Distinct().ToList();
            keyPoints.Sort(); // Not really usefull

            // Compute solution
            long minLocation = long.MaxValue;
            for (int i = 0; i < seeds.Count; i += 2)
            {
                long firstSeed = seeds[i];
                long lastSeed = seeds[i] + seeds[i + 1] - 1;
                foreach (long keyPoint in keyPoints)
                {
                    if (keyPoint < firstSeed || keyPoint > lastSeed) continue;
                    long tmpValue = keyPoint;
                    foreach (MapConverter mc in allMapConverters)
                    {
                        tmpValue = mc.ConvertFromTo(tmpValue);
                    }
                    if (tmpValue < minLocation) minLocation = tmpValue;
                }
            }

            // Done
            return minLocation.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allMapConverters.Clear();
            MapConverter? currentConverter = null;
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                if (rawData[pos].StartsWith("seeds:"))
                {
                    seeds = Array.ConvertAll<string, long>(rawData[pos].Split(':')[1].Split(" ", StringSplitOptions.RemoveEmptyEntries), i => long.Parse(i)).ToList();
                }
                else
                {
                    if (rawData[pos].EndsWith("map:"))
                    {
                        // New map
                        if (currentConverter != null) allMapConverters.Add(currentConverter);
                        currentConverter = new MapConverter(rawData[pos]);
                    }
                    else if (rawData[pos] != string.Empty && currentConverter != null)
                    {
                        // New conversion info
                        currentConverter.AddConversionInfo(rawData[pos]);
                    }
                }
            }
            if (currentConverter != null) allMapConverters.Add(currentConverter);
        }
    }
}
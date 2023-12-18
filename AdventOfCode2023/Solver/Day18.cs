using System.Drawing;

namespace AdventOfCode2023.Solver
{
    internal class Day18 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Lavaduct Lagoon";

        private class DigInfos
        {
            public string Direction = string.Empty;
            public int Length = 0;
            public Color Color = Color.White;

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public DigInfos(string rawData, bool step2)
            {
                if (step2)
                {
                    // Use wrong "color info" as length and direction
                    rawData = rawData.Substring(rawData.IndexOf("#") + 1).Trim(')');
                    Length = Convert.ToInt32(rawData.Substring(0, 5), 16);
                    Direction = "RDLU".Substring(int.Parse(rawData.Substring(5)), 1);
                }
                else
                {
                    // Use dir and length + save color (at step 1, didn't know it will be useless)
                    Direction = rawData.Substring(0, 1);
                    rawData = rawData.Substring(2);
                    Length = int.Parse(rawData.Substring(0, rawData.IndexOf(' ')));
                    rawData = rawData.Substring(rawData.IndexOf("#") + 1).Trim(')');
                    Color = Color.FromArgb(Convert.ToByte(rawData.Substring(0, 2), 16), Convert.ToByte(rawData.Substring(2, 2), 16), Convert.ToByte(rawData.Substring(4, 2), 16));
                }
            }
        }

        private List<DigInfos> allDigInfos = new List<DigInfos>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day18(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData(false);

            // Done
            return GetLagoonVolume().ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData(true);

            // Done
            return GetLagoonVolume().ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private long GetLagoonVolume()
        {
            // Initialization
            int xPos = 0;
            int yPos = 0;
            int trenchLength = 0;
            List<(long X, long Y)> lagoonData = new List<(long X, long Y)>();

            // Scan each corner
            for (int i = 0; i < allDigInfos.Count; i++)
            {
                trenchLength += allDigInfos[i].Length;
                lagoonData.Add((xPos, yPos));
                if (allDigInfos[i].Direction == "R") xPos += allDigInfos[i].Length;
                if (allDigInfos[i].Direction == "L") xPos -= allDigInfos[i].Length;
                if (allDigInfos[i].Direction == "D") yPos += allDigInfos[i].Length;
                if (allDigInfos[i].Direction == "U") yPos -= allDigInfos[i].Length;
            }

            // Get Area from Shoelace formula
            long Area = AreaFromShoelace(lagoonData);

            // Done
            return (trenchLength / 2) + Area + 1;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData(bool step2)
        {
            // Extract dig infos
            allDigInfos.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allDigInfos.Add(new DigInfos(rawData[pos], step2));
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private long AreaFromShoelace(List<(long X, long Y)> polygon)
        {
            // Frome here https://rosettacode.org/wiki/Shoelace_formula_for_polygonal_area#C# with some minor tweaks
            int n = polygon.Count;
            double result = 0.0;
            for (int i = 0; i < n - 1; i++)
            {
                result += polygon[i].X * polygon[i + 1].Y - polygon[i + 1].X * polygon[i].Y;
            }
            result = Math.Abs(result + polygon[n - 1].X * polygon[0].Y - polygon[0].X * polygon[n - 1].Y) / 2d;
            return (long)result;
        }
    }
}
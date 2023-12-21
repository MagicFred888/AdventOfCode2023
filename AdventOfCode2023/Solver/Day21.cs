using System.Drawing;

namespace AdventOfCode2023.Solver
{
    internal class Day21 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Step Counter";

        private Size mapSize = new Size();
        private HashSet<string> allStones = new HashSet<string>();
        private List<(int x, int y)> possiblePlace = new List<(int x, int y)>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day21(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Make some calculation...
            for (int i = 0; i < (allStones.Count < 100 ? 6 : 64); i++)
            {
                List<(int x, int y)> newPossiblePlace = new List<(int x, int y)>();
                foreach ((int x, int y) pos in possiblePlace)
                {
                    if (IsValid(pos.x + 1, pos.y)) newPossiblePlace.Add((pos.x + 1, pos.y));
                    if (IsValid(pos.x - 1, pos.y)) newPossiblePlace.Add((pos.x - 1, pos.y));
                    if (IsValid(pos.x, pos.y + 1)) newPossiblePlace.Add((pos.x, pos.y + 1));
                    if (IsValid(pos.x, pos.y - 1)) newPossiblePlace.Add((pos.x, pos.y - 1));
                }
                possiblePlace = newPossiblePlace.Distinct().ToList();
            }

            // Done
            return possiblePlace.Count.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // Make some calculation...
            Dictionary<int, int> keyPoints = new Dictionary<int, int>() { { 65, 0 }, { 196, 0 }, { 327, 0 } };
            for (int i = 0; i <= keyPoints.Keys.Max(); i++)
            {
                if (keyPoints.ContainsKey(i))
                {
                    keyPoints[i] = possiblePlace.Count;
                    if (i == 327) break;
                }

                List<(int x, int y)> newPossiblePlace = new List<(int x, int y)>();
                foreach ((int x, int y) pos in possiblePlace)
                {
                    if (IsValid(pos.x + 1, pos.y)) newPossiblePlace.Add((pos.x + 1, pos.y));
                    if (IsValid(pos.x - 1, pos.y)) newPossiblePlace.Add((pos.x - 1, pos.y));
                    if (IsValid(pos.x, pos.y + 1)) newPossiblePlace.Add((pos.x, pos.y + 1));
                    if (IsValid(pos.x, pos.y - 1)) newPossiblePlace.Add((pos.x, pos.y - 1));
                }
                possiblePlace = newPossiblePlace.Distinct().ToList();
            }

            // Compute answer by code, better than how I find it with Excel... Thanks to https://pastebin.com/d0tD8Uwx
            (double a, double b, double c) poly = SimplifiedLagrange(Array.ConvertAll<int, double>(keyPoints.Values.ToArray(), i => (double)i));
            double target = (26_501_365 - 65) / 131;
            double answer = poly.a * target * target + poly.b * target + poly.c;

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private (double a, double b, double c) SimplifiedLagrange(double[] values)
        {
            (double a, double b, double c) result = (values[0] / 2 - values[1] + values[2] / 2, -3 * (values[0] / 2) + 2 * values[1] - values[2] / 2, values[0]);
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private bool IsValid(int x, int y)
        {
            x = (x + 1000 * mapSize.Width) % mapSize.Width;
            y = (y + 1000 * mapSize.Height) % mapSize.Height;
            if (allStones.Contains($"{x}-{y}")) return false;
            return true;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allStones.Clear();
            possiblePlace.Clear();
            mapSize = new Size(rawData[0].Length, rawData.Count);
            for (int y = 0; y < rawData.Count; y++)
            {
                for (int x = 0; x < rawData[y].Length; x++)
                {
                    if (rawData[y].Substring(x, 1) == "S") possiblePlace.Add((x, y));
                    else if (rawData[y].Substring(x, 1) == "#") allStones.Add($"{x}-{y}");
                }
            }
        }
    }
}
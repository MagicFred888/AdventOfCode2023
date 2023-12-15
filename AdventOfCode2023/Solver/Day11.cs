namespace AdventOfCode2023.Solver
{
    internal class Day11 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Cosmic Expansion";
        private class PointL
        {
            public long X = 0;
            public long Y = 0;

            public PointL(long x, long y)
            {
                X = x;
                Y = y;
            }
        }

        private List<PointL> allPoints = new List<PointL>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day11(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Add space
            AddSpace(2);

            // Done
            return SumDistances().ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Add space
            AddSpace(rawData.Count > 10 ? 1000000 : 100);

            // Done
            return SumDistances().ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private long SumDistances()
        {
            // Compute solution
            long result = 0;
            for (int i = 0; i < allPoints.Count; i++)
            {
                for (int j = i + 1; j < allPoints.Count; j++)
                {
                    result += Math.Abs(allPoints[i].X - allPoints[j].X) + Math.Abs(allPoints[i].Y - allPoints[j].Y);
                }
            }
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void AddSpace(long spaceSize)
        {
            // Add vertical space
            List<long> columnWithStar = allPoints.ConvertAll(p => p.X).Distinct().ToList();
            List<long> rowWithStar = allPoints.ConvertAll(p => p.Y).Distinct().ToList();
            columnWithStar.Sort();
            rowWithStar.Sort();
            allPoints = allPoints.ConvertAll(p => new PointL(p.X + (spaceSize - 1) * (p.X - columnWithStar.IndexOf(p.X)), p.Y + (spaceSize - 1) * (p.Y - rowWithStar.IndexOf(p.Y))));
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allPoints.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allPoints.AddRange(rawData[pos].ToArray().Select((b, i) => b == '#' ? i : -1).Where(i => i != -1).ToList().ConvertAll(i => new PointL(i, pos)));
            }
        }
    }
}
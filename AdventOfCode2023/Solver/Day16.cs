using System.Drawing;

namespace AdventOfCode2023.Solver
{
    internal class Day16 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "The Floor Will Be Lava";

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private class LightInfos
        {
            public Point Position { get; set; } = new Point();
            public Point MovingDirection { get; set; } = new Point();

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public LightInfos(Point position, Point movingDirection)
            {
                Position = position;
                MovingDirection = movingDirection;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private class TileInfos
        {
            public string MirrorType { get; private set; } = string.Empty;
            private List<Point> enteringBeamDirection = new List<Point>();

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public TileInfos(string rawData)
            {
                MirrorType = rawData;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public int GetIsEnergizedAndReset()
            {
                int answer = enteringBeamDirection.Count > 0 ? 1 : 0;
                enteringBeamDirection.Clear();
                return answer;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public List<Point> GetExitsDirectionFromEntryDirection(Point enteringDirection)
            {
                // Check if already treated and save it if not
                if (enteringBeamDirection.Contains(enteringDirection)) return new List<Point>();
                enteringBeamDirection.Add(enteringDirection);

                // Different cases
                if (MirrorType == "." || (MirrorType == "-" && enteringDirection.Y == 0) || (MirrorType == "|" && enteringDirection.X == 0))
                {
                    // Pass through
                    return new Point[] { new Point(enteringDirection.X, enteringDirection.Y) }.ToList();
                }
                else if (MirrorType == "|" || MirrorType == "-")
                {
                    // Split 90°
                    return (enteringDirection.X == 0 ? new Point[] { new Point(-1, 0), new Point(1, 0) } : new Point[] { new Point(0, -1), new Point(0, 1) }).ToList();
                }
                else if (MirrorType == "/")
                {
                    return new Point[] { new Point(-enteringDirection.Y, -enteringDirection.X) }.ToList();
                }
                else
                {
                    return new Point[] { new Point(enteringDirection.Y, enteringDirection.X) }.ToList();
                }
            }
        }

        private TileInfos[,] cavern = new TileInfos[0, 0];
        private List<LightInfos> lightToCheck = new List<LightInfos>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day16(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Done
            return ComputeAnswer(new LightInfos(new Point(-1, 0), new Point(1, 0))).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // Make 
            int maxValue = 0;
            for (int yScan = 0; yScan < cavern.GetLength(1); yScan++)
            {
                int answer = ComputeAnswer(new LightInfos(new Point(-1, yScan), new Point(1, 0)));
                if (answer > maxValue) maxValue = answer;
                answer = ComputeAnswer(new LightInfos(new Point(cavern.GetLength(0), yScan), new Point(-1, 0)));
                if (answer > maxValue) maxValue = answer;
            }
            for (int xScan = 0; xScan < cavern.GetLength(0); xScan++)
            {
                int answer = ComputeAnswer(new LightInfos(new Point(xScan, -1), new Point(0, 1)));
                if (answer > maxValue) maxValue = answer;
                answer = ComputeAnswer(new LightInfos(new Point(xScan, cavern.GetLength(1)), new Point(0, -1)));
                if (answer > maxValue) maxValue = answer;
            }

            // Done
            return maxValue.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private int ComputeAnswer(LightInfos startCondition)
        {
            // Compute answer based on received lightInfo for start condition
            lightToCheck.Add(startCondition);
            while (lightToCheck.Count > 0)
            {
                CheckLight();
            }

            // Count energized tiles and reset value for next calculation
            int answer = 0;
            for (int x = 0; x < cavern.GetLength(0); x++)
            {
                for (int y = 0; y < cavern.GetLength(1); y++)
                {
                    answer += cavern[x, y].GetIsEnergizedAndReset();
                }
            }
            return answer;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void CheckLight()
        {
            // Get top value in list
            LightInfos li = lightToCheck[0];
            lightToCheck.RemoveAt(0);

            // Compute position and check if within Matrix
            Point pos = new Point(li.Position.X, li.Position.Y);
            pos.Offset(li.MovingDirection.X, li.MovingDirection.Y);
            if (pos.X < 0 || pos.Y < 0 || pos.X > cavern.GetUpperBound(0) || pos.Y > cavern.GetUpperBound(1)) return;

            // Get next points and save them in list of points to review
            foreach (Point nDir in cavern[pos.X, pos.Y].GetExitsDirectionFromEntryDirection(li.MovingDirection))
            {
                lightToCheck.Add(new LightInfos(pos, nDir));
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            cavern = new TileInfos[rawData[0].Length, rawData.Count];
            for (int y = 0; y < rawData.Count; y++)
            {
                for (int x = 0; x < rawData[y].Length; x++)
                {
                    cavern[x, y] = new TileInfos(rawData[y].Substring(x, 1));
                }
            }
        }
    }
}
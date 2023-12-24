using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdventOfCode2023.Solver
{
    internal class Day23 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "A Long Walk";

        private class WalkingPath
        {
            public int X { get; set; } = 0;
            public int Y { get; set; } = 0;
            public HashSet<string> VisitedPos { get; set; } = new HashSet<string>();

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public WalkingPath(int x, int y)
            {
                X = x;
                Y = y;
                VisitedPos.Add($"{x}.{y}");
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public WalkingPath Clone(int newX, int newY)
            {
                string key = $"{newX}.{newY}";
                WalkingPath result = new WalkingPath(newX, newY);
                result.VisitedPos = new HashSet<string>(this.VisitedPos);
                result.VisitedPos.Add(key);
                return result;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public int WalkDisance
            {
                get { return VisitedPos.Count; }
            }
        }

        private Point startPos = new Point();
        private Point destPos = new Point();
        private WalkingPath[,] walkingPathMap = new WalkingPath[0, 0];
        private char[,] mapInfos = new char[0, 0];
        private PriorityQueue<WalkingPath, int> walkingPathToCheck = new PriorityQueue<WalkingPath, int>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day23(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Analyze map
            WalkingPath iniPos = new WalkingPath(startPos.X, startPos.Y);
            walkingPathMap[startPos.X, startPos.Y] = iniPos;
            walkingPathToCheck.Enqueue(iniPos, iniPos.WalkDisance);

            int nbrCycles = 0;
            WalkingPath solution = new WalkingPath(destPos.X, destPos.Y);
            while (walkingPathToCheck.Count > 0)
            {
                nbrCycles++;
                WalkingPath currentPos = walkingPathToCheck.Dequeue();
                List<WalkingPath> newWP = GetItemsAroundThis(currentPos, false);
                foreach (WalkingPath wp in newWP)
                {
                    if (wp.X == destPos.X && wp.Y == destPos.Y)
                    {
                        if (wp.WalkDisance > solution.WalkDisance) solution = wp;
                    }
                    else
                    {
                        walkingPathToCheck.Enqueue(wp, 20000 - wp.WalkDisance);
                    }
                }
            }

            // Done
            return (solution.WalkDisance - 1).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // Analyze map
            WalkingPath iniPos = new WalkingPath(startPos.X, startPos.Y);
            walkingPathMap[startPos.X, startPos.Y] = iniPos;
            walkingPathToCheck.Enqueue(iniPos, iniPos.WalkDisance);
            WalkingPath solution = new WalkingPath(destPos.X, destPos.Y);
            bool needLoop = true;
            while (walkingPathToCheck.Count > 0)
            {
                //nbrCycles++;
                WalkingPath currentPos = walkingPathToCheck.Dequeue();
                do
                {
                    List<WalkingPath> newWP = GetItemsAroundThis(currentPos, true);
                    needLoop = newWP.Count > 0;
                    for (int i = 0; i < newWP.Count; i++)
                    {
                        if (newWP[i].X == destPos.X && newWP[i].Y == destPos.Y)
                        {
                            // Save solution
                            if (newWP[i].WalkDisance > solution.WalkDisance)
                            {
                                solution = newWP[i];
                                Debug.WriteLine("Best solution --> " + (solution.WalkDisance - 1).ToString());
                            }
                        }
                        else if (i == 0)
                        {
                            // For next loop
                            currentPos = newWP[0];
                        }
                        else
                        {
                            // Stack for later
                            walkingPathToCheck.Enqueue(newWP[i], 20000 - newWP[i].WalkDisance);
                        }
                    }
                }
                while (needLoop);
            }

            // Done
            return (solution.WalkDisance - 1).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private List<WalkingPath> GetItemsAroundThis(WalkingPath refPos, bool ignoreSlope)
        {
            // Define possible next points
            List<WalkingPath> result = new List<WalkingPath>();
            List<Point> pts = new List<Point>(new Point[] { new Point(refPos.X - 1, refPos.Y), new Point(refPos.X + 1, refPos.Y), new Point(refPos.X, refPos.Y - 1), new Point(refPos.X, refPos.Y + 1) });
            if (!ignoreSlope)
            {
                char mapChar = mapInfos[refPos.X, refPos.Y];
                if (mapChar == '^') pts = new Point[1] { new Point(refPos.X, refPos.Y - 1) }.ToList();
                if (mapChar == 'v') pts = new Point[1] { new Point(refPos.X, refPos.Y + 1) }.ToList();
                if (mapChar == '<') pts = new Point[1] { new Point(refPos.X - 1, refPos.Y) }.ToList();
                if (mapChar == '>') pts = new Point[1] { new Point(refPos.X + 1, refPos.Y) }.ToList();
            }

            // Check each of them one by one
            List<Point> validPts = new List<Point>();
            foreach (Point p in pts)
            {
                // Check if in map, not on stone and not on slop
                if (p.X < 0 || p.Y < 0 || p.X > walkingPathMap.GetUpperBound(0) || p.Y > walkingPathMap.GetUpperBound(1)) continue;
                if (mapInfos[p.X, p.Y] == '#') continue;
                if (refPos.VisitedPos.Contains($"{p.X}.{p.Y}")) continue;
                validPts.Add(p);
            }

            if (validPts.Count == 0)
            {
                // Empty
                return result;
            }
            else if (validPts.Count == 1)
            {
                // Move (Faster)
                refPos.VisitedPos.Add($"{validPts[0].X}.{validPts[0].Y}");
                refPos.X = validPts[0].X;
                refPos.Y = validPts[0].Y;
                result.Add(refPos);
            }
            else
            {
                for (int i = 0; i < validPts.Count; i++)
                {
                    if (i < validPts.Count - 1)
                    {
                        WalkingPath newPts = refPos.Clone(validPts[i].X, validPts[i].Y);
                        result.Add(newPts);
                    }
                    else
                    {
                        refPos.VisitedPos.Add($"{validPts[i].X}.{validPts[i].Y}");
                        refPos.X = validPts[i].X;
                        refPos.Y = validPts[i].Y;
                        result.Add(refPos);
                    }
                }
            }

            // Done
            return result;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            // Build map
            startPos = new Point(-1, -1);
            destPos = new Point(-1, -1);
            walkingPathToCheck.Clear();
            mapInfos = new char[rawData[0].Length, rawData.Count];
            walkingPathMap = new WalkingPath[rawData[0].Length, rawData.Count];
            for (int y = 0; y < rawData.Count; y++)
            {
                for (int x = 0; x < rawData[y].Length; x++)
                {
                    string type = rawData[y].Substring(x, 1);
                    if (y == 0 && type == "." & startPos.X == -1 && startPos.Y == -1) startPos = new Point(x, y);
                    if (y == rawData.Count - 1 && type == "." & destPos.X == -1 && destPos.Y == -1) destPos = new Point(x, y);
                    mapInfos[x, y] = rawData[y].Substring(x, 1).ToCharArray()[0];
                    walkingPathMap[x, y] = new WalkingPath(x, y);
                }
            }
        }
    }
}
using System.ComponentModel.Design;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solver
{
    internal class Day10 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Pipe Maze";

        private class MazeNode
        {
            public string Letter = string.Empty;
            public Point Position = new Point();
            public int DistanceFromS = -1;
            public List<Point> PossibleMoveDir = new List<Point>();

            public MazeNode(string letter, Point position)
            {
                Letter = letter;
                Position = position;
                if ("-J7".Contains(Letter)) PossibleMoveDir.Add(new Point(-1, 0));
                if ("-LF".Contains(Letter)) PossibleMoveDir.Add(new Point(1, 0));
                if ("|JL".Contains(Letter)) PossibleMoveDir.Add(new Point(0, -1));
                if ("|7F".Contains(Letter)) PossibleMoveDir.Add(new Point(0, 1));
            }
        }

        private MazeNode[,] maze = new MazeNode[0, 0];
        private Point sPos = new Point();
        private List<Point> pointToScan = new List<Point>();
        private int floodFillMaxDistance = 0;

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day10(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // FloodFill the loop
            FloodFillLoop();

            // Done
            return floodFillMaxDistance.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // FloodFill the loop
            FloodFillLoop();

            // Scan Maze horizontally... We switch from OUT to IN everytime we cross a wall
            int nbrPosInsideTheLoop = 0;
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                int nbrOfCrossedWall = 0;
                string inCorner = string.Empty;
                for (int x = 0; x < maze.GetLength(0); x++)
                {
                    // Wall pass or point check?
                    if (maze[x, y].DistanceFromS == -1)
                    {
                        // Check if IN
                        if (nbrOfCrossedWall % 2 == 1) nbrPosInsideTheLoop++;
                    }
                    else
                    {
                        if (maze[x, y].Letter == "|")
                        {
                            // Easy case
                            nbrOfCrossedWall += 1;
                        }
                        else if ("7FLJ".Contains((maze[x, y].Letter)))
                        {
                            // We can't know yet, need see what kind of corner we get at the end
                            if (inCorner == string.Empty)
                            {
                                // Save entry corner
                                inCorner = maze[x, y].Letter;
                            }
                            else
                            {
                                // Check and decide if wall as been passed
                                if (inCorner == "F" && maze[x, y].Letter == "J" || inCorner == "L" && maze[x, y].Letter == "7") nbrOfCrossedWall++;
                                inCorner = string.Empty;
                            }
                        }
                    }
                }
            }

            // Done
            return nbrPosInsideTheLoop.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void FloodFillLoop()
        {
            // Fix start position
            maze[sPos.X, sPos.Y] = new MazeNode(FindPipeType(sPos), sPos);
            maze[sPos.X, sPos.Y].DistanceFromS = 0;

            // Initialization of the Scan
            pointToScan = new List<Point>();
            pointToScan.Add(sPos);

            // Scan while we have points
            while (pointToScan.Count > 0)
            {
                ScanThisPoint(pointToScan[0]);
                pointToScan.RemoveAt(0);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private string FindPipeType(Point pt)
        {
            List<Tuple<Point, Point, string>> caseToCheck = new Tuple<Point, Point, string>[]
            {
                new Tuple<Point, Point, string>(new Point(1, 0), new Point(0, 1), "J"),
                new Tuple<Point, Point, string>(new Point(1, 0), new Point(0, -1), "7"),
                new Tuple<Point, Point, string>(new Point(-1, 0), new Point(0, -1), "F"),
                new Tuple<Point, Point, string>(new Point(-1, 0), new Point(0, 1), "L"),
                new Tuple<Point, Point, string>(new Point(0, 1), new Point(0, -1), "|"),
                new Tuple<Point, Point, string>(new Point(1, 0), new Point(-1, 0), "-")
            }.ToList();

            foreach (Tuple<Point, Point, string> t in caseToCheck)
            {
                if (pt.X - t.Item1.X < 0 || pt.X - t.Item1.X > maze.GetUpperBound(0) || pt.Y - t.Item1.Y < 0 || pt.Y - t.Item1.Y > maze.GetUpperBound(1)) continue;
                if (pt.X - t.Item2.X < 0 || pt.X - t.Item2.X > maze.GetUpperBound(0) || pt.Y - t.Item2.Y < 0 || pt.Y - t.Item2.Y > maze.GetUpperBound(1)) continue;
                if (maze[pt.X - t.Item1.X, pt.Y - t.Item1.Y].PossibleMoveDir.Contains(t.Item1) && maze[pt.X - t.Item2.X, pt.Y - t.Item2.Y].PossibleMoveDir.Contains(t.Item2)) return t.Item3;
            }

            // Should never arrive here
            throw new InvalidOperationException();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void ScanThisPoint(Point pt)
        {
            int distance = maze[pt.X, pt.Y].DistanceFromS;
            foreach (Point d in maze[pt.X, pt.Y].PossibleMoveDir)
            {
                Point newP = new Point(pt.X + d.X, pt.Y + d.Y);
                if (newP.X < 0 || newP.X > maze.GetUpperBound(0) || newP.Y < 0 || newP.Y > maze.GetUpperBound(1) || maze[newP.X, newP.Y].DistanceFromS >= 0) continue;
                maze[pt.X + d.X, pt.Y + d.Y].DistanceFromS = distance + 1;
                if (distance + 1 > floodFillMaxDistance) floodFillMaxDistance = distance + 1;
                pointToScan.Add(newP);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            maze = new MazeNode[rawData[0].Length, rawData.Count];
            for (int y = 0; y < rawData.Count; y++)
            {
                for (int x = 0; x < rawData[y].Length; x++)
                {
                    maze[x, y] = new MazeNode(rawData[y].Substring(x, 1), new Point(x, y));
                    if (maze[x, y].Letter == "S") sPos = new Point(x, y);
                }
            }
        }
    }
}
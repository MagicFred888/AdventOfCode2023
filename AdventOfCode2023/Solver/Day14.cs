using System.Diagnostics;

namespace AdventOfCode2023.Solver
{
    internal class Day14 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Parabolic Reflector Dish";

        private int[,] table = new int[0, 0];
        private List<int> valuesAfterEachCycleForSol2 = new List<int>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day14(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Incline Table to North direction (UP)
            MoveTableNorth();

            // Done
            return ComputeLoad().ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Incline Table to North direction (UP)
            int stepForStabilization = 50;
            int patternSize = 0;
            for (int cycle = 0; cycle < 1000000000; cycle++)
            {
                // Make one cycle
                for (int rotCycle = 0; rotCycle < 4; rotCycle++)
                {
                    MoveTableNorth();
                    RotateMatrixClockwise();
                }
                valuesAfterEachCycleForSol2.Add(ComputeLoad());

                // Try to find a repetitive pattern
                patternSize = FindPattern(stepForStabilization);
                if (patternSize == 0) stepForStabilization += 10;
                if (patternSize > 0) break;
            }

            // Extrapolate value at 1000000000
            int offset = (1000000000 - stepForStabilization) % patternSize;
            int answer = valuesAfterEachCycleForSol2[stepForStabilization + patternSize + offset - 1];

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private int FindPattern(int stepsForStabilization)
        {
            // To be tweked later if it doesn't work
            int compareBlockSize = 50;

            // Check if enough data
            if (valuesAfterEachCycleForSol2.Count < stepsForStabilization + 2 * compareBlockSize) return -1;

            // Search
            List<int> patterns = valuesAfterEachCycleForSol2.GetRange(stepsForStabilization, compareBlockSize);
            for (int offset = 1; offset < compareBlockSize; offset++)
            {
                bool fullScanOk = true;
                for (int i = 0; i < patterns.Count; i++)
                {
                    if (valuesAfterEachCycleForSol2[stepsForStabilization + offset + i] != patterns[i])
                    {
                        fullScanOk = false;
                        break;
                    }
                }
                if (fullScanOk) return offset;
            }
            return 0;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private int ComputeLoad()
        {
            int answer = 0;
            for (int y = 0; y <= table.GetUpperBound(1); y++)
            {
                int nbrOfStoneInRow = 0;
                for (int x = 0; x <= table.GetUpperBound(0); x++)
                {
                    nbrOfStoneInRow += (table[x, y] == -1 ? 1 : 0);
                }
                answer += (table.GetUpperBound(1) - y + 1) * nbrOfStoneInRow;
            }
            return answer;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void RotateMatrixClockwise()
        {
            int[,] newTable = new int[table.GetUpperBound(1) + 1, table.GetUpperBound(0) + 1];
            int newTableMaxX = newTable.GetUpperBound(0);
            for (int x = 0; x <= table.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= table.GetUpperBound(1); y++)
                {
                    newTable[newTableMaxX - y, x] = table[x, y];
                }
            }
            table = newTable;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void MoveTableNorth()
        {
            for (int x = 0; x <= table.GetUpperBound(0); x++)
            {
                int freeSpotY = -1;
                for (int y = 0; y <= table.GetUpperBound(1); y++)
                {
                    if (freeSpotY == -1)
                    {
                        if (table[x, y] == 0) freeSpotY = y;
                    }
                    else
                    {
                        if (table[x, y] == -1)
                        {
                            table[x, freeSpotY] = -1;
                            table[x, y] = 0;
                            freeSpotY++;
                        }
                        else if (table[x, y] == 1)
                        {
                            freeSpotY = -1;
                        }
                    }
                    if (freeSpotY > table.GetUpperBound(1)) break;
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            valuesAfterEachCycleForSol2.Clear();
            table = new int[rawData[0].Length, rawData.Count];
            for (int y = 0; y < rawData.Count; y++)
            {
                for (int x = 0; x < rawData.Count; x++)
                {
                    string tmpChar = rawData[y].Substring(x, 1);
                    table[x, y] = (tmpChar == "O" ? -1 : (tmpChar == "#" ? 1 : 0));
                }
            }
        }
    }
}
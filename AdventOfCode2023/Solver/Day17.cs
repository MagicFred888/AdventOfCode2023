namespace AdventOfCode2023.Solver
{
    internal class Day17 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Clumsy Crucible";

        private class CrucibleInfo
        {
            public int PosX { get; private set; } = -1;
            public int PosY { get; private set; } = -1;
            public int DirX { get; private set; } = -1;
            public int DirY { get; private set; } = -1;
            public int TotalEnergyLost { get; set; } = 0;
            public int NbrOfStepInCurrentDirection { get; private set; } = 0;

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public CrucibleInfo(int pX, int pY, int dX, int dY, int totalEnergyLost = 0, int nbrOfStepInCurrentDirection = 0)
            {
                PosX = pX;
                PosY = pY;
                DirX = dX;
                DirY = dY;
                TotalEnergyLost = totalEnergyLost;
                NbrOfStepInCurrentDirection = nbrOfStepInCurrentDirection;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public string Key
            {
                get { return $"{PosX}.{PosY}.{DirX}.{DirY}.{NbrOfStepInCurrentDirection}"; }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private class CityBlock
        {
            public int PosX { get; private set; } = -1;
            public int PosY { get; private set; } = -1;
            public int EnergyLose { get; private set; } = 0;

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public CityBlock(int pX, int pY, int energyLoose)
            {
                PosX = pX;
                PosY = pY;
                EnergyLose = energyLoose;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public List<CrucibleInfo> GetPossibleExitCrucibleInfo(CrucibleInfo ci, bool ultraCrucible)
            {
                List<CrucibleInfo> result = new List<CrucibleInfo>();
                if (ci.DirY == 0)
                {
                    // Horizontal move
                    if ((ultraCrucible && ci.NbrOfStepInCurrentDirection >= 4) || !ultraCrucible)
                    {
                        result.Add(new CrucibleInfo(PosX, PosY - 1, 0, -1, ci.TotalEnergyLost, 1));
                        result.Add(new CrucibleInfo(PosX, PosY + 1, 0, 1, ci.TotalEnergyLost, 1));
                    }
                    if (ci.NbrOfStepInCurrentDirection < (ultraCrucible ? 10 : 3))
                    {
                        result.Add(new CrucibleInfo(PosX + ci.DirX, PosY, ci.DirX, ci.DirY, ci.TotalEnergyLost, ci.NbrOfStepInCurrentDirection + 1));
                    }
                }
                else
                {
                    // Vertical move
                    if ((ultraCrucible && ci.NbrOfStepInCurrentDirection >= 4) || !ultraCrucible)
                    {
                        result.Add(new CrucibleInfo(PosX - 1, PosY, -1, 0, ci.TotalEnergyLost, 1));
                        result.Add(new CrucibleInfo(PosX + 1, PosY, 1, 0, ci.TotalEnergyLost, 1));
                    }
                    if (ci.NbrOfStepInCurrentDirection < (ultraCrucible ? 10 : 3))
                    {
                        result.Add(new CrucibleInfo(PosX, PosY + ci.DirY, ci.DirX, ci.DirY, ci.TotalEnergyLost, ci.NbrOfStepInCurrentDirection + 1));
                    }
                }
                return result;
            }
        }

        private CityBlock[,] city = new CityBlock[0, 0];
        private HashSet<string> allreadyChecked = new HashSet<string>();
        private PriorityQueue<CrucibleInfo, int> crucibleToCheck = new PriorityQueue<CrucibleInfo, int>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day17(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Scan the Map
            crucibleToCheck.Clear();
            crucibleToCheck.Enqueue(new CrucibleInfo(0, 0, 1, 0), 0);
            while (crucibleToCheck.Count > 0)
            {
                int answer = MoveLowestEnergyLostCrucible(false);
                if (answer > 0) return answer.ToString();
            }
            return "-1";
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // Scan the Map
            crucibleToCheck.Clear();
            crucibleToCheck.Enqueue(new CrucibleInfo(0, 0, 1, 0), 0);
            while (crucibleToCheck.Count > 0)
            {
                int answer = MoveLowestEnergyLostCrucible(true);
                if (answer > 0) return answer.ToString();
            }
            return "-1";
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private int MoveLowestEnergyLostCrucible(bool ultraCrucible)
        {
            // Get next position to review
            CrucibleInfo ci = crucibleToCheck.Dequeue();

            // Compute position and check if within Matrix
            foreach (CrucibleInfo newCI in city[ci.PosX, ci.PosY].GetPossibleExitCrucibleInfo(ci, ultraCrucible))
            {
                // Check if in map and fix energy lost
                if (newCI.PosX < 0 || newCI.PosY < 0 || newCI.PosX > city.GetUpperBound(0) || newCI.PosY > city.GetUpperBound(1)) continue;
                newCI.TotalEnergyLost += city[newCI.PosX, newCI.PosY].EnergyLose;

                // Arrived at destination ?
                if (ci.PosX == city.GetUpperBound(0) && ci.PosY == city.GetUpperBound(1) && ci.NbrOfStepInCurrentDirection >= (ultraCrucible ? 4 : 0))
                {
                    return ci.TotalEnergyLost;
                }

                // Check if already reviewed
                if (allreadyChecked.Contains(newCI.Key)) continue;

                // Add to queue and mark as treated
                crucibleToCheck.Enqueue(newCI, newCI.TotalEnergyLost);
                allreadyChecked.Add(newCI.Key);
            }
            return -1;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            crucibleToCheck.Clear();
            allreadyChecked.Clear();
            city = new CityBlock[rawData[0].Length, rawData.Count];
            for (int y = 0; y < rawData.Count; y++)
            {
                for (int x = 0; x < rawData[y].Length; x++)
                {
                    city[x, y] = new CityBlock(x, y, int.Parse(rawData[y].Substring(x, 1)));
                }
            }
        }
    }
}
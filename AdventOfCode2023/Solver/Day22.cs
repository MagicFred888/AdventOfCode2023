namespace AdventOfCode2023.Solver
{
    internal class Day22 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Sand Slabs";

        private class BrickInfos
        {
            public int ID { get; private set; } = -1;
            public List<(int X, int Y, int Z)> Cubes { get; private set; } = new List<(int X, int Y, int Z)>();
            public List<int> BlockedBy { get; private set; } = new List<int>();

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public BrickInfos(int id, string rawData)
            {
                ID = id;
                int[] values = rawData.Replace("~", ",").Split(',').ToList().ConvertAll(i => int.Parse(i)).ToArray();
                for (int x = values[0]; x <= values[3]; x++)
                {
                    for (int y = values[1]; y <= values[4]; y++)
                    {
                        for (int z = values[2]; z <= values[5]; z++)
                        {
                            Cubes.Add((x, y, z));
                        }
                    }
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public void ZShift(int zDir)
            {
                if (zDir == 0) return;
                for (int i = 0; i < Cubes.Count; i++)
                {
                    Cubes[i] = (Cubes[i].X, Cubes[i].Y, Cubes[i].Z + zDir);
                }
            }
        }

        private List<BrickInfos> allBrickInfos = new List<BrickInfos>();
        private List<List<(int X, int Y, int ID)>> stack = new List<List<(int X, int Y, int ID)>>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day22(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Stack Down
            for (int i = 0; i < allBrickInfos.Count; i++)
            {
                StackDown(allBrickInfos[i]);
            }

            // Count
            int nbrCanBeDisintegrated = 0;
            for (int i = 0; i < allBrickInfos.Count; i++)
            {
                int nbr = allBrickInfos.Count(bi => bi.BlockedBy.Count == 1 && bi.BlockedBy.Contains(i));
                if (nbr == 0) nbrCanBeDisintegrated++;
            }

            // Done
            return nbrCanBeDisintegrated.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // Stack Down
            for (int i = 0; i < allBrickInfos.Count; i++)
            {
                StackDown(allBrickInfos[i]);
            }

            // Count moves (Not efficient but short of time, still give answer in about 1 second)
            int answer = 0;
            for (int i = 0; i < allBrickInfos.Count; i++)
            {
                List<int> willMove = new int[] { i }.ToList();
                bool loopAgain = false;
                do
                {
                    loopAgain = false;
                    List<BrickInfos> blockedBy = allBrickInfos.FindAll(bi => bi.BlockedBy.Count == bi.BlockedBy.Count(i => willMove.Contains(i)));
                    foreach (BrickInfos bi in blockedBy)
                    {
                        if (!willMove.Contains(bi.ID))
                        {
                            willMove.Add(bi.ID);
                            loopAgain = true;
                        }
                    }
                } while (loopAgain);
                answer += (willMove.Count - 1);
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void StackDown(BrickInfos toMove)
        {
            int minZ = toMove.Cubes.Min(b => b.Z);
            if (stack.Count == 0)
            {
                // Move maximum down
                toMove.ZShift(-minZ);
                toMove.BlockedBy.Add(-1);
            }
            else
            {
                int maxZ = stack.Count;
                if (minZ <= maxZ)
                {
                    // Remove up one step above
                    toMove.ZShift(1 + maxZ - minZ);
                }
                else
                {
                    // Shift one pos above
                    toMove.ZShift(maxZ - minZ + 1);
                }

                // Check loop
                bool okToMoveDown = true;
                do
                {
                    foreach ((int X, int Y, int Z) currentCube in toMove.Cubes)
                    {
                        if (currentCube.Z - 1 < 0)
                        {
                            // Blocked by ground
                            okToMoveDown = false;
                            if (!toMove.BlockedBy.Contains(-1)) toMove.BlockedBy.Add(-1);
                        }
                        else if (currentCube.Z - 1 < stack.Count)
                        {
                            // Check if blocked in the stack
                            (int X, int Y, int ID) blockingElement = stack[currentCube.Z - 1].Find(i => i.X == currentCube.X && i.Y == currentCube.Y);
                            if (blockingElement.X != 0 || blockingElement.Y != 0 || blockingElement.ID != 0)
                            {
                                okToMoveDown = false;
                                if (!toMove.BlockedBy.Contains(blockingElement.ID)) toMove.BlockedBy.Add(blockingElement.ID);
                            }
                        }
                    }
                    if (okToMoveDown) toMove.ZShift(-1);
                } while (okToMoveDown);
            }

            // Save part in the stack for speed
            foreach ((int X, int Y, int Z) cube in toMove.Cubes)
            {
                while (stack.Count <= cube.Z)
                {
                    stack.Add(new List<(int X, int Y, int ID)>());
                }
                stack[cube.Z].Add((cube.X, cube.Y, toMove.ID));
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            // Extract all bricks infos and sort by Z (looks to be the trick to make it work)
            stack.Clear();
            allBrickInfos.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allBrickInfos.Add(new BrickInfos(pos, rawData[pos]));
            }
            allBrickInfos.Sort(delegate (BrickInfos i1, BrickInfos i2) { return i1.Cubes.Min(c => c.Z).CompareTo(i2.Cubes.Min(c => c.Z)); });
        }
    }
}
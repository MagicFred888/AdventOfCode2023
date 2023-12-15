namespace AdventOfCode2023.Solver
{
    internal class Day15 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Lens Library";

        private class Instruction
        {
            public int FullHash { get; private set; } = -1;
            public int BoxId { get; private set; } = -1;
            public string LenseCode { get; private set; } = string.Empty;
            public int Action { get; private set; } = 0;
            public int FocalLength { get; private set; } = -1;

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public Instruction(string rawData)
            {
                FullHash = rawData.ToCharArray().Aggregate(0, (hash, c) => ((hash + c) * 17) % 256);
                Action = rawData.EndsWith("-") ? -1 : 1;
                LenseCode = rawData.Substring(0, rawData.IndexOf(Action == 1 ? '=' : '-'));
                if (Action == 1) FocalLength = int.Parse(rawData.Substring(rawData.IndexOf("=") + 1));
                BoxId = LenseCode.ToCharArray().Aggregate(0, (hash, c) => ((hash + c) * 17) % 256);
            }
        }

        private List<Instruction> allInstructions = new List<Instruction>();
        private List<Dictionary<string, int>> allBoxes = new List<Dictionary<string, int>>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day15(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Simple, so just one line
            return allInstructions.Aggregate(0, (answer, i) => answer + i.FullHash).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // Perform all lenses shifts
            foreach (Instruction i in allInstructions)
            {
                if (i.Action == 1)
                {
                    // Add
                    if (!allBoxes[i.BoxId].ContainsKey(i.LenseCode)) allBoxes[i.BoxId].Add(i.LenseCode, 0);
                    allBoxes[i.BoxId][i.LenseCode] = i.FocalLength;
                }
                else
                {
                    // Remove
                    if (allBoxes[i.BoxId].ContainsKey(i.LenseCode))
                    {
                        allBoxes[i.BoxId].Remove(i.LenseCode);
                        allBoxes[i.BoxId] = new Dictionary<string, int>(allBoxes[i.BoxId]); // Needed otherwise when adding the same key it will back to it's original position
                    }
                }
            }

            // Compute solution
            int answer = 0;
            for (int i = 0; i < allBoxes.Count; i++)
            {
                List<string> keys = new List<string>(allBoxes[i].Keys);
                answer += allBoxes[i].Aggregate(0, (result, kvp) => result + ((i + 1) * (keys.IndexOf(kvp.Key) + 1) * kvp.Value));
            }


            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allInstructions.Clear();
            allBoxes = new Dictionary<string, int>[256].ToList().ConvertAll(i => new Dictionary<string, int>());
            foreach (string data in rawData[0].Split(','))
            {
                allInstructions.Add(new Instruction(data));
            }
        }
    }
}
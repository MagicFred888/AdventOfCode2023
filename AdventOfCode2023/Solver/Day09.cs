namespace AdventOfCode2023.Solver
{
    internal class Day09 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Mirage Maintenance";
        private class DataSerie
        {
            private List<int> allData = new List<int>();

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public DataSerie(List<int> dataSerie)
            {
                allData = dataSerie;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public int NextValue
            {
                get { return Predict(true); }
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public int PreviousValue
            {
                get { return Predict(false); }
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            private int Predict(bool next)
            {
                // Prepare data
                List<List<int>> steps = new List<List<int>>();
                steps.Add(allData);
                if (next) steps[0].Reverse();

                // Scan down to zero
                while (steps[0].Count(v => v == 0) != steps[0].Count())
                {
                    List<int> newSet = new List<int>();
                    for (int i = 0; i < steps[0].Count - 1; i++)
                    {
                        newSet.Add(steps[0][i] - steps[0][i + 1]);
                    }
                    steps.Insert(0, newSet);
                }

                // Back up...
                for (int i = 1; i < steps.Count; i++)
                {
                    steps[i].Insert(0, steps[i - 1][0] + steps[i][0]);
                }

                // Done
                return steps[steps.Count - 1][0];
            }
        }

        private List<DataSerie> allDataSerie = new List<DataSerie>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day09(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Compute solution
            return allDataSerie.Sum(d => d.NextValue).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Compute solution
            return allDataSerie.Sum(d => d.PreviousValue).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allDataSerie.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allDataSerie.Add(new DataSerie(Array.ConvertAll(rawData[pos].Split(' ', StringSplitOptions.RemoveEmptyEntries), i => int.Parse(i)).ToList()));
            }
        }
    }
}
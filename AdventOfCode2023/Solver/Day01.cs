namespace AdventOfCode2023.Solver
{
    internal class Day01 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Trebuchet?!";

        private List<string> allNumbers = new List<string>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day01(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Case 1
            allNumbers = new List<string>(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            return SolveChallenge();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Case 2
            allNumbers = new List<string>(new string[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
            return SolveChallenge();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private string SolveChallenge()
        {
            List<int> allValue = new List<int>();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                string result = string.Empty;
                for (int i = 0; i < rawData[pos].Length; i++)
                {
                    int matchPos = allNumbers.FindIndex(c => rawData[pos].Substring(i).StartsWith(c));
                    if (matchPos >= 0)
                    {
                        result = allNumbers[matchPos].Length == 1 ? allNumbers[matchPos] : (matchPos + 1).ToString();
                        break;
                    }
                }
                for (int i = rawData[pos].Length - 1; i >= 0; i--)
                {
                    int matchPos = allNumbers.FindIndex(c => rawData[pos].Substring(i).StartsWith(c));
                    if (matchPos >= 0)
                    {
                        result += allNumbers[matchPos].Length == 1 ? allNumbers[matchPos] : (matchPos + 1).ToString();
                        break;
                    }
                }
                allValue.Add(int.Parse(result));
            }

            // Done
            return allValue.Sum().ToString();
        }
    }
}
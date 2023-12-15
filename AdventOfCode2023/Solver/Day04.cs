namespace AdventOfCode2023.Solver
{
    internal class Day04 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Scratchcards";

        private class Scratchcard
        {
            public int CardNumber { get; private set; } = 0;
            public List<int> WinningNumbers { get; private set; } = new List<int>();
            public List<int> Numbers { get; private set; } = new List<int>();

            public Scratchcard(string rawData)
            {
                CardNumber = int.Parse(rawData.Substring(rawData.IndexOf(" ") + 1).Split(':')[0]);
                string[] numbersSeries = rawData.Substring(rawData.IndexOf(":") + 1).Split('|');
                WinningNumbers = Array.ConvertAll<string, int>(numbersSeries[0].Split(' ', StringSplitOptions.RemoveEmptyEntries), n => int.Parse(n)).ToList();
                Numbers = Array.ConvertAll<string, int>(numbersSeries[1].Split(' ', StringSplitOptions.RemoveEmptyEntries), n => int.Parse(n)).ToList();
            }

            public int NbrOfWinningNumber
            {
                get { return Numbers.FindAll(n => WinningNumbers.Contains(n)).Count; }
            }

            public int CardWinningPoints
            {
                get { return (int)(NbrOfWinningNumber == 0 ? 0 : Math.Pow(2, NbrOfWinningNumber - 1)); }
            }
        }

        private List<Scratchcard> allScratchcards = new List<Scratchcard>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day04(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData();

            // Done
            return allScratchcards.Sum(s => s.CardWinningPoints).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData();

            // Count cards
            List<int> nbrOfCards = new List<int>(new int[allScratchcards.Count]).ConvertAll(s => 1); ;
            for (int i = 0; i < allScratchcards.Count; i++)
            {
                for (int j = 0; j < allScratchcards[i].NbrOfWinningNumber && i + j + 1 < nbrOfCards.Count; j++)
                {
                    nbrOfCards[i + j + 1] += nbrOfCards[i];
                }
            }

            // Done
            return nbrOfCards.Sum().ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allScratchcards.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allScratchcards.Add(new Scratchcard(rawData[pos]));
            }
        }
    }
}
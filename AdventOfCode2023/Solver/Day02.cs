using System.Text.RegularExpressions;

namespace AdventOfCode2023.Solver
{
    internal class Day02 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Cube Conundrum";

        private static List<string> colors = new List<string>();
        private static Regex rgxNonDigits = new Regex(@"[^\d]+");

        private class Game
        {
            public int GameId = -1;
            public List<List<KeyValuePair<int, int>>> Results = new List<List<KeyValuePair<int, int>>>();
            public Dictionary<int, int> MaxNbrPerDie = new Dictionary<int, int>();

            public Game(string rawData)
            {
                GameId = int.Parse(rgxNonDigits.Replace(rawData.Split(':')[0], ""));
                string[] games = rawData.Split(':')[1].Split(";");
                foreach (string game in games)
                {
                    List<KeyValuePair<int, int>> currentRound = new List<KeyValuePair<int, int>>();
                    string[] diesCombination = game.Trim().Split(",");
                    foreach (string dieCombination in diesCombination)
                    {
                        // Data for question 1 
                        string[] infos = dieCombination.Trim().Split(" ");
                        if (!colors.Contains(infos[1])) colors.Add(infos[1]);
                        currentRound.Add(new KeyValuePair<int, int>(colors.IndexOf(infos[1]), int.Parse(infos[0])));

                        // Data for question 2
                        if (!MaxNbrPerDie.ContainsKey(colors.IndexOf(infos[1]))) MaxNbrPerDie.Add(colors.IndexOf(infos[1]), 0);
                        if (MaxNbrPerDie[colors.IndexOf(infos[1])] < int.Parse(infos[0])) MaxNbrPerDie[colors.IndexOf(infos[1])] = int.Parse(infos[0]);
                    }
                    Results.Add(currentRound);
                }
            }
        }

        private List<Game> allGames = new List<Game>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day02(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // count
            int answer = 0;
            Dictionary<string, int> dies = new Dictionary<string, int>() { { "red", 12 }, { "green", 13 }, { "blue", 14 } };
            foreach (Game game in allGames)
            {
                bool allRoundsOk = true;
                foreach (List<KeyValuePair<int, int>> roundInfos in game.Results)
                {
                    foreach (KeyValuePair<int, int> info in roundInfos)
                    {
                        if (info.Value > dies[colors[info.Key]])
                        {
                            allRoundsOk = false;
                            break;
                        }
                    }
                    if (!allRoundsOk) break;
                }
                if (allRoundsOk) answer += game.GameId;
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // count
            int answer = 0;
            foreach (Game game in allGames)
            {
                int power = 1;
                Array.ForEach(game.MaxNbrPerDie.Values.ToArray(), i => power *= i);
                answer += power;
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allGames.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allGames.Add(new Game(rawData[pos]));
            }
        }
    }
}
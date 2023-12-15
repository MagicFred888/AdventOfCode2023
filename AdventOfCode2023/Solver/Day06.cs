namespace AdventOfCode2023.Solver
{
    internal class Day06 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Wait For It";
        private class Race
        {
            public long Time { get; private set; } = 0;
            public long Distance { get; private set; } = 0;

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public Race(long time, long distance)
            {
                Time = time;
                Distance = distance;
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public long PossibleWinningStrategies
            {
                get
                {
                    // Mathematicien way
                    double p1 = (Time - Math.Sqrt(Math.Pow(Time, 2) - 4 * Distance)) / 2;
                    p1 = p1 == Math.Ceiling(p1) ? p1 + 1 : Math.Ceiling(p1);
                    double p2 = (Time + Math.Sqrt(Math.Pow(Time, 2) - 4 * Distance)) / 2;
                    p2 = p2 == Math.Floor(p2) ? p2 - 1 : Math.Floor(p2);
                    return (long)Math.Round(p2 - p1 + 1, 0);

                    // Brut-Force style
                    /*
                    long answer = 0;
                    for (long i = 1; i < Time; i++)
                    {
                        if ((Time - i) * i > Distance) answer++;
                    }
                    return answer;
                    */
                }
            }
        }

        private List<Race> allRaces = new List<Race>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day06(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Clean data
            BuildData1();

            // Compute solution
            long answer = 1;
            foreach (var race in allRaces)
            {
                answer *= race.PossibleWinningStrategies;
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Clean data
            BuildData2();

            // Done
            return allRaces[0].PossibleWinningStrategies.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData1()
        {
            allRaces.Clear();
            List<long> time = Array.ConvertAll<string, long>(rawData[0].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries), i => int.Parse(i)).ToList();
            List<long> distance = Array.ConvertAll<string, long>(rawData[1].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries), i => int.Parse(i)).ToList();
            for (int pos = 0; pos < time.Count; pos++)
            {
                allRaces.Add(new Race(time[pos], distance[pos]));
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData2()
        {
            allRaces.Clear();
            long time = long.Parse(rawData[0].Split(':')[1].Replace(" ", ""));
            long distance = long.Parse(rawData[1].Split(':')[1].Replace(" ", ""));
            allRaces.Add(new Race(time, distance));
        }
    }
}
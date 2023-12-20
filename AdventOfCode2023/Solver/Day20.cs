namespace AdventOfCode2023.Solver
{
    internal class Day20 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Pulse Propagation";

        private class Module
        {
            public string Type { get; private set; } = string.Empty;
            public string Name { get; private set; } = string.Empty;
            public List<string> Children { get; private set; } = new List<string>();
            public bool FlipFlopState { get; private set; } = false;
            public Dictionary<string, bool> Inputs { get; private set; } = new Dictionary<string, bool>();
            public int NbrOfLowPulseEmitted { get; private set; } = 0;
            public int NbrOfHighPulseEmitted { get; private set; } = 0;

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public Module(string rawData)
            {
                if (rawData.StartsWith("broadcaster"))
                {
                    Type = "broadcaster";
                    rawData = "x" + rawData; // Trick to have correct name later, x has no matter, just need 1 char
                }
                else if (rawData.StartsWith("%"))
                {
                    Type = "FlipFlop";
                }
                else
                {
                    Type = "Conjunction";
                }
                Name = rawData.Substring(1, rawData.IndexOf(' ') - 1);
                Children = rawData.Substring(rawData.IndexOf("->") + 2).Replace(" ", "").Split(',').Where(i => i != "").ToList();
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public void AddInput(string inputName)
            {
                Inputs.Add(inputName, false);
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public List<(string source, string target, bool pulseType)> Input(string source, bool highPulse)
            {
                bool emitHighPulse = false;
                if (Type == "broadcaster")
                {
                    // Emit same
                    emitHighPulse = highPulse;
                }
                else if (Type == "FlipFlop")
                {
                    // If high nothing, otherwise switch flipFlop and emit new state
                    if (highPulse) return new List<(string source, string target, bool pulseType)>();
                    FlipFlopState = !FlipFlopState;
                    emitHighPulse = FlipFlopState;
                }
                else
                {
                    // Emit low if all at High
                    Inputs[source] = highPulse;
                    emitHighPulse = Inputs.Values.Count(i => i == true) != Inputs.Values.Count();
                }

                // Make count and return value
                if (emitHighPulse) NbrOfHighPulseEmitted += Children.Count;
                if (!emitHighPulse) NbrOfLowPulseEmitted += Children.Count;
                return Children.ConvertAll(i => (Name, i, emitHighPulse)).ToList();
            }
        }

        private Dictionary<string, Module> allModules = new Dictionary<string, Module>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day20(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Execute 1000 cycles
            for (int i = 0; i < 1000; i++)
            {
                List<(string source, string target, bool pulseType)> toTreat = new List<(string source, string target, bool pulseType)>();
                toTreat.Add(("", "broadcaster", false));
                while (toTreat.Count > 0)
                {
                    (string source, string target, bool pulseType) data = toTreat[0];
                    toTreat.RemoveAt(0);
                    toTreat.AddRange(allModules[data.target].Input(data.source, data.pulseType));
                }
            }

            // Count results
            int low = 1000 + allModules.Values.Sum(v => v.NbrOfLowPulseEmitted);
            int high = allModules.Values.Sum(v => v.NbrOfHighPulseEmitted);
            return (low * high).ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // List items to monitor (lx found by checking puzzle input manually)
            Dictionary<string, long> rxInputs = new Dictionary<string, long>();
            foreach (string input in allModules["lx"].Inputs.Keys) { rxInputs.Add(input, 0); }

            // Execute cycle 
            int i = 0;
            do
            {
                List<(string source, string target, bool pulseType)> toTreat = new List<(string source, string target, bool pulseType)>();
                toTreat.Add(("", "broadcaster", false));
                while (toTreat.Count > 0)
                {
                    (string source, string target, bool pulseType) data = toTreat[0];
                    toTreat.RemoveAt(0);
                    foreach ((string source, string target, bool pulseType) newData in allModules[data.target].Input(data.source, data.pulseType))
                    {
                        if (rxInputs.ContainsKey(newData.source) && newData.pulseType == true && rxInputs[newData.source] == 0)
                        {
                            rxInputs[newData.source] = i + 1;
                            if (rxInputs.Values.Count == rxInputs.Values.Count(v => v > 0))
                            {
                                // Quick and dirty
                                long answer = LCM(rxInputs.Values.ToArray());
                                return answer.ToString();
                            }
                        }
                        toTreat.Add(newData);
                    }
                }
                i++;
            } while (true);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allModules.Clear();
            List<string> keys = new List<string>();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                Module newModule = new Module(rawData[pos]);
                allModules.Add(newModule.Name, newModule);
            }
            for (int i = 0; i < allModules.Count; i++)
            {
                string key = allModules.Keys.ToList()[i];
                foreach (string child in allModules[key].Children)
                {
                    if (!allModules.ContainsKey(child))
                    {
                        allModules.Add(child, new Module($"&{child} ->"));
                    }
                    allModules[child].AddInput(allModules[key].Name);
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        // For LCM and GCD, thanks to https://www.geeksforgeeks.org/lcm-of-given-array-elements/
        //------------------------------------------------------------------------------------------------------------------------------------------------------ 
        private long LCM(long[] numbers)
        {
            return numbers.Aggregate((long x, long y) => x * y / GCD(x, y));
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private long GCD(long a, long b)
        {
            if (b == 0) return a;
            return GCD(b, a % b);
        }
    }
}
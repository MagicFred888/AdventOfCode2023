namespace AdventOfCode2023.Solver
{
    internal class Day19 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Aplenty";

        private class PartInfos
        {
            public Dictionary<string, int> TypeVal = new Dictionary<string, int>();

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public PartInfos(string rawData)
            {
                int[] values = Array.ConvertAll<string, int>(rawData.Trim('}').Replace(',', '=').Split('=').Where(i => Microsoft.VisualBasic.Information.IsNumeric(i)).ToArray(), i => int.Parse(i));
                TypeVal = new Dictionary<string, int>() { { "x", values[0] }, { "m", values[1] }, { "a", values[2] }, { "s", values[3] } };
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private class Workflow
        {
            public string Name { get; private set; } = string.Empty;
            public List<(string param, string sign, int value, string target)> Operations { get; private set; } = new List<(string param, string sign, int value, string target)>();

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public Workflow(string rawData)
            {
                Name = rawData.Substring(0, rawData.IndexOf('{'));
                string[] ops = rawData.Substring(rawData.IndexOf('{') + 1).Trim('}').Split(',');
                foreach (string op in ops)
                {
                    if (op.Contains(':'))
                    {
                        string param = op.Substring(0, 1);
                        string sign = op.Substring(1, 1);
                        string target = op.Substring(op.IndexOf(':') + 1);
                        string value = op.Substring(2);
                        value = value.Substring(0, value.IndexOf(':'));
                        Operations.Add((param, sign, int.Parse(value), target));
                    }
                    else
                    {
                        Operations.Add(("", "", -1, op));
                    }
                }
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public string GetResult(PartInfos part)
            {
                // Scan each operation to get first one who match
                foreach ((string param, string sign, int value, string target) op in Operations)
                {
                    if (op.param == string.Empty) return op.target;
                    if (op.sign == ">" && part.TypeVal[op.param] > op.value || op.sign == "<" && part.TypeVal[op.param] < op.value) return op.target;
                }
                throw new NotSupportedException();
            }

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public List<RangeInfos> GetRangesInfos(RangeInfos rangeToCheck)
            {
                List<RangeInfos> result = new List<RangeInfos>();
                foreach ((string param, string sign, int value, string target) op in Operations)
                {
                    if (op.param == string.Empty)
                    {
                        // Last item, we use rangeToCheck with correct target
                        rangeToCheck.Target = op.target;
                        result.Add(rangeToCheck);
                        return result;
                    }

                    // Prepare object for check
                    RangeInfos metCriteria = rangeToCheck.Clone();
                    metCriteria.Target = op.target;

                    // Check case
                    if (op.sign == ">")
                    {
                        // Bigger than
                        metCriteria.Ranges[op.param][0] = op.value + 1;
                        result.Add(metCriteria);
                        rangeToCheck.Ranges[op.param][1] = op.value;
                    }
                    else
                    {
                        // Smaller than
                        metCriteria.Ranges[op.param][1] = op.value - 1;
                        result.Add(metCriteria);
                        rangeToCheck.Ranges[op.param][0] = op.value;
                    }
                }
                throw new NotSupportedException();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private class RangeInfos
        {
            public string Target { get; set; } = string.Empty;
            public Dictionary<string, int[]> Ranges { get; private set; } = new Dictionary<string, int[]>
            {
                {"x",  new int[]{1, 4000}}, {"m", new int[]{1, 4000}}, {"a", new int[]{1, 4000 }}, {"s", new int[]{1, 4000}}
            };

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public RangeInfos Clone()
            {
                RangeInfos result = new RangeInfos();
                result.Target = Target;
                result.Ranges = new Dictionary<string, int[]>();
                foreach (KeyValuePair<string, int[]> kvp in Ranges)
                {
                    result.Ranges.Add(kvp.Key, new int[] { kvp.Value[0], kvp.Value[1] });
                }
                return result;
            }
        }

        private List<PartInfos> allParts = new List<PartInfos>();
        private Dictionary<string, Workflow> allWorkflows = new Dictionary<string, Workflow>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day19(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            // Build clean data set
            BuildData();

            // Process each parts
            int answer = 0;
            foreach (PartInfos part in allParts)
            {
                string targetWorkflow = "in";
                do
                {
                    targetWorkflow = allWorkflows[targetWorkflow].GetResult(part);
                    if (targetWorkflow == "R") break;
                    if (targetWorkflow == "A")
                    {
                        answer += part.TypeVal.Values.Sum();
                        break;
                    }
                } while (true);
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            // Build clean data set
            BuildData();

            // Process each workflow by range
            long answer = 0;
            RangeInfos iniRange = new RangeInfos();
            iniRange.Target = "in";
            List<RangeInfos> workflowToCheck = new RangeInfos[] { iniRange }.ToList();
            do
            {
                RangeInfos targetWorkflow = workflowToCheck[0];
                workflowToCheck.RemoveAt(0);
                foreach (RangeInfos range in allWorkflows[targetWorkflow.Target].GetRangesInfos(targetWorkflow))
                {
                    if (range.Target == "R") continue; // We stop that branch
                    if (range.Target == "A")
                    {
                        // Add as possible solution
                        answer += range.Ranges.Values.Aggregate(1L, (long result, int[] i) => result * ((long)i[1] - (long)i[0] + 1L));
                        continue;
                    }
                    workflowToCheck.Add(range); // Need to check that branch
                }
            } while (workflowToCheck.Count > 0);

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData()
        {
            allParts.Clear();
            allWorkflows.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                if (rawData[pos] == string.Empty) continue;
                if (rawData[pos].StartsWith("{"))
                {
                    allParts.Add(new PartInfos(rawData[pos]));
                }
                else
                {
                    Workflow newWf = new Workflow(rawData[pos]);
                    allWorkflows.Add(newWf.Name, newWf);
                }
            }
        }
    }
}
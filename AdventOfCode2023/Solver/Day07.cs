namespace AdventOfCode2023.Solver
{
    internal class Day07 : BaseSolver
    {
        public override string PuzzleTitle { get; } = "Camel Cards";

        private enum HandsName
        {
            FiveOfAKkind = 6, // AAAAA
            FourOfAKind = 5,  // AA8AA
            FullHouse = 4,    // 23332
            ThreeOfAKind = 3, // TTT98
            TwoPair = 2,      // 23432
            OnePair = 1,      // A23A4
            HighCard = 0      // 23456
        }

        private class HandInfos
        {
            public int[] Cards { get; private set; } = null;
            public int Bid { get; private set; } = 0;
            public HandsName HandsName { get; private set; } = HandsName.HighCard;

            //------------------------------------------------------------------------------------------------------------------------------------------------------
            public HandInfos(string rawData, bool withJockerRules)
            {
                // Card value
                string cardsValues = withJockerRules ? "J23456789TQKA" : "23456789TJQKA";

                // Compute Hands data structure
                Cards = Array.ConvertAll<char, int>(rawData.Split(' ')[0].ToCharArray(), c => cardsValues.IndexOf(c));
                Dictionary<int, KeyValuePair<int, int>> tmpHand = new Dictionary<int, KeyValuePair<int, int>>();
                foreach (int card in Cards)
                {
                    if (!tmpHand.ContainsKey(card))
                        tmpHand.Add(card, new KeyValuePair<int, int>(card, 1));
                    else
                        tmpHand[card] = new KeyValuePair<int, int>(card, tmpHand[card].Value + 1);
                }
                List<KeyValuePair<int, int>> hand = new List<KeyValuePair<int, int>>(tmpHand.Values);
                hand.Sort(delegate (KeyValuePair<int, int> i1, KeyValuePair<int, int> i2)
                {
                    if (i1.Value != i2.Value) return i2.Value.CompareTo(i1.Value);
                    return i2.Key.CompareTo(i1.Key);
                });

                // Improve structure if using Jocker rule
                if (withJockerRules && Cards.Contains(0) && hand.Count > 1)
                {
                    KeyValuePair<int, int> jockerPair = hand.Find(h => h.Key == 0);
                    hand.Remove(jockerPair);
                    hand[0] = new KeyValuePair<int, int>(0, hand[0].Value + jockerPair.Value);
                }

                // Compute hand type
                switch (hand.Count)
                {
                    case 1:
                        HandsName = HandsName.FiveOfAKkind;
                        break;
                    case 2:
                        HandsName = hand[0].Value == 4 ? HandsName.FourOfAKind : HandsName.FullHouse;
                        break;
                    case 3:
                        HandsName = hand[0].Value == 3 ? HandsName.ThreeOfAKind : HandsName.TwoPair;
                        break;
                    case 4:
                        HandsName = HandsName.OnePair;
                        break;

                    default:
                        break;
                }

                // Bid
                Bid = int.Parse(rawData.Split(' ')[1]);
            }
        }

        private List<HandInfos> allHandInfos = new List<HandInfos>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public Day07(int day) : base(day) { }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution1()
        {
            return GetSolution(false);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        public override string GetSolution2()
        {
            return GetSolution(true);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private string GetSolution(bool withJockerRule)
        {
            // Clean data
            BuildData(withJockerRule);

            // Sort data
            allHandInfos.Sort(delegate (HandInfos i1, HandInfos i2)
            {
                // Different hands ?
                if (i1.HandsName != i2.HandsName) return i1.HandsName.CompareTo(i2.HandsName);

                // Compare cards value
                for (int i = 0; i < i1.Cards.Length; i++)
                {
                    if (i1.Cards[i] != i2.Cards[i]) return i1.Cards[i].CompareTo(i2.Cards[i]);
                }

                // Tie
                return 0;
            });

            // Compute answer
            int answer = 0;
            for (int i = 0; i < allHandInfos.Count; i++)
            {
                answer += (i + 1) * allHandInfos[i].Bid;
            }

            // Done
            return answer.ToString();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------
        private void BuildData(bool withJockerRule)
        {
            allHandInfos.Clear();
            for (int pos = 0; pos < rawData.Count; pos++)
            {
                allHandInfos.Add(new HandInfos(rawData[pos], withJockerRule));
            }
        }
    }
}
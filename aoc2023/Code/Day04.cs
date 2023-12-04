namespace aoc2023.Code;

internal class Day04 : BaseDay
{
    class Card
    {
        public List<int> Winning { get; set; }
        public List<int> Have { get; set; }
        public int Count { get; set; }
        public int Points { get; set; }

        public int Id { get; set; }

        public Card(string id, string row)
        {
            Id = int.Parse(id[(id.IndexOf(' ') + 1)..]) - 1;

            var data = row.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Winning = data[0].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
            Have = data[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList();

            Count = Winning.Intersect(Have).Count();
            Points = Count > 0 ? (int) Math.Pow(2, Count - 1) : 0;
        }
    }

    protected override object Part1() => ReadAllLinesSplit(":", true).Select(x => new Card(x[0], x[1])).Sum(x => x.Points);

    protected override object Part2()
    {
        var cards = ReadAllLinesSplit(":", true).Select(x => new Card(x[0], x[1])).ToList();
        var pile = cards.Count;

        var queue = new Queue<Card>(cards);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();

            if (c.Count > 0)
            {
                for (int i = c.Id + 1; i < c.Id + c.Count + 1; i++)
                {
                    queue.Enqueue(cards[i]);
                    pile++;
                }
            }
        }

        return pile;
    }
}
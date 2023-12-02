namespace aoc2023.Code;

internal class Day02 : BaseDay
{
    record CubeSet(int R, int G, int B);

    class Game
    {
        public int Id { get; }
        public List<CubeSet> Cubes { get; } = [];

        public Game(string[] row)
        {
            Id = int.Parse(row[0].Split(' ')[1]);

            for (int i = 1; i < row.Length; i++)
            {
                var rgb = row[i].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                var r = Extract(rgb, "red");
                var g = Extract(rgb, "green");
                var b = Extract(rgb, "blue");

                Cubes.Add(new CubeSet(r, g, b));
            }

            static int Extract(string[] rgb, string what)
            {
                foreach (var d in rgb)
                {
                    if (d.Contains(what))
                    {
                        return int.Parse(d.Split(' ')[0]);
                    }
                }
                return 0;
            }
        }

        public bool IsPossible(int r, int g, int b) => Cubes.All(y => y.R <= r && y.G <= g && y.B <= b);

        public int Power()
        {
            var r = Cubes.MaxBy(x => x.R)!.R;
            var g = Cubes.MaxBy(x => x.G)!.G;
            var b = Cubes.MaxBy(x => x.B)!.B;

            return r * g * b;
        }
    }

    IEnumerable<Game> Games => ReadAllLinesSplit(":;", true).Select(x => new Game(x));

    protected override object Part1() => Games.Where(x => x.IsPossible(12, 13, 14)).Sum(x => x.Id);

    protected override object Part2() => Games.Sum(x => x.Power());
}
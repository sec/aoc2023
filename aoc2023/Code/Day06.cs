namespace aoc2023.Code;

internal class Day06 : BaseDay
{
    record Race(long Length, long Record);

    static long CountWays(Race race)
    {
        var r = 0L;
        for (var speed = 0L; speed <= race.Length; speed++)
        {
            var dist = speed * (race.Length - speed);
            if (dist > race.Record)
            {
                r++;
            }
        }
        return r;
    }

    static IEnumerable<Race> ParseRace(string time, string distance)
    {
        var z1 = time[5..].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse);
        var z2 = distance[9..].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse);

        return z1.Zip(z2, (t, d) => new Race(t, d));
    }

    static long Solve(IEnumerable<Race> races) => races.Aggregate(1L, (x, y) => x * CountWays(y));

    protected override object Part1()
    {
        var input = ReadAllLines(true);

        return Solve(ParseRace(input[0], input[1]));
    }

    protected override object Part2()
    {
        var input = ReadAllLines(true);

        return Solve(ParseRace(input[0].Replace(" ", string.Empty), input[1].Replace(" ", string.Empty)));
    }
}
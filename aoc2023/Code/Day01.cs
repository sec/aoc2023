namespace aoc2023.Code;

internal class Day01 : BaseDay
{
    readonly List<string> words = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
    readonly List<string> sdrow = ["eno", "owt", "eerht", "ruof", "evif", "xis", "neves", "thgie", "enin"];

    static int GetInt(string row)
    {
        var tmp = row.Where(char.IsDigit);

        return int.Parse($"{tmp.First()}{tmp.Last()}");
    }

    static int GetIntExt(string row, List<string> words)
    {
        for (int i = 0; i < row.Length; i++)
        {
            if (char.IsDigit(row[i]))
            {
                return row[i] - '0';
            }

            var check = new StringBuilder();

            for (int j = i; j < row.Length; j++)
            {
                if (!char.IsLetter(row[j]))
                {
                    break;
                }
                check.Append(row[j]);
                var index = words.IndexOf(check.ToString());
                if (index > -1)
                {
                    return index + 1;
                }
            }
        }
        throw new InvalidDataException();
    }

    protected override object Part1() => ReadAllLines(true).Sum(GetInt);

    protected override object Part2() => ReadAllLines(true).Sum(x => int.Parse($"{GetIntExt(x, words)}{GetIntExt(x.Rev(), sdrow)}"));
}
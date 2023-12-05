global using System;
global using System.Linq;
global using System.Reflection;
global using System.Collections.Generic;
global using System.IO;
global using System.Text.RegularExpressions;
global using System.Text;
global using aoc2023.Infra;
global using aoc2023.Code;
global using System.Collections;
global using System.Numerics;

var d = DateTime.Now.Day - 1;
if (args.Length == 1)
{
    d = int.Parse(args[0]) - 1;
}
var days = new BaseDay[]
{
        new Day01(), new Day02(), new Day03(), new Day04(),
        new Day05(), new Day06(), new Day07(), new Day08(),
        new Day09(), new Day10(), new Day11(), new Day12(),
        new Day13(), new Day14(), new Day15(), new Day16(),
        new Day17(), new Day18(), new Day19(), new Day20(),
        new Day21(), new Day22(), new Day23(), new Day24(),
        new Day25()
};

days[d].Solve(true);
days[d].Solve(false);
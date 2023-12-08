﻿namespace aoc2023.Code;

internal class Day07 : BaseDay
{
    static readonly Dictionary<char, int> CardsOrder = new()
    {
            { 'A', 12 }, { 'K', 11 }, { 'Q', 10 }, { 'J', 09 }, { 'T', 08 }, { '9', 07 }, { '8', 06 },
            { '7', 05 }, { '6', 04 }, { '5', 03 }, { '4', 02 }, { '3', 01 }, { '2', 00 }
    };

    class Hand : IComparable
    {
        public int Strength { get; set; }
        public int Value { get; private set; }
        public char[] Cards { get; private set; }

        readonly Dictionary<char, int> _count;

        public Hand(char[] cards, int value)
        {
            Cards = cards;
            Value = value;

            _count = [];
            foreach (var c in cards)
            {
                _count.TryGetValue(c, out var current);
                _count[c] = current + 1;
            }

            if (_count.ContainsValue(5))
            {
                Strength = 7; // Five of a kind
            }
            else if (_count.ContainsValue(4))
            {
                Strength = 6; // Four of a kind
            }
            else if (_count.ContainsValue(3) && _count.ContainsValue(2))
            {
                Strength = 5; // Full house
            }
            else if (_count.ContainsValue(3) && _count.ContainsValue(1))
            {
                Strength = 4; // Three of a kind
            }
            else if (_count.Values.Count(x => x == 2) == 2)
            {
                Strength = 3; // Two pair
            }
            else if (_count.Values.Count(x => x == 2) == 1)
            {
                Strength = 2; // One Pair
            }
            else
            {
                Strength = 1; // Single Card
            }
        }

        public int CompareTo(object? obj)
        {
            if (obj is null || obj is not Hand other)
            {
                throw new NotImplementedException();
            }

            if (Strength == other.Strength)
            {
                for (int i = 0; i < Cards.Length; i++)
                {
                    if (Cards[i] == other.Cards[i])
                    {
                        continue;
                    }
                    return CardsOrder[Cards[i]].CompareTo(CardsOrder[other.Cards[i]]);
                }
            }

            return Strength.CompareTo(other.Strength);
        }
    }

    static long Count(List<Hand> deck)
    {
        deck.Sort();
        var i = 1;

        return deck.Sum(y => y.Value * i++);
    }

    protected override object Part1()
    {
        CardsOrder['J'] = 9;

        return Count(ReadAllLinesSplit(" ", true).Select(x => new Hand(x[0].ToCharArray(), int.Parse(x[1]))).ToList());
    }

    protected override object Part2()
    {
        CardsOrder['J'] = -1;

        var deck = new List<Hand>();

        foreach (var line in ReadAllLinesSplit(" ", true))
        {
            var cards = line[0].ToCharArray();
            var value = int.Parse(line[1]);
            var hand = new Hand(cards, value);
            var jokers = PlayWithJokers(cards, value).ToList();
            if (jokers.Count > 0)
            {
                hand.Strength = jokers.Max(x => x.Strength);
            }
            deck.Add(hand);
        }

        return Count(deck);
    }

    static IEnumerable<Hand> PlayWithJokers(char[] cards, int value)
    {
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] == 'J')
            {
                foreach (var nc in new[] { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2' })
                {
                    if (!cards.Contains(nc))
                    {
                        continue;
                    }

                    var newcards = cards.ToArray();
                    newcards[i] = nc;

                    yield return new Hand(newcards, value);

                    foreach (var n in PlayWithJokers(newcards, value))
                    {
                        yield return n;
                    }
                }
            }
        }
    }
}
using System.Collections.Generic;

namespace Assets.markins._2048.Runtime.Game.Services
{
    public interface IPowerGenerator
    {
        int GetRandomPower();
        int GetRamPower2();
    }

    public class PowerGenerator : IPowerGenerator
    {

        List<MyRandom.Event<int>> powers = new List<MyRandom.Event<int>>
        {
            new MyRandom.Event<int>(1, 30),
            new MyRandom.Event<int>(2, 30),
            new MyRandom.Event<int>(3, 30),
            new MyRandom.Event<int>(4, 10),
            new MyRandom.Event<int>(5, 5),

        };

        List<MyRandom.Event<int>> allPowers = new List<MyRandom.Event<int>>
        {
            new MyRandom.Event<int>(1, 50),
            new MyRandom.Event<int>(2, 50),
            new MyRandom.Event<int>(3, 50),
            new MyRandom.Event<int>(4, 50),
            new MyRandom.Event<int>(5, 50),
            new MyRandom.Event<int>(6, 50),
            new MyRandom.Event<int>(7, 50),
            new MyRandom.Event<int>(8, 50),
            new MyRandom.Event<int>(9, 50),
            new MyRandom.Event<int>(10, 50),
            new MyRandom.Event<int>(11, 50),
            new MyRandom.Event<int>(12, 50),
            new MyRandom.Event<int>(13, 50),
            new MyRandom.Event<int>(14, 50),
        };

        public int GetRandomPower()
        {
            return MyRandom.main.ValueByProbability(powers, string.Empty);
        }

        public int GetRamPower2()
        {
            return MyRandom.main.ValueByProbability(allPowers, string.Empty);

        }
    }
}
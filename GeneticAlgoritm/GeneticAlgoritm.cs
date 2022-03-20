﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgoritm
{
    internal class GeneticAlgoritm
    {
        private const int TESTING_PARAMETER = 10;

        public static List<Factory> bestSpecimens(List<Factory> population, ref List<ConnectionCost> connectionCosts, ref List<ConnectionFlow> connectionFlow)
        {
            List<Factory> bestSpecimens = new List<Factory>();
            var currentPopulation = population;

            Console.WriteLine($"Current population size: {currentPopulation.Count()}");

            for (int i = 0; i < TESTING_PARAMETER; i++)
            {
                var testPop = Selection.TournamentSelection.newPopulation(currentPopulation);

                List<Factory> childSpecimens = new List<Factory>();

                if (testPop.Count() % 2 != 0)
                {
                    childSpecimens.Add(testPop.Last());
                    testPop.Remove(testPop.Last());
                }

                var specimenPairs = testPop.Select((v, i) => new { Index = i, Value = v })
                                    .GroupBy(x => x.Index / 2, x => x.Value)
                                    .Select(g => Tuple.Create(g.First(), g.Skip(1).First())).ToList();

                foreach (var elem in specimenPairs)
                {
                    var newChildren = Crossover.SinglePointCrossover.cross(elem.Item1, elem.Item2, connectionCosts, connectionFlow);
                    childSpecimens.Add(newChildren.Item1);
                    childSpecimens.Add(newChildren.Item2);
                }

                Mutation.BinaryBitFlip.mutation(ref childSpecimens);

                
                var min = childSpecimens.Min(a => a.score);
                bestSpecimens.Add(childSpecimens.Find(a => a.score ==  min));

                   

                foreach(var child in bestSpecimens)
                {
                    Console.WriteLine($"Najlepszy: {child.score}");
                }
                currentPopulation = childSpecimens;
                Console.WriteLine($"Current population size: {currentPopulation.Count()}");
                Console.WriteLine("Koniec populacji");
            }
            return bestSpecimens;
        }



    }
}
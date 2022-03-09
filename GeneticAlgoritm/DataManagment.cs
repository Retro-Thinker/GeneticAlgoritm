﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace GeneticAlgoritm
{
    internal class DataManagment
    {

        private static Random random = new Random();

        public static List<T> ReadData<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(filePath));
        }

        public static int fitnessFunction(List<ConnectionCost> cc, List<ConnectionFlow> cf, ref Factory factory)
        {

            var machines = factory.Machines;

            int result = 0;
            var connectionSource = 0;
            var connectionDest = 0;

            foreach (var machine in machines)
            {
                var connections = cc.Where(a => a.Source == machine).ToList();


                foreach (var connection in connections)
                {
                    connectionSource = connection.Source;
                    connectionDest = connection.Dest;


                    result += (getDistance(connection.Source,connection.Dest, ref factory)) * connection.Cost * 
                        cf.Where(a => a.Source == connectionSource && a.Dest == connectionDest).Select(a => a.Amount).First();
                }
            }

            Console.WriteLine(result);

            return result;
        }

        private static int getDistance(int source, int dest, ref Factory f)
        {
            int distance = 0;
 
            (int, int) sourcePlacement = getPlacement(f, source);
            (int, int) destPlacement = getPlacement(f, dest);

            int xDistance = sourcePlacement.Item1 - destPlacement.Item1;
            xDistance =  xDistance < 0 ? -xDistance : xDistance;

            int yDistance = sourcePlacement.Item2 - destPlacement.Item2;
            yDistance = yDistance < 0 ? -yDistance : yDistance;

            distance += xDistance + yDistance;
            
            return distance;
        }


        private static (int, int) getPlacement(Factory f, int source)
        {
            (int, int) coordinates = (-1, -1);

            for (int i = 0; i < f.FactoryDimX; i++)
            {
                for (int j = 0; j < f.FactoryDimY; j++)
                {
                    if (f.Genotype[i, j] == source) coordinates = (i, j);
                }
            }

            return coordinates;
        }

        public static Factory factoriesInit(List<ConnectionCost> cc, int factoryDimX, int factoryDimY)
        {
            var dests = cc.
                 Select(a => a.Dest)
                 .Distinct();

            var sources = cc.
                Select(a => a.Source)
                .Distinct();

            var machines = dests.Concat(sources).Distinct().ToList();
            var machineCount = machines.Count();


            int[,] genotype = generateGrid(factoryDimX, factoryDimY, machineCount);

          
            return new Factory(genotype, machines, factoryDimX, factoryDimY);
        }

        private static int[,] generateGrid(int factoryDimX, int factoryDimY, int machinesCount)
        {
            int size = factoryDimX * factoryDimY;
            int tmpIndex;

            List<int> generatedPlaces = new List<int>();
            int[,] grid = new int[factoryDimX, factoryDimY];

            for (int i = 0; i < factoryDimX; i++)
            {
                for (int j = 0; j < factoryDimY; j++)
                {
                    grid[i,j] = -1;
                }
            }
           
            while(machinesCount > 0)
            {
                tmpIndex = random.Next(0, size);
                if (!generatedPlaces.Contains(tmpIndex))
                {
                    generatedPlaces.Add(tmpIndex);
                    machinesCount--;
                }
            }

            List<int> tmparray1D = new List<int>();

            for (int i = 0; i < factoryDimX; i++)
            {
                for (int j = 0; j < factoryDimY; j++)
                {
                    tmparray1D.Add(grid[i, j]);
                }
            }

            int k = 0;

            foreach (var elem in generatedPlaces)
            {
                tmparray1D[elem] = k;
                k++;
            }


            tmparray1D.ForEach(Console.WriteLine);

            for (int i = 0; i < factoryDimX; i++)
            {
                for (int j = 0; j < factoryDimY; j++)
                {
                    grid[i,j] = tmparray1D[0];
                    tmparray1D.RemoveAt(0);
                }
            }

            return grid;
        }
    }
}
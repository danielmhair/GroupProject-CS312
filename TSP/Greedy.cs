using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TSP
{
    class Greedy
    {
        private City[] cities;
        private ArrayList VisitedCities;
        private double bestSolutionSoFar;
        public TimeSpan time;

        public Greedy(City[] cities)
        {
            this.cities = cities;
            this.VisitedCities = new ArrayList();
        }

        public double BestSolutionSoFar()
        {
            return bestSolutionSoFar;
        }

        public ArrayList RunGreedyTSP()
        {
            Stopwatch timer = new Stopwatch();
            timer.Reset();
            TimeSpan greedy = new TimeSpan();
            timer.Start();
            ArrayList Route = new ArrayList();
            int firstCityIndex = 0;
            while (Route.Count < cities.Length)
            {
                double lowestCityCost = double.PositiveInfinity;
                int lowestCostJIndex = -1;
                VisitedCities.Clear();
                Route.Clear();
                Route.Add(cities[firstCityIndex]);
                bestSolutionSoFar = 0;
                for (int i = 0; i < cities.Length; i = lowestCostJIndex)
                {
                    if (Route.Count == cities.Length)
                    {
                        break;
                    }
                    for (int j = nextCity(i); j != i; j = nextCity(j))
                    {
                        double curCityCost = cities[i].costToGetTo(cities[j]);
                        if (curCityCost < lowestCityCost)
                        {
                            lowestCityCost = curCityCost;
                            lowestCostJIndex = j;
                        }
                    }
                    if (!Route.Contains(cities[lowestCostJIndex]))
                    {
                        Route.Add(cities[lowestCostJIndex]);
                    }
                    VisitedCities.Add(i);
                    bestSolutionSoFar += lowestCityCost;
                    lowestCityCost = double.PositiveInfinity;
                }
                firstCityIndex++;
            }
            timer.Stop();
            greedy = timer.Elapsed;
            time = greedy;
            return Route;
        }

        public int nextCity(int cur)
        {
            cur++;
            if (cur >= cities.Length)
            {
                cur = 0;
            }
            while(VisitedCities.Contains(cur))
            {
                cur++;
            }
            if (cur >= cities.Length)
            {
                cur = 0;
            }
            return cur;
        }
    }
}

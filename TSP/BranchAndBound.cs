using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TSP
{
    class BranchAndBound
    {
        public BranchAndBound() { }

        public ArrayList Route;
        public int maxQueueCount;
            //Program.MainForm.tspOther.Text = "maxQueueCount: " + maxQueueCount + ", bssfUpdatesAmt: " + bssfUpdatesAmt + ", totalStatesCreated: " + totalStatesCreated + ", prunedAmt: " + prunedAmt + ", time: " + bAndBFormat.TotalSeconds.ToString() + ", cost: " + bssf.costOfRoute();
        public int bssfUpdatesAmt;
        public int totalStatesCreated;
        public int prunedAmt;
        public TimeSpan bAndBTime;
        public double count;

        public ArrayList RunTSP(City[] Cities)
        {
            Route = new ArrayList();

            Stopwatch timer = new Stopwatch();
            bAndBTime = new TimeSpan();
            timer.Start();

            Greedy greedyAlgorithm = new Greedy(Cities);
            ArrayList greedyRoute = greedyAlgorithm.RunGreedyTSP();

            double[,] matrix = new double[Cities.Length, Cities.Length];
            //Populate each array item with each respective edge cost
            for (int i = 0; i < Cities.Length; i++)
            {//O(n^2)
                for (int j = 0; j < Cities.Length; j++)
                {
                    if (i == j)
                    {
                        matrix[i, j] = double.PositiveInfinity;
                    }
                    else
                    {
                        matrix[i, j] = Cities[i].costToGetTo(Cities[j]);
                    }
                }
            }
            IntervalHeap<State> queue = new IntervalHeap<State>();

            double bestSolutionSoFar = greedyAlgorithm.BestSolutionSoFar();
            bssfUpdatesAmt = 0;
            prunedAmt = 0;
            State bssfState = null;
            totalStatesCreated = 0;
            //Start with some problem
            State curState = new State(matrix, 0, new Edge(-1, -1));
            totalStatesCreated++;
            //Reduce Matrix
            //O(4n^2)
            curState.Reduce();
            //Let the queue be the set of active subproblems
            //O(log(n))
            queue.Add(curState);
            maxQueueCount = 0;

            bool timesUp = false;
            while (queue.Count > 0)
            {//O(2^n) or O(2^(8n^2 + 9n + 2log(n)))
                if (timer.Elapsed.Seconds >= 30)
                {
                    timesUp = true;
                    break;
                }
                //Choose a subproblem and remove it from the queue
                if (queue.Count > maxQueueCount)
                {
                    maxQueueCount = queue.Count;
                }
                //O(log(n))
                curState = queue.DeleteMin();
                if (curState.Cost() < bestSolutionSoFar)
                {//O(8n^2 + 9n + 2log(n))
                    //For each lowest cost (each 0)
                    double highestScore = double.NegativeInfinity;
                    State[] includeExcludeStates = new State[2];
                    foreach (Edge edge in curState.LowestNums())
                    {//O(8n^2 + 9n)
                        //Include Matrix
                        State includeState = new State(curState, edge); //O(n)
                        totalStatesCreated++;
                        includeState.IncludeMatrix(edge.Row(), edge.Col()); //O(4n^2 + 7n)
                        //Exclude Matrix
                        State excludeState = new State(curState, edge); //O(n)
                        totalStatesCreated++;
                        excludeState.ExcludeMatrix(edge.Row(), edge.Col());//O(4n^2)
                        //Find the score for that edge (Exclude cost - Include cost)
                        double score = excludeState.Cost() - includeState.Cost();
                        if (score > highestScore)
                        {
                            includeExcludeStates[0] = includeState;
                            includeExcludeStates[1] = excludeState;
                            highestScore = score;
                        }
                    }

                    foreach (State subproblemState in includeExcludeStates)
                    {//O(2log(n))
                        //if each P (subproblem) chosen is a complete solution, update the bssf
                        if (subproblemState.CompleteSolution() && subproblemState.Cost() < bestSolutionSoFar)
                        {
                            bestSolutionSoFar = subproblemState.Cost();
                            bssfState = subproblemState;
                            bssfUpdatesAmt++;
                        }
                        //else if lowerBound < bestSolutionSoFar
                        else if (!subproblemState.CompleteSolution() && subproblemState.Cost() < bestSolutionSoFar)
                        {
                            //O(log(n))
                            queue.Add(subproblemState);
                        }
                        else
                        {
                            prunedAmt++;
                        }
                    }
                }
            }

            if (!timesUp) prunedAmt += queue.Count;
            //Call this the best solution so far.  bssf is the route that will be drawn by the Draw method.
            if (bssfState != null)
            {
                int index = bssfState.Exited(0);
                Route.Add(Cities[index]);
                index = bssfState.Exited(bssfState.Exited(0));
                while (index != bssfState.Exited(0))
                {//O(n)
                    Route.Add(Cities[index]);
                    index = bssfState.Exited(index);
                }
            }
            else
            {
                Route = greedyRoute;
            }

            timer.Stop();
            bAndBTime = timer.Elapsed;
            this.count = bssfState.Cost();

            // update the cost of the tour.
            timer.Reset();
            return Route;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    public class State : IComparable<State>
    {
        private double[,] matrix;
        private double cost;
        private Edge edge;
        private List<Edge> lowestNums;
        private int includedEdgesLength;
        private int[] entered;
        private int[] exited;

        public State(double cost)
        {
            this.cost = cost;
        }

        public State(double[,] matrix, double cost, Edge cell)
        {
            this.matrix = matrix;
            this.cost = cost;
            this.lowestNums = new List<Edge>();
            this.edge = cell;
            this.includedEdgesLength = 0;
            entered = new int[matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                entered[i] = -1;
            }
            exited = new int[matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                exited[i] = -1;
            }
        }

        public State(State state)
        {
            this.matrix = new double[state.Matrix().GetLength(0), state.Matrix().GetLength(1)];
            Array.Copy(state.matrix, this.matrix, this.matrix.Length);
            this.cost = state.cost;
            this.edge = state.edge;
            this.includedEdgesLength = state.includedEdgesLength;
            this.lowestNums = new List<Edge>(state.lowestNums);
            this.entered = new int[state.entered.Length];
            Array.Copy(state.entered, this.entered, state.entered.Length);
            this.exited = new int[state.exited.Length];
            Array.Copy(state.exited, this.exited, state.exited.Length);
        }

        public State(State state, Edge newEdge)
        {
            this.matrix = new double[state.Matrix().GetLength(0), state.Matrix().GetLength(1)];
            Array.Copy(state.matrix, this.matrix, this.matrix.Length);
            this.cost = state.cost;
            this.edge = newEdge;
            this.includedEdgesLength = state.includedEdgesLength;
            this.lowestNums = new List<Edge>(state.lowestNums);
            this.entered = new int[state.entered.Length];
            Array.Copy(state.entered, this.entered, state.entered.Length);
            this.exited = new int[state.exited.Length];
            Array.Copy(state.exited, this.exited, state.exited.Length);
        }

        public Edge Edge()
        {
            return edge;
        }

        public int IncludedEdgesSize()
        {
            return includedEdgesLength;
        }

        public void IncrementIncludedEdgesSize()
        {
            includedEdgesLength++;
        }

        public double[,] Matrix()
        {
            return matrix;
        }

        public double Cost()
        {
            return cost;
        }

        public List<Edge> LowestNums()
        {
            return lowestNums;
        }

        public int Exited(int index)
        {
            return exited[index];
        }

        public int Entered(int index)
        {
            return entered[index];
        }

        public void SetExited(int index, int num)
        {
            exited[index] = num;
        }

        public void SetEntered(int index, int num)
        {
            entered[index] = num;
        }

        public void SetCost(double cost)
        {
            this.cost = cost;
        }

        public void SetMatrix(double[,] matrix)
        {
            this.matrix = matrix;
        }


        public void RemoveCycles(int row, int col)
        {//O(5n)
            entered[col] = row;
            exited[row] = col;
            int start = row;
            int end = col;
            // The new edge may be part of a partial solution. Go to the end of that solution.
            while (exited[end] != -1 && !CompleteSolution()) //O(2n)
            {
                end = exited[end];
            }
            // Similarly, go to the start of the new partial solution.
            while (entered[start] != -1 && !CompleteSolution()) //O(2n)
            {
                start = entered[start];
            }
            // Delete the edges that would make partial cycles, unless we’re ready to finish the tour
            if (includedEdgesLength < matrix.GetLength(0) - 1)
            {
                while(start != col) //O(n)
                {
                    if (matrix[end, start] == 0) lowestNums.Remove(new Edge(end, start));
                    matrix[end, start] = double.PositiveInfinity;
                    if (matrix[col, start] == 0) lowestNums.Remove(new Edge(col, start));
                    matrix[col, start] = double.PositiveInfinity;
                    start = exited[start];
                }
            }
        }

        public void IncludeMatrix(int row, int col)
        {//O(4n^2 + 7n)
            for (int i = 0; i < matrix.GetLength(0); i++)
            {//O(n)
                if (matrix[row, i] == 0) lowestNums.Remove(new Edge(row, i));
                matrix[row, i] = double.PositiveInfinity;
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {//O(n)
                if (matrix[i, col] == 0) lowestNums.Remove(new Edge(i, col));
                matrix[i, col] = double.PositiveInfinity;
            }

            //This is the opposite - if its (3,5), then (5,3) needs to be infinity
            //O(5n)
            RemoveCycles(row, col);
            //O(4n^2)
            Reduce();
        }

        public int EnteredSize()
        {//O(n)
            includedEdgesLength = 0;
            for (int i = 0; i < entered.Length; i++)
            {
                if (entered[i] != -1)
                {
                    includedEdgesLength++;
                }
            }
            return includedEdgesLength;
        }

        public void ExcludeMatrix(int row, int col)
        {//O(4n^2)
            if (matrix[row, col] == 0) lowestNums.Remove(new Edge(row, col));
            matrix[row, col] = double.PositiveInfinity;
            //O(4n^2)
            Reduce();
        }
        
        public void Reduce()
        {//O(4n^2)
            double lowest = double.PositiveInfinity;
            //Find the lowest number
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (exited[i] == -1)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[i, j] < lowest)
                        {
                            lowest = matrix[i, j];
                        }
                    }

                    if (lowest > 0 && lowest < double.PositiveInfinity)
                    {
                        for (int j = 0; j < matrix.GetLength(1); j++)
                        {
                            matrix[i, j] -= lowest;
                            if (matrix[i, j] == 0)
                            {
                                lowestNums.Add(new Edge(i, j));
                            }
                        }
                    }
                    cost += lowest;
                }
                lowest = double.PositiveInfinity;
            }

            lowest = double.PositiveInfinity;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (entered[i] == -1)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (matrix[j, i] < lowest)
                        {
                            lowest = matrix[j, i];
                        }
                    }
                    if (lowest > 0 && lowest < double.PositiveInfinity)
                    {
                        for (int j = 0; j < matrix.GetLength(1); j++)
                        {
                            matrix[j, i] -= lowest;
                            if (matrix[j, i] == 0)
                            {
                                lowestNums.Add(new Edge(j, i));
                            }
                        }
                    }
                    cost += lowest;
                }
                lowest = double.PositiveInfinity;
            }
        }


        /** 
         *  If the lowestNums has anything in its list, then there is at least 1 zero in the solution and still needs to be expanded.
         *  If not, then check if all edges are infinite.
         */
        public bool CompleteSolution()
        {//O(n)
            return EnteredSize() == matrix.GetLength(0);
        }
        
        public int CompareTo(State other)
        {
            if (other == null) return 1;
            else if (cost == other.cost) return 0;
            else if (cost < other.cost) return -1;
            else return 1;
        }

        public static bool operator <(State s1, State s2)
        {
            return s1.CompareTo(s2) < 0;
        }

        public static bool operator >(State s1, State s2)
        {
            return s1.CompareTo(s2) > 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSP
{
    public class Edge
    {
        private int row;
        private int col;

        public Edge(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public Edge(Edge edge)
        {
            this.row = edge.row;
            this.col = edge.col;
        }

        public int Row()
        {
            return row;
        }

        public int Col()
        {
            return col;
        }

        public static bool operator ==(Edge e1, Edge e2)
        {
            return e1.row == e2.row && e1.col == e2.col;
        }

        public static bool operator !=(Edge e1, Edge e2)
        {
            return e1.row != e2.row && e1.col != e2.col;
        }
        
        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Edge other = (Edge) obj;
            return row == other.row && col == other.col;
        }
    }
}

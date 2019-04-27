using System;
using System.Linq;
using System.Collections.Generic;

using rand_dungeon;

using Delaunay;

namespace SpanningTree
{
    public class Kruskal
    {
        IEnumerable<Line> lines; 
        List<Line> lineList;

        int vNum;

        Unifold uf;

        public Kruskal(int  vNum, IEnumerable<Line> lines)
        {
            this.vNum = vNum;
            this.lines = lines;
            lineList = new List<Line>();

            uf = new Unifold(vNum);
        }

        public List<Line> Compute()
        {
            lines = lines.OrderBy(x => x.sqLength);
            foreach(var edge in lines){
                if(!uf.InSameSet(edge.start.label, edge.end.label))
                {
                    uf.Unite(edge.start.label, edge.end.label);
                    lineList.Add(edge);
                }
            }
            return lineList;
        }
    }

    public class Unifold
    {

        List<int> par= new List<int>(), rank = new List<int>();
        public Unifold(int vNum){
            for (int i = 0; i < vNum; i++)
            {
                par.Add(i);
                rank.Add(0);
            }
        }

        public int Root(int node)
        {
            if(par[node] == node)
            {
                return node;
            }
            else
            {
                int r = Root(par[node]);
                par[node] = r;
                return r;
            }
        }

        public bool InSameSet(int node1, int node2)
        {
            return Root(node1) == Root(node2);
        }

        public void Unite(int node1, int node2)
        {
            var x = Root(node1);
            var y = Root(node2);

            if (x == y){
                return;
            }
            else if (rank[x] < rank[y])
            {
                par[x] = y;
            }
            else
            {
                par[y] = x;
                if(rank[x] == rank[y])
                {
                    rank[x]++;
                }
            }
        }
    }
}

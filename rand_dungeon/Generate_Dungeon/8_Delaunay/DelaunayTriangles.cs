using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using rand_dungeon;

namespace Delaunay
{
    public class DelaunayTriangles
    {
        public HashSet<Triangle> triangleSet;
        asd.Layer2D layer;
        asd.Vector2DF range;

        Dungeon dungeon;

        public DelaunayTriangles(List<Point> pointList, asd.Layer2D layer, asd.Vector2DF N, Dungeon dungeon)
        {
            this.layer = layer;
            this.range = N;
            this.dungeon = dungeon;

            DelaunayTriangulation(pointList);
        }

        private void DelaunayTriangulation(List<Point> pointList)
        {  
            // 三角形リストを初期化  
            triangleSet = new HashSet<Triangle>();  
      
            // 巨大な外部三角形をリストに追加  
            Triangle hugeTriangle = GetHugeTriangle();
            triangleSet.Add(hugeTriangle);
          
            try {
                // --------------------------------------  
                // 点を逐次添加し、反復的に三角分割を行う  
                // --------------------------------------  
                foreach(Point p in pointList)
                {
                    // --------------------------------------  
                    // 追加候補の三角形を保持する一時ハッシュ  
                    // --------------------------------------  
                    // 追加候補の三角形のうち、「重複のないものだけ」を  
                    // 三角形リストに新規追加する  
                    //          → 重複管理のためのデータ構造  
                    // tmpTriangleSet  
                    //  - Key   : 三角形  
                    //  - Value : 重複していないかどうか  
                    //            - 重複していない : true  
                    //            - 重複している   : false  
                    // --------------------------------------  
                    Dictionary<Triangle, bool> tmpTriangleSet = new Dictionary<Triangle, bool>();


                    // --------------------------------------  
                    // 現在の三角形リストから要素を一つずつ取り出して、  
                    // 与えられた点が各々の三角形の外接円の中に含まれるかどうか判定  
                    // --------------------------------------
                    foreach (Triangle t in new HashSet<Triangle>(triangleSet))
                    {

                        // その外接円を求める。
                        Circle c = t.outCircle;
                              
                        // --------------------------------------  
                        // 追加された点が外接円内部に存在する場合、  
                        // その外接円を持つ三角形をリストから除外し、  
                        // 新たに分割し直す  
                        // --------------------------------------  
                        if (p.SqDist(c.center) <= c.radius * c.radius) {  
                            // 新しい三角形を作り、一時ハッシュに入れる  
                            AddElementToRedundanciesMap(tmpTriangleSet, new Triangle(p, t.p1, t.p2));  
                            AddElementToRedundanciesMap(tmpTriangleSet, new Triangle(p, t.p2, t.p3));  
                            AddElementToRedundanciesMap(tmpTriangleSet, new Triangle(p, t.p3, t.p1));

                            // 旧い三角形をリストから削除
                            triangleSet.Remove(t);
                        }
                    }

                    // --------------------------------------  
                    // 一時ハッシュのうち、重複のないものを三角形リストに追加   
                    // --------------------------------------

                    foreach(KeyValuePair<Triangle, bool> entry in tmpTriangleSet)
                    {
                        if(entry.Value) {  
                            triangleSet.Add(entry.Key);  
                        }
                    }
                }
                  
                // 最後に、外部三角形の頂点を削除
                foreach (Triangle t in new HashSet<Triangle>(triangleSet).Where(x => hugeTriangle.HasCommonPoints(x)))
                {
                    triangleSet.Remove(t);
                }

            }
            catch
            {  
                return;  
            }
        }

        // ======================================  
        // 描画  
        // ======================================  
        public void Draw() {
            foreach(Triangle t in triangleSet)
            {
                t.Draw(layer);
            }
        }
        
        
        // ======================================  
        // ※ ここからprivateメソッド  
        // ======================================  
        
        
        // ======================================  
        // 一時ハッシュを使って重複判定  
        // hashMap  
        //  - Key   : 三角形  
        //  - Value : 重複していないかどうか  
        //            - 重複していない : true  
        //            - 重複している   : false  
        // ======================================  
        void AddElementToRedundanciesMap(Dictionary<Triangle, bool> hashMap, Triangle t)  
        {
            // 重複あり : Keyに対応する値にFalseをセット  
            // 重複なし : 新規追加し、  
            try
            {
                hashMap.AddOrReplace(t, !hashMap.ContainsKey(t));
            }
            catch
            {
                return;
            }
        }

        // ======================================  
        // 最初に必要な巨大三角形を求める  
        // ======================================  
        // 画面全体を包含する正三角形を求める  
        Triangle GetHugeTriangle()
        {
            var cpos = dungeon.cameraPos;
            return GetHugeTriangle(cpos - range/2, cpos + range/2);
        }
        // 任意の矩形を包含する正三角形を求める  
        // 引数には矩形の左上座標および右下座標を与える  
        Triangle GetHugeTriangle(asd.Vector2DF start, asd.Vector2DF end)
        {
            // start: 矩形の左上座標、  
            // end  : 矩形の右下座標…になるように  
            if (end.X < start.X)
            {
                float tmp = start.X;
                start.X = end.X;
                end.X = tmp;
            }
            if (end.Y < start.Y)
            {
                float tmp = start.Y;
                start.Y = end.Y;
                end.Y = tmp;
            }

            // 1) 与えられた矩形を包含する円を求める  
            //      円の中心 c = 矩形の中心  
            //      円の半径 r = |p - c| + ρ  
            //    ただし、pは与えられた矩形の任意の頂点  
            //    ρは任意の正数  
            Point center = new Point((float)((end.X - start.X) / 2.0), (float)((end.Y - start.Y) / 2.0));
            float radius = (float)(center.Dist(new Point(start)) + 1.0);

            // 2) その円に外接する正三角形を求める  
            //    重心は、円の中心に等しい  
            //    一辺の長さは 2√3･r  
            float x1 = (float)(center.x - Math.Sqrt(3) * radius);
            float y1 = center.y - radius;
            Point p1 = new Point(x1, y1);

            float x2 = (float)(center.x + Math.Sqrt(3) * radius);
            float y2 = center.y - radius;
            Point p2 = new Point(x2, y2);

            float x3 = center.x;
            float y3 = center.y + 2 * radius;
            Point p3 = new Point(x3, y3);

            return new Triangle(p1, p2, p3);
        }
    }
}

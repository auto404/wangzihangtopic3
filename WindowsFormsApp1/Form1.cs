using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static int INFINITE_DISTANCE = 65535; // 无限大距离
        public static double COORDINATE_RANGE = 100.0;// 横纵坐标范围为[-100,100]
        public Point a1, b1, a2, b2;              //保存分割后两个子集中最小点对
        public Point a, b;            //最近点对
        public class Point
        {// 二维坐标上的点Point
            public double x;
            public double y;
        }
        class XComparer : IComparer<Point>
        {
            //实现升序

            public int Compare(Point x, Point y)
            {
                return (x.x.CompareTo(y.x));
            }
        }
        class YComparer : IComparer<Point>
        {
            //实现升序

            public int Compare(Point x, Point y)
            {
                return (x.y.CompareTo(y.y));
            }
        }
        double Distance(Point a, Point b)
        {//平面上任意两点对之间的距离公式计算
            return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }

        double ClosestPair(List<Point> points, int length, ref Point a, ref Point b)
        {// 求出最近点对记录，并将两点记录再a、b中
            double distance;                   //记录集合points中最近两点距离 
            double d1, d2;                     //记录分割后两个子集中各自最小点对距离
            int i = 0, j = 0, k = 0, x = 0;

            if (length < 2)
                return INFINITE_DISTANCE;    //若子集长度小于2，定义为最大距离，表示不可达
            else if (length == 2)
            {//若子集长度等于2，直接返回该两点的距离
                a = points[0];
                b = points[1];
                distance = Distance(points[0], points[1]);
            }
            else
            {//子集长度大于3，进行分治求解
                List<Point> pts1 = new List<Point>();   //开辟两个子集
                List<Point> pts2 = new List<Point>();

                points.Sort(new XComparer());    //调用algorithm库中的sort函数对points进行排序，compareX为自定义的排序规则
                double mid = points[(length - 1) / 2].x;    //排完序后的中间下标值，即中位数

                for (i = 0; i < length / 2; i++)
                    pts1.Add(points[i]);
                for (j = 0, i = length / 2; i < length; i++)
                    pts2.Add(points[i]);

                d1 = ClosestPair(pts1, length / 2, ref a1, ref b1);             //分治求解左半部分子集的最近点  
                d2 = ClosestPair(pts2, length - length / 2, ref a2, ref b2);    //分治求解右半部分子集的最近点  

                if (d1 < d2) { distance = d1; a = a1; b = b1; }            //记录最近点，最近距离
                else { distance = d2; a = a2; b = b2; }

                //merge - 进行子集合解合并
                //求解跨分割线并在δ×2δ区间内的最近点对
                List<Point> pts3 = new List<Point>();

                for (i = 0, k = 0; i < length; i++)                        //取得中线2δ宽度的所有点对共k个    
                    if (Math.Abs(points[i].x - mid) <= distance)
                        pts3.Add(points[i]);
                pts3.Sort(new YComparer());                                    // 以y排序矩形阵内的点集合

                for (i = 0; i < k; i++)
                {
                    if (pts3[j].x - mid >= 0)                                             // 只判断左侧部分的点
                        continue;
                    x = 0;
                    for (j = i + 1; j <= i + 6 + x && j < k; j++)            //只需与有序的领接的的6个点进行比较
                    {
                        if (pts3[j].x - mid < 0)
                        {//  假如i点是位于mid左边则只需判断在mid右边的j点即可
                            x++;
                            continue;
                        }
                        if (Distance(pts3[i], pts3[j]) < distance)
                        {//如果跨分割线的两点距离小于已知最小距离，则记录该距离和两点
                            distance = Distance(pts3[i], pts3[j]);
                            a = pts3[i];
                            b = pts3[j];
                        }
                    }
                }
            }
            return distance;
        }
        void SetPoints(ref List<Point> points, int length)
        {//随机函数对点数组points中的二维点进行初始化



            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                Point p = new Point();
                p.x = (rd.Next() % COORDINATE_RANGE * 200) / COORDINATE_RANGE - COORDINATE_RANGE;
                p.y = (rd.Next() % COORDINATE_RANGE * 200) / COORDINATE_RANGE - COORDINATE_RANGE;
                points.Add(p);
            }
        }
        public Form1()
        {
            InitializeComponent();


        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            double diatance;    //点对距离
            chart1.Series[0].Points.Clear();
            int num = int.Parse(textBox1.Text.Trim());
            if (num < 2)
            {
                listBox1.Items.Add("请输入大于等于2的点个数！！");
                listBox1.Items.Clear();
            }

            else
            {
                listBox1.Items.Add("随机生成的" + num + "个二维点对如下：");
                List<Point> points = new List<Point>();
             
                SetPoints(ref points, num);
                for (int i = 0; i < num; i++)
                {
                    chart1.Series[0].Points.AddXY(points[i].x, points[i].y);
                    listBox1.Items.Add("(" + points[i].x + "," + points[i].y + ")");
                }

                diatance = ClosestPair(points, num, ref a, ref b);
                listBox1.Items.Add("按横坐标排序后的点对:");
                for (int i = 0; i < num; i++)
                    listBox1.Items.Add("(" + points[i].x + "," + points[i].y + ")");
                listBox1.Items.Add("最近点对为：");
                listBox1.Items.Add("(" + a.x + "," + a.y + ")和(" + b.x + "," + b.y + ")");
                listBox1.Items.Add("最近点对距离为：" + diatance);

            }

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

     

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

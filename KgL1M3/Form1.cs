using KgL1M3;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace KgL1M3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
        }
        Object File_OBJ = new Object();

      
        double Y = 0, X = 0, Z = 0;
        double XKG = 240, YKG = 210;
        double scale = 80;
        double LX = -60, LY = 30, LZ = 40;
        List<int> list = new List<int>();
       public static int ymax=0;
        public static int ymin=999;
        public static int xmax=999;
        public static int xmin =0;

        //Структура ячейки, из которых будет состоять Z-буфер.
        struct Cell
        {
           public double z;
           public int edge;
          
        };



        //Класс Z-буфера.
        class ZBuffer
        {

           public Cell[,] Zb;
          
            public int sX, sY; // Размер Z-Буфера
            public ZBuffer(int x, int y)
            {
                sX = x;
                sY = y;
                Zb = new Cell[sX,sY];
          
            }

         
        }
        public double[] kf= new double[4];

       public void koefs(double v1x, double v1y, double v1z, double v2x, double v2y, double v2z, double v3x, double v3y, double v3z)
        {

           kf[0] = (v2y - v1y) * (v3z - v1z) - (v2z - v1z) * (v3y - v1y);
          kf[1]  = (v2z - v1z) * (v3x - v1x) - (v2x - v1x) * (v3z - v1z);
           kf[2] = (v2x - v1x) * (v3y - v1y) - (v2y - v1y) * (v3x - v1x);
            kf[3] = -1 * (kf[0] * v1x + kf[1] * v1y + kf[2] * v1z);
            
        }






        private void PaintOBJ()
        {
            if (!File_OBJ.loaded) return;
            Graphics g = pictureBox1.CreateGraphics();
            SolidBrush b = new SolidBrush(Color.White);
            g.FillRectangle(b, 0, 0, pictureBox1.Width, pictureBox1.Height);
            Pen p = new Pen(Color.FromArgb(255, 160, 235, 255), 3);
            Color cl = Color.FromArgb(235, 235, 235);
            int alpha = 150;

            g.DrawLine(p,120, 300, 240, 210);
            g.DrawLine(p, 360, 300, 240, 210);
            g.DrawLine(p, 240, 70, 240, 210);
            p = new Pen(Color.FromArgb(alpha, alpha, alpha, alpha), 3);



            File_OBJ.offset(LX, LY, LZ, XKG, YKG, scale);
            ZBuffer buf = new ZBuffer(503, 455);

            int[] surf = new int[20];
            int fl = 0;
            int ct = 0;
            for (int j = ymin; j < ymax; j++)
            {
                for (int n = 0; n < File_OBJ.Surfaces.Count; n++)
                {
                    for (int m = 0; m < File_OBJ.Surfaces[n].NumPoint3d.Count - 1; m++)
                    {
                        if (File_OBJ.VertexsN[File_OBJ.Surfaces[n].NumPoint3d[m]].y >= j)
                        {
                            if (File_OBJ.VertexsN[File_OBJ.Surfaces[n].NumPoint3d[m + 1]].y < j)
                            {

                                if (ct == 0)
                                {
                                    surf[fl] = n;
                                    fl++;
                                    ct++;
                                }


                            }
                        }
                    }
                    for (int m = 0; m < File_OBJ.Surfaces[n].NumPoint3d.Count - 1; m++)
                    {
                        if (File_OBJ.VertexsN[File_OBJ.Surfaces[n].NumPoint3d[m]].y < j)
                        {
                            if (File_OBJ.VertexsN[File_OBJ.Surfaces[n].NumPoint3d[m + 1]].y >= j)
                            {

                                if (ct == 0)
                                {
                                    surf[fl] = n;
                                    fl++;
                                    ct++;
                                }


                            }
                        }
                    }
                    
                    ct = 0;

                }
                for (int o = 0; o < fl; o++)
                {
                    Point3d[] pt = new Point3d[4] { new Point3d( 0, 0, 0 ), new Point3d( 0, 0, 0 ), new Point3d( 0, 0, 0 ), new Point3d( 0, 0, 0 )};

                    int flag = 0;
                    for (int m = 0; m < File_OBJ.Surfaces[surf[o]].NumPoint3d.Count - 1; m++)
                    {
                        if (File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m]].y >= j)
                        {
                            if (File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m + 1]].y < j)
                            {
                                pt[flag].x = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m]].x;
                                pt[flag].y = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m]].y;
                                flag++;
                                pt[flag].x = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m+1]].x;
                                pt[flag].y = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m+1]].y;
                                flag++;
                            }
                        }
                        if (File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m]].y <= j)
                        {
                            if (File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m + 1]].y > j)
                            {
                                pt[flag].x = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m + 1]].x;
                                pt[flag].y = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m + 1]].y;
                                
                                flag++;
                                pt[flag].x = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m]].x;
                                pt[flag].y = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[m]].y;
                                flag++;
                            }
                        }
                        
                        }

                    if (File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[File_OBJ.Surfaces[surf[o]].NumPoint3d.Count - 1]].y >= j)
                    {
                        if (File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].y < j)
                        {
                            pt[flag].x = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[File_OBJ.Surfaces[surf[o]].NumPoint3d.Count - 1]].x;
                            pt[flag].y = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[File_OBJ.Surfaces[surf[o]].NumPoint3d.Count - 1]].y;
                            flag++;
                            pt[flag].x = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].x;
                            pt[flag].y = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].y;
                            flag++;
                        }
                    }
                    if (File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[File_OBJ.Surfaces[surf[o]].NumPoint3d.Count - 1]].y <=j)
                    {
                        if (File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].y >j)
                        {
                            pt[flag].x = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].x;
                            pt[flag].y = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].y;
                           
                            flag++;
                            pt[flag].x = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[File_OBJ.Surfaces[surf[o]].NumPoint3d.Count - 1]].x;
                            pt[flag].y = File_OBJ.VertexsN[File_OBJ.Surfaces[surf[o]].NumPoint3d[File_OBJ.Surfaces[surf[o]].NumPoint3d.Count - 1]].y;
                            flag++;
                        }
                    }


                    int x1 = Convert.ToInt32(pt[0].x + ((j - pt[0].y) * (pt[1].x - pt[0].x)) / (pt[1].y - pt[0].y));
                            int x2 = Convert.ToInt32(pt[2].x + ((j - pt[2].y) * (pt[3].x - pt[2].x)) / (pt[3].y - pt[2].y));

                            if (x2 <= x1)
                            {
                                int temp = x2;
                                x2 = x1;
                                x1 = temp;
                            }

                            koefs(File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].x,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].y,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[0]].z,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[1]].x,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[1]].y,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[1]].z,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[2]].x,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[2]].y,
                   File_OBJ.VertexsM[File_OBJ.Surfaces[surf[o]].NumPoint3d[2]].z);
                            double z0 = 0;
                            double z1 = 0;
                            for (int i = x1; i <= x2; i++)
                            {
                        if (z0 == 0)
                        {
                            z0 = (-kf[3] - kf[0] * i - kf[1] * j) / kf[2];
                            z1 = z0;
                        }
                        else
                        {
                            z1 = (z0-kf[0]/kf[2]);
                        }
                                if (z1 > buf.Zb[i, j].z)
                                {
                                    buf.Zb[i, j].z = z1;
                            buf.Zb[i, j].edge = surf[o];
                                }
                                if (buf.Zb[i, j].z==0)
                        {
                            buf.Zb[i, j].z = z1;
                            buf.Zb[i, j].edge = surf[o];
                        }
                                z0 = z1;
                            }





                        }
                for (int i = 0; i < (buf.sX); i++)
                   
                 
                    {if (buf.Zb[i, j].z != 0)
                    {
                        double k = buf.Zb[i, j].z;

                        int color =Math.Abs( (100 + (Convert.ToInt32(buf.Zb[i, j].z-1))))%255 ;

                        p = new Pen(Color.FromArgb(255, color, 0, 0), 2);
                        g.DrawLine(p, Convert.ToInt32(i), Convert.ToInt32(j), Convert.ToInt32(i) - 1, Convert.ToInt32(j) - 1);
                    }
                    }

                        fl = 0;
                for (int i = 0; i < 20; i++)
                {
                    surf[i] = -1;
                }

                    }









            }
        






        class Point3d
        {
            public double x = 0;
            public double y = 0;
            public double z = 0;
            public Point3d(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        };

        class Surface
        {
            public List<int> NumPoint3d = new List<int>();
        };

        class Object
        {
            public List<Point3d> Vertexs = new List<Point3d>();
            public List<Surface> Surfaces = new List<Surface>();
            public List<Point3d> VertexsN = new List<Point3d>();
            public List<Point3d> VertexsM = new List<Point3d>();
            public Point3d pointw = new Point3d(0, 0, 0);
            public Point3d centr = new Point3d(0, 0, 0);
            public bool loaded = false;

            public Object()
            {
            }

            public Object(string file_name)
            {
                string[] lines = File.ReadAllLines(file_name);
                {
                    foreach (string line in lines)
                        if (line.ToLower().StartsWith("v "))
                        {
                            var vx = line.Split(' ').Skip(1).Select(v => Double.Parse(v.Replace('.', ','))).ToArray();
                            Vertexs.Add(new Point3d((vx[0]), (vx[1]), (vx[2])));
                            VertexsN.Add(new Point3d((vx[0]), (vx[1]), 0));
                            VertexsM.Add(new Point3d((vx[0]), (vx[1]), (vx[2])));
                        }
                        else if (line.ToLower().StartsWith("f"))
                        {
                            string str;
                            bool key = true;
                            if (line[line.Length - 1] == ' ')
                            {
                                str = line.Remove(line.Length - 1, 1);
                            }
                            else
                            {
                                str = line;
                            }
                            while (key)
                            {
                                int h = 0, k = 0;
                                for (int i = 0; i < str.Length; i++)
                                {
                                    if (str[i] == '/')
                                    {
                                        h = i;
                                        int j = i;
                                        while (str[j] != ' ' && j < str.Length - 1)
                                        {
                                            k++;
                                            j++;
                                        }
                                        if (j == str.Length - 1)
                                        {
                                            k++;
                                        }
                                        str = str.Remove(h, k);
                                        break;
                                    }
                                    if (i == str.Length - 1)
                                    {
                                        key = false;
                                    }
                                }
                            }
                            var vx = str.Split(' ').Skip(1).Select(v => int.Parse(v)).ToArray();
                            Surface temp = new Surface();
                            foreach (int d in vx)
                            {
                                temp.NumPoint3d.Add(d - 1);
                            }
                            Surfaces.Add(temp);
                        }
                }
                loaded = true;
            }

            public void offset(double LX, double LY, double LZ, double XKG, double YKG, double scale)
            {
                double X1, Y1, Z1, X2, Y2, Z2, X3, Y3, Z3, XC = 0, YC = 0, ZC = 0;
                double COS;
                double SIN;
                for (int i = 0; i < Vertexs.Count; i++)
                {
                    X1 = Vertexs[i].x;
                    Y1 = Vertexs[i].y;
                    Z1 = Vertexs[i].z;
                    COS = Math.Cos(LX * Math.PI / 180);
                    SIN = Math.Sin(LX * Math.PI / 180);
                    Y2 = Math.Round(Y1 * COS - Z1 * SIN, 5) ;
                    Z2 = Math.Round(Y1 * SIN + Z1 * COS, 5);
                    COS = Math.Cos(LY * Math.PI / 180);
                    SIN = Math.Sin(LY * Math.PI / 180);
                    X2 = Math.Round(X1 * COS - Z2 * SIN, 5);
                    Z3 = Math.Round(X1 * SIN + Z2 * COS, 5);
                    COS = Math.Cos(LZ * Math.PI / 180);
                    SIN = Math.Sin(LZ * Math.PI / 180);
                    X3 = Math.Round(X2 * COS - Y2 * SIN, 5);
                    Y3 = Math.Round(X2 * SIN + Y2 * COS, 5);
                    X3 *= scale;
                    Y3 *= scale;
                    Z3 *= scale;

                    Point3d NN = new Point3d(X3 + XKG, Y3 + YKG, Z3);

                    VertexsM[i] = NN;
                   // VertexsN[i].x *= Math.Sin(30);
                  // VertexsN[i].y *= Math.Sin(30);
                
                    //VertexsN[i].x = (X3 - Z3) / Math.Sqrt(2);
                    //VertexsN[i].y = (X3 + 2 * Y3 + Z3) / Math.Sqrt(6);
                    VertexsN[i].x = X3;
                    VertexsN[i].y = Y3;
                    VertexsN[i].z = Z3;
                    VertexsN[i].x += XKG;
                    VertexsN[i].y += YKG;
                    if(ymax< VertexsN[i].y)
                    {
                        ymax =Convert.ToInt32( VertexsN[i].y);
                    }
                    if (ymin > VertexsN[i].y)
                    {
                        ymin = Convert.ToInt32(VertexsN[i].y);
                    }
                    if (xmax < VertexsN[i].x)
                    {
                        ymax = Convert.ToInt32(VertexsN[i].x);
                    }
                    if (xmin > VertexsN[i].x)
                    {
                        xmin = Convert.ToInt32(VertexsN[i].x);
                    }
                    XC += VertexsN[i].x;
                    YC += VertexsN[i].y;
                    ZC += X3;
                }

                centr = new Point3d((XC / Vertexs.Count), (YC / Vertexs.Count), (ZC / Vertexs.Count));

                X1 = 0; Y1 = 0 * Math.Cos(LX) + 0 * Math.Sin(LX);
                Z1 = 0 * Math.Sin(LX) + 0 * Math.Cos(LX);
                X2 = X1 * Math.Cos(LY) - Z1 * Math.Sin(LY); Y2 = Y1;
                Z2 = X1 * Math.Sin(LY) + Z1 * Math.Cos(LY);
                X3 = X2 * Math.Cos(LZ) + Y2 * Math.Sin(LZ);
                Y3 = -X2 * Math.Sin(LZ) + Y2 * Math.Cos(LZ); Z3 = Z2;
                X3 *= scale; Y3 *= scale; Z3 *= scale;
                X3 += XKG; Y3 += YKG;

                Point3d N = new Point3d(X3, Y3, Z3);
                pointw = N;
            }





        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {


            File_OBJ = new Object("Path to icasoadr");
            PaintOBJ();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File_OBJ = new Object("Path to obj cube");
            PaintOBJ();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            File_OBJ = new Object("Path to obj dodekaedr");
            PaintOBJ();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            scale += 15;
            XKG += 0.08 * scale / 80;
            YKG -= 0.08 * scale / 80;
            PaintOBJ();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            scale -= 15;
            XKG -= 0.08 * scale / 80;
            YKG += 0.08 * scale / 80;
            PaintOBJ();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LX += 30;
            PaintOBJ();

        }

        private void button7_Click(object sender, EventArgs e)
        {
            LY += 30;
            PaintOBJ();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LZ += 30;
            PaintOBJ();
        }

        private void button9_Click(object sender, EventArgs e)
        {

            XKG -= 30;
            PaintOBJ();

        }

        private void button10_Click(object sender, EventArgs e)
        {
            YKG -= 30;
            PaintOBJ();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            XKG += 30;
            PaintOBJ();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            YKG += 30;
            PaintOBJ();
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            XKG += 30;
            PaintOBJ();
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            XKG -= 30;
            PaintOBJ();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            YKG -= 30;
            PaintOBJ();
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            YKG += 30;
            PaintOBJ();
        }
    }
}

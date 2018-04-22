using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

///Algorithms Project
///Intelligent Scissors
///

namespace ImageQuantization
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    /// 
   
   
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }
    public class edge
    {
        public int from;
        public int to;
        public double w;

        public edge(int From, int To, double W)
        {
            from = From;
            to = To;
            w = W;
        }
        
        public double CompareTo(edge other)
        {
            return w.CompareTo(other.w);
        }
        
    };

    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        public static List<int>[] adjmatrix;
        public static int[, ,] HashOfColor;
        public static int[] Arrayofnodes;
        public static int[] ArrayofConnect;
        public static int[] ArrayOfConnecs_red;
        public static int[] ArrayOfConnecs_blue;
        public static int[] ArrayOfConnecs_green;
        public static List<RGBPixel> dist_color;
        public static edge[] arr = new edge[100000000];
        public static double total = 0;
        public static int sz = 0;
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }
        public static void mergsort(List<edge> col, int low, int high)
        {
            int mid;
            if (low < high)
            {
                mid = (low + high) / 2;
                mergsort(col, low, mid);
                mergsort(col, mid + 1, high);
                merg(col, low, high, mid);
            }
            return;

        }
        public static void merg(List<edge>col, int low, int high, int mid)
        {
            int i, j, k;
            i = low;
            k = low;
            j = mid + 1;
            while (i <= mid && j <= high)
            {

                if (col[i].w < col[j].w)
                {
                    arr[k] = col[i];
                    k++;
                    i++;
                }
                else
                {
                    arr[k] = col[j];
                    k++;
                    j++;
                }
            }
            while (i <= mid)
            {
                arr[k] = col[i];
                k++;
                i++;
            }
            while (j <= high)
            {
                arr[k] = col[j];
                k++;
                j++;
            }
            //  MessageBox.Show(low.ToString() + " " + k.ToString() + " " + high.ToString());

            for (i = low; i < k; i++)
            {
                col[i] = arr[i];
            }
        }


        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
    
        public static void distinct_color(RGBPixel[,] ImageMatrix) //  set distinct color in sortedset of int
        {
            int[, ,] arr_color = new int[256, 256, 256];
            dist_color = new List<RGBPixel>();
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int red = (int)ImageMatrix[i, j].red;
                    int green = (int)ImageMatrix[i, j].green;
                    int blue = (int)ImageMatrix[i, j].blue;
                    if (arr_color[red, green, blue] == 0)
                    {
                        arr_color[red, green, blue] = 1;
                        dist_color.Add(ImageMatrix[i, j]);
                    }
                }
            }
           //MessageBox.Show(dist_color.Count().ToString());
        }
        public static List<edge> create_graph_and_min_spanning(RGBPixel[,] ImageMatrix)
        {
            
            distinct_color(ImageMatrix);
            int curnode = 0;
            int mini = -1;
            double min;
            List<edge> graph = new List<edge>(10000001);
            double[] dist = new double[10000001]; // to save min value to connect edge out of graph to current built tree
            int[] vis = new int[10000001]; // chech if node visited 
            int[] prev = new int[10000001]; // save number of node that connect it(curindex)
            for (int i = 0; i < 10000001; i++)
                dist[i] = 2000000000;
            for (int i = 0; i < dist_color.Count() - 1; i++)
            {
                mini = -1;
                min = 2000000000;
                vis[curnode] = 1;
                edge e = new edge(-1, -1, -1);
                for (int j = 0; j < dist_color.Count(); j++)
                {
                    if (vis[j] == 0)
                    {
                        int red = dist_color.ElementAt(curnode).red - dist_color.ElementAt(j).red;
                        int green = dist_color.ElementAt(curnode).green - dist_color.ElementAt(j).green;
                        int blue = dist_color.ElementAt(curnode).blue - dist_color.ElementAt(j).blue;

                        double weight = Math.Sqrt((red * red) + (green * green) + (blue * blue)); // calc weight from curnode to anoter node
                        if (weight < dist[j])
                        {
                            dist[j] = weight;
                            prev[j] = curnode;
                        }
                        if (dist[j] < min)
                        {
                            mini = j;
                            min = dist[j];
                        }
                    }
                }
                if (mini == -1) //check if  graph is disconnected
                    break;
                e.from = prev[mini];
                e.to = mini;
                e.w = min;
                curnode = mini;
                total += min;
                graph.Add(e);
                
            }
            //MessageBox.Show(total.ToString());
            //graph.Sort(delegate(edge e1, edge e2) { return e1.w.CompareTo(e2.w); });
             mergsort(graph, 0, graph.Count() - 1);
            return graph;
        }
        public static void dfs(int cur, int cnt)   //depth _ first _ search 
        {
            if (Arrayofnodes[cur] != 0)
                return;
            ArrayofConnect[cnt]++;
            ArrayOfConnecs_red[cnt] += (int)dist_color.ElementAt(cur).red;
            ArrayOfConnecs_green[cnt] += (int)dist_color.ElementAt(cur).green;
            ArrayOfConnecs_blue[cnt] += (int)dist_color.ElementAt(cur).blue;
            Arrayofnodes[cur] = cnt;
            if (adjmatrix[cur] == null)
                return;
            for (int j = 0; j < adjmatrix[cur].Count(); j++) 
               dfs(adjmatrix[cur].ElementAt(j), cnt);
        }
        public static void Graph_Clusters(int number, RGBPixel[,] ImageMatrix)
        {
            HashOfColor = new int[256, 256, 256];
            List<edge> kursucal = new List<edge>();
            kursucal = create_graph_and_min_spanning(ImageMatrix);
            ArrayofConnect = new int[number+1];              //number of elements for every connect compont.
            Arrayofnodes = new int[dist_color.Count()+1];          // node join --> connect compont.
            ArrayOfConnecs_blue = new int[number+1];        // sumation color blue for every connect compont.
            ArrayOfConnecs_red = new int[number+1];           //// sumation color red for every connect compont.
            ArrayOfConnecs_green = new int[number+1];         // sumation color green for every connect compont.
            adjmatrix= new List<int>[dist_color.Count()+1];
            bool[] mark = new bool[kursucal.Count() + 1];
            for (int j = 0; j < dist_color.Count(); j++)
            {
                int red=(int)dist_color.ElementAt(j).red , blue=(int)dist_color.ElementAt(j).blue,green=(int)dist_color.ElementAt(j).green;
                HashOfColor[red, green, blue] = j;
                adjmatrix[j] = new List<int>();
            }
            number--;
            int cnt = 0, u, v;
            /*
            double maxi;
            while (number != 0)    // o(number*kursucal.Count()) , where number is k_cutlers.
            {
                int idx=0;
                maxi = double.MinValue;
                for (int j = 0; j < kursucal.Count(); j++)
                {
                    if (maxi < kursucal.ElementAt(j).w)
                    {
                        maxi = kursucal.ElementAt(j).w;
                        idx = j;
                    }
                }
                mark[idx] = true;
                kursucal.ElementAt(idx).w=double.MinValue;
                number--;
            }
             */ 
            for (int j = 0; j < kursucal.Count()-number; j++)
            {
                //if (mark[j])
                    //continue;
                u = kursucal.ElementAt(j).from;
                v = kursucal.ElementAt(j).to;
                adjmatrix[u].Add(v);
                adjmatrix[v].Add(u);
            }
            for (int j = 0; j < dist_color.Count(); j++)  //  o(e+v) where e is edge while v is node
            {
                if (Arrayofnodes[j] == 0)    // node is not belong to any group 
                {
                    cnt++;
                    dfs(j, cnt);
                }
            }
            //MessageBox.Show(cnt.ToString());
        }
        public static RGBPixel[,]Quazation_of_image(int Number,RGBPixel[,] ImageMatrix,TextBox T1,TextBox T2)
        {
            Graph_Clusters(Number, ImageMatrix);
            T1.Text = total.ToString();
            T2.Text = dist_color.Count().ToString();
            int row = GetHeight(ImageMatrix);
            int col = GetWidth(ImageMatrix);
            RGBPixel[,] res = new RGBPixel[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    int red = ImageMatrix[i, j].red, green = ImageMatrix[i, j].green, blue = ImageMatrix[i, j].blue;
                    int idx = HashOfColor[red, green, blue],idx_compont;
                    idx_compont = Arrayofnodes[idx];
                    double red1 = (double)ArrayOfConnecs_red[idx_compont] / (double)ArrayofConnect[idx_compont], green1 = (double)ArrayOfConnecs_green[idx_compont] / (double)ArrayofConnect[idx_compont], blue1 = (double)ArrayOfConnecs_blue[idx_compont] / (double)ArrayofConnect[idx_compont];
                    res[i, j].red = (byte)red1;
                    res[i, j].green = (byte)green1;
                    res[i, j].blue = (byte)blue1;
                }
            }
            return res;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Drawing.Imaging;

namespace SimLorenzAtrc
{

    public partial class Form2 : Form
    {
        int dimension = 3;
        double scale = 100;
        double pixelunit;
        internal float pos_left = -50.0f;
        internal float pos_up = 45.0f;  // <-- 90.0f;
        internal float shift_left = 0.0f;
        internal float shift_up = 0.0f;
        internal PointF leftUp;

        int imax = 900, jmax = 900;

        internal double[] arg = new double[3];
        double[, ,] rot = new double[3, 3, 3];
        double[,] basis = { { 100.0, 0.0, 0.0 }, { 0.0, 100.0, 0.0 }, { 0.0, 0.0, 100.0 } };
        double[,] vec = new double[3, 3];
        int arr_size = 48000;
        double[,] arr;
        double param_s = 10.0, param_r = 50.0, param_b = 8.0 / 3.0;
        double[] offset = new double[]{
            0.19700702,
            0.1629469,
            45.41953137
        };

        //Thread mythread;
        bool running;
        int color1 = 0, color2 = 0, color3 = 90, dc1, dc2;

// designer
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(20, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(996, 846);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(996, 846);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 1017);
            this.Controls.Add(this.panel1);
            this.Name = "Form2";
            this.Text = "Lorenz Attractor 2D";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
	    //
	    // saveFileDialog1
	    //
	    this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();  // image File
            this.saveFileDialog1.Filter = "jpgファイル|*.jpg|bmpファイル|*.bmp";
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
	private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        
        public Point Value2Pixel(double x, double y)
        {
            int m, n;
            m = (int)((x - (double)leftUp.X) / pixelunit);
            n = (int)(((double)leftUp.Y - y) / pixelunit);
            return new Point(m, n);
        }

        public void SetMatrix()
        {
            leftUp.X = pos_left + shift_left;
            leftUp.Y = pos_up + shift_up;

            // 0
            rot[0, 0, 0] = Math.Cos(arg[0]);
            rot[0, 0, 1] = -Math.Sin(arg[0]);
            rot[0, 0, 2] = 0.0;
            rot[0, 1, 0] = Math.Sin(arg[0]);
            rot[0, 1, 1] = Math.Cos(arg[0]);
            rot[0, 1, 2] = 0.0;
            rot[0, 2, 0] = 0.0;
            rot[0, 2, 1] = 0.0;
            rot[0, 2, 2] = 1.0;

            // 1
            rot[1, 0, 0] = Math.Cos(arg[1]);
            rot[1, 0, 1] = 0.0;
            rot[1, 0, 2] = Math.Sin(arg[1]);
            rot[1, 1, 0] = 0.0;
            rot[1, 1, 1] = 1.0;
            rot[1, 1, 2] = 0.0;
            rot[1, 2, 0] = -Math.Sin(arg[1]);
            rot[1, 2, 1] = 0.0;
            rot[1, 2, 2] = Math.Cos(arg[1]);

            // 2
            rot[2, 0, 0] = Math.Cos(arg[2]);
            rot[2, 0, 1] = -Math.Sin(arg[2]);
            rot[2, 0, 2] = 0.0;
            rot[2, 1, 0] = Math.Sin(arg[2]);
            rot[2, 1, 1] = Math.Cos(arg[2]);
            rot[2, 1, 2] = 0.0;
            rot[2, 2, 0] = 0.0;
            rot[2, 2, 1] = 0.0;
            rot[2, 2, 2] = 1.0;

        }

        public void RotateBasis()
        {
            int i, j, k, basis_no;
            double[] tmp1 = new double[3];
            double[] tmp2 = new double[3];
            double sum;

            for (basis_no = 0; basis_no < dimension; basis_no++)
            {
                for (i = 0; i < 3; i++)
                {
                    tmp1[i] = basis[basis_no, i];
                }
                for (k = 0; k < 3; k++)
                {
                    for (i = 0; i < 3; i++)
                    {
                        sum = 0.0;
                        for (j = 0; j < 3; j++)
                        {
                            sum += rot[k, i, j] * tmp1[j];
                        }
                        tmp2[i] = sum;
                    }
                    for (i = 0; i < 3; i++)
                    {
                        tmp1[i] = tmp2[i];
                    }
                }
                for (i = 0; i < 3; i++)
                {
                    vec[basis_no, i] = tmp2[i];
                }
            }
        }

        public double[] RotateVec(double[] vec)
        {
            int i, j, k;
            double[] tmp1 = new double[3];
            double[] tmp2 = new double[3];
            double sum;

            for (i = 0; i < 3; i++)
            {
                tmp1[i] = vec[i];
            }
            for (k = 0; k < 3; k++)
            {
                for (i = 0; i < 3; i++)
                {
                    sum = 0.0;
                    for (j = 0; j < 3; j++)
                    {
                        sum += rot[k, i, j] * tmp1[j];
                    }
                    tmp2[i] = sum;
                }
                for (i = 0; i < 3; i++)
                {
                    tmp1[i] = tmp2[i];
                }
            }
            return tmp2;
        }

        public void DrawRotatedBasis()
        {
            Pen[] pen = { new Pen(Color.Black), new Pen(Color.Green), new Pen(Color.Blue) };

            Point[] pnt = new Point[3];
            Brush brush0 = new SolidBrush(Color.White);
            Brush brush1 = new SolidBrush(Color.Red);
            int i;
            Point org = Value2Pixel(0.0, 0.0);

            for (i = 0; i < 3; i++)
            {
                pnt[i] = Value2Pixel(vec[i, 1], vec[i, 2]);
            }

            Graphics g = Graphics.FromImage(pictureBox1.Image);
            g.FillRectangle(brush0, 0, 0, imax, jmax);
            for (i = 0; i < 3; i++)
            {
                g.DrawLine(pen[i], org, pnt[i]);
            }
        }

        public void CalcAttractor()
        {
            double x, y, z, xx, yy, zz, s, r, b, dt;
            int i;
            s = param_s;
            r = param_r;
            b = param_b;


            dt = 0.001;
            x = 5.0; y = 8.0; z = 10.0;

            arr = new double[arr_size, 3];
            arr[0, 0] = x - offset[0];
            arr[0, 1] = y - offset[1];
            arr[0, 2] = z - offset[2];

            for (i = 1; i < arr_size; i++)
            {
                xx = x + s * (-x + y) * dt;
                yy = y + (-x * z + r * x - y) * dt;
                zz = z + (x * y - b * z) * dt;
                arr[i, 0] = xx - offset[0];
                arr[i, 1] = yy - offset[1];
                arr[i, 2] = zz - offset[2];
                x = xx;
                y = yy;
                z = zz;
            }
        }

        public void DrawAttractor()
        {
            double[] pos = new double[3];
            Point pnt = new Point();
            int i, j;

            Brush brush0 = new SolidBrush(Color.White);
            Brush brush1 = new SolidBrush(Color.Red);

            Graphics g = Graphics.FromImage(pictureBox1.Image);

            for (i = 0; i < arr_size; i++)
            {
                for (j = 0; j < 3; j++)
                {
                    pos[j] = arr[i, j];
                }
                pos = RotateVec(pos);

                pnt = Value2Pixel(pos[1], pos[2]);
                g.FillRectangle(brush1, pnt.X, pnt.Y, 1, 1);
            }

            pictureBox1.Refresh();
        }

        public void DrawAttractor2()
        {
            double x, y, z, xx, yy, zz, s, r, b, dt;

            double[] pos = new double[3];
            Point pnt = new Point();

            Brush brushtmp;
            int cnt = 0;

            Brush brush0 = new SolidBrush(Color.White);
            Brush brush1 = new SolidBrush(Color.Red);

            Graphics g = Graphics.FromImage(pictureBox1.Image);

            s = param_s;
            r = param_r;
            b = param_b;


            dt = 0.001;
            x = 5.0; y = 8.0; z = 10.0;

            while (running)
            {
                xx = x + s * (-x + y) * dt;
                yy = y + (-x * z + r * x - y) * dt;
                zz = z + (x * y - b * z) * dt;
                pos[0] = x - offset[0];
                pos[1] = y - offset[1];
                pos[2] = z - offset[2];
                pos = RotateVec(pos);
                pnt = Value2Pixel(pos[1], pos[2]);

                brushtmp = new SolidBrush(Color.FromArgb(color1, color2, color3));

                g.FillRectangle(brushtmp, pnt.X, pnt.Y, 1, 1);
                brushtmp.Dispose();

                pictureBox1.Refresh();

                x = xx;
                y = yy;
                z = zz;

                cnt++;
                if (cnt % 100 == 0)
                {
                    if (color1 == 0 && color2 == 0)
                    {
                        dc1 = 1;
                        dc2 = 0;
                    }
                    else if (color1 == 255 && color2 == 0)
                    {
                        dc1 = 0;
                        dc2 = 1;
                    }
                    else if (color1 == 0 && color2 == 255)
                    {
                        dc1 = 0;
                        dc2 = -1;
                    }
                    else if (color1 == 255 && color2 == 255)
                    {
                        dc1 = -1;
                        dc2 = 0;
                    }
                    color1 += dc1;
                    color2 += dc2;
                }

                //Application.DoEvents();
            }
        }

        public void DrawOrthonormalBasis()
        {
            SetMatrix();
            RotateBasis();
            DrawRotatedBasis();
            DrawAttractor();
        }

        public void DrawOrthonormalBasis2()
        {
            SetMatrix();
            RotateBasis();
            DrawRotatedBasis();
            DrawAttractor2();
        }

        //delegate void simulateDelegate();
        //void worker()
        //{
        //    Invoke(new simulateDelegate(DrawOrthonormalBasis2));
        //}

        public Form2(int width, int height)
        {
            InitializeComponent();
	    imax = width;
	    jmax = height;
            pictureBox1.Image = new Bitmap(imax, jmax);
            pixelunit = scale / imax;
            leftUp = new PointF(pos_left, pos_up);
        }

        public async void SampleAsync() {
            await Task.Run( () => {
                DrawOrthonormalBasis2();
            } );
            Console.WriteLine("SampleAsync is completed.");
        }

        internal void start_drawing()
        {
            //SetEnabled(false);
            running = true;
            //mythread = new Thread(new ThreadStart(worker));
            //mythread.Start();
            SampleAsync();
        }
        internal void stop_drawing()
        {
            // stop
            running = false;
            //SetEnabled(true);
        }
        internal void save_image()
        {
            if ( saveFileDialog1.ShowDialog() == DialogResult.OK )
            {
                 if ( saveFileDialog1.FilterIndex == 1 ){
                     pictureBox1.Image.Save(saveFileDialog1.FileName,ImageFormat.Jpeg);
                 }
                 else if ( saveFileDialog1.FilterIndex == 2 ){
                     pictureBox1.Image.Save(saveFileDialog1.FileName,ImageFormat.Bmp);
                 }
            }
        }
    }/* end Form2 */



    public partial class Form1 : Form
    {
        Form2 fm2;

        //
        // 引数の解析
        //
        private void AnalyzeArgs(ref int width, ref int height)
        {
            int pos = -1, len;
            string opt, val;

            string [] args = Environment.GetCommandLineArgs();
            if ( args.Length > 1 ){
                for ( int i = 0; i < args.Length; i++ ){
                  //Console.WriteLine(args[i]);

                  pos = args[i].IndexOf(":");
                  if ( pos > 0 )
                  {
                    len = args[i].Length;
                    opt = args[i].Substring(0,pos);
                    val = args[i].Substring(pos+1,len - pos -1);

                    switch ( opt )
                    {
                       case "/w":
                                    width = int.Parse(val);
                                    break;
                       case "/h":
                                    height = int.Parse(val);
                                    break;
                       default:
                                    break;
                   }
                 }
               }
           }
       }
// designer
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.trackBar4 = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBar5 = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(152, 65);
            this.trackBar1.Maximum = 50;
            this.trackBar1.Minimum = -50;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(395, 90);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(152, 128);
            this.trackBar2.Maximum = 50;
            this.trackBar2.Minimum = -50;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(395, 90);
            this.trackBar2.TabIndex = 1;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(152, 193);
            this.trackBar3.Maximum = 50;
            this.trackBar3.Minimum = -50;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(395, 90);
            this.trackBar3.TabIndex = 2;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(554, 65);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 31);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "0";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(554, 128);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 31);
            this.textBox2.TabIndex = 4;
            this.textBox2.Text = "0";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(554, 193);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 31);
            this.textBox3.TabIndex = 5;
            this.textBox3.Text = "0";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(152, 510);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(200, 40);
            this.button1.TabIndex = 6;
            this.button1.Text = "start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(152, 583);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(200, 40);
            this.button2.TabIndex = 7;
            this.button2.Text = "stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(152, 653);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(200, 40);
            this.button3.TabIndex = 8;
            this.button3.Text = "save image";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(28, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(316, 27);
            this.label1.TabIndex = 9;
            this.label1.Text = "Rotation (Eulerian Angular)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(104, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 24);
            this.label2.TabIndex = 10;
            this.label2.Text = "ψ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(101, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 24);
            this.label3.TabIndex = 11;
            this.label3.Text = "θ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(101, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 24);
            this.label4.TabIndex = 12;
            this.label4.Text = "φ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(27, 464);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(213, 27);
            this.label5.TabIndex = 13;
            this.label5.Text = "Attractor Drawing";
            // 
            // trackBar4
            // 
            this.trackBar4.Location = new System.Drawing.Point(140, 299);
            this.trackBar4.Maximum = 50;
            this.trackBar4.Minimum = -50;
            this.trackBar4.Name = "trackBar4";
            this.trackBar4.Size = new System.Drawing.Size(395, 90);
            this.trackBar4.TabIndex = 14;
            this.trackBar4.Scroll += new System.EventHandler(this.trackBar4_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(25, 264);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 27);
            this.label6.TabIndex = 15;
            this.label6.Text = "Shift";
            // 
            // trackBar5
            // 
            this.trackBar5.Location = new System.Drawing.Point(142, 369);
            this.trackBar5.Maximum = 50;
            this.trackBar5.Minimum = -50;
            this.trackBar5.Name = "trackBar5";
            this.trackBar5.Size = new System.Drawing.Size(395, 90);
            this.trackBar5.TabIndex = 16;
            this.trackBar5.Scroll += new System.EventHandler(this.trackBar5_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(40, 312);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 24);
            this.label7.TabIndex = 17;
            this.label7.Text = "vertical";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(37, 369);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 24);
            this.label8.TabIndex = 18;
            this.label8.Text = "horizontal";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(554, 305);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 31);
            this.textBox4.TabIndex = 19;
            this.textBox4.Text = "0";
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(554, 369);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 31);
            this.textBox5.TabIndex = 20;
            this.textBox5.Text = "0";
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 790);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.trackBar5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.trackBar4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar1);
            this.Name = "Form1";
            this.Text = "Control Form";
            //this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackBar4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar trackBar5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
//

        public Form1()
        {
	    int width =900;
	    int height =900;

            InitializeComponent();
	    AnalyzeArgs(ref width, ref height);
	    Console.WriteLine("  width={0}   height={1}", width, height);
            fm2 = new Form2(width,height);
            fm2.Show();
            ((Form2)fm2).CalcAttractor();
            ((Form2)fm2).DrawOrthonormalBasis();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = (trackBar1.Value).ToString();
            ((Form2)fm2).arg[0] = ((double)trackBar1.Value) * Math.PI / 100.0;
            ((Form2)fm2).DrawOrthonormalBasis();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = (trackBar2.Value).ToString();
            ((Form2)fm2).arg[1] = ((double)trackBar2.Value) * Math.PI / 100.0;
            ((Form2)fm2).DrawOrthonormalBasis();

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            textBox3.Text = (trackBar3.Value).ToString();
            ((Form2)fm2).arg[2] = ((double)trackBar3.Value) * Math.PI / 100.0;
            ((Form2)fm2).DrawOrthonormalBasis();

        }

        private void SetEnabled(bool val)
        {
            trackBar1.Enabled = val; trackBar2.Enabled = val; trackBar3.Enabled = val; trackBar4.Enabled = val; trackBar5.Enabled = val;
            button1.Enabled = val;
            button3.Enabled = val;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            ((Form2)fm2).start_drawing();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetEnabled(true);
            ((Form2)fm2).stop_drawing();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ((Form2)fm2).save_image();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ((Form2)fm2).stop_drawing();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            textBox4.Text = (trackBar4.Value).ToString();
            ((Form2)fm2).shift_up = -0.5f*(float)(trackBar4.Value);
            ((Form2)fm2).DrawOrthonormalBasis();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            textBox5.Text = (trackBar5.Value).ToString();
            ((Form2)fm2).shift_left = -0.2f*(float)(trackBar5.Value);
            ((Form2)fm2).DrawOrthonormalBasis();
        }
    }/* end Form1 */


    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}

//
//  Lorenz Attractor 3D with OpenTK
//
//    using OpenTK.dll and OpenTK.GLControl.dll
//
//    /reference:OpenTK.dll
//    /reference:OpenTK.GLControl.dll
//
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

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;


namespace LorenzAtrc3D
{
    public class Form2 : Form
    {
        const int dimension = 3;
	const int num_angle = 3;

        int width = 2600, height = 1200;
        int iteration;

        internal double[] angle = new double[num_angle];
        double[, ,] rot = new double[num_angle, dimension, dimension];

        List<double[]> attr_pnts = new List<double[]>();

        //Thread mythread;
        //bool running;

        double param_s = 10.0, param_r = 50.0, param_b = 8.0 / 3.0; // Lorenz Attractor Param
        double[] vec_a = new double[3];
        double[] vec_b = new double[3];

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
            this.glControl = new OpenTK.GLControl();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.Color.SkyBlue;

            this.glControl.Location = new System.Drawing.Point(10, 10);

            this.glControl.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(width, height);
            Console.WriteLine("  width = {0} height = {1}",width,height);

            this.glControl.TabIndex = 0;
            this.glControl.VSync = false;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(width, height);

            this.Controls.Add(this.glControl);
            this.Name = "Form2";
            this.Text = "Lorenz Attractor 3D";
            this.ResumeLayout(false);

            Console.WriteLine(" GL width = {0} height = {1}",this.glControl.Width,this.glControl.Height);
	    //
	    // saveFileDialog1
	    //
	    this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();  // image File
            this.saveFileDialog1.Filter = "jpgファイル|*.jpg|bmpファイル|*.bmp";
        }

        #endregion

	private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private OpenTK.GLControl glControl;
        
        private void glControl_Load(object sender, EventArgs e)
        {
            // 背景色の設定
            GL.ClearColor(glControl.BackColor);

            // ビューポートの設定
            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            // 視体積の設定
            GL.MatrixMode(MatrixMode.Projection);

            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver6, glControl.AspectRatio,0.1f,400.0f);
            GL.LoadMatrix(ref proj);

            // 視界の設定
            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 look = Matrix4.LookAt(new Vector3(0, 0, -80.0f), Vector3.Zero, Vector3.UnitY);

            GL.LoadMatrix(ref look);

            // デプスバッファの使用
            GL.Enable(EnableCap.DepthTest);

            // 光源の使用
            GL.Enable(EnableCap.Lighting);

            float[] position = new float[] { 100.0f, 200.0f, -50.0f, 0.0f };
            GL.Light(LightName.Light0, LightParameter.Position, position);
            GL.Enable(EnableCap.Light0);
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            // ビューポートの設定
            GL.Viewport(0, 0, glControl.Width, glControl.Height);
        }

        public void glControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            draw_attractor();
        }

        public void draw_attractor()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Color4.Red);
            my_draw_attractor();
            glControl.SwapBuffers();
	}

        public void calc_attractor()
        {
            double x, y, z, xx, yy, zz, s, r, b, dt;
            int interval = 10;

            dt = 0.001;
            x = 5.0; y = 8.0; z = 10.0;

            s = param_s;
            r = param_r;
            b = param_b;

            double[] offset = new double[]{
                0.19700702,
                0.1629469,
                45.41953137
            };

            for (int i = 0; i < iteration; i++)
            {
                xx = x + s * (-x + y) * dt;
                yy = y + (-x * z + r * x - y) * dt;
                zz = z + (x * y - b * z) * dt;
                if ( i % interval == 0 ){
                    attr_pnts.Add( new double[] { x - offset[0], y - offset[1], z - offset[2] } );
                }
                x = xx;
                y = yy;
                z = zz;
            }
	    Console.WriteLine("calculation : attr_pnts.Count = {0}",attr_pnts.Count());
        }

        public void my_draw_attractor()
        {
            double size = 0.5;

	    SetMatrix();

            foreach ( var pnt in attr_pnts )
            {
                vec_b = new double[] { pnt[0], pnt[1], pnt[2] };
                vec_a = RotateVec(vec_b);
                cube( vec_a[0], vec_a[1], vec_a[2], size);
            }
        }

        void cube(double x, double y, double z, double size)
        {
	    double w, h, d;
            double half_size = size / 2.0;
            w = half_size;
            h = half_size;
            d = half_size;

            GL.PushMatrix();

            GL.Begin(PrimitiveType.TriangleStrip); // right
	    {
                GL.Normal3( Vector3.UnitX);
                GL.Vertex3( x+w, y+h, z-d);
                GL.Vertex3( x+w, y+h, z+d);
                GL.Vertex3( x+w, y-h, z-d);
                GL.Vertex3( x+w, y-h, z+d);
	    }
            GL.End();
            GL.Begin(PrimitiveType.TriangleStrip); // left
	    {
                GL.Normal3(-Vector3.UnitX);
                GL.Vertex3( x-w, y-h, z-d);
                GL.Vertex3( x-w, y-h, z+d);
                GL.Vertex3( x-w, y+h, z-d);
                GL.Vertex3( x-w, y+h, z+d);
	    }
            GL.End();
            GL.Begin(PrimitiveType.TriangleStrip); // up
	    {
                GL.Normal3( Vector3.UnitY);
                GL.Vertex3( x+w, y+h, z-d);
                GL.Vertex3( x-w, y+h, z-d);
                GL.Vertex3( x+w, y+h, z+d);
                GL.Vertex3( x-w, y+h, z+d);
	    }
            GL.End();
            GL.Begin(PrimitiveType.TriangleStrip); // down
	    {
                GL.Normal3(-Vector3.UnitY);
                GL.Vertex3( x-w, y-h, z+d);
                GL.Vertex3( x-w, y-h, z-d);
                GL.Vertex3( x+w, y-h, z+d);
                GL.Vertex3( x+w, y-h, z-d);
	    }
            GL.End();
            GL.Begin(PrimitiveType.TriangleStrip); // front
	    {
                GL.Normal3( Vector3.UnitZ);
                GL.Vertex3( x+w, y+h, z+d);
                GL.Vertex3( x-w, y+h, z+d);
                GL.Vertex3( x+w, y-h, z+d);
                GL.Vertex3( x-w, y-h, z+d);
	    }
            GL.End();
            GL.Begin(PrimitiveType.TriangleStrip); // back
	    {
                GL.Normal3(-Vector3.UnitZ);
                GL.Vertex3( x+w, y-h, z-d);
                GL.Vertex3( x-w, y-h, z-d);
                GL.Vertex3( x+w, y+h, z-d);
                GL.Vertex3( x-w, y+h, z-d);
	    }
            GL.End();

            GL.PopMatrix();
        }

        public void SetMatrix()
        {
            // Rotation Matrix 0
            rot[0, 0, 0] = Math.Cos(angle[0]);
            rot[0, 0, 1] = -Math.Sin(angle[0]);
            rot[0, 0, 2] = 0.0;
            rot[0, 1, 0] = Math.Sin(angle[0]);
            rot[0, 1, 1] = Math.Cos(angle[0]);
            rot[0, 1, 2] = 0.0;
            rot[0, 2, 0] = 0.0;
            rot[0, 2, 1] = 0.0;
            rot[0, 2, 2] = 1.0;

            // Rotation Matrix 1
            rot[1, 0, 0] = Math.Cos(angle[1]);
            rot[1, 0, 1] = 0.0;
            rot[1, 0, 2] = Math.Sin(angle[1]);
            rot[1, 1, 0] = 0.0;
            rot[1, 1, 1] = 1.0;
            rot[1, 1, 2] = 0.0;
            rot[1, 2, 0] = -Math.Sin(angle[1]);
            rot[1, 2, 1] = 0.0;
            rot[1, 2, 2] = Math.Cos(angle[1]);

            // Rotation Matrix 2
            rot[2, 0, 0] = Math.Cos(angle[2]);
            rot[2, 0, 1] = -Math.Sin(angle[2]);
            rot[2, 0, 2] = 0.0;
            rot[2, 1, 0] = Math.Sin(angle[2]);
            rot[2, 1, 1] = Math.Cos(angle[2]);
            rot[2, 1, 2] = 0.0;
            rot[2, 2, 0] = 0.0;
            rot[2, 2, 1] = 0.0;
            rot[2, 2, 2] = 1.0;
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

	/*
        public void DrawAttractor2()
        {
            int sim_iteration;
            int current_n;
            double size = 0.5;

	    current_n = 0;
            //for ( current_n=0; current_n < attr_pnts.Count() && running; current_n++ )
	    sim_iteration = attr_pnts.Count() / 2;
	    Console.WriteLine("   attr_pnts.Count = {0}",attr_pnts.Count());
	    Console.WriteLine("   sim_ite = {0}",sim_iteration);
            //for ( current_n=0; current_n < attr_pnts.Count(); current_n++ )
            for ( current_n=0; current_n < sim_iteration; current_n++ )
            {
                if ( running ){
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, Color4.Red);
	    //Console.WriteLine("   current_n = {0}",current_n);
                    for ( int i=0; i<current_n; i++ ){
                        vec_b = new double[] { attr_pnts[i][0], attr_pnts[i][1], attr_pnts[i][2] };
                        vec_a = RotateVec(vec_b);
                        cube( vec_a[0], vec_a[1], vec_a[2], size);
                    }
		    glControl.SwapBuffers();
		}
		else {
                    break;
		}

                Application.DoEvents();
            }
	    Console.WriteLine("   attr_pnts.Count = {0}",attr_pnts.Count());
	    Console.WriteLine("   sim_ite = {0}",sim_iteration);
	    Console.WriteLine("   current_n = {0}",current_n);
	    Console.WriteLine("DrawAttractor2 is finished.");
        }

        delegate void simulateDelegate();
        void worker()
        {
            Invoke(new simulateDelegate(DrawAttractor2));
        }
	*/

        public Form2(int width, int height, int iteration, double theta)
        {
	    this.width = width;
	    this.height = height;
            this.iteration = iteration;
            this.angle[1] = theta;

            InitializeComponent();
            calc_attractor();

            //イベントの追加 
            glControl.Load += glControl_Load;
            glControl.Paint += glControl_Paint;
            glControl.Resize += glControl_Resize;
        }

        //public async void SampleAsync() {
        //    await Task.Run( () => {
        //        DrawAttractor2();
        //    } );
        //    Console.WriteLine("SampleAsync is completed.");
        //}

	/*
        internal void start_drawing()
        {
            //SetEnabled(false);
            running = true;
            mythread = new Thread(new ThreadStart(worker));
            mythread.Start();
            //SampleAsync();
        }
        internal void stop_drawing()
        {
            // stop
            running = false;
            //SetEnabled(true);
        }
	*/

        internal void save_image()
        {
            if ( saveFileDialog1.ShowDialog() == DialogResult.OK )
            {
                 GLrefresh();
                 Image img = TakeScreenshot();
                 if ( saveFileDialog1.FilterIndex == 1 ){
                     img.Save(saveFileDialog1.FileName,ImageFormat.Jpeg);
                 }
                 else if ( saveFileDialog1.FilterIndex == 2 ){
                     img.Save(saveFileDialog1.FileName,ImageFormat.Bmp);
                 }
            }
        }

        /* get from stack overflow web site */
        public Bitmap TakeScreenshot()
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();
            int w = glControl.ClientSize.Width;
            int h = glControl.ClientSize.Height;
            Bitmap bmp = new Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(glControl.ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly,
                             System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, w, h, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        public void GLrefresh()
        {
            //glControl.Invalidate();
            //glControl.Update();
            glControl.Refresh();
        }

    }/* end Form2 */



    public partial class Form1 : Form
    {
        Form2 fm2;

        //
        // 引数の解析
        //
        private void AnalyzeArgs(ref int width, ref int height, ref int iteration, ref double theta)
        {
            int pos = -1, len;
            string opt, val;

            string [] args = Environment.GetCommandLineArgs();
            if ( args.Length > 1 ){
                for ( int i = 0; i < args.Length; i++ ){

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
                       case "/iteration":
                                    iteration = int.Parse(val);
                                    break;
                       case "/theta":
                                    theta = double.Parse(val);
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
            //this.trackBar4 = new System.Windows.Forms.TrackBar();
            //this.label6 = new System.Windows.Forms.Label();
            //this.trackBar5 = new System.Windows.Forms.TrackBar();
            //this.label7 = new System.Windows.Forms.Label();
            //this.label8 = new System.Windows.Forms.Label();
            //this.textBox4 = new System.Windows.Forms.TextBox();
            //this.textBox5 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
            //((System.ComponentModel.ISupportInitialize)(this.trackBar5)).BeginInit();
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
            //this.button1.Location = new System.Drawing.Point(152, 510);
            this.button1.Location = new System.Drawing.Point(152, 310);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(200, 40);
            this.button1.TabIndex = 6;
            this.button1.Text = "start";
            this.button1.UseVisualStyleBackColor = true;
            //this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.Enabled = false;
            // 
            // button2
            // 
            //this.button2.Location = new System.Drawing.Point(152, 583);
            this.button2.Location = new System.Drawing.Point(152, 383);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(200, 40);
            this.button2.TabIndex = 7;
            this.button2.Text = "stop";
            this.button2.UseVisualStyleBackColor = true;
            //this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button2.Enabled = false;
            // 
            // button3
            // 
            //this.button3.Location = new System.Drawing.Point(152, 653);
            this.button3.Location = new System.Drawing.Point(152, 453);
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
            //this.label5.Location = new System.Drawing.Point(27, 464);
            this.label5.Location = new System.Drawing.Point(27, 264);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(213, 27);
            this.label5.TabIndex = 13;
            this.label5.Text = "Attractor Drawing";
            // 
            // trackBar4
            // 
	    /*
            this.trackBar4.Location = new System.Drawing.Point(140, 299);
            this.trackBar4.Maximum = 50;
            this.trackBar4.Minimum = -50;
            this.trackBar4.Name = "trackBar4";
            this.trackBar4.Size = new System.Drawing.Size(395, 90);
            this.trackBar4.TabIndex = 14;
            this.trackBar4.Scroll += new System.EventHandler(this.trackBar4_Scroll);
            this.trackBar4.Enabled = false;
	    */
            // 
            // label6
            // 
	    /*
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(25, 264);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 27);
            this.label6.TabIndex = 15;
            this.label6.Text = "Shift";
	    */
            // 
            // trackBar5
            // 
	    /*
            this.trackBar5.Location = new System.Drawing.Point(142, 369);
            this.trackBar5.Maximum = 50;
            this.trackBar5.Minimum = -50;
            this.trackBar5.Name = "trackBar5";
            this.trackBar5.Size = new System.Drawing.Size(395, 90);
            this.trackBar5.TabIndex = 16;
            this.trackBar5.Scroll += new System.EventHandler(this.trackBar5_Scroll);
            this.trackBar5.Enabled = false;
	    */
            // 
            // label7
            // 
	    /*
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(40, 312);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 24);
            this.label7.TabIndex = 17;
            this.label7.Text = "vertical";
	    */
            // 
            // label8
            // 
	    /*
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(37, 369);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(106, 24);
            this.label8.TabIndex = 18;
            this.label8.Text = "horizontal";
	    */
            // 
            // textBox4
            // 
	    /*
            this.textBox4.Location = new System.Drawing.Point(554, 305);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 31);
            this.textBox4.TabIndex = 19;
            this.textBox4.Text = "0";
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox4.Enabled = false;
	    */
            // 
            // textBox5
            // 
	    /*
            this.textBox5.Location = new System.Drawing.Point(554, 369);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 31);
            this.textBox5.TabIndex = 20;
            this.textBox5.Text = "0";
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox5.Enabled = false;
	    */
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 790);
            //this.Controls.Add(this.textBox5);
            //this.Controls.Add(this.textBox4);
            //this.Controls.Add(this.label8);
            //this.Controls.Add(this.label7);
            //this.Controls.Add(this.trackBar5);
            //this.Controls.Add(this.label6);
            //this.Controls.Add(this.trackBar4);
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
            //((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
            //((System.ComponentModel.ISupportInitialize)(this.trackBar5)).EndInit();
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
        //private System.Windows.Forms.TrackBar trackBar4;
        //private System.Windows.Forms.Label label6;
        //private System.Windows.Forms.TrackBar trackBar5;
        //private System.Windows.Forms.Label label7;
        //private System.Windows.Forms.Label label8;
        //private System.Windows.Forms.TextBox textBox4;
        //private System.Windows.Forms.TextBox textBox5;

        public Form1()
        {
	    int width =2600;
	    int height =1200;
	    int iteration = 48000;
	    //int iteration = 12000;
	    double  theta = 0.0;

            InitializeComponent();

	    AnalyzeArgs(ref width, ref height, ref iteration, ref theta);
            fm2 = new Form2( width, height, iteration, theta);
            fm2.Show();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox1.Text = (trackBar1.Value).ToString();
            ((Form2)fm2).angle[0] = ((double)trackBar1.Value) * Math.PI / 100.0;
            ((Form2)fm2).draw_attractor();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox2.Text = (trackBar2.Value).ToString();
            ((Form2)fm2).angle[1] = ((double)trackBar2.Value) * Math.PI / 100.0;
            ((Form2)fm2).draw_attractor();

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            textBox3.Text = (trackBar3.Value).ToString();
            ((Form2)fm2).angle[2] = ((double)trackBar3.Value) * Math.PI / 100.0;
            ((Form2)fm2).draw_attractor();

        }

        private void SetEnabled(bool val)
        {
            trackBar1.Enabled = val; trackBar2.Enabled = val; trackBar3.Enabled = val;
            //trackBar4.Enabled = val; trackBar5.Enabled = val;
            button1.Enabled = val;
            button3.Enabled = val;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetEnabled(false);
            //((Form2)fm2).start_drawing();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetEnabled(true);
            //((Form2)fm2).stop_drawing();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ((Form2)fm2).save_image();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //((Form2)fm2).stop_drawing();
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

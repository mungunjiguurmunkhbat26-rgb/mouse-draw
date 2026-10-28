using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace mouse_draw
{
    public partial class Form1 : Form
    {
        ToolStrip toolStrip1;
        PictureBox pictureBox1;
        ColorDialog colorDialog1;
        Panel pnlCurrentColor;

        ToolContext ctx = new ToolContext();
        List<Tool> tools = new List<Tool>();
        Tool currentTool;
        bool drawing;
        bool isShapeTool => currentTool is ShapeTool;

        Stack<Bitmap> undoStack = new Stack<Bitmap>();
        Stack<Bitmap> redoStack = new Stack<Bitmap>();

        public Form1()
        {
            InitializeComponent();

            Text = "Paint";
            Size = new Size(1100, 650);
            KeyPreview = true;
            KeyDown += Form1_KeyDown;
            Load += Form1_Load;
            Resize += Form1_Resize;

            // Хэрэгслүүдийг бүртгэх: шинэ хэрэгсэл нэмэхэд энд л нэмнэ
            tools.Add(new PenTool());
            tools.Add(new EraserTool());
            tools.Add(new LineTool());
            tools.Add(new RectangleTool());
            tools.Add(new EllipseTool());
            tools.Add(new FillTool());
            currentTool = tools[0];

            colorDialog1 = new ColorDialog();

            pictureBox1 = new PictureBox { Dock = DockStyle.Fill, BackColor = Color.White };
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.Paint += pictureBox1_Paint;

            toolStrip1 = new ToolStrip
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden
            };

            Controls.Add(pictureBox1);
            Controls.Add(toolStrip1);

            BuildToolbar();
        }

        // ================= TOOLBAR =================
        void BuildToolbar()
        {
            ComboBox cmbTool = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 110
            };
            foreach (Tool t in tools) cmbTool.Items.Add(t);
            cmbTool.SelectedIndex = 0;
            cmbTool.SelectedIndexChanged += (s, e) => currentTool = (Tool)cmbTool.SelectedItem;
            toolStrip1.Items.Add(new ToolStripControlHost(cmbTool));

            toolStrip1.Items.Add(new ToolStripSeparator());

            toolStrip1.Items.Add(new ToolStripLabel("Зузаан:"));
            NumericUpDown nudWidth = new NumericUpDown { Minimum = 1, Maximum = 50, Value = 2, Width = 50 };
            nudWidth.ValueChanged += (s, e) => ctx.Pen.Width = (float)nudWidth.Value;
            toolStrip1.Items.Add(new ToolStripControlHost(nudWidth));

            toolStrip1.Items.Add(new ToolStripLabel("Баллуур:"));
            NumericUpDown nudEraser = new NumericUpDown { Minimum = 5, Maximum = 100, Value = 20, Width = 50 };
            nudEraser.ValueChanged += (s, e) => ctx.EraserSize = (int)nudEraser.Value;
            toolStrip1.Items.Add(new ToolStripControlHost(nudEraser));

            toolStrip1.Items.Add(new ToolStripSeparator());

            toolStrip1.Items.Add(new ToolStripLabel("Өнгө:"));
            Color[] palette =
            {
                Color.Black, Color.Gray, Color.White, Color.Red,
                Color.Orange, Color.Yellow, Color.Green, Color.Cyan,
                Color.Blue, Color.Purple, Color.Pink, Color.Brown
            };
            foreach (Color c in palette)
            {
                Color col = c;
                Panel box = new Panel
                {
                    BackColor = col,
                    Size = new Size(20, 20),
                    BorderStyle = BorderStyle.FixedSingle,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(1)
                };
                box.Click += (s, e) => SetColor(col);
                toolStrip1.Items.Add(new ToolStripControlHost(box));
            }

            AddButton("Өөр...", () =>
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                    SetColor(colorDialog1.Color);
            });

            pnlCurrentColor = new Panel
            {
                BackColor = ctx.Color,
                Size = new Size(26, 26),
                BorderStyle = BorderStyle.Fixed3D
            };
            toolStrip1.Items.Add(new ToolStripControlHost(pnlCurrentColor));

            toolStrip1.Items.Add(new ToolStripSeparator());
            AddButton("Шинэ", NewCanvas);
            AddButton("Нээх", OpenImage);
            AddButton("Хадгалах", SaveImage);
            toolStrip1.Items.Add(new ToolStripSeparator());
            AddButton("Undo", Undo);
            AddButton("Redo", Redo);
            toolStrip1.Items.Add(new ToolStripSeparator());
            AddButton("Гарах", Close);
        }

        void AddButton(string text, Action action)
        {
            ToolStripButton b = new ToolStripButton(text);
            b.Click += (s, e) => action();
            toolStrip1.Items.Add(b);
        }

        void SetColor(Color c)
        {
            ctx.Color = c;
            ctx.Pen.Color = c;
            pnlCurrentColor.BackColor = c;
        }

        // ================= CANVAS =================
        private void Form1_Load(object sender, EventArgs e)
        {
            ctx.Canvas = new Bitmap(Math.Max(pictureBox1.Width, 1),
                                    Math.Max(pictureBox1.Height, 1));
            using (Graphics g = Graphics.FromImage(ctx.Canvas))
                g.Clear(Color.White);
        }

        void SaveState()
        {
            undoStack.Push((Bitmap)ctx.Canvas.Clone());
            foreach (var b in redoStack) b.Dispose();
            redoStack.Clear();
        }

        void Undo()
        {
            if (undoStack.Count == 0) return;
            redoStack.Push((Bitmap)ctx.Canvas.Clone());
            ctx.Canvas.Dispose();
            ctx.Canvas = undoStack.Pop();
            pictureBox1.Invalidate();
        }

        void Redo()
        {
            if (redoStack.Count == 0) return;
            undoStack.Push((Bitmap)ctx.Canvas.Clone());
            ctx.Canvas.Dispose();
            ctx.Canvas = redoStack.Pop();
            pictureBox1.Invalidate();
        }

        void NewCanvas()
        {
            SaveState();
            using (Graphics g = Graphics.FromImage(ctx.Canvas))
                g.Clear(Color.White);
            pictureBox1.Invalidate();
        }

        void SaveImage()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp";
                sfd.FileName = "zurag";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                ImageFormat fmt = ImageFormat.Png;
                string ext = Path.GetExtension(sfd.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg") fmt = ImageFormat.Jpeg;
                else if (ext == ".bmp") fmt = ImageFormat.Bmp;

                ctx.Canvas.Save(sfd.FileName, fmt);
            }
        }

        void OpenImage()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Зураг|*.png;*.jpg;*.jpeg;*.bmp";
                if (ofd.ShowDialog() != DialogResult.OK) return;

                SaveState();
                using (Bitmap loaded = new Bitmap(ofd.FileName))
                {
                    Bitmap old = ctx.Canvas;
                    ctx.Canvas = new Bitmap(Math.Max(loaded.Width, pictureBox1.Width),
                                            Math.Max(loaded.Height, pictureBox1.Height));
                    using (Graphics g = Graphics.FromImage(ctx.Canvas))
                    {
                        g.Clear(Color.White);
                        g.DrawImage(loaded, 0, 0, loaded.Width, loaded.Height);
                    }
                    old.Dispose();
                }
                pictureBox1.Invalidate();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (ctx.Canvas == null) return;
            int w = Math.Max(ctx.Canvas.Width, pictureBox1.Width);
            int h = Math.Max(ctx.Canvas.Height, pictureBox1.Height);
            if (w == ctx.Canvas.Width && h == ctx.Canvas.Height) return;

            Bitmap bigger = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(bigger))
            {
                g.Clear(Color.White);
                g.DrawImage(ctx.Canvas, 0, 0);
            }
            ctx.Canvas.Dispose();
            ctx.Canvas = bigger;
        }

        // ================= ХУЛГАНА (хэрэгсэл рүү дамжуулна) =================
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            SaveState();
            drawing = true;
            currentTool.MouseDown(ctx, e.Location);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawing) return;
            currentTool.MouseMove(ctx, e.Location);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!drawing) return;
            drawing = false;
            currentTool.MouseUp(ctx, e.Location);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (ctx.Canvas == null) return;
            e.Graphics.DrawImage(ctx.Canvas, 0, 0);

            if (drawing && isShapeTool)
                currentTool.DrawPreview(e.Graphics, ctx);
        }

        // ================= ТОВЧЛУУР =================
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z) Undo();
            else if (e.Control && e.KeyCode == Keys.Y) Redo();
            else if (e.Control && e.KeyCode == Keys.S) SaveImage();
            else if (e.Control && e.KeyCode == Keys.N) NewCanvas();
            else if (e.Control && e.KeyCode == Keys.O) OpenImage();
        }
    }
}
using mouse_draw;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace mouse_draw
{
    // Шулуун, тэгш өнцөгт, эллипсийн нийтлэг суурь
    public abstract class ShapeTool : Tool
    {
        protected Point start, current;

        public override void MouseDown(ToolContext ctx, Point pt)
        {
            start = current = pt;
        }

        public override void MouseMove(ToolContext ctx, Point pt)
        {
            current = pt;
        }

        public override void MouseUp(ToolContext ctx, Point pt)
        {
            using (Graphics g = Graphics.FromImage(ctx.Canvas))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Draw(g, ctx.Pen, start, pt);
            }
        }

        public override void DrawPreview(Graphics g, ToolContext ctx)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Draw(g, ctx.Pen, start, current);
        }

        // Дүрс бүр өөрөө хэрхэн зурахаа тодорхойлно
        protected abstract void Draw(Graphics g, Pen pen, Point a, Point b);

        protected static Rectangle GetRect(Point a, Point b) =>
            new Rectangle(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y),
                          Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));
    }
}
using mouse_draw;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace mouse_draw
{
    public class PenTool : Tool
    {
        Point last;
        public override string Name => "Үзэг";

        public override void MouseDown(ToolContext ctx, Point pt)
        {
            last = pt;
        }

        public override void MouseMove(ToolContext ctx, Point pt)
        {
            using (Graphics g = Graphics.FromImage(ctx.Canvas))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawLine(ctx.Pen, last, pt);
            }
            last = pt;
        }
    }
}
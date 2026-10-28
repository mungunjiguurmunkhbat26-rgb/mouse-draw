using mouse_draw;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace mouse_draw
{
    public class EraserTool : Tool
    {
        Point last;
        public override string Name => "Баллуур";

        public override void MouseDown(ToolContext ctx, Point pt)
        {
            last = pt;
        }

        public override void MouseMove(ToolContext ctx, Point pt)
        {
            using (Graphics g = Graphics.FromImage(ctx.Canvas))
            using (Pen eraser = new Pen(Color.White, ctx.EraserSize))
            {
                eraser.StartCap = LineCap.Round;
                eraser.EndCap = LineCap.Round;
                g.DrawLine(eraser, last, pt);
            }
            last = pt;
        }
    }
}
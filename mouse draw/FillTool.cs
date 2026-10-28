using mouse_draw;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Drawing;

namespace mouse_draw
{
    public class FillTool : Tool
    {
        public override string Name => "Будах";

        public override void MouseDown(ToolContext ctx, Point pt)
        {
            Bitmap bmp = ctx.Canvas;
            if (pt.X < 0 || pt.Y < 0 || pt.X >= bmp.Width || pt.Y >= bmp.Height) return;

            int target = bmp.GetPixel(pt.X, pt.Y).ToArgb();
            int fill = ctx.Color.ToArgb();
            if (target == fill) return;

            Stack<Point> stack = new Stack<Point>();
            stack.Push(pt);

            while (stack.Count > 0)
            {
                Point q = stack.Pop();
                if (bmp.GetPixel(q.X, q.Y).ToArgb() != target) continue;

                int left = q.X, right = q.X;
                while (left > 0 && bmp.GetPixel(left - 1, q.Y).ToArgb() == target) left--;
                while (right < bmp.Width - 1 && bmp.GetPixel(right + 1, q.Y).ToArgb() == target) right++;

                for (int x = left; x <= right; x++)
                {
                    bmp.SetPixel(x, q.Y, ctx.Color);
                    if (q.Y > 0 && bmp.GetPixel(x, q.Y - 1).ToArgb() == target)
                        stack.Push(new Point(x, q.Y - 1));
                    if (q.Y < bmp.Height - 1 && bmp.GetPixel(x, q.Y + 1).ToArgb() == target)
                        stack.Push(new Point(x, q.Y + 1));
                }
            }
        }
    }
}
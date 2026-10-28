using mouse_draw;
using System.Drawing;

namespace mouse_draw
{
    public class LineTool : ShapeTool
    {
        public override string Name => "Шулуун";

        protected override void Draw(Graphics g, Pen pen, Point a, Point b)
        {
            g.DrawLine(pen, a, b);
        }
    }
}
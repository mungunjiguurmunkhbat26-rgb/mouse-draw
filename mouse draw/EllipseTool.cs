using mouse_draw;
using System.Drawing;

namespace mouse_draw
{
    public class EllipseTool : ShapeTool
    {
        public override string Name => "Эллипс";

        protected override void Draw(Graphics g, Pen pen, Point a, Point b)
        {
            g.DrawEllipse(pen, GetRect(a, b));
        }
    }
}
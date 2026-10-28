using mouse_draw;
using System.Drawing;

namespace mouse_draw
{
    public class RectangleTool : ShapeTool
    {
        public override string Name => "Тэгш өнцөгт";

        protected override void Draw(Graphics g, Pen pen, Point a, Point b)
        {
            g.DrawRectangle(pen, GetRect(a, b));
        }
    }
}
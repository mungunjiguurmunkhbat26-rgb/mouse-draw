using System.Drawing;

namespace mouse_draw
{
    // Бүх хэрэгслийн хуваалцдаг мэдээлэл
    public class ToolContext
    {
        public Bitmap Canvas;
        public Pen Pen = new Pen(Color.Black, 2)
        {
            StartCap = System.Drawing.Drawing2D.LineCap.Round,
            EndCap = System.Drawing.Drawing2D.LineCap.Round,
            LineJoin = System.Drawing.Drawing2D.LineJoin.Round
        };
        public Color Color = Color.Black;
        public int EraserSize = 20;
    }

    // Бүх хэрэгслийн суурь класс
    public abstract class Tool
    {
        public abstract string Name { get; }

        public virtual void MouseDown(ToolContext ctx, Point pt) { }
        public virtual void MouseMove(ToolContext ctx, Point pt) { }
        public virtual void MouseUp(ToolContext ctx, Point pt) { }

        // Чирч байх үеийн урьдчилсан харагдац (дүрсүүдэд)
        public virtual void DrawPreview(Graphics g, ToolContext ctx) { }

        public override string ToString() => Name;
    }
}
namespace AttendEdgeMobileClient.CustomeControls
{
    public class CurvedNavBar : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float width = dirtyRect.Width;
            float height = dirtyRect.Height;

            float dipWidth = 80;
            float dipDepth = 30;
            float centerX = width / 2;

            canvas.FillColor = Colors.White;

            var path = new PathF();

            path.MoveTo(0, 0);
            path.LineTo(centerX - (dipWidth / 2), 0);

            // Simulate curved dip using 2 quad curves
            path.QuadTo(centerX - (dipWidth / 4), 0, centerX - (dipWidth / 4), dipDepth);
            path.QuadTo(centerX, dipDepth + 10, centerX + (dipWidth / 4), dipDepth);
            path.QuadTo(centerX + (dipWidth / 4), 0, centerX + (dipWidth / 2), 0);

            path.LineTo(width, 0);
            path.LineTo(width, height);
            path.LineTo(0, height);
            path.Close();

            canvas.FillPath(path);
        }
    }
}

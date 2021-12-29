using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Puzzle
{
    public partial class MainForm : Form
    {
        private GameTable game;
        private bool IsStart = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void StartToolMenuItem_Click(object sender, EventArgs e)
        {
            game = new GameTable(DefaultParametres.ElementsRGB,
                                 DefaultParametres.Level,
                                 DefaultParametres.SectorSize,
                                 DefaultParametres.TexturesRGB);
            GameBox.Image = game.Show();
            IsStart = true;
        }

        private void GameBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsStart)
            {
                Sector sector = game.GetSectorInPoint(e.Location);
                if (sector != null && game.SectorSelect(sector.TblLocation))
                    GameBox.Image = game.Show();
            }
        }

        private void GameBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsStart)
            {
                Sector sector = game.GetSectorInPoint(e.Location);
                if (sector != null)
                {
                    if (sector.HasElement)
                        sector.SelectElement();
                    else if (game.SelectedElement != null && game.SelectedElement.LocationSector.IsNeighbors(sector, out Sector.Direction? dir) && dir.HasValue && game.SelectedElement.LocationSector.CanGo[dir.Value])
                    {
                        game.SelectedElement.Move(sector);
                        GameBox.Image = game.Show();
                        if (game.WinCheck())
                        {
                            GameBox.Image = game.Show();
                            MessageBox.Show("You win!");
                            IsStart = false;
                        }
                    }
                }
            }
        }
    }

    public static class DefaultParametres
    {
        public static Dictionary<int, string> ElementsRGB => new Dictionary<int, string>
        {
            { 1, "Red" },
            { 2, "Green" },
            { 3, "Blue" }
        };
        public static (Sector.TypeSector sectorTypes, int elementsTypes)[,] Level =>
            new (Sector.TypeSector sectorTypes, int elementsTypes)[7, 5]
            {
                { (Sector.TypeSector.Indicator, 1), (Sector.TypeSector.Space, 0), (Sector.TypeSector.Indicator, 2), (Sector.TypeSector.Space, 0), (Sector.TypeSector.Indicator, 3) },
                { (Sector.TypeSector.Indicator, 0), (Sector.TypeSector.Space, 0), (Sector.TypeSector.Indicator, 0), (Sector.TypeSector.Space, 0), (Sector.TypeSector.Indicator, 0) },
                { (Sector.TypeSector.Empty, 2), (Sector.TypeSector.Block, 0), (Sector.TypeSector.Empty, 3), (Sector.TypeSector.Block, 0), (Sector.TypeSector.Empty, 1) },
                { (Sector.TypeSector.Empty, 2), (Sector.TypeSector.Empty, 0), (Sector.TypeSector.Empty, 3), (Sector.TypeSector.Empty, 0), (Sector.TypeSector.Empty, 1) },
                { (Sector.TypeSector.Empty, 2), (Sector.TypeSector.Block, 0), (Sector.TypeSector.Empty, 3), (Sector.TypeSector.Block, 0), (Sector.TypeSector.Empty, 1) },
                { (Sector.TypeSector.Empty, 2), (Sector.TypeSector.Empty, 0), (Sector.TypeSector.Empty, 3), (Sector.TypeSector.Empty, 0), (Sector.TypeSector.Empty, 1) },
                { (Sector.TypeSector.Empty, 2), (Sector.TypeSector.Block, 0), (Sector.TypeSector.Empty, 3), (Sector.TypeSector.Block, 0), (Sector.TypeSector.Empty, 1) }
            };
        public static Size SectorSize => new Size(60, 60);// 300, 420
        public static (Image background, Image selectSector, Image completeSector, Image emptySector, Image blockSector, Dictionary<string, Image>) TexturesRGB =>
            (Images.Square(new Size(SectorSize.Width * Level.GetLength(1), SectorSize.Height * Level.GetLength(0)), Color.White),
             Images.SquareInFrame(SectorSize, Color.Transparent, 3, Color.Aqua),
             Images.Star(SectorSize, Color.Gold),
             Images.Square(SectorSize, Color.LightGray),
             Images.SquareInFrame(SectorSize, Color.DarkGray, 5, Color.Black),
             new Dictionary<string, Image>
             {
                 { "Select", Images.SquareInFrame(SectorSize, Color.Transparent, 11, Color.DarkViolet) },
                 { "Red", Images.SquareInFrame(SectorSize, Color.Red, 1, Color.Black) },
                 { "Green", Images.SquareInFrame(SectorSize, Color.Green, 1, Color.Black) },
                 { "Blue", Images.SquareInFrame(SectorSize, Color.Blue, 1, Color.Black) }
             });
    }

    public static class Images
    {
        public static Image Square(Size size, Color color)
        {
            Image img = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(img))
            {
                g.Clear(color);
            }
            return img;
        }
        public static Image SquareInFrame(Size size, Color squareColor, int width, Color frameColor)
        {
            Image img = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(img))
            {
                g.Clear(squareColor);
                g.DrawLines(new Pen(frameColor, width + 1), new Point[4] { new Point(0, 0), new Point(0, size.Height), new Point(size.Width, size.Height), new Point(size.Width, 0) });
            }
            return img;

        }
        public static Image Star(Size size, Color color)
        {
            Image img = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(img))
            {
                double Ф = (Math.Sqrt(5) + 1) / 2;
                int n = 5;               // число вершин

                double R = (size.Width * Ф) / (2 * Ф + 1) * Math.Sin(34.0 / 45.0 * Math.PI) / Math.Sin(Math.PI/5),
                       r = R * (2 - Ф);   // радиусы
                double x0 = size.Width / 2, y0 = size.Height / 2; // центр

                PointF[] points = new PointF[2 * n + 1];

                double a = 3 * Math.PI / 2, da = Math.PI / n, l;
                for (int k = 0; k < 2 * n + 1; k++)
                {
                    l = k % 2 == 0 ? R : r;
                    points[k] = new PointF((float)(x0 + l * Math.Cos(a)), (float)(y0 + l * Math.Sin(a)));
                    a += da;
                }

                g.FillPolygon(new SolidBrush(color), points);
            }
            return img;
        }
    }
}

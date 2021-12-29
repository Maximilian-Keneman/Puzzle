using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzle
{
    public class Sector
    {
        public enum TypeSector
        {
            Empty,
            Space,
            Block,
            Indicator
        }
        public TypeSector Type { get; }

        private GameTable Owner;
        public Point TblLocation;

        public enum Direction
        {
            Up,
            Right,
            Down,
            Left
        }

        private Dictionary<Direction, Sector> Neighbors => new Dictionary<Direction, Sector>
        {
            { Direction.Up, Owner[TblLocation.X - 1, TblLocation.Y] },
            { Direction.Right, Owner[TblLocation.X, TblLocation.Y + 1] },
            { Direction.Down, Owner[TblLocation.X + 1, TblLocation.Y] },
            { Direction.Left, Owner[TblLocation.X, TblLocation.Y - 1] }
        };
        public bool IsNeighbors(Sector sector, out Direction? direction)
        {
            foreach (var neighbor in Neighbors)
            {
                if (neighbor.Value == sector)
                {
                    direction = neighbor.Key;
                    return true;
                }
            }
            direction = null;
            return false;
        }

        private bool Access => Type == TypeSector.Empty && Element == null;
        public Dictionary<Direction, bool> CanGo => new Dictionary<Direction, bool>
        {
            { Direction.Up, Neighbors[Direction.Up] != null && Neighbors[Direction.Up].Access },
            { Direction.Right, Neighbors[Direction.Right] != null && Neighbors[Direction.Right].Access },
            { Direction.Down, Neighbors[Direction.Down] != null && Neighbors[Direction.Down].Access },
            { Direction.Left, Neighbors[Direction.Left] != null && Neighbors[Direction.Left].Access }
        };
        public bool IsBlocked => !CanGo.ContainsValue(true);

        private Element Element = null;
        public bool Selected = false;
        public bool Complete = false;

        public Image Texture { get; }
        public Image SelectTexture { get; }
        public Image CompleteTexture { get; }
        public Size Size { get; }
        public Point ImgLocation { get; }

        public Sector(TypeSector type, Image texture, Image selectTexture, Image completeTexture, Size size, GameTable owner, Point location)
        {
            Type = type;
            Texture = texture;
            SelectTexture = selectTexture;
            CompleteTexture = completeTexture;
            Size = size;
            Owner = owner;
            TblLocation = location;
            int Y = Neighbors[Direction.Left] != null ? Neighbors[Direction.Left].ImgLocation.Y + Neighbors[Direction.Left].Size.Width : 0;
            int X = Neighbors[Direction.Up] != null ? Neighbors[Direction.Up].ImgLocation.X + Neighbors[Direction.Up].Size.Height : 0;
            ImgLocation = new Point(X, Y);
        }

        public bool AddElement(Element element)
        {
            if ((Type == TypeSector.Empty || Type == TypeSector.Indicator) && Element == null)
            {
                element.LocationSector = this;
                Element = element;
                return true;
            }
            else
                return false;
        }
        public void RemoveElement()
        {
            Element = null;
        }
        public bool HasElement => Element != null;
        public string ElementType => Element?.Type;
        public void SelectElement()
        {
            if (!IsBlocked)
            {
                Owner.ElementsDeselect();
                Element.Selected = true;
            }
        }
    }

    public class Element
    {
        private GameTable Owner;
        public Sector LocationSector;
        public string Type { get; }
        private Image MainTexture;
        private Image SelectTexture;
        public Image Texture
        {
            get
            {
                if (Selected)
                {
                    Image img = (Image)MainTexture.Clone();
                    using (Graphics g = Graphics.FromImage(img))
                        g.DrawImage(SelectTexture, new Point(0, 0));
                    return img;
                }
                else
                    return MainTexture;
            }
        }
        public bool Selected = false;

        public Element(string type, Image mainTexture, Image selectTexture, GameTable owner)
        {
            Type = type;
            MainTexture = mainTexture;
            SelectTexture = selectTexture;
            Owner = owner;
        }

        public bool Move(Sector sector)
        {
            Sector ownSector = LocationSector;
            ownSector.RemoveElement();
            if (sector.AddElement(this))
            {
                return true;
            }
            else
            {
                ownSector.AddElement(this);
                return false;
            }
        }
    }

    public class GameTable
    {
        private Sector[,] Sectors;
        private (string type, Sector[] indicators)[] Indicators;
        public Sector this[int x, int y]
        {
            get
            {
                try
                {
                    return Sectors[x, y];
                }
                catch (IndexOutOfRangeException)
                {
                    return null;
                }
            }
        }
        public Size Size => new Size(Sectors.GetLength(0), Sectors.GetLength(1));
        public Point ImgLocation { get; }

        private Element[] Elements;
        public Element SelectedElement => Elements.SingleOrDefault(E => E.Selected);

        public GameTable(Dictionary<int, string> elementsTypes, (Sector.TypeSector sectorType, int elementType)[,] level, Size sectorSize,
                         (Image background, Image selectSector, Image completeSector, Dictionary<string, Image> elements) textures)
        {
            FillSectors(elementsTypes, level, sectorSize, (textures.selectSector, textures.completeSector, null, null, textures.elements));
            Background = textures.background;
        }
        public GameTable(Dictionary<int, string> elementsTypes, (Sector.TypeSector sectorType, int elementType)[,] level, Size sectorSize,
                         (Image background, Image selectSector, Image completeSector, Image emptySector, Image blockSector, Dictionary<string, Image> elements) textures)
        {
            FillSectors(elementsTypes, level, sectorSize, (textures.selectSector, textures.completeSector, textures.emptySector, textures.blockSector, textures.elements));
            Background = CreateBackground(textures.background);
        }
        private void FillSectors(Dictionary<int, string> elementsTypes, (Sector.TypeSector sectorType, int elementType)[,] level, Size sectorSize,
                                (Image selectSector, Image completeSector, Image emptySector, Image blockSector, Dictionary<string, Image> elements) textures)
        {
            Sectors = new Sector[level.GetLength(1), level.GetLength(0)];
            var sectorTextures = new Dictionary<Sector.TypeSector, Image>
            {
                { Sector.TypeSector.Empty, textures.emptySector },
                { Sector.TypeSector.Block, textures.blockSector },
                { Sector.TypeSector.Space, null },
                { Sector.TypeSector.Indicator, null }
            };
            List<Element> elements = new List<Element>();
            List<Sector> indicators = new List<Sector>();
            for (int x = 0; x < Size.Width; x++)
                for (int y = 0; y < Size.Height; y++)
                {
                    var (sectorType, elementType) = level[y, x];
                    Sectors[x, y] = new Sector(sectorType, sectorTextures[sectorType], textures.selectSector, textures.completeSector, sectorSize, this, new Point(x, y));
                    if (Sectors[x, y].Type == Sector.TypeSector.Indicator)
                        indicators.Add(Sectors[x, y]);
                    if (elementType > 0)
                    {
                        Element element = new Element(elementsTypes[elementType], textures.elements[elementsTypes[elementType]], textures.elements["Select"], this);
                        if (Sectors[x, y].AddElement(element))
                            elements.Add(element);
                    }
                }
            Elements = elements.ToArray();
            List<(string, Sector[])> duoindicators = new List<(string, Sector[])>();
            for (int i = 1; i < indicators.Count; i++)
            {
                if (indicators[i].IsNeighbors(indicators[0], out Sector.Direction? dir))
                {
                    string type = indicators[0].HasElement ? indicators[0].ElementType : indicators[i].ElementType;
                    if (dir == Sector.Direction.Up || dir == Sector.Direction.Down)
                    {
                        List<Sector> sectors = new List<Sector>();
                        int y = indicators[0].TblLocation.Y;
                        for (int x = 0; x < Size.Width; x++)
                            sectors.Add(Sectors[x, y]);
                        duoindicators.Add((type, sectors.ToArray()));
                    }
                    else if (dir == Sector.Direction.Left || dir == Sector.Direction.Right)
                    {
                        List<Sector> sectors = new List<Sector>();
                        int x = indicators[0].TblLocation.X;
                        for (int y = 0; y < Size.Height; y++)
                            sectors.Add(Sectors[x, y]);
                        duoindicators.Add((type, sectors.ToArray()));
                    }
                    indicators.RemoveAt(i);
                    indicators.RemoveAt(0);
                    i = 0;
                }
            }
            Indicators = duoindicators.ToArray();
        }

        public Image Background { get; private set; }
        private Image CreateBackground(Image backbackground)
        {
            using (Graphics g = Graphics.FromImage(backbackground))
            {
                for (int x = 0; x < Size.Width; x++)
                    for (int y = 0; y < Size.Height; y++)
                    {
                        if (Sectors[x,y].Type != Sector.TypeSector.Space && Sectors[x, y].Type != Sector.TypeSector.Indicator)
                        g.DrawImage(Sectors[x,y].Texture, ImgLocation + (Size)Sectors[x,y].ImgLocation);
                    }
            }
            return backbackground;
        }

        public Image Show()
        {
            Image img = (Image)Background.Clone();
            using (Graphics g = Graphics.FromImage(img))
            {
                foreach (var element in Elements)
                {
                    g.DrawImage(element.Texture, ImgLocation + (Size)element.LocationSector.ImgLocation);
                }
                for (int x = 0; x < Size.Width; x++)
                    for (int y = 0; y < Size.Height; y++)
                    {
                        Sector sector = Sectors[x, y];
                        if (sector.Type == Sector.TypeSector.Empty)
                        {
                            if (sector.Selected)// && (SelectedElement == null ? !sector.IsBlocked : SelectedElement.LocationSector.IsNeighbors(sector, out _)))
                                g.DrawImage(sector.SelectTexture, ImgLocation + (Size)sector.ImgLocation);
                            if (sector.Complete)
                                g.DrawImage(sector.CompleteTexture, ImgLocation + (Size)sector.ImgLocation);
                        }
                    }
            }
            return img;
        }

        public bool SectorSelect(Point point)
        {
            for (int x = 0; x < Size.Width; x++)
                for (int y = 0; y < Size.Height; y++)
                {
                    if (Sectors[x, y].Selected)
                    {
                        if (new Point(x, y) == point)
                            return false;
                        else
                            Sectors[x, y].Selected = false;
                    }
                }
            Sectors[point.X, point.Y].Selected = true;
            return true;
        }
        public Sector GetSectorInPoint(Point point)
        {
            int X = -1, Y = -1;
            for (int x = 0; x < Size.Width; x++)
            {
                if (ImgLocation.X + Sectors[x, 0].ImgLocation.X < point.X && ImgLocation.X + Sectors[x, 0].ImgLocation.X + Sectors[x, 0].Size.Height > point.X)
                {
                    X = x;
                    break;
                }
            }
            for (int y = 0; y < Size.Height; y++)
            {
                if (ImgLocation.Y + Sectors[0, y].ImgLocation.Y < point.Y && ImgLocation.Y + Sectors[0, y].ImgLocation.Y + Sectors[0, y].Size.Width > point.Y)
                {
                    Y = y;
                    break;
                }
            }
            if (X != -1 && Y != -1)
                return Sectors[X, Y];
            else
                return null;
        }

        public void ElementsDeselect()
        {
            if (SelectedElement != null)
                SelectedElement.Selected = false;
        }

        public bool WinCheck()
        {
            var rows = Indicators.Select(I => (I.type, sectors: I.indicators.Where(S => S.Type == Sector.TypeSector.Empty)
                                                                             .Select(S => (Sector: S, S.ElementType))
                                                                             .ToList()));
            bool complete = true;
            foreach (var (type, sectors) in rows)
            {
                if (sectors.TrueForAll(S => S.ElementType == type))
                    foreach (var item in sectors)
                    {
                        item.Sector.Complete = true;
                    }
                else
                {
                    foreach (var item in sectors)
                    {
                        item.Sector.Complete = false;
                    }
                    complete = false;
                }
            }
            return complete;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Resources;

namespace FrameMarker
{
    public partial class frmFrameMarker : Form
    {
        string FormName = "Frame Marker";

        //////////////////////////////////////////////////////////////////////////////////////                                
        static Editor Ed;

        //////////////////////////////////////////////////////////////////////////////////////                                        
        ScreenBox selectedNode;
        Marker selectedMarker;
        Link selectedLink;

        bool moving;
        int sx, sy, ex, ey;
        static int mousex, mousey;
        bool selecting;

        //////////////////////////////////////////////////////////////////////////////////////                                
        public frmFrameMarker()
        {
            InitializeComponent();                                                
            panelSentences.SetDoubleBuffered();

            Ed = CreateEditor();
            CreateMissingFrameMarkers();            
            RefreshUI();
            SetTitle();            
        }
        //////////////////////////////////////////////////////////////////////////////////////                                        
        static Color GetColor(int idx)
        {
            return HighContrastColors.BoyntonOptimized[idx % HighContrastColors.BoyntonOptimized.Count];
        }
        //////////////////////////////////////////////////////////////////////////////////////                                        

        static Font text_font = new Font(FontFamily.GenericSansSerif, 10);
        static Font small_font = new Font("Courier New", 8);
        static Font small_font_bold = new Font("Courier New", 8, FontStyle.Bold);

        // Attālums starp kastēm koka attēlojumā 
        static int XSpacing = 20; 
        static int YSpacing = 20; 
        //////////////////////////////////////////////////////////////////////////////////////                                                                       
        string GetLastOpenedFile()
        {
            if(File.Exists("lastopened"))
            {
                return File.ReadAllText("lastopened");
            }
            return null;
        }

        void StoreLastOpenedFile(string filename)
        {
            try
            {
                File.WriteAllText("lastopened", filename);
            }
            catch (Exception)
            {
                
            }            
        }

        // Vārdu ietverošās kastes augstums
        int GetWordBoxHeight()
        {
            int height = 20;

            if (tsmShowLemmas.Checked)
                height += 12;

            if (tsmShowMorph.Checked)
                height += 22;

            return height;
        }

        void PaintControl(Graphics g)
        {
            var fmt = new StringFormat(StringFormatFlags.FitBlackBox);
            fmt.Alignment = StringAlignment.Center;
            fmt.LineAlignment = StringAlignment.Center;

            var nodes = GetScreenNodes();            
            var size = nodes.FirstOrDefault(o => o.Level == 0).Rect.Size;
            var bottom = nodes.Max(o => o.Rect.Y + o.Rect.Height);
            markerControl.AutoScrollMinSize = new Size(size.Width + XSpacing * 2, bottom + YSpacing * 2);
                        
            // Attēlo visas grafiskos elementus - tainsstūru kastes - teikuma koku
            foreach(var node in nodes.OfType<ScreenTreeBox>())
            {
                if(!node.IsMainWord)
                {
                    var selNode = selectedNode as ScreenTreeBox;
                    var isSelected = selNode != null && selNode.Node.Word == node.Node.Word;
                    var color = GetColor(node.Level);
                    g.FillRectangle(Brushes.White, node.Rect);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(isSelected? 60: 30, color)), node.Rect);
                                        
                    var pen = new Pen(Color.Black, isSelected ? 2 : 1);
                    if(node.Node.Word.IsConstituent)
                    {
                        pen.DashPattern = new []{8.0f, 8.0f};
                    }

                    g.DrawRectangle(pen, node.Rect);
                }
                                
                if(!node.IsContainer)
                {
                    var midx = node.Center.X;
                    var midy = node.Center.Y;
                    int height = GetWordBoxHeight();

                    var rect = new Rectangle(node.Rect.Location.X + 10, midy - height/2, node.Rect.Width - 10 * 2, height);
                    int offy = midy - height / 2 + 10;
                    
                    g.FillRectangle(Brushes.White, rect);
                    g.DrawRectangle(Pens.Gray, rect);
                    g.DrawString(node.Node.Word.Original, text_font, Brushes.Black, midx, offy, fmt);                    
                    offy += 14;

                    if (tsmShowLemmas.Checked)
                    {
                        g.DrawString(node.Node.Word.Lemma, small_font, Brushes.Gray, midx, offy, fmt);
                        offy += 12;
                    }
                                            
                    if (tsmShowMorph.Checked)
                    {
                        g.DrawString(node.Node.Word.Morph1, small_font, Brushes.Gray, midx, offy, fmt);
                        offy += 10;
                        g.DrawString(node.Node.Word.Morph2, small_font, Brushes.Gray, midx, offy, fmt);
                        offy += 10;
                    }                                        
                }                
            }

            
            // Attēlo visus teikuma freimu marķierus
            foreach (var marker in GetSelectedSentence().GetFrameMarkers())
            {
                var refNodes = nodes.OfType<ScreenNamedEntityBox>()
                    .Where(o => marker.FrameInstance.TargetID == o.NamedEntity.ID && o.Word.Index == marker.FrameInstance.WordIndex);
                
                foreach(var node in refNodes)
                {
                    g.DrawLine(new Pen(Brushes.DarkCyan, 3), node.Center, marker.Location);    
                }                                            
            }

            // Attēlo visas saites
            var links = GetLinks(nodes);
                        
            foreach(var link in links)
            {
                bool selected = selectedLink != null
                    && link.ElementReference.Equals(selectedLink.ElementReference)
                    && link.FrameInstanceMarker.FrameInstance.Equals(selectedLink.FrameInstanceMarker.FrameInstance);

                link.Draw(g, selected);
            }
            
            // Attēlo visas kokam pievienotās ID kastes
            foreach (var node in nodes.OfType<ScreenNamedEntityBox>())
            {                
                g.FillRectangle(node.NamedEntity.Frames.Count > 0? Brushes.DarkCyan: Brushes.BlueViolet, node.Rect);                
                g.DrawString("ID["+node.NamedEntity.ID+"]", small_font_bold, Brushes.White, node.Center, fmt);
            }
                        
            
            // Attēlo visus pārvietojamos objektus - named entity marķierus
            foreach(var node in nodes.OfType<MovableEntity>().Where(o => o.Marker is NamedEntityMarker))
            {
                var neMarker = (node.Marker as NamedEntityMarker);
                var selFeMarker = (selectedMarker as NamedEntityMarker);
                bool isSelected = selFeMarker != null && selFeMarker.NamedEntity == neMarker.NamedEntity;

                g.FillRectangle(Brushes.BlueViolet, node.Rect);
                g.DrawRectangle(new Pen(Brushes.Black, isSelected ? 2: 1), node.Rect);
                var bold = new Font(text_font, FontStyle.Bold);

                g.DrawString("ID["+neMarker.NamedEntity.ID + "]: " + neMarker.NamedEntity.Name, isSelected ? bold : text_font, Brushes.White, node.Rect, fmt);                
            }

            // Attēlo visus pārvietojamos objektus - freimu marķierus
            foreach (var node in nodes.OfType<MovableEntity>().Where(o => o.Marker is FrameInstanceMarker))
            {
                var feMarker = (node.Marker as FrameInstanceMarker);
                var selFeMarker = (selectedMarker as FrameInstanceMarker);
                bool isSelected = selFeMarker != null && selFeMarker.FrameInstance == feMarker.FrameInstance;
                                
                g.FillRectangle(Brushes.DarkCyan, node.Rect);
                g.DrawRectangle(new Pen(Brushes.Black, isSelected ? 2 : 1), node.Rect);
                var bold = new Font(text_font, FontStyle.Bold);
                                
                var rect = new RectangleF(node.Rect.X, node.Rect.Y - 10, node.Rect.Width, node.Rect.Height);
                g.DrawString("EV[" + feMarker.FrameInstance.TargetID + "]: " + feMarker.FrameInstance.Frame.Name, isSelected ? bold : text_font, Brushes.White, rect, fmt);                
            }
            
        }

        // Saites klase - sasaista freima instances freima elementu ar noteiktu entīti
        class Link
        {
            
            public Link(FrameInstanceMarker feMarker, ScreenBox box, KeyValuePair<FrameElement, Reference> reference, int linkBoxWidth)
            {
                FrameInstanceMarker = feMarker;
                DestBox = box;
                ElementReference = reference;
                LinkBoxWidth = linkBoxWidth;
            }

            
            public void Draw(Graphics g, bool selected)
            {
                var fmt = new StringFormat(StringFormatFlags.FitBlackBox);
                fmt.Alignment = StringAlignment.Center;
                fmt.LineAlignment = StringAlignment.Center;

                bool isFrameRef = Ed.Doc.NamedEntities[ElementReference.Value.ID].Frames.Count > 0;
                var brush = isFrameRef ? Brushes.DarkCyan : Brushes.BlueViolet;
                g.DrawLine(new Pen(brush, selected?4:2), DestBox.Center, FrameInstanceMarker.Location);

                var rect = GetMiddleBoxRect();
                var midx = (DestBox.Center.X + FrameInstanceMarker.Location.X) / 2;
                var midy = (DestBox.Center.Y + FrameInstanceMarker.Location.Y) / 2;

                g.FillRectangle(brush, rect);
                g.DrawString(ElementReference.Key.Name, selected? new Font(text_font, FontStyle.Bold): text_font, Brushes.White, midx, midy, fmt);                                
            }
            
            public Rectangle GetMiddleBoxRect()
            {
                var midx = (DestBox.Center.X + FrameInstanceMarker.Location.X) / 2;
                var midy = (DestBox.Center.Y + FrameInstanceMarker.Location.Y) / 2;

                return new Rectangle(midx - LinkBoxWidth / 2, midy - 10, LinkBoxWidth, 20);
            }

            // Freima instances marķieris no kura vilkt saiti uz entīti
            public FrameInstanceMarker FrameInstanceMarker;
            
            // Grafiskais elements, kurš apzīmē saistīto entīti
            public ScreenBox DestBox;

            // Norāda, kurš freima elementu apraksta šī saite kā arī saistīto entīti
            public KeyValuePair<FrameElement, Reference> ElementReference;
            
            // Saites apraksta kastes izmērs
            public int LinkBoxWidth;            
        }

        // Atgriež sarakstu ar visām dotajā teikuma saitēm
        List<Link> GetLinks(List<ScreenBox> nodes)
        {
            var links = new List<Link>();
            var sent = GetSelectedSentence();
                        
            foreach (var marker in sent.GetFrameMarkers())
            {                
                foreach (var reference in marker.FrameInstance.ElementReferences)
                {
                    var width = TextRenderer.MeasureText(reference.Key.Name, text_font).Width;

                    var namedNodes = nodes.OfType<ScreenNamedEntityBox>()
                        .Where(o => o.NamedEntity.ID == reference.Value.ID && o.Word.Index == reference.Value.WordIndex);

                    foreach (var namedNode in namedNodes)
                    {
                        links.Add(new Link(marker, namedNode, reference, width));
                    }

                    var frameMovableNodes = nodes.OfType<MovableEntity>()
                        .Where(o => o.Marker is FrameInstanceMarker
                            && (o.Marker as FrameInstanceMarker).FrameInstance.TargetID == reference.Value.ID);

                    foreach (var frameMovableNode in frameMovableNodes)
                    {
                        if (!sent.GetFrames().Contains((frameMovableNode.Marker as FrameInstanceMarker).FrameInstance))
                        {
                            links.Add(new Link(marker, frameMovableNode, reference, width));    
                        }
                    }
                    
                    var namedMovableNodes = nodes.OfType<MovableEntity>()
                        .Where(o => o.Marker is NamedEntityMarker
                            && (o.Marker as NamedEntityMarker).NamedEntity.ID == reference.Value.ID);

                    foreach (var namedMovableNode in namedMovableNodes)
                    {
                        links.Add(new Link(marker, namedMovableNode, reference, width));
                    }                     
                }
            }
            
            return links;
        }
        
        // Atgriež tukšas vārda kastes augstumu
        int GetEmptyBoxHeight()
        {
            return 30 + GetWordBoxHeight();
        }
        
        // Atgriež sarakstu ar grafiskajiem elementiem - taisnstūriem
        List<ScreenBox> GetScreenNodes()
        {
            return GetScreenNodes(GetSelectedSentence());
        }

        List<ScreenBox> GetScreenNodes(Sentence sent)
        {
            var rootWord = sent.Words.First(o => o.ParentIndex <= 0); // *** pagaidām nav viennozīmīgas skaidrības, conll paredz 0
            //var rootWord = sent.Words.First(o => o.ParentIndex == -1);
            var rootNode = new TreeNode(rootWord, sent.Words);
            var nodes = new List<ScreenBox>();
            GetScreenNodes(rootNode, rootNode.GetSize(GetEmptyBoxHeight()).Height, XSpacing, YSpacing, nodes, 0);
            var treeNodes = nodes.OfType<ScreenTreeBox>();

            
            // Pievieno mazās ID kastes katram anotētajam named entity
            foreach (var pair in sent.WordToNamedEntityMap)
            {
                var node = treeNodes.First(o => o.Node.Word == pair.Key && (o.IsMainWord || o.Node.Kids.Count == 0));
                var box = new ScreenNamedEntityBox { Word = node.Node.Word, NamedEntity = pair.Value };

                var prect = node.Rect;
                box.Rect = new Rectangle(prect.X + prect.Width / 2 - 30, prect.Y + prect.Height - 17, 60, 14);
                if (node.IsMainWord)
                    box.Rect.Y += YSpacing;
                box.Level = node.Level + 1;
                nodes.Add(box);
            }
                                                
            // Pievieno sarakstu ar freimu marķieru kastēm
            foreach (var marker in sent.GetFrameMarkers())
            {
                var width = TextRenderer.MeasureText(marker.FrameInstance.Frame.Name, text_font).Width + 60;
                var rect = new Rectangle(marker.Location.X - width / 2, marker.Location.Y - 20, width, 40);
                var box = new MovableEntity { Level = 100, Marker = marker, Rect = rect };
                nodes.Add(box);
            }
 
            // Pievieno sarakstu ar named entity marķieru kastēm
            foreach (var marker in sent.NamedEntityMarkers)
            {
                var width = TextRenderer.MeasureText(marker.NamedEntity.Name, text_font).Width + 60;
                var rect = new Rectangle(marker.Location.X - width / 2, marker.Location.Y - 20, width, 40);
                var box = new MovableEntity { Level = 100, Marker = marker, Rect = rect };
                nodes.Add(box);
            }
            
            return nodes;
        }
                                                        
        // Atgriež koka mezgla izkārtojuma kārtību
        int GetOrder(TreeNode e, TreeNode root)
        {
            var allNodes = e.GetAllKids(); 
            allNodes.Add(e);

            if (e != root)
            {
                return allNodes.Select(o => o.Word.Index).Min();                
            }                
            else
            {
                return e.Word.Index;            
            }                
        }
        
        // Rekursīvi pievieno screenElements sarakstam grafiskos elementus, kas veido grafisko teikuma koku
        // root - koka saknes mezgls
        // parentHeight - vecāka grafiskā elementa augstums
        // sx, sy - dotā sākuma pozīcija
        // level - koka dziļums
        void GetScreenNodes(TreeNode root, int parentHeight, int sx, int sy, List<ScreenBox> screenElements, int level)
        {            
            var snode = new ScreenTreeBox();
            snode.Node = root;
            snode.Level = level;
            screenElements.Add(snode);            

            int offx = 0;
            
            var orderedKids = root.Kids.ToList();
            orderedKids.Add(root);
            orderedKids = orderedKids.OrderBy(o => GetOrder(o, root)).ToList();
                                    
            foreach(var kid in orderedKids)
            {                
                if(kid != root)
                {
                    GetScreenNodes(kid, parentHeight - YSpacing * 2, sx + offx + XSpacing, sy + YSpacing, screenElements, level + 1);
                    offx += kid.GetSize(GetEmptyBoxHeight()).Width + XSpacing;
                }
                else if(root.Kids.Count != 0)
                {
                    var rootNodeWidth = root.GetWordboxSize();
                    var innerNode = new ScreenTreeBox();
                    innerNode.Node = root;
                    innerNode.Rect = new Rectangle(sx + offx + XSpacing, sy + YSpacing, rootNodeWidth, parentHeight - YSpacing * 2);
                    innerNode.IsMainWord = true;
                    innerNode.Level = level;
                    
                    offx += rootNodeWidth + XSpacing;
                    screenElements.Add(innerNode);                    
                }                                                
            }

            var rootSize = root.GetSize(GetEmptyBoxHeight());
            snode.Rect = new Rectangle(new Point(sx, sy), new Size(rootSize.Width, parentHeight));

            if(root.Kids.Count > 0)
            {
                snode.IsContainer = true;                                
            }            
        }

        // Bāzes klase grafiskajam elementam
        public class ScreenBox
        {
            public Rectangle Rect;
            public int Level;
            public Point Center
            {
                get { return new Point(Rect.X + Rect.Width/2, Rect.Y + Rect.Height/2); }
            }
        }
        
        // Grafiskais elements - teikuma koka kaste
        public class ScreenTreeBox:ScreenBox
        {            
            public TreeNode Node;
            public bool IsContainer;
            public bool IsMainWord;            
        }

        // Grafiskais elements - nosauktās entītes ID kaste, kas pievienojas klāt teikuma kokam/vārdam
        public class ScreenNamedEntityBox:ScreenBox
        {
            public NamedEntity NamedEntity;
            public Word Word;
        }

        // Grafiskais elements - freima entītes ID kaste, kas pievienojas klāt teikuma kokam/vārdam
        public class ScreenFrameEntityBox:ScreenBox
        {
            public FrameInstance FrameInstance;
            public Word Word;
        }

        // Grafiskas elements - pārvietojams elements
        public class MovableEntity: ScreenBox
        {
            public Marker Marker;
        }
        
        // Koka mezgla klase, kas tiek izveidota no vārdu saraksta
        public class TreeNode
        {
            public TreeNode(Word word, List<Word> words)
            {
                Word = word;
                Kids = words.Where(o => o.ParentIndex == word.Index).Select(o => new TreeNode(o, words)).ToList();
            }
            public Word Word;
            public List<TreeNode> Kids = new List<TreeNode>();

            // Atgriež šī koka mezgla grafiskā izkārtojuma izmēru
            public Size GetSize(int emptyHeight)
            {
                var width = GetWordboxSize();
                if(Kids.Count == 0)
                {                    
                    return new Size(width, emptyHeight);
                }
                else
                {
                    var sum = new Size(XSpacing * 2, YSpacing * 2);
                    
                    foreach(var kid in Kids)
                    {
                        var size = kid.GetSize(emptyHeight);
                        sum.Width += size.Width + XSpacing;
                        sum.Height = Math.Max(size.Height + YSpacing*2, sum.Height);
                    }
                    
                    return sum + new Size(width, 0);
                }
            }

            // Atgriež šī koka mezgla visus bērnus (rekursīvs izsaukums)
            public List<TreeNode> GetAllKids()
            {
                var allKids = new List<TreeNode>();
                foreach (var kid in Kids)
                {
                    if (kid.Kids.Count > 0)
                    {
                        allKids.AddRange(kid.GetAllKids());
                    }

                    allKids.Add(kid);
                }
                return allKids;
            }

            // Atgriež šī koka mezgla vārda kastes grafiskā elementa izmēru
            public int GetWordboxSize()
            {
                var origLen = TextRenderer.MeasureText(Word.Original, text_font).Width;
                var morph1Len = TextRenderer.MeasureText(Word.Morph1, small_font).Width;
                var morph2Len = TextRenderer.MeasureText(Word.Morph2, small_font).Width;
                var rootNodeWidth = Math.Max(origLen, Math.Max(morph1Len, morph2Len)) + XSpacing * 2;
                return rootNodeWidth;
            }
        }

        // Atgriež aktīvo teikumu
        Sentence GetSelectedSentence()
        {                             
            return Ed.Doc.Sentences[Ed.SelectedSentenceIndex];            
        }
       
        private void markerControl_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TranslateTransform(markerControl.AutoScrollPosition.X, markerControl.AutoScrollPosition.Y);
            {                
                PaintControl(g);
            }                                                                            
        }        
                                                                                          
        private void cbFrames_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMarker = null;
            RefreshFrameElementsList();
        }
        
        void RefreshFrameElementsList()
        {            
            if(selectedMarker != null && selectedMarker is FrameInstanceMarker)
            {
                RefreshFrameElementsList((selectedMarker as FrameInstanceMarker).FrameInstance);
            }
            else
            {                
                if(cbFrames.SelectedItem != null)
                {
                    var frame = cbFrames.SelectedItem as Frame;
                    lbFrameElements.Items.Clear();
                    lbFrameElements.Items.AddRange(frame.Elements.Where(o => o.Type == "Core").ToArray());

                    lbOtherElements.Items.Clear();
                    lbOtherElements.Items.AddRange(frame.Elements.Where(o => o.Type != "Core").ToArray());    
                }
                
            }            
        }
        
        void RefreshFrameElementsList(FrameInstance frameInstance)
        {
            var unusedCoreElemensList = frameInstance.Frame.Elements
                .Where(o => o.Type == "Core")
                .Except(frameInstance.ElementReferences.Keys).ToArray();

            var unusedOtherElemensList = frameInstance.Frame.Elements
                .Where(o => o.Type != "Core")
                .Except(frameInstance.ElementReferences.Keys).ToArray();
            
            lbFrameElements.Items.Clear();
            lbFrameElements.Items.AddRange(unusedCoreElemensList);

            lbOtherElements.Items.Clear();
            lbOtherElements.Items.AddRange(unusedOtherElemensList);                

        }
                       
        void SetSelectedFrameMarker(FrameInstanceMarker frameMarker)
        {
            cbFrames.SelectedItem = frameMarker.FrameInstance.Frame;
            RefreshFrameElementsList(frameMarker.FrameInstance);
            selectedMarker = frameMarker;
        }
        
        private void markerControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                sx = e.Location.X - markerControl.AutoScrollPosition.X;
                sy = e.Location.Y - markerControl.AutoScrollPosition.Y;
                ex = sx;
                ey = sy;

                var nodes = GetScreenNodes();

                selectedLink = null;
                selectedMarker = null;
                selectedNode = PickSelectedNode(sx, sy, nodes);
                
                if (selectedNode != null)
                {
                    if(selectedNode is MovableEntity)
                    {
                        selectedMarker = (selectedNode as MovableEntity).Marker;
                        if (selectedMarker != null)
                        {
                            if(e.Button == MouseButtons.Left)
                            {
                                moving = true;
                                Ed.MoveStartPos = selectedMarker.Location;    
                            }                            
                                                        
                            var frameInstMarker = selectedMarker as FrameInstanceMarker;

                            if (frameInstMarker != null)
                            {
                                SetSelectedFrameMarker(frameInstMarker);
                            }
                            
                        }    
                    }

                    if (e.Button == MouseButtons.Left)
                    {
                        if (selectedNode is ScreenNamedEntityBox)
                        {
                            DoDragDrop(selectedNode, DragDropEffects.All);
                        }

                        if (selectedNode is ScreenFrameEntityBox)
                        {
                            DoDragDrop(selectedNode, DragDropEffects.All);
                        }    
                    }
                    
                    if(e.Button == MouseButtons.Right)
                    {
                        if (selectedNode is ScreenTreeBox)
                        {
                            var treebox = selectedNode as ScreenTreeBox;
                            markerControl.Invalidate();
                                                        
                            if (GetSelectedSentence().WordToNamedEntityMap.ContainsKey(treebox.Node.Word))
                            {                                
                                tsmDeleteEntity.Enabled = true;
                                tsmDeleteEntityInstance.Enabled = true;
                                tsmCreateNamedEntity.Enabled = false;                             
                            }
                            else
                            {                            
                                tsmDeleteEntity.Enabled = false;
                                tsmDeleteEntityInstance.Enabled = false;
                                tsmCreateNamedEntity.Enabled = true;                                
                            }
                            
                            cmTreeNode.Show(markerControl, e.Location);
                        }
                        else
                        {
                            var frameInstMarker = selectedMarker as FrameInstanceMarker;
                            if (frameInstMarker != null)
                            {
                                DoDragDrop(frameInstMarker.FrameInstance, DragDropEffects.All);    
                            }

                            var namedEntityMarker = selectedMarker as NamedEntityMarker;
                            if (namedEntityMarker != null)
                            {
                                DoDragDrop(namedEntityMarker, DragDropEffects.All);
                            }                            
                        }
                    }                                        
                }
                
                if(selectedNode == null)
                {
                    var links = GetLinks(nodes);
                    selectedLink = PickSelectedLink(sx, sy, links);
                    
                    if (e.Button == MouseButtons.Left && selectedLink != null)
                    {
                        DoDragDrop(selectedLink, DragDropEffects.All);    
                    }
                }      
                
                
            }            
        }

        private void markerControl_MouseMove(object sender, MouseEventArgs e)
        {
            var lx = mousex;
            var ly = mousey;
            mousex = e.X - markerControl.AutoScrollPosition.X;
            mousey = e.Y - markerControl.AutoScrollPosition.Y;
            
            if (selecting)
            {
                ex = e.Location.X - markerControl.AutoScrollPosition.X;
                ey = e.Location.Y - markerControl.AutoScrollPosition.Y;
            }

            if(moving)
            {
                var newLoc = selectedMarker.Location;
                newLoc.X += mousex - lx;
                newLoc.Y += mousey - ly;
                selectedMarker.Location = newLoc;

                markerControl.Invalidate();
            }

            markerControl.Invalidate();
        }

        private void markerControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selecting = false;
            }

            if (e.Button == MouseButtons.Left)
            {
                // Tiek kustināts pārvietojams elements - marķieris
                if (moving)
                {
                    moving = false;
                    var m = selectedMarker;
                    var newPos = m.Location;
                    var oldPos = Ed.MoveStartPos;
                    
                    // Ja pozīcija mainījusies, izveido jaunu atceļamu darbību
                    if (newPos.X != oldPos.X || newPos.Y != oldPos.Y)
                    {
                        Ed.Undo.PerformSimpleAction
                        (
                            () =>
                            {
                                m.Location = newPos;
                                markerControl.Invalidate();
                            },
                            () =>
                            {
                                m.Location = oldPos;
                                markerControl.Invalidate();
                            },
                            true
                        );
                    }
                }    
            }                        
        }
       
        // Atgriež izvēlēto saiti no saraksta
        // x, y - peles pozīcija
        // links - saišu saraksts
        Link PickSelectedLink(int x, int y, List<Link> links)
        {
            foreach(var link in links)
            {
                var dist = DistanceToLineSegment(new Point(x, y), link.DestBox.Center, link.FrameInstanceMarker.Location);
                if(dist <= 10.0)
                {
                    return link;
                }
                else if(link.GetMiddleBoxRect().Contains(x,y))
                {
                    return link;
                }
            }
            return null;
        }
        
        // Atgriež attālumsu starp punktiem
        static double Distance(Point p1, Point p2)
        {
            var x = p1.X - p2.X;
            var y = p1.Y - p2.Y;

            return Math.Sqrt(x*x + y*y);
        }

        // Atgriež attālumu līdz nogrieznim
        static double DistanceToLineSegment(Point p, Point start, Point end)
        {
            var dist = Distance(start, end);
            if(dist != 0)
            {
                var t = ((p.X - start.X) * (end.X - start.X) + (p.Y - start.Y) * (end.Y - start.Y)) / (dist*dist);
                                
                if (t < 0) 
                    return Distance(p, start);

                if (t > 1) 
                    return Distance(p, end);
                                
                return Distance(p, new Point((int)(start.X + t * (end.X - start.X)),
                                             (int)(start.Y + t * (end.Y - start.Y))));
            }
            return Distance(p, start);
        }
       
        // Atgriež izvēlēto grafisko elementu no saraksta
        // x, y - peles pozīcija
        // screenElements - saraksta ar grafiskajiem elementiem
        ScreenBox PickSelectedNode(int x, int y, List<ScreenBox> screenElements)
        {            
            return screenElements.Where(o => o.Rect.Contains(x, y)).OrderByDescending(o => o.Level).FirstOrDefault();
        }
        
        // Izdzēš izvēlēto koka mezgla entīti
        // deleteAll - vai izdzēst visas entītes, vai arī tikai doto entītes teikuma instanci
        void DeleteSelectedNodeEntity(bool deleteAll)
        {
            if (selectedNode is ScreenTreeBox)
            {
                var node = selectedNode as ScreenTreeBox;
                var word = node.Node.Word;
                var sent = GetSelectedSentence();
                                
                if (sent.WordToNamedEntityMap.ContainsKey(word))
                {
                    if(deleteAll)
                    {
                        Act_DeleteNamedEntity(sent.WordToNamedEntityMap[word]);    
                    }
                    else
                    {
                        Act_DeleteNamedEntityInstance(word);
                    }
                    
                }                
            }
        }
        
        private void markerControl_KeyDown(object sender, KeyEventArgs e)
        {
            // Ja lietotājs nospiež delete taustiņu
            if(e.KeyData == Keys.Delete)
            {                
                // Ja ir izvēlēta saite
                if(selectedLink != null)
                {
                    var frameInstance = selectedLink.FrameInstanceMarker.FrameInstance;
                    var reference = selectedLink.ElementReference;
                    Ed.Undo.PerformSimpleAction
                    (
                        () =>
                        {
                            frameInstance.ElementReferences.Remove(reference.Key);
                            selectedLink = null;
                            RefreshFrameElementsList();
                            markerControl.Invalidate();
                        },
                        () =>
                        {
                            frameInstance.ElementReferences.Add(reference.Key, reference.Value);
                            RefreshFrameElementsList();
                            markerControl.Invalidate();
                        }                        
                    );                    
                }
                // Ja ir izvēlēts named entity marķieris
                else if (selectedMarker is NamedEntityMarker)
                {
                    var marker = selectedMarker as NamedEntityMarker;
                    Act_DeleteNamedEntityMarker(marker);
                }
                // Ja ir izvēlēts freima instances marķieris
                else if (selectedMarker is FrameInstanceMarker)
                {
                    var ent = selectedMarker as FrameInstanceMarker;
                    Act_DeleteFrameInstance(Ed./*DB*/Doc.NamedEntities[ent.FrameInstance.TargetID], ent.FrameInstance);
                }
                else
                {
                   DeleteSelectedNodeEntity(deleteAll:e.Shift);
                }                
            }

            Ed.IsCtrlPressed = e.Control;
            Ed.IsShiftPressed = e.Shift;
            Ed.IsAltPressed = e.Alt;
        }

        void Act_DeleteNamedEntityMarker(NamedEntityMarker marker)
        {            
            var sent = GetSelectedSentence();
            var sentFrames = sent.WordToNamedEntityMap.SelectMany(o => o.Value.Frames).ToList();
            var references = sentFrames.ToDictionary(o => o.ElementReferences, 
                o => o.ElementReferences.Where(r => r.Value.WordIndex == -1 && r.Value.ID == marker.NamedEntity.ID).ToList());
                        
            Ed.Undo.PerformSimpleAction(
                () =>
                {
                    foreach (var framePair in references)
                    {
                        foreach (var elRef in framePair.Value)
                        {
                            framePair.Key.Remove(elRef.Key);
                        }
                    }

                    sent.NamedEntityMarkers.Remove(marker);

                    selectedMarker = null;
                    selectedLink = null;

                    markerControl.Invalidate();                                      
                },
                () =>
                {
                    foreach (var framePair in references)
                    {
                        foreach (var elRef in framePair.Value)
                        {
                            framePair.Key.Add(elRef.Key, elRef.Value);
                        }
                    }

                    sent.NamedEntityMarkers.Add(marker);
                                      
                    markerControl.Invalidate();                   
                }
            );  
        }

        void RefreshNamedEntityList()
        {
            lvEntities.Items.Clear();
            foreach (var entity in Ed./*DB*/Doc.NamedEntities.Values.Where(o => o.Name.ToLowerInvariant().Contains(txtNamedEntitySearch.Text.ToLowerInvariant())))
            {
                var aliasStr = string.Join(", ", entity.AliasSet);
                if (entity.AliasSet.Count == 0)
                {
                    aliasStr = "-       ";
                }
                var it = new ListViewItem(new[] { entity.ID.ToString().PadRight(4), entity.Name, entity.Type, aliasStr });
                it.Tag = entity;
                lvEntities.Items.Add(it);
            }                      
        }

        void RefreshFrameEntityList()
        {
            lbFrameEntities.Items.Clear();
            lbFrameEntities.Items.AddRange(Ed./*DB*/Doc.GetAllFrameInstances().ToArray());
        }
        void RefreshUI()
        {
            Ed.Undo.OnUndoListChanged();            
            cbFrames.Items.Clear();
            cbFrames.Items.AddRange(Ed.Frames.ToArray());
            RefreshNamedEntityList();
            RefreshFrameEntityList();
            RefreshSelectedSentence();
            SetSentencePanelSize();
        }

        void SetTitle()
        {
            Text = Path.GetFileNameWithoutExtension(Ed.Filename) + (Ed.Undo.IsDirty() ? "*" : "") + " - " + FormName;
        }

        void OnUndoListChanged(object sender, EventArgs eventArgs)
        {
            tsmUndo.Enabled = Ed.Undo.CanUndo();
            tsmRedo.Enabled = Ed.Undo.CanRedo();
            SetTitle();
        }

        Editor CreateEditor(bool openDefaultFile = true)
        {
            var ed = new Editor();
            ed.Undo.UndoListChanged += OnUndoListChanged;

            try
            {                
                ed.LoadFramesCSV(File.ReadAllText("frames.csv"));            
            }
            catch (Exception ex)
            {
                // Ja neeksistē ārējs freimu definīciju fails, tad izmanto noklusēto resursu
                ed.LoadFramesCSV(DataResource.Frames);
                /*
                Debug.WriteLine(@"Resources:");
                string[] names = System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceNames();
                foreach (string name in names)
                    Debug.WriteLine(name);
                */
                /*
                MessageBox.Show(this, "Couldn't open frames-new.csv.", "Frame Marker", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                Close();
                */
            }
                        
            /*
            try
            {
                ed.DB.Open("SemanticDB.xml", ed.Frames);    
            }            
            catch(Exception ex)
            {
                MessageBox.Show(this, "Couldn't open SemanticDB.xml, some sentences might not load.", "Frame Marker", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
            */
            
            if (openDefaultFile)
            {
                var lastOpened = GetLastOpenedFile();
                
                if (lastOpened == null || !ed.Open(lastOpened))
                    ed.CreateDefaultSentence();
            }
            else
            {
                ed.CreateDefaultSentence();
            }

            return ed;
        }
        

        bool AskToSaveChanges()
        {
            bool saved = true;

            if (Ed.Undo.IsDirty())
            {
                DialogResult res = MessageBox.Show("Save changes to " + Path.GetFileNameWithoutExtension(Ed.Filename) + "?", FormName, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                {
                    saved = Save();
                }

                if (res == DialogResult.Cancel)
                {
                    saved = false;
                }
            }

            return saved;
        }

        public void Open(string filename)
        {
            var ed = CreateEditor(openDefaultFile:false);
            if(ed.Open(filename))
            {
                Ed = ed;
                RefreshUI();                                
                CreateMissingFrameMarkers();
            }                                    
        }

        bool Save()
        {
            if (Ed.IsNew)
            {
                return SaveAs();
            }
            else if (Ed.Undo.IsDirty())
            {
                Ed.Save();
                SetTitle();
            }

            return true;
        }

        bool SaveAs()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter =
               "Marked Sentence (*.json)|*.json";
               //"Marked Sentence (*.xml)|*.xml";

            saveDialog.Title = "Save As";
            saveDialog.InitialDirectory = Directory.GetCurrentDirectory();

            if (!Ed.IsNew)
                saveDialog.FileName = Ed.Filename;

            DialogResult res = saveDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                if (Path.GetFullPath(saveDialog.FileName) != Ed.Filename || Ed.Undo.IsDirty())
                    Ed.Save(saveDialog.FileName);

                SetTitle();

                return true;
            }

            return false;
        }
        
        private void tsmNew_Click(object sender, EventArgs e)
        {            
            if (AskToSaveChanges())
            {
                Ed = CreateEditor(openDefaultFile: false);
                RefreshUI();
                SetTitle();
            }                            
        }

        private void tsmOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog opend = new OpenFileDialog();
            opend.Filter =
               "Marked Sentence (*.json)|*.json";
               //"Marked Sentence (*.xml)|*.xml";

            opend.Title = "Open";
            opend.InitialDirectory = Directory.GetCurrentDirectory();
            DialogResult openres = opend.ShowDialog();

            if (openres == DialogResult.OK)
            {
                if (AskToSaveChanges())
                {
                    Open(opend.FileName);
                    RefreshUI();
                    SetTitle();
                }
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void tsmSaveAs_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void tsmExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsmSelectAll_Click(object sender, EventArgs e)
        {
            //Selection = GetFullSelection();
            markerControl.Invalidate();
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AskToSaveChanges())
            {
                e.Cancel = true;
            }
            else
            {
                StoreLastOpenedFile(Ed.Filename);
            }
        }

        private void tsmUndo_Click(object sender, EventArgs e)
        {
            Ed.Undo.Op_Undo();
        }

        private void tsmRedo_Click(object sender, EventArgs e)
        {
            Ed.Undo.Op_Redo();
        }
        
        private void tsmDelete_Click(object sender, EventArgs e)
        {
            //Act_RemoveSelectedFrame();
        }

        private void tsmExportBitmap_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Portable Network Graphics (*.png)|*.png|Graphics Interchange Format (*.gif)|*.gif|Windows Bitmap (*.bmp)|*.bmp";
            saveDialog.Title = "Export Bitmap File";            
            saveDialog.InitialDirectory = Directory.GetCurrentDirectory();
            saveDialog.FileName = Path.GetFileNameWithoutExtension(Ed.Filename);
            DialogResult res = saveDialog.ShowDialog();
            
            if (res == DialogResult.OK)
            {
                using (var bitmap = new Bitmap(markerControl.AutoScrollMinSize.Width, markerControl.AutoScrollMinSize.Height))
                {
                    var g = Graphics.FromImage(bitmap);
                    g.Clear(Color.White);
                    PaintControl(g);
                    var imageFormat = ImageFormat.Png;

                    if (saveDialog.FilterIndex == 2)
                        imageFormat = ImageFormat.Gif;

                    if (saveDialog.FilterIndex == 3)
                        imageFormat = ImageFormat.Bmp;
                    
                    bitmap.Save(saveDialog.FileName, imageFormat);
                }
            }                                    
        }

        private void markerControl_KeyUp(object sender, KeyEventArgs e)
        {
            Ed.IsCtrlPressed = e.Control;
            Ed.IsShiftPressed = e.Shift;
            Ed.IsAltPressed = e.Alt;
        }
        
        private void tsmShowMorph_Click(object sender, EventArgs e)
        {
            
            markerControl.Invalidate();
        }

        private void tsmShowLemmas_Click(object sender, EventArgs e)
        {
            
            markerControl.Invalidate();
        }
        
        private void tsmShowNamedEntities_Click(object sender, EventArgs e)
        {
            markerControl.Invalidate();
        }
      
        private void cbFrames_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        void CreateMissingFrameMarkers()
        {                        
            foreach(var sent in Ed.Doc.Sentences)
            {
                var nodes = GetScreenNodes(sent).OfType<ScreenTreeBox>();
                foreach (var frameWithNoMarkers in sent.GetFrames().Where(o => o.Marker == null))
                {
                    var node = nodes.First(o => o.Node.Word.Index == frameWithNoMarkers.WordIndex && (o.IsMainWord || o.Node.Kids.Count == 0));                    
                    var marker = new LayoutMarker
                    {                        
                        X = node.Center.X, 
                        Y = node.Rect.Y + node.Rect.Height + 150
                    };
                    frameWithNoMarkers.Marker = marker;
                }                
            }            
        }

        private void butCreateFrame_Click(object sender, EventArgs e)
        {
            if(cbFrames.SelectedItem != null)
            {
                if(selectedNode != null && selectedNode is ScreenTreeBox)
                {                    
                    var node = selectedNode as ScreenTreeBox;                    
                    Act_CreateFrame(node, cbFrames.SelectedItem as Frame);           
                }
            }            
        }

        void Act_CreateFrame(ScreenTreeBox node, Frame frame)
        {            
            var sent = GetSelectedSentence();
            var pos = new Point(node.Center.X, node.Rect.Y + node.Rect.Height + 150);

            if (!sent.WordToNamedEntityMap.ContainsKey(node.Node.Word))
            {
                var namedEntity = GetNewNamedEntity(node.Node);

                // ***

                // TODO: vajag pievienot struktūru arī teikuma frames struktūrai !!!
                FrameInstance frameInstance = new FrameInstance
                {
                    Frame = frame,
                    Marker = new LayoutMarker { X = pos.X, Y = pos.Y },
                    TargetID = namedEntity.ID,
                    SentenceID = sent.ID,
                    sentenceIndex = sent.index,    /* *** lai varētu atbrīvoties no SentenceID */
                    WordIndex = node.Node.Word.Index
                };

                sent.Frames.Add(frameInstance);
                namedEntity.Frames.Add(frameInstance);

                //namedEntity.Frames.Add(new FrameInstance
                //{
                //    Frame = frame,
                //    Marker = new LayoutMarker { X = pos.X, Y = pos.Y },
                //    TargetID = namedEntity.ID,
                //    SentenceID = GetSelectedSentence().ID,
                //    sentenceIndex = GetSelectedSentence().index,    /* *** lai varētu atbrīvoties no SentenceID */
                //    WordIndex = node.Node.Word.Index
                //});

                Act_CreateNamedEntity(sent, node.Node.Word, namedEntity);                
            }
            else
            {
                var namedEntity = sent.WordToNamedEntityMap[node.Node.Word];

                var frameInst = new FrameInstance
                {
                    Frame = frame,
                    Marker = new LayoutMarker {X = pos.X, Y = pos.Y + namedEntity.Frames.Count * 50},
                    TargetID = namedEntity.ID,
                    SentenceID = sent.ID,
                    sentenceIndex = sent.index,    /* *** lai varētu atbrīvoties no SentenceID */
                    WordIndex = node.Node.Word.Index
                };

                Ed.Undo.PerformSimpleAction
                (                
                () =>
                    {
                        sent.Frames.Add(frameInst);
                        namedEntity.Frames.Add(frameInst);                        
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    },
                    () =>
                    {
                        sent.Frames.Remove(frameInst);
                        namedEntity.Frames.Remove(frameInst);                        
                        selectedNode = null;
                        selectedMarker = null;
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    }
                );
            }
        }

        
        bool Act_RelinkNamedEntity(ScreenTreeBox node, NamedEntity entity, Word oldWord)
        {
            var sent = GetSelectedSentence();
            var newWord = node.Node.Word;
            if (!sent.WordToNamedEntityMap.ContainsKey(newWord))
            {                
                // *** lai atbrīvotos no SentenceID */
                //var movedFrames = entity.Frames.Where(o => o.WordIndex == oldWord.Index && o.SentenceID == sent.ID).ToList();
                var movedFrames = entity.Frames.Where(o => o.WordIndex == oldWord.Index && o.sentenceIndex == sent.index).ToList();
                var movedElReferences = Ed./*DB*/Doc.GetAllElementReferences(entity.ID)
                    .Where(o => o.Key.sentenceIndex == sent.index)
                    /*.Where(o => o.Key.SentenceID == sent.ID)*/ /* *** lai atbrīvotos no SentenceID */
                    .ToDictionary(p => p.Key, p => p.Value.Where(o => o.Value.WordIndex == oldWord.Index).ToList());
                
                Ed.Undo.PerformSimpleAction
                (
                    () =>
                    {
                        // ***
                        oldWord.namedEntityID = -1;
                        newWord.namedEntityID = entity.ID;
                        sent.WordToNamedEntityMap.Remove(oldWord);
                        sent.WordToNamedEntityMap.Add(newWord, entity);   

                        foreach(var frame in movedFrames)
                        {
                            frame.WordIndex = newWord.Index;
                        }

                        foreach(var refPair in movedElReferences)
                        {
                            foreach(var el in refPair.Value)
                            {
                                el.Value.WordIndex = newWord.Index;
                            }
                        }
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    },
                    () =>
                    {                        
                        // ***
                        newWord.namedEntityID = -1;
                        oldWord.namedEntityID = entity.ID;
                        sent.WordToNamedEntityMap.Remove(newWord);
                        sent.WordToNamedEntityMap.Add(oldWord, entity);

                        foreach (var frame in movedFrames)
                        {
                            frame.WordIndex = oldWord.Index;
                        }

                        foreach (var refPair in movedElReferences)
                        {
                            foreach (var el in refPair.Value)
                            {
                                el.Value.WordIndex = oldWord.Index;
                            }
                        }

                        selectedNode = null;
                        selectedMarker = null;
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    }
                );
                return true;
            }

            return false;
        }
                
        bool Act_LinkNamedEntity(ScreenTreeBox node, NamedEntity entity)
        {
            var sent = GetSelectedSentence();
            if (!sent.WordToNamedEntityMap.ContainsKey(node.Node.Word))
            {                                                
                
                Ed.Undo.PerformSimpleAction
                (
                    () =>
                    {
                        node.Node.Word.namedEntityID = entity.ID;
                        sent.WordToNamedEntityMap.Add(node.Node.Word, entity);                                                
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    },
                    () =>
                    {
                        node.Node.Word.namedEntityID = -1;
                        sent.WordToNamedEntityMap.Remove(node.Node.Word);                                                
                        selectedNode = null;
                        selectedMarker = null;
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    }
                );
                return true;
            }

            return false;
        }
        

        /*
        void Act_LinkFrameElement(MovableEntity movable, FrameEntityMarker marker, FrameElement element)
        {
            if (marker != null && !marker.FrameEntity.ElementReferences.ContainsKey(element))
            {
                var refID = -1;
                if(movable.Marker is FrameEntityMarker)
                {
                    refID = (movable.Marker as FrameEntityMarker).FrameEntity.ID;
                }

                if (movable.Marker is NamedEntityMarker)
                {
                    refID = (movable.Marker as NamedEntityMarker).NamedEntity.ID;
                }

                Ed.Undo.PerformSimpleAction
                (
                    () =>
                    {
                        marker.FrameEntity.ElementReferences.Add(element, refID);
                        markerControl.Invalidate();
                        RefreshFrameElementsList();
                    },
                    () =>
                    {
                        marker.FrameEntity.ElementReferences.Remove(element);
                        markerControl.Invalidate();
                        RefreshFrameElementsList();
                    }
                );
            }
        }
        */

        void Act_DeleteSentence(Sentence sent)
        {
            if(Ed.Doc.Sentences.Count > 1)
            {
                var idx = Ed.Doc.Sentences.IndexOf(sent);
                Ed.Undo.PerformSimpleAction
                (
                    () =>
                    {
                        Ed.Doc.Sentences.RemoveAt(idx);
                        Ed.SelectedSentenceIndex = Math.Min(Ed.SelectedSentenceIndex, Ed.Doc.Sentences.Count - 1);
                        SetSentencePanelSize();
                        RefreshSelectedSentence();
                    },
                    () =>
                    {
                        Ed.Doc.Sentences.Insert(idx, sent);
                        SetSentencePanelSize();
                        RefreshSelectedSentence();
                    }
                );
                
                foreach (var entity in sent.WordToNamedEntityMap.Values)
                {
                    Act_DeleteNamedEntity(entity);
                    Ed.Undo.MergeWithLast();
                }                
            }            
        }
        
        void Act_RelinkFrameElement(ScreenTreeBox treeNode, FrameInstanceMarker marker, FrameElement element)
        {
            var sent = GetSelectedSentence();            

            if (marker != null)
            {
                bool merge = false;
                if (!sent.WordToNamedEntityMap.ContainsKey(treeNode.Node.Word))
                {
                    Act_CreateNamedEntity(treeNode);
                    merge = true;
                }

                var elRef = marker.FrameInstance.ElementReferences[element];

                var newWord = treeNode.Node.Word.Index;                
                var oldWord = elRef.WordIndex;
                var newTarget = sent.WordToNamedEntityMap[treeNode.Node.Word].ID;
                var oldTarget = elRef.ID;
                
                Ed.Undo.PerformSimpleAction
                (
                    () =>
                    {
                        elRef.WordIndex = newWord;
                        elRef.ID = newTarget;
                        markerControl.Invalidate();
                        RefreshFrameElementsList();
                    },
                    () =>
                    {
                        elRef.WordIndex = oldWord;
                        elRef.ID = oldTarget;
                        markerControl.Invalidate();
                        RefreshFrameElementsList();
                    }
                );               

                if(merge)
                    Ed.Undo.MergeWithLast();
            }
            else
            {
                
            }
        }
        
        void Act_LinkFrameElementWithNamedEntityMarker(FrameInstance frameInstance, MovableEntity movable, FrameElement frameElement)
        {
            var namedEntity = (movable.Marker as NamedEntityMarker).NamedEntity;                        
            
            var elRef = new Reference { ID = namedEntity.ID };

            var existingFE = frameInstance.ElementReferences.FirstOrDefault(o => o.Key == frameElement && o.Value.ID == elRef.ID && o.Value.WordIndex == -1).Key;

            Ed.Undo.PerformSimpleAction
            (
                () =>
                {
                    frameInstance.ElementReferences.Add(frameElement, elRef);

                    if (existingFE != null)
                        frameInstance.ElementReferences.Remove(existingFE);

                    markerControl.Invalidate();
                    RefreshFrameElementsList();
                },
                () =>
                {

                    frameInstance.ElementReferences.Remove(frameElement);

                    if (existingFE != null)
                        frameInstance.ElementReferences.Add(existingFE, elRef);

                    markerControl.Invalidate();
                    RefreshFrameElementsList();
                }
            );              
        }

        void Act_LinkFrameElement(TreeNode node, FrameInstanceMarker marker, FrameElement element)
        {
            if (marker != null && !marker.FrameInstance.ElementReferences.ContainsKey(element))
            {                
                var sent = GetSelectedSentence();                
                var elRef = new Reference {WordIndex = node.Word.Index};

                if (sent.WordToNamedEntityMap.ContainsKey(node.Word))
                {
                    elRef.ID = sent.WordToNamedEntityMap[node.Word].ID;
                }

                if (elRef.ID != -1)
                {
                    // Replace existing element reference with the new one
                    var frameInstance = marker.FrameInstance;
                    FrameElement existingFE = null;

                    if (frameInstance.ElementReferences.ContainsValue(elRef))
                    {
                        existingFE = frameInstance.ElementReferences.First(o => o.Value.Equals(elRef)).Key;
                    }

                    Ed.Undo.PerformSimpleAction
                    (
                        () =>
                        {
                            frameInstance.ElementReferences.Add(element, elRef);    

                            if (existingFE != null)
                                frameInstance.ElementReferences.Remove(existingFE);
                                                                                                                                                                        
                            markerControl.Invalidate();
                            RefreshFrameElementsList();
                        },
                        () =>
                        {                            
                            
                            frameInstance.ElementReferences.Remove(element);

                            if (existingFE != null)
                                frameInstance.ElementReferences.Add(existingFE, elRef);    
                                                                                  
                            markerControl.Invalidate();
                            RefreshFrameElementsList();
                        }
                    );                    
                }
                else
                {
                    // Completely new element reference
                    var entity = GetNewNamedEntity(node);
                    Ed.Undo.PerformSimpleAction
                    (
                        () =>
                        {
                            marker.FrameInstance.ElementReferences.Add(element, 
                                new Reference{ID = entity.ID, WordIndex = node.Word.Index});

                            RefreshFrameElementsList();
                        },
                        () =>
                        {
                            marker.FrameInstance.ElementReferences.Remove(element);
                            RefreshFrameElementsList();
                        }
                    );
                    
                    Act_CreateNamedEntity(sent, node.Word, entity);
                    Ed.Undo.MergeWithLast();
                }
            }
        }
        
        NamedEntity GetNewNamedEntity(TreeNode node)
        {
            return GetNewNamedEntity(node, "Unknown");
        }

        NamedEntity GetNewNamedEntity(TreeNode node, string type)
        {
            var flattenedNodes = node.GetAllKids();
            flattenedNodes.Add(node);

            var orderedNodes = flattenedNodes.OrderBy(o => o.Word.Index).ToList();
            var joinedLemmas = string.Join(" ", orderedNodes.Select(o => o.Word.Lemma));
            
            // Atmetam teikuma daļu pēc komata
            var split = joinedLemmas.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);

            var namedEntityName = "noname";

            if(split.Count() > 0)
            {
                namedEntityName = split[0].Trim();
            }
            var entity = new NamedEntity
            {
                ID = Ed./*DB*/Doc.GetNewEntityID(),
                Name = namedEntityName,
                Type = type
            };
            return entity;
        }

        NamedEntity Act_CreateNamedEntity(ScreenTreeBox treeNode)
        {
            var entity = GetNewNamedEntity(treeNode.Node);
            Act_CreateNamedEntity(GetSelectedSentence(), treeNode.Node.Word, entity);
            return entity;
        }


        void Act_CreateNamedEntity(Sentence sent, Word word, NamedEntity entity)
        {                                    
            if(!sent.WordToNamedEntityMap.ContainsKey(word))
            {
                Ed.Undo.PerformSimpleAction
                (
                    () =>
                    {
                        Ed./*DB*/Doc.NamedEntities.Add(entity.ID, entity);
                        sent.WordToNamedEntityMap.Add(word, entity);
                        // ***
                        word.namedEntityID = entity.ID;
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                        RefreshNamedEntityList();
                        RefreshFrameEntityList();
                    },
                    () =>
                    {
                        Ed./*DB*/Doc.NamedEntities.Remove(entity.ID);
                        sent.WordToNamedEntityMap.Remove(word);
                        // ***
                        word.namedEntityID = -1;

                        selectedNode = null;
                        selectedMarker = null;
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                        RefreshNamedEntityList();
                        RefreshFrameEntityList();
                    }
                );                 
            }            
        }

        private void lbFrameElements_MouseDown(object sender, MouseEventArgs e)
        {
            if (lbFrameElements.SelectedItem != null)
            {
                DoDragDrop(lbFrameElements.SelectedItem, DragDropEffects.All);
            }
        }

        private void lbOtherElements_MouseDown(object sender, MouseEventArgs e)
        {
            if (lbOtherElements.SelectedItem != null)
            {
                DoDragDrop(lbOtherElements.SelectedItem, DragDropEffects.All);
            }
        }
        
        void Act_MergeEntity(ScreenTreeBox node, NamedEntity ent)
        {
            var curSent = GetSelectedSentence();
            var oldEntity = curSent.WordToNamedEntityMap[node.Node.Word];

            var mergedEntities = new Dictionary<Sentence, List<KeyValuePair<Word, NamedEntity>>>();
            //r mergedMarkers = new Dictionary<Sentence, List<Marker>>();
            
            foreach (var sent in Ed.Doc.Sentences)
            {
                var entities = sent.WordToNamedEntityMap.Where(o => o.Value.ID == oldEntity.ID).ToList();
                mergedEntities.Add(sent, entities);                
            }

            
            var mergedReferences = Ed./*DB*/Doc.GetAllElementReferences(oldEntity.ID);

            
            var newAliasSet = new HashSet<string>();
            var oldAliasSet = newAliasSet;
            
            newAliasSet = new HashSet<string>(ent.AliasSet.Union(oldEntity.AliasSet));
            newAliasSet.Add(oldEntity.Name);
            oldAliasSet = ent.AliasSet;
            

            Ed.Undo.PerformSimpleAction(
                () =>
                {
                    Ed./*DB*/Doc.NamedEntities.Remove(oldEntity.ID);                                        
                    ent.AliasSet = newAliasSet;
                                        
                    foreach (var sentEnt in mergedEntities)
                    {
                        foreach (var fe in sentEnt.Value)
                        {
                            sentEnt.Key.WordToNamedEntityMap[fe.Key] = ent;
                        }
                    }

                    foreach(var frame in oldEntity.Frames)
                    {
                        frame.TargetID = ent.ID;
                        ent.Frames.Add(frame);
                    }

                    foreach(var frameRef in mergedReferences)
                    {
                        foreach(var entRef in frameRef.Value)
                        {
                            entRef.Value.ID = ent.ID;
                        }
                    }
                                                                             
                    markerControl.Invalidate();
                    panelSentences.Invalidate();
                    RefreshFrameEntityList();
                    RefreshNamedEntityList();
                },
                () =>
                {
                    Ed./*DB*/Doc.NamedEntities.Add(oldEntity.ID, oldEntity);
                    ent.AliasSet = oldAliasSet;

                    foreach (var sentEnt in mergedEntities)
                    {
                        foreach (var fe in sentEnt.Value)
                        {
                            sentEnt.Key.WordToNamedEntityMap[fe.Key] = oldEntity;
                        }
                    }

                    foreach (var frame in oldEntity.Frames)
                    {
                        frame.TargetID = oldEntity.ID;
                        ent.Frames.Remove(frame);
                    }

                    foreach (var frameRef in mergedReferences)
                    {
                        foreach (var entRef in frameRef.Value)
                        {
                            entRef.Value.ID = oldEntity.ID;
                        }
                    }
                                                                                                
                    markerControl.Invalidate();
                    panelSentences.Invalidate();
                    RefreshFrameEntityList();
                    RefreshNamedEntityList();
                }
           );             
        }
 

        public void Act_RelinkFrameInstance(ScreenTreeBox node, FrameInstance frameInst)
        {            
            var sent = GetSelectedSentence();
            var oldNamedEntity = Ed./*DB*/Doc.NamedEntities[frameInst.TargetID];

            NamedEntity newNamedEntity = null;
            bool merge = false;
            
            if(sent.WordToNamedEntityMap.ContainsKey(node.Node.Word))
            {                
                newNamedEntity =  sent.WordToNamedEntityMap[node.Node.Word];                                
            }
            else
            {
                newNamedEntity = Act_CreateNamedEntity(node);
                merge = true;
            }
            var newWordIndex = node.Node.Word.Index;
            var oldWordIndex = frameInst.WordIndex;

            Ed.Undo.PerformSimpleAction
            (
                () =>
                {
                    newNamedEntity.Frames.Add(frameInst);
                    oldNamedEntity.Frames.Remove(frameInst);
                    frameInst.TargetID = newNamedEntity.ID;
                    frameInst.WordIndex = newWordIndex;
                    panelSentences.Invalidate();
                    markerControl.Invalidate();
                    RefreshFrameEntityList();
                },
                () =>
                {
                    newNamedEntity.Frames.Remove(frameInst);
                    oldNamedEntity.Frames.Add(frameInst);
                    frameInst.TargetID = oldNamedEntity.ID;
                    frameInst.WordIndex = oldWordIndex;

                    panelSentences.Invalidate();
                    markerControl.Invalidate();
                    RefreshFrameEntityList();
                }
            );   
            
            if(merge)
            {
                Ed.Undo.MergeWithLast();
            }            
        }

        bool MergeEntity(ScreenTreeBox treeNode, NamedEntity ent)
        {
            var curSent = GetSelectedSentence();
            var oldNamedEntity = curSent.WordToNamedEntityMap[treeNode.Node.Word];

            if(oldNamedEntity != ent)
            {
                var res = MessageBox.Show(this, "Merge named entity \"" + oldNamedEntity.Name + "\" into named entity \"" + ent.Name + "\"?", FormName, MessageBoxButtons.YesNoCancel);

                if (res == DialogResult.Yes)
                {
                    Act_MergeEntity(treeNode, ent);
                    return true;
                }             
            }
            return false;
        }

        private void markerControl_DragDrop(object sender, DragEventArgs e)
        {
            var point = markerControl.PointToClient(new Point(e.X-markerControl.AutoScrollPosition.X, e.Y-markerControl.AutoScrollPosition.Y));
            
            var node = PickSelectedNode(point.X, point.Y, GetScreenNodes());
            if (node != null)            
            {
                if(node is ScreenTreeBox)
                {
                    var treeNode = node as ScreenTreeBox;

                    if (e.Data.GetDataPresent(typeof(NamedEntity)))
                    {
                        var data = e.Data.GetData(typeof(NamedEntity)) as NamedEntity;
                        
                        if(!Act_LinkNamedEntity(treeNode, data))
                        {
                            MergeEntity(treeNode, data);                   
                        }                        
                    }

                    if (e.Data.GetDataPresent(typeof(NamedEntity)))
                    {
                        var data = e.Data.GetData(typeof(NamedEntity)) as NamedEntity;

                        if (!Act_LinkNamedEntity(treeNode, data))
                        {
                            MergeEntity(treeNode, data);
                        }       
                    }
                    /*
                    if (e.Data.GetDataPresent(typeof(NamedEntityMarker)))
                    {
                        var data = e.Data.GetData(typeof(NamedEntityMarker)) as NamedEntityMarker;

                        if (!Act_LinkNamedEntity(treeNode, data.NamedEntity))
                        {
                            if(MergeEntity(treeNode, data.NamedEntity))
                            {                                
                                Ed.Undo.MergeWithLast();    
                            }
                        }
                        else
                        {
                            var sent = GetSelectedSentence();
                            var removedMarker = sent.NamedEntityMarkers.FirstOrDefault(o => o.NamedEntity.ID == data.NamedEntity.ID);                                    
                            Ed.Undo.PerformSimpleAction
                            (
                                () =>
                                {
                                    sent.NamedEntityMarkers.Remove(removedMarker);
                                    markerControl.Invalidate();                                    
                                },
                                () =>
                                {                                                     
                                    sent.NamedEntityMarkers.Add(removedMarker);
                                    markerControl.Invalidate();                                    
                                }
                            );   
                            Ed.Undo.MergeWithLast();
                        }
                    }
                    */

                    if (e.Data.GetDataPresent(typeof(FrameInstance)))
                    {
                        var data = e.Data.GetData(typeof(FrameInstance)) as FrameInstance;
                        Act_RelinkFrameInstance(treeNode, data);
                    }
                                        
                    if (e.Data.GetDataPresent(typeof(Frame)))
                    {
                        var data = e.Data.GetData(typeof(Frame)) as Frame;
                        Act_CreateFrame(treeNode, data);
                    }
                    
                    if (e.Data.GetDataPresent(typeof(FrameElement)))
                    {
                        var data = e.Data.GetData(typeof(FrameElement)) as FrameElement;
                        var marker = selectedMarker as FrameInstanceMarker;                        
                        Act_LinkFrameElement(treeNode.Node, marker, data);
                    }    
                  
                    if(e.Data.GetDataPresent(typeof(ScreenNamedEntityBox)))
                    {
                        var data = e.Data.GetData(typeof(ScreenNamedEntityBox)) as ScreenNamedEntityBox;
                        if(!Act_RelinkNamedEntity(treeNode, data.NamedEntity, data.Word))
                        {
                            MergeEntity(treeNode, data.NamedEntity);
                        }
                    }

                    if (e.Data.GetDataPresent(typeof(Link)))
                    {
                        var data = e.Data.GetData(typeof(Link)) as Link;                        
                        Act_RelinkFrameElement(treeNode, data.FrameInstanceMarker, data.ElementReference.Key);
                    }
                }
                
                if (node is MovableEntity)
                {
                    var movable = node as MovableEntity;
                    
                    if (e.Data.GetDataPresent(typeof(FrameElement)) && movable.Marker is NamedEntityMarker)
                    {
                        if(selectedMarker is FrameInstanceMarker)
                        {
                            var data = e.Data.GetData(typeof(FrameElement)) as FrameElement;
                            Act_LinkFrameElementWithNamedEntityMarker((selectedMarker as FrameInstanceMarker).FrameInstance, movable, data);    
                        }                        
                    }

                    if (movable.Marker is NamedEntityMarker && e.Data.GetDataPresent(typeof(Link)))
                    {
                        var data = e.Data.GetData(typeof(Link)) as Link;                        
                        var marker = data.FrameInstanceMarker;

                        if (marker != null)
                        {
                            var namedEntityMarker = movable.Marker as NamedEntityMarker;
                            
                            var element = data.ElementReference.Key;                            
                            var elRef = marker.FrameInstance.ElementReferences[element];

                            var newWord = -1;
                            var oldWord = elRef.WordIndex;
                            var newTarget = namedEntityMarker.NamedEntity.ID;
                            var oldTarget = elRef.ID;

                            Ed.Undo.PerformSimpleAction
                            (
                                () =>
                                {
                                    elRef.WordIndex = newWord;
                                    elRef.ID = newTarget;
                                    markerControl.Invalidate();
                                    RefreshFrameElementsList();
                                },
                                () =>
                                {
                                    elRef.WordIndex = oldWord;
                                    elRef.ID = oldTarget;
                                    markerControl.Invalidate();
                                    RefreshFrameElementsList();
                                }
                            );
                        }                        
                    }
                }                
            }
            else
            {                
                var sent = GetSelectedSentence();

                if (e.Data.GetDataPresent(typeof(NamedEntity)))
                {
                    var entity = e.Data.GetData(typeof(NamedEntity)) as NamedEntity;

                    var marker = new NamedEntityMarker
                    {
                        NamedEntity = entity,
                        Location = point
                    };

                    if(!sent.NamedEntityMarkers.Any(o => o.NamedEntity == entity)
                    && !sent.WordToNamedEntityMap.ContainsValue(entity))
                    {
                        Ed.Undo.PerformSimpleAction
                        (
                            () =>
                            {
                                sent.NamedEntityMarkers.Add(marker);
                                markerControl.Invalidate();
                            },
                            () =>
                            {
                                sent.NamedEntityMarkers.Remove(marker);
                                selectedMarker = null;
                                markerControl.Invalidate();
                            }
                        );                        
                    }                                        
                }

                /*
                if (e.Data.GetDataPresent(typeof(FrameEntity)))
                {
                    var entity = e.Data.GetData(typeof(FrameEntity)) as FrameEntity;
                                        
                    var marker = new FrameEntityMarker
                    {
                        FrameEntity = entity,
                        Location = point
                    };

                    if (!sent.Markers.OfType<FrameEntityMarker>().Any(o => o.FrameEntity == entity))
                    {
                        Ed.Undo.PerformSimpleAction
                        (
                            () =>
                            {
                                sent.Markers.Add(marker);
                                markerControl.Invalidate();
                            },
                            () =>
                            {
                                sent.Markers.Remove(marker);
                                selectedMarker = null;
                                markerControl.Invalidate();
                            }
                        );
                    }
                }
                */
            }            
        }

        private void markerControl_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
           
        }

        private void markerControl_DragOver(object sender, DragEventArgs e)
        {
            
        }

        private void butCreateFrame_MouseDown(object sender, MouseEventArgs e)
        {
            if (cbFrames.SelectedItem != null)
            {
                DoDragDrop(cbFrames.SelectedItem, DragDropEffects.All);
            }
        }

        private void lbNamedEntities_MouseDown(object sender, MouseEventArgs e)
        {                        
            if (lvEntities.SelectedItems.Count > 0)
            {
                if(e.Button == MouseButtons.Left)
                {
                    DoDragDrop(lvEntities.SelectedItems[0].Tag, DragDropEffects.All);    
                }
                else
                {                    
                    cmNamedEntities.Show(lvEntities, e.Location);   
                }
            }
        }

        
        private void tsmNamedEntitiesDialog_Click(object sender, EventArgs e)
        {
            if(lvEntities.SelectedItems.Count > 0)
            {
                var diag = new frmNamedEntities(Ed./*DB*/Doc, lvEntities.SelectedItems[0].Tag as NamedEntity);
                var res = diag.ShowDialog(this);

                var oldValues = Ed./*DB*/Doc.NamedEntities.SerializeClone();
                var newValues = diag.NamedEntities;

                if (res == DialogResult.OK)
                {
                    Ed.Undo.PerformSimpleAction
                    (
                        () =>
                        {
                            foreach (var pair in newValues)
                            {
                                var ne = Ed./*DB*/Doc.NamedEntities[pair.Key];
                                ne.Name = pair.Value.Name;
                                ne.Type = pair.Value.Type;
                                ne.AliasSet = pair.Value.AliasSet;
                            }
                            RefreshNamedEntityList();
                        },
                        () =>
                        {
                            foreach (var pair in oldValues)
                            {
                                var ne = Ed./*DB*/Doc.NamedEntities[pair.Key];
                                ne.Name = pair.Value.Name;
                                ne.Type = pair.Value.Type;
                                ne.AliasSet = pair.Value.AliasSet;
                            }
                            RefreshNamedEntityList();
                        }
                    );
                }


                RefreshNamedEntityList();    
            }
            
        }        
      
        private void lbFrameEntities_MouseDown(object sender, MouseEventArgs e)
        {
            if (lbFrameEntities.SelectedItem != null)
            {
                DoDragDrop(lbFrameEntities.SelectedItem, DragDropEffects.All);
            }
        }

        private void tsbToStart_Click(object sender, EventArgs e)
        {
            Ed.SelectedSentenceIndex = 0;
            RefreshSelectedSentence();
        }

        private void tsbToLast_Click(object sender, EventArgs e)
        {
            Ed.SelectedSentenceIndex = Ed.Doc.Sentences.Count-1;
            RefreshSelectedSentence();
        }

        private void tsbPrevious_Click(object sender, EventArgs e)
        {
            Ed.SelectedSentenceIndex = Math.Max(0, Ed.SelectedSentenceIndex - 1);
            RefreshSelectedSentence();
        }

        private void tsbNext_Click(object sender, EventArgs e)
        {
            Ed.SelectedSentenceIndex = Math.Min(Ed.Doc.Sentences.Count-1, Ed.SelectedSentenceIndex + 1);
            RefreshSelectedSentence();
        }
        
        void RefreshSelectedSentence()
        {
            selectedNode = null;
            selectedMarker = null;
            selectedLink = null;
            markerControl.Invalidate();
            txtCurrentSentence.TextChanged -= txtCurrentSentence_TextChanged;
            txtCurrentSentence.Text = (Ed.SelectedSentenceIndex + 1) + "/" + Ed.Doc.Sentences.Count;
            txtCurrentSentence.TextChanged += txtCurrentSentence_TextChanged;
            panelSentences.Invalidate();
        }

        int sentenceHeight = 32;
        int sentenceOffX = 23;
        int sentenceOffY = 4;
        
        void SetSentencePanelSize()
        {                        
            panelSentences.AutoScrollMinSize = new Size(0, sentenceHeight * Ed.Doc.Sentences.Count);
        }

        private void panelSentences_Paint(object sender, PaintEventArgs e)
        {
            
            var g = e.Graphics;
            var firstSent = -panelSentences.AutoScrollPosition.Y/sentenceHeight;
            g.TranslateTransform(panelSentences.AutoScrollPosition.X, panelSentences.AutoScrollPosition.Y);
            g.Clear(Color.White);
            
            var fmt = new StringFormat();
            fmt.Alignment = StringAlignment.Center;

            var height = Math.Max(panelSentences.AutoScrollMinSize.Height, panelSentences.Height);
            g.FillRectangle(SystemBrushes.ButtonFace, 0, 0, sentenceOffX - 5, height);
            g.FillRectangle(Brushes.Gray, sentenceOffX - 5 - 3, 0, 3, height);

            for(int n = firstSent; n < firstSent + panelSentences.Height/sentenceHeight + 2 && n < Ed.Doc.Sentences.Count; n++)
            {                
                var sent = Ed.Doc.Sentences[n];
                float xoff = sentenceOffX;
                float yoff = n*sentenceHeight;
                
                if(n % 2 == 0)
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(230, 230, 230)), sentenceOffX - 5, yoff, panelSentences.Width, sentenceHeight);    
                }
                

                if (n == Ed.SelectedSentenceIndex)
                {
                    g.DrawImage(Properties.Resources.play_green, 1, yoff+3 + sentenceOffY,16,16);
                }

                foreach(var word in sent.Words.OrderBy(o => o.Index))
                {
                    var size = TextRenderer.MeasureText(word.Original, SystemFonts.DefaultFont);
                    var rect = new RectangleF(xoff, yoff + sentenceOffY, size.Width - 2, size.Height + 10);
                    
                    if(sent.WordToNamedEntityMap.ContainsKey(word))
                    {
                        var namedEntity = sent.WordToNamedEntityMap[word];
                        g.FillRectangle(namedEntity.Frames.Count > 0 ? Brushes.DarkCyan : Brushes.DarkViolet, rect);
                        g.DrawString("ID[" + namedEntity.ID + "]",
                            SystemFonts.DefaultFont, Brushes.White, xoff + size.Width / 2, yoff + 10 + sentenceOffY, fmt);
                    }
                                        
                    bool covered = sent.WordToNamedEntityMap.ContainsKey(word);
                                        
                    g.DrawString(word.Original,
                        SystemFonts.DefaultFont, covered ? Brushes.White : Brushes.Black, xoff, yoff + sentenceOffY);
                    xoff += size.Width + 2;                                        
                }
                              
            }
        }
        
        private void panelSentences_MouseDown(object sender, MouseEventArgs e)
        {
            var idx = GetSentenceIndexFromMouseCoords(e.Location);            
            var sent = Ed.Doc.Sentences[idx];

            float yoff = idx * sentenceHeight;
            float xoff = sentenceOffX;

            bool draggingEntity = false;
            
            if(e.Button == MouseButtons.Left)
            {
                foreach (var word in sent.Words.OrderBy(o => o.Index))
                {
                    var size = TextRenderer.MeasureText(word.Original, SystemFonts.DefaultFont);
                    var rect = new RectangleF(xoff, yoff + sentenceOffY, size.Width - 2, size.Height + 10);

                    if (rect.Contains(e.X, e.Y - panelSentences.AutoScrollPosition.Y))
                    {                        
                        if (sent.WordToNamedEntityMap.ContainsKey(word))
                        {
                            DoDragDrop(sent.WordToNamedEntityMap[word], DragDropEffects.All);
                            draggingEntity = true;
                            break;
                        }                        
                    }

                    xoff += size.Width + 2;
                }    
            }

            if (!draggingEntity || e.Button == MouseButtons.Right)
            {
                Ed.SelectedSentenceIndex = idx;
                panelSentences.Focus();
                RefreshSelectedSentence();    

                if(e.Button == MouseButtons.Right)
                {
                    cmPanelSentences.Show(panelSentences, e.Location);   
                }                
            }                        
        }

        public int GetSentenceIndexFromMouseCoords(Point pos)
        {
            return Math.Min(Ed.Doc.Sentences.Count - 1, (pos.Y - panelSentences.AutoScrollPosition.Y) / sentenceHeight);            
            
        }

        private void panelSentences_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        private void panelSentences_DragDrop(object sender, DragEventArgs e)
        {
            
        }
        
        private void lbFrameEntities_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete)
            {
                if(lbFrameEntities.SelectedItem != null)
                {                    
                    var frameInst = lbFrameEntities.SelectedItem as FrameInstance;
                    Act_DeleteFrameInstance(Ed./*DB*/Doc.NamedEntities[frameInst.TargetID], frameInst);                    
                }                
            }
        }

        private void lbNamedEntities_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (lvEntities.SelectedItems.Count > 0)
                {                    
                    var ent = lvEntities.SelectedItems[0].Tag as NamedEntity;
                    Act_DeleteNamedEntity(ent);                    
                }
            }
        }    
    
        void Act_DeleteFrameInstance(NamedEntity ent, FrameInstance frameInst)
        {            
            Ed.Undo.PerformSimpleAction(
                () =>
                {
                    ent.Frames.Remove(frameInst);
                    // ***
                    Ed.Doc.Sentences[frameInst.sentenceIndex].Frames.Remove(frameInst);

                    markerControl.Invalidate();
                    panelSentences.Invalidate();
                    RefreshFrameEntityList();                   
                },
                () =>
                {
                    ent.Frames.Add(frameInst);
                    // ***
                    Ed.Doc.Sentences[frameInst.sentenceIndex].Frames.Add(frameInst);

                    selectedMarker = null;
                    selectedLink = null;
                    markerControl.Invalidate();
                    panelSentences.Invalidate();
                    RefreshFrameEntityList();                    
                }
            );  
        }

        
        void Act_DeleteNamedEntity(NamedEntity ent)
        {            
            var removedEntities = new Dictionary<Sentence, List<KeyValuePair<Word, NamedEntity>>>();
            var removedMarkers = new Dictionary<Sentence, List<Marker>>();

            foreach (var sent in Ed.Doc.Sentences)
            {
                var entities = sent.WordToNamedEntityMap.Where(o => o.Value.ID == ent.ID).ToList();
                removedEntities.Add(sent, entities);                
            }

            var removedFrameElementReferences = Ed./*DB*/Doc.GetAllElementReferences(ent.ID);
            
            Ed.Undo.PerformSimpleAction(
               () =>
               {
                    Ed./*DB*/Doc.NamedEntities.Remove(ent.ID);
                                                            
                    foreach (var sentEnt in removedEntities)
                    {
                        foreach (var fe in sentEnt.Value)
                        {
                            // ***
                            fe.Key.namedEntityID = -1;
                            sentEnt.Key.WordToNamedEntityMap.Remove(fe.Key);
                        }
                    }
                    
                    foreach (var sentElemRef in removedFrameElementReferences)
                    {
                        foreach (var it in sentElemRef.Value)
                        {
                            sentElemRef.Key.ElementReferences.Remove(it.Key);
                        }
                    }

                    markerControl.Invalidate();
                    panelSentences.Invalidate();
                    RefreshFrameEntityList();
                    RefreshNamedEntityList();
                },
                () =>
                {

                    Ed./*DB*/Doc.NamedEntities.Add(ent.ID, ent);

                    foreach (var sentEnt in removedEntities)
                    {
                        foreach (var fe in sentEnt.Value)
                        {
                            // ***
                            fe.Key.namedEntityID = fe.Value.ID;
                            sentEnt.Key.WordToNamedEntityMap.Add(fe.Key, fe.Value);
                        }
                    }
                   
                    foreach (var sentElemRef in removedFrameElementReferences)
                    {
                        foreach (var it in sentElemRef.Value)
                        {
                            sentElemRef.Key.ElementReferences.Add(it.Key, it.Value);
                        }
                    }
                    
                    selectedMarker = null;
                    selectedLink = null;
                    markerControl.Invalidate();
                    panelSentences.Invalidate();
                    RefreshFrameEntityList();
                    RefreshNamedEntityList();                    
                }
            );            
        }

        void Act_DeleteNamedEntityInstance(Word word)
        {
            var sent = GetSelectedSentence();
            var oldEnt = sent.WordToNamedEntityMap[word];
                           
            var sentFrames = sent.WordToNamedEntityMap.SelectMany(o => o.Value.Frames).ToList();
            var references = sentFrames.ToDictionary(o => o.ElementReferences, o => o.ElementReferences.Where(r => r.Value.WordIndex == word.Index).ToList());

            Ed.Undo.PerformSimpleAction(
                () =>
                {
                    // ***
                    word.namedEntityID = -1;
                    sent.WordToNamedEntityMap.Remove(word);
                    
                    foreach(var framePair in references)
                    {
                        foreach(var elRef in framePair.Value)
                        {
                            framePair.Key.Remove(elRef.Key);
                        }
                    }

                    markerControl.Invalidate();
                    panelSentences.Invalidate();
                    RefreshFrameEntityList();                    
                },
                () =>
                {                  
                    // ***
                    word.namedEntityID = oldEnt.ID;
                    sent.WordToNamedEntityMap.Add(word, oldEnt);

                    foreach (var framePair in references)
                    {
                        foreach (var elRef in framePair.Value)
                        {
                            framePair.Key.Add(elRef.Key, elRef.Value);
                        }
                    }

                    selectedMarker = null;
                    selectedLink = null;
                    markerControl.Invalidate();
                    panelSentences.Invalidate();
                    RefreshFrameEntityList();                    
                }
            );
        }
        
        private void tsmDeleteEntity_Click(object sender, EventArgs e)
        {
            DeleteSelectedNodeEntity(deleteAll:true);
        }
        

        private void tsmDeleteEntityInstance_Click(object sender, EventArgs e)
        {
            DeleteSelectedNodeEntity(deleteAll:false);
        }        

        private void tsmCreateNamedEntity_Click(object sender, EventArgs e)
        {
            if (selectedNode is ScreenTreeBox)
            {
                Act_CreateNamedEntity(selectedNode as ScreenTreeBox);
            }                
        }
      
        private void tsmImportConll_Click(object sender, EventArgs e)
        {
            OpenFileDialog opend = new OpenFileDialog();
            opend.Filter =
               "CoNLL (*.conll)|*.conll";

            opend.Title = "Open";
            opend.InitialDirectory = Directory.GetCurrentDirectory();
            DialogResult openres = opend.ShowDialog();

            if (openres == DialogResult.OK)
            {
                var lines = File.ReadAllLines(opend.FileName);
                var wordLines = new List<string>();
                var strSentences = new List<List<string>>();
                foreach(var line in lines)
                {
                    if(String.IsNullOrEmpty(line))
                    {
                        if(wordLines.Count > 0)
                        {
                            strSentences.Add(new List<string>(wordLines));
                            wordLines.Clear();
                        }                        
                    }
                    else
                    {
                        wordLines.Add(line);    
                    }                    
                }

                if(wordLines.Count > 0)
                {
                    strSentences.Add(new List<string>(wordLines));
                }

                var namedEntityMap = new Dictionary<int, int>();

                var sentences = strSentences.Select((sentence, i) => 
                {
                    var sent = new Sentence
                    {
                        ID = Path.GetFileNameWithoutExtension(opend.FileName) + (i + 1),                        
                    };

                    var wordFieldList = sentence.Select(word_line => word_line.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList()).ToList();

                    var wordMap = wordFieldList.Select((word_fields, word_index) => 
                            new Word
                            {
                                Index = word_index,
                                Original = word_fields[1],
                                Lemma = word_fields[2],
                                Morph1 = word_fields[3],
                                Morph2 = word_fields[4],
                                ParentIndex = int.Parse(word_fields[6] == "_"? "0" :word_fields[6]) - 1
                            })
                        .ToDictionary(o => o.Index, o => o);
                                                
                    foreach(var word in wordMap.Values)
                    {
                        wordMap.TryGetValue(word.ParentIndex, out word.Parent);
                    }

                    sent.Words = wordMap.Values.ToList();
                    sent.Text = String.Join(" ", wordMap.Values.Select(o => o.Original));

                    for(int word_index = 0; word_index < wordFieldList.Count; word_index++)
                    {
                        var wordFields = wordFieldList[word_index];

                        if(wordFields.Count == 13)
                        {
                            if(wordFields[10] != "_")
                            {
                                int localID = -1;
                                if(int.TryParse(wordFields[10], out localID))
                                {
                                    NamedEntity namedEntity = null;
                                    if(namedEntityMap.ContainsKey(localID))
                                    {
                                        namedEntity = Ed./*DB*/Doc.NamedEntities[namedEntityMap[localID]];
                                    }
                                    else
                                    {
                                        namedEntity = GetNewNamedEntity(new TreeNode(wordMap[word_index], sent.Words));
                                        namedEntityMap.Add(localID, namedEntity.ID);
                                        Ed./*DB*/Doc.NamedEntities.Add(namedEntity.ID, namedEntity);
                                    }
                                    
                                    if(wordFields[11] != "_")
                                    {
                                        namedEntity.Type = wordFields[11];
                                    }
                                    
                                    // ***
                                    wordMap[word_index].namedEntityID = namedEntity.ID;
                                    sent.WordToNamedEntityMap.Add(wordMap[word_index], namedEntity);
                                }                                
                            }
                        }
                    }                 
                    
                    return sent;
                }
                ).ToList();

                RefreshNamedEntityList();

                Ed.Undo.PerformSimpleAction
                (
                    () =>
                    {
                        Ed.Doc.Sentences.AddRange(sentences);
                        SetSentencePanelSize();
                        RefreshSelectedSentence();
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    },
                    () =>
                    {                        
                        foreach(var sent in sentences)
                        {
                            Ed.Doc.Sentences.Remove(sent);
                        }
                        Ed.SelectedSentenceIndex = 0;
                        RefreshSelectedSentence();
                        SetSentencePanelSize();                        
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    }
                );                  
            }
        }

        private void txtCurrentSentence_TextChanged(object sender, EventArgs e)
        {
            var txt = txtCurrentSentence.Text.Trim();
            var parts = txt.Split('/');
            if(parts.Count() == 2)
            {
                int first = 1;
                int second = 1;
                if(int.TryParse(parts[0], out first) && int.TryParse(parts[1], out second))
                {
                    if(first > 0 && first <= Ed.Doc.Sentences.Count)
                    {
                        Ed.SelectedSentenceIndex = first - 1;
                        RefreshSelectedSentence();                        
                    }
                }
            }
            else
            {
                int number = 1;
                if(int.TryParse(txt, out number))
                {
                    if (number > 0 && number <= Ed.Doc.Sentences.Count)
                    {
                        Ed.SelectedSentenceIndex = number - 1;
                        RefreshSelectedSentence();
                    }
                }
            }
        }

        private void tsmDeleteSentence_Click(object sender, EventArgs e)
        {
            if(Ed.Doc.Sentences.Count > 0)
            {
                Act_DeleteSentence(Ed.Doc.Sentences[Ed.SelectedSentenceIndex]);    
            }            
        }

        private void txtNamedEntitySearch_TextChanged(object sender, EventArgs e)
        {
            RefreshNamedEntityList();
        }

        // FIXME: this ain't working yet
        // Imports no pirmās semantic DB versijas uz otro
        private void tsmImportLegacy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature disabled.");
            return;

            OpenFileDialog opend = new OpenFileDialog();
            opend.Filter =
               "XML (*.xml)|*.xml";

            opend.Title = "Import Legacy";
            opend.InitialDirectory = Directory.GetCurrentDirectory();
            DialogResult openres = opend.ShowDialog();

            if (openres == DialogResult.OK)
            {
                var DB = new Legacy.SemanticDatabase();
                //DB.Open("SemanticDB.xml", Ed.Frames);
                
                foreach(var file in Directory.GetFiles(Path.GetDirectoryName(opend.FileName)))
                {
                    if(file != "SemanticDB.xml")
                    {
                        var doc = Legacy.Document.Open(opend.FileName, DB);        
                    }                    
                }
                
                
                /*
                Ed.Undo.PerformSimpleAction
                (
                    () =>
                    {
                        Ed.Doc.Sentences.AddRange(sentences);
                        SetSentencePanelSize();
                        RefreshSelectedSentence();
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    },
                    () =>
                    {
                        foreach (var sent in sentences)
                        {
                            Ed.Doc.Sentences.Remove(sent);
                        }
                        Ed.SelectedSentenceIndex = 0;
                        RefreshSelectedSentence();
                        SetSentencePanelSize();
                        markerControl.Invalidate();
                        panelSentences.Invalidate();
                    }
                );
                */
            }
        }

        private void tsmImportFrames_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature disabled.");
            return;

            OpenFileDialog opend = new OpenFileDialog();
            opend.Filter =
               "XML (*.xml)|*.xml";

            opend.Title = "Import Frames";
            opend.InitialDirectory = Directory.GetCurrentDirectory();
            DialogResult openres = opend.ShowDialog();

            if (openres == DialogResult.OK)
            {
                var frameInstances = new List<FrameInstance>();
                var xdoc = XDocument.Load(opend.FileName);
                if (xdoc != null)
                {                    
                    if (xdoc.Root.Element("Frames") != null)
                    {
                        foreach (var frameEl in xdoc.Root.Element("Frames").Elements("Frame"))
                        {
                            var frameInst = new FrameInstance();
                            //frameInst.TargetID = int.Parse(frameEl.Attribute("TargetID").Value);
                            frameInst.SentenceID = frameEl.Attribute("SentenceID").Value;
                            frameInst.WordIndex = int.Parse(frameEl.Attribute("WordIndex").Value);

                            if (frameEl.Attribute("TypeLV") != null)
                            {
                                frameInst.TypeLV = frameEl.Attribute("TypeLV").Value;
                            }

                            var type = frameEl.Attribute("Type").Value;
                            frameInst.Frame = Ed.Frames.FirstOrDefault(o => o.Name == type);

                            foreach (var elementEl in frameEl.Elements("Element"))
                            {
                                var elementName = elementEl.Attribute("Name").Value;
                                var frameElement = frameInst.Frame.Elements.FirstOrDefault(o => o.Name == elementName);

                                //var refId = int.Parse(elementEl.Attribute("RefID").Value);
                                var wordIndex = int.Parse(elementEl.Attribute("WordIndex").Value);
                                var elReference = new Reference {WordIndex = wordIndex};                                
                                frameInst.ElementReferences.Add(frameElement, elReference);
                            }

                            foreach (var markerEl in frameEl.Elements("Marker"))
                            {
                                frameInst.Marker = new LayoutMarker();
                                frameInst.Marker.X = int.Parse(markerEl.Attribute("X").Value);
                                frameInst.Marker.Y = int.Parse(markerEl.Attribute("Y").Value);                                
                            }

                            frameInstances.Add(frameInst);
                        }

                        foreach(var frameInstance in frameInstances)
                        {
                            // *** vajag atbrīvoties no SentenceID
                            //var sent = Ed.Doc.Sentences.FirstOrDefault(o => o.ID == frameInstance.SentenceID);
                            var sent = Ed.Doc.Sentences.FirstOrDefault(o => o.index == frameInstance.sentenceIndex);
                            if(sent != null)
                            {                                
                                NamedEntity namedEntity = CreateNamedEntityOrUseExisting(sent, frameInstance.WordIndex);
                                frameInstance.TargetID = namedEntity.ID;
                                namedEntity.Frames.Add(frameInstance);
                                // ***
                                sent.Frames.Add(frameInstance);

                                foreach(var reference in frameInstance.ElementReferences.Values)
                                {
                                    var referencedNamedEntity = CreateNamedEntityOrUseExisting(sent, reference.WordIndex);
                                    reference.ID = referencedNamedEntity.ID;
                                }
                            }
                        }

                        CreateMissingFrameMarkers();

                        Ed.Undo.SetDirty(true);
                        panelSentences.Refresh();
                        markerControl.Refresh();
                        RefreshFrameEntityList();
                        RefreshNamedEntityList();                        
                    }                    
                }
            }      
        }
        
        NamedEntity CreateNamedEntityOrUseExisting(Sentence sent, int wordIndex)
        {
            var word = sent.Words.FirstOrDefault(o => o.Index == wordIndex);
            NamedEntity namedEntity;

            if (sent.WordToNamedEntityMap.ContainsKey(word))
            {
                namedEntity = sent.WordToNamedEntityMap[word];
            }
            else
            {
                namedEntity = GetNewNamedEntity(new TreeNode(word, sent.Words));
                // ***
                word.namedEntityID = namedEntity.ID;
                sent.WordToNamedEntityMap.Add(word, namedEntity);
                Ed./*DB*/Doc.NamedEntities.Add(namedEntity.ID, namedEntity);
            }

            return namedEntity;
        }
    }

    public static class DoubleBuffering
    {
        public static void SetDoubleBuffered(this System.Windows.Forms.Control c)
        {
            //Taxes: Remote Desktop Connection and painting
            //http://blogs.msdn.com/oldnewthing/archive/2006/01/03/508694.aspx
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }
    }
}

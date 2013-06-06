using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace FrameMarker
{
    public class Document
    {
        public List<Sentence> Sentences = new List<Sentence>();

        static string GetValue(XAttribute attr)
        {
            if (attr == null)
                return "";
            else
                return attr.Value;
        }

        public bool Save(string filename)
        {            
            if (!String.IsNullOrEmpty(filename))
            {
                new XDocument(
                    new XElement("Sentences",
                        Sentences.Select(s =>
                            new XElement("Sentence",
                                new XAttribute("ID", s.ID),
                                new XAttribute("Text", s.Text),

                                new XElement("Words",
                                    s.Words.Select((o, i) =>
                                    {
                                        var el = new XElement("Word",
                                            new XAttribute("Index", i),
                                            new XAttribute("Original", o.Original),
                                            new XAttribute("Morph1", o.Morph1),
                                            new XAttribute("Morph2", o.Morph2),
                                            new XAttribute("Lemma", o.Lemma)
                                        );

                                        if (o.IsConstituent)
                                        {
                                            el.Add(new XAttribute("Constituent", o.ParentIndex));
                                        }
                                        else
                                        {
                                            el.Add(new XAttribute("Parent", o.ParentIndex));
                                        }
                                        return el;
                                    }
                                    )
                                ),

                                new XElement("NamedEntities",
                                    s.WordToNamedEntityMap.Select(o =>
                                        new XElement("NamedEntity",
                                            new XAttribute("ID", o.Value.ID),
                                            new XAttribute("WordIndex", o.Key.Index)
                                        )
                                    )
                                ),

                                new XElement("Markers",
                                    s.NamedEntityMarkers.Select(o =>
                                        new XElement("NamedEntityMarker",
                                            new XAttribute("ID", o.NamedEntity.ID),
                                            new XAttribute("X", o.Location.X),
                                            new XAttribute("Y", o.Location.Y)
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Save(filename);

                return true;
            }
            return false;
        }

        public static Document Open(string filename, SemanticDatabase DB)
        {
            if (!File.Exists(filename))
                return null;

            var xdoc = XDocument.Load(filename);
            if (xdoc != null)
            {
                var doc = new Document();
                foreach (var sentenceEl in xdoc.Root.Elements("Sentence"))
                {
                    var sent = new Sentence();
                    var namedEntityMarkers = new List<Tuple<int, int, int>>();
                    sent.Text = sentenceEl.Attribute("Text").Value;
                    sent.ID = sentenceEl.Attribute("ID").Value;

                    if (sentenceEl.Element("Words") != null)
                    {
                        foreach (var wordEl in sentenceEl.Element("Words").Elements("Word"))
                        {
                            var word = new Word();

                            word.Index = int.Parse(wordEl.Attribute("Index").Value);
                            word.Original = GetValue(wordEl.Attribute("Original"));
                            word.Morph1 = GetValue(wordEl.Attribute("Morph1"));
                            word.Morph2 = GetValue(wordEl.Attribute("Morph2"));
                            word.Lemma = GetValue(wordEl.Attribute("Lemma"));

                            if (wordEl.Attribute("Parent") != null)
                            {
                                if (!int.TryParse(GetValue(wordEl.Attribute("Parent")), out word.ParentIndex))
                                {
                                    word.ParentIndex = -1;
                                }
                            }
                            else
                            {
                                if (!int.TryParse(GetValue(wordEl.Attribute("Constituent")), out word.ParentIndex))
                                {
                                    word.ParentIndex = -1;
                                }
                                else
                                {
                                    word.IsConstituent = true;
                                }
                            }

                            sent.Words.Add(word);
                        }
                        sent.Words = sent.Words.OrderBy(o => o.Index).ToList();
                    }

                    if (sentenceEl.Element("NamedEntities") != null)
                    {
                        foreach (var entity in sentenceEl.Element("NamedEntities").Elements("NamedEntity"))
                        {
                            var ID = int.Parse(entity.Attribute("ID").Value);
                            var wordIndex = int.Parse(entity.Attribute("WordIndex").Value);
                            sent.WordToNamedEntityMap.Add(sent.Words[wordIndex], DB.NamedEntities[ID]);
                        }
                    }

                    if (sentenceEl.Element("Markers") != null)
                    {
                        foreach (var entity in sentenceEl.Element("Markers").Elements("NamedEntityMarker"))
                        {
                            var ID = int.Parse(entity.Attribute("ID").Value);
                            var x = int.Parse(entity.Attribute("X").Value);
                            var y = int.Parse(entity.Attribute("Y").Value);

                            namedEntityMarkers.Add(Tuple.Create(ID, x, y));
                        }
                    }

                    doc.Sentences.Add(sent);

                    foreach (var tuple in namedEntityMarkers)
                    {
                        sent.NamedEntityMarkers.Add(new NamedEntityMarker
                        {
                            Location = new Point(tuple.Item2, tuple.Item3),
                            NamedEntity = DB.NamedEntities[tuple.Item1]
                        });
                    }
                }
                
                return doc;
            }

            return null;
        }
    }

    public class Sentence
    {
        public string ID;
        public string Text;
        public List<Word> Words = new List<Word>();
        public Dictionary<Word, NamedEntity> WordToNamedEntityMap = new Dictionary<Word, NamedEntity>();
        public List<NamedEntityMarker> NamedEntityMarkers = new List<NamedEntityMarker>();

        public List<FrameInstance> GetFrames()
        {            
            return WordToNamedEntityMap.Values.SelectMany(o => o.Frames.Where(f => f.SentenceID == ID)).ToList();
        }

        public List<FrameInstanceMarker> GetFrameMarkers()
        {
            var sentFrames = GetFrames();
            var markers = sentFrames.Where(o => o.Marker != null)
                .Select(o =>
                    new FrameInstanceMarker
                    {
                        FrameInstance = o,
                        Location = new Point(o.Marker.X, o.Marker.Y)
                    }
                 ).ToList();

            return markers;
        }
    }

    public abstract class Marker
    {
        public virtual Point Location { get; set; }
    }
            
    
    public class NamedEntityMarker : Marker
    {
        public NamedEntity NamedEntity;        
    }
    
    public class FrameInstanceMarker : Marker
    {
        public override Point Location { 
            get{
                return new Point(FrameInstance.Marker.X, FrameInstance.Marker.Y);
            }            
            set { 
                FrameInstance.Marker.X = value.X;
                FrameInstance.Marker.Y = value.Y;
            }
        }
        public FrameInstance FrameInstance;
    }
    
    public class Word
    {
        public int Index;
        public string Original = "Vārds";
        public string Morph1 = "";
        public string Morph2 = "";
        public string Lemma = "";
        public int ParentIndex;
        public Word Parent;
        public bool IsConstituent;
    }
    
    [Serializable]
    public class NamedEntity
    {
        public int ID;
        public string Type;

        public string Name;
        public HashSet<string> AliasSet = new HashSet<string>();
        
        [NonSerialized]
        public List<FrameInstance> Frames = new List<FrameInstance>();
        public override string ToString()
        {
            var str = "[" + ID + "]:" + Type + " - " + Name;

            if (AliasSet.Count > 0)
                str += " - " + AliasSet.Aggregate((sum, s) => sum + ", " + s);

            return str;
        }
    }

    [Serializable]
    public class Reference
    {
        public int ID = -1;
        public int WordIndex = -1;

        public override bool Equals(object obj)
        {
            var reference = obj as Reference;
            if(reference != null)
            {
                return ID == reference.ID && WordIndex == reference.WordIndex;
            }
            return false;
        }
    }

    public class LayoutMarker
    {
        public int X, Y;
    }

        
    public class FrameInstance
    {        
        public Frame Frame;

        public string TypeLV;

        public Dictionary<FrameElement, Reference> ElementReferences = new Dictionary<FrameElement, Reference>();        
        public int TargetID;
                
        public string SentenceID;
        public int WordIndex;

        public LayoutMarker Marker;
    
        
        public override string ToString()
        {
            return "EV[" + TargetID + "]: " + Frame.Name; //+ " - " + Name;
        }        
    }
           
    public class SemanticDatabase
    {        
        public Dictionary<int, NamedEntity> NamedEntities = new Dictionary<int, NamedEntity>();
        
        public List<FrameInstance> GetAllFrameInstances()
        {
            return NamedEntities.SelectMany(o => o.Value.Frames).ToList();
        }

        public List<FrameInstance> GetSentenceFrameInstances(string sentenceID)
        {
            return NamedEntities.SelectMany(o => o.Value.Frames.Where(f => f.SentenceID == sentenceID)).ToList();
        }

        public Dictionary<FrameInstance, Dictionary<FrameElement, Reference>> GetElementReferences(List<FrameInstance> frames, int targetID)
        {
            return frames.ToDictionary(
                o => o,
                o => o.ElementReferences.Where(e => e.Value.ID == targetID).ToDictionary(p => p.Key, p => p.Value)
            );
        }

        public Dictionary<FrameInstance, Dictionary<FrameElement, Reference>> GetAllElementReferences(int targetID)
        {
            return GetElementReferences(GetAllFrameInstances(), targetID);
        }


        public void Save(string filename)
        {
            var frameInstances = NamedEntities.Values.SelectMany(o => o.Frames).ToList();

            if (!String.IsNullOrEmpty(filename))
            {
                new XDocument(
                    new XElement("SemanticDB",                        
                        new XElement("NamedEntities",
                            NamedEntities.Select(o =>
                                new XElement("NamedEntity",
                                    new XAttribute("ID", o.Value.ID),
                                    new XAttribute("Type", o.Value.Type),
                                    new XElement("PrimeAlias",
                                        new XAttribute("Label", o.Value.Name)                                      
                                    ),
                                    o.Value.AliasSet.Select(e =>
                                        new XElement("Alias",
                                            new XAttribute("Label", e)
                                        )
                                    )
                                )
                            )
                        ),
                        new XElement("Frames",
                            frameInstances.Select(o =>
                                new XElement("Frame",
                                    new XAttribute("TargetID", o.TargetID),
                                    new XAttribute("Type", o.Frame.Name),
                                    new XAttribute("TypeLV", o.Frame.NameLV ?? ""),
                                    new XAttribute("SentenceID", o.SentenceID),
                                    new XAttribute("WordIndex", o.WordIndex),                                
                                    o.ElementReferences.Select(e =>
                                        new XElement("Element",
                                            new XAttribute("Name", e.Key.Name),
                                            new XAttribute("NameLV", e.Key.Name),
                                            new XAttribute("RefID", e.Value.ID),
                                            new XAttribute("WordIndex", e.Value.WordIndex)
                                        )
                                    ),
                                    o.Marker == null?
                                        null : new XElement("Marker", 
                                            new XAttribute("X", o.Marker.X), 
                                            new XAttribute("Y", o.Marker.Y))
                                )
                            ) 
                        )
                        
                    )
                ).Save(filename);                
            }
        }

        public void Open(string filename, List<Frame> frames)
        {
            var frameInstances=  new Dictionary<int, FrameInstance>();
            var xdoc = XDocument.Load(filename);
            if (xdoc != null)
            {                                                                
                foreach (var entityEl in xdoc.Root.Element("NamedEntities").Elements("NamedEntity"))
                {

                    var entity = new NamedEntity();
                    entity.ID = int.Parse(entityEl.Attribute("ID").Value);
                    entity.Name = entityEl.Element("PrimeAlias").Attribute("Label").Value;
                    entity.Type = entityEl.Attribute("Type").Value;

                    foreach (var aliasEl in entityEl.Elements("Alias"))
                    {
                        entity.AliasSet.Add(aliasEl.Attribute("Label").Value);
                    }

                    NamedEntities.Add(entity.ID, entity);                    
                }

                if (xdoc.Root.Element("Frames") != null)
                {
                    foreach (var frameEl in xdoc.Root.Element("Frames").Elements("Frame"))
                    {
                        var frameInst = new FrameInstance();
                        frameInst.TargetID = int.Parse(frameEl.Attribute("TargetID").Value);
                        frameInst.SentenceID = frameEl.Attribute("SentenceID").Value;
                        frameInst.WordIndex = int.Parse(frameEl.Attribute("WordIndex").Value);
                        
                        if (frameEl.Attribute("TypeLV") != null)
                        {
                            frameInst.TypeLV = frameEl.Attribute("TypeLV").Value;    
                        }
                        
                        var type = frameEl.Attribute("Type").Value;
                        frameInst.Frame = frames.FirstOrDefault(o => o.Name == type);

                        foreach (var elementEl in frameEl.Elements("Element"))
                        {
                            var elementName = elementEl.Attribute("Name").Value;
                            var frameElement = frameInst.Frame.Elements.FirstOrDefault(o => o.Name == elementName);

                            var refId = int.Parse(elementEl.Attribute("RefID").Value);
                            var wordIndex = int.Parse(elementEl.Attribute("WordIndex").Value);
                            var elReference = new Reference {ID = refId, WordIndex = wordIndex};
                            frameInst.ElementReferences.Add(frameElement, elReference);                            
                        }

                        foreach (var markerEl in frameEl.Elements("Marker"))
                        {
                            frameInst.Marker = new LayoutMarker();
                            frameInst.Marker.X = int.Parse(markerEl.Attribute("X").Value);
                            frameInst.Marker.Y = int.Parse(markerEl.Attribute("Y").Value);
                        }

                        frameInstances.Add(frameInst.TargetID, frameInst);
                    }                                        
                }   
             
                foreach(var frameInst in frameInstances)
                {
                    NamedEntities[frameInst.Value.TargetID].Frames.Add(frameInst.Value);
                }
            }            
        }
        
        public int GetNewEntityID()
        {
            if(NamedEntities.Count > 0)
            {
                return NamedEntities.Values.Max(o => o.ID) + 1;    
            }

            return 1;
        }
        
    }                   
}

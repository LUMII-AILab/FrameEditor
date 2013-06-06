using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using FrameMarker;

namespace Legacy
{
    public class Document
    {
        public List<Sentence> Sentences = new List<Sentence>();
       
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

                                new XElement("FrameEntities",
                                    s.Entities.Where(o => o.Value is FrameEntity).Select(o =>
                                        new XElement("FrameEntity",
                                            new XAttribute("ID", o.Value.ID),
                                            new XAttribute("WordIndex", o.Key.Index)
                                        )
                                    )
                                ),

                                new XElement("NamedEntities",
                                    s.Entities.Where(o => o.Value is NamedEntity).Select(o =>
                                        new XElement("NamedEntity",
                                            new XAttribute("ID", o.Value.ID),
                                            new XAttribute("WordIndex", o.Key.Index)
                                        )
                                    )
                                ),

                                new XElement("Markers",
                                    s.Markers.OfType<FrameEntityMarker>().Select(o =>
                                        new XElement("FrameEntityMarker",
                                            new XAttribute("ID", o.FrameEntity.ID),
                                            new XAttribute("X", o.Location.X),
                                            new XAttribute("Y", o.Location.Y)
                                        )
                                    ),
                                    s.Markers.OfType<NamedEntityMarker>().Select(o =>
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

                    if (sentenceEl.Element("FrameEntities") != null)
                    {
                        foreach (var entity in sentenceEl.Element("FrameEntities").Elements("FrameEntity"))
                        {
                            var ID = int.Parse(entity.Attribute("ID").Value);
                            var wordIndex = int.Parse(entity.Attribute("WordIndex").Value);
                            sent.Entities.Add(sent.Words[wordIndex], DB.FrameEntities[ID]);
                        }
                    }

                    if (sentenceEl.Element("NamedEntities") != null)
                    {
                        foreach (var entity in sentenceEl.Element("NamedEntities").Elements("NamedEntity"))
                        {
                            var ID = int.Parse(entity.Attribute("ID").Value);
                            var wordIndex = int.Parse(entity.Attribute("WordIndex").Value);
                            sent.Entities.Add(sent.Words[wordIndex], DB.NamedEntities[ID]);
                        }
                    }

                    if (sentenceEl.Element("Markers") != null)
                    {
                        foreach (var entity in sentenceEl.Element("Markers").Elements("FrameEntityMarker"))
                        {
                            var ID = int.Parse(entity.Attribute("ID").Value);
                            var x = int.Parse(entity.Attribute("X").Value);
                            var y = int.Parse(entity.Attribute("Y").Value);
                            sent.Markers.Add(
                                new FrameEntityMarker
                                {
                                    Location = new Point(x, y),
                                    FrameEntity = DB.FrameEntities.Values.FirstOrDefault(o => o.ID == ID)
                                }
                            );
                        }

                        foreach (var entity in sentenceEl.Element("Markers").Elements("NamedEntityMarker"))
                        {
                            var ID = int.Parse(entity.Attribute("ID").Value);
                            var x = int.Parse(entity.Attribute("X").Value);
                            var y = int.Parse(entity.Attribute("Y").Value);
                            sent.Markers.Add(
                                new NamedEntityMarker
                                {
                                    Location = new Point(x, y),
                                    NamedEntity = DB.NamedEntities.Values.FirstOrDefault(o => o.ID == ID)
                                }
                            );
                        }
                    }

                    doc.Sentences.Add(sent);
                }

                return doc;
            }

            return null;
        }

        static string GetValue(XAttribute attr)
        {
            if (attr == null)
                return "";
            else
                return attr.Value;
        }

    }

    public class Sentence
    {
        public string ID;
        public string Text;
        public List<Word> Words = new List<Word>();
        public Dictionary<Word, Entity> Entities = new Dictionary<Word, Entity>();
        public List<Marker> Markers = new List<Marker>();

        public Dictionary<Word, FrameEntity> GetFrameEntities()
        {
            return Entities.Where(o => o.Value is FrameEntity).ToDictionary(o => o.Key, o => o.Value as FrameEntity);
        }

        public Dictionary<Word, NamedEntity> GetNamedEntities()
        {
            return Entities.Where(o => o.Value is NamedEntity).ToDictionary(o => o.Key, o => o.Value as NamedEntity);
        }

    }

    public abstract class Marker
    {
        public Point Location;
    }

    public class NamedEntityMarker : Marker
    {
        public NamedEntity NamedEntity;
    }

    public class FrameEntityMarker : Marker
    {
        public FrameEntity FrameEntity;
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
    public class Entity
    {
        public int ID;
    }

    [Serializable]
    public class NamedEntity : Entity
    {
        public string Type;

        public string Name;
        public HashSet<string> AliasSet = new HashSet<string>();

        public override string ToString()
        {
            var str = "[" + ID + "]:" + Type + " - " + Name;

            if (AliasSet.Count > 0)
                str += " - " + AliasSet.Aggregate((sum, s) => sum + ", " + s);

            return str;
        }
    }

    public class FrameEntity : Entity
    {
        public Frame Frame;

        public string Name;
        public Dictionary<FrameElement, int> ElementReferences = new Dictionary<FrameElement, int>();

        public override string ToString()
        {
            return "EV[" + ID + "]: " + Frame.Name + " - " + Name;
        }
    }

    public class SemanticDatabase
    {
        public Dictionary<int, Entity> Entities = new Dictionary<int, Entity>();
        public Dictionary<int, NamedEntity> NamedEntities = new Dictionary<int, NamedEntity>();
        public Dictionary<int, FrameEntity> FrameEntities = new Dictionary<int, FrameEntity>();

        public void Save(string filename)
        {
            if (!String.IsNullOrEmpty(filename))
            {
                new XDocument(
                    new XElement("SemanticDB",
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
                        ),
                        FrameEntities.Select(o =>
                            new XElement("FrameEntity",
                                new XAttribute("ID", o.Value.ID),
                                new XAttribute("Type", o.Value.Frame.Name),
                                new XElement("PrimeAlias",
                                    new XAttribute("Label", o.Value.Name)
                                ),
                                o.Value.ElementReferences.Select(e =>
                                    new XElement("Element",
                                        new XAttribute("Name", e.Key.Name),
                                        new XAttribute("RefID", e.Value)
                                    )
                                )
                            )
                        )
                    )
                ).Save(filename);
            }
        }

        public void Open(string filename, List<Frame> frames)
        {
            var xdoc = XDocument.Load(filename);
            if (xdoc != null)
            {
                foreach (var entityEl in xdoc.Root.Elements("NamedEntity"))
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
                    Entities.Add(entity.ID, entity);
                }

                foreach (var entityEl in xdoc.Root.Elements("FrameEntity"))
                {
                    var entity = new FrameEntity();
                    entity.ID = int.Parse(entityEl.Attribute("ID").Value);
                    entity.Name = entityEl.Element("PrimeAlias").Attribute("Label").Value;

                    var type = entityEl.Attribute("Type").Value;
                    entity.Frame = frames.FirstOrDefault(o => o.Name == type);

                    foreach (var elementEl in entityEl.Elements("Element"))
                    {
                        var elementName = elementEl.Attribute("Name").Value;
                        var frameElement = entity.Frame.Elements.FirstOrDefault(o => o.Name == elementName);
                        entity.ElementReferences.Add(frameElement, int.Parse(elementEl.Attribute("RefID").Value));
                    }

                    FrameEntities.Add(entity.ID, entity);
                    Entities.Add(entity.ID, entity);
                }
            }
        }

        public int GetNewEntityID()
        {
            if (Entities.Count > 0)
            {
                return Entities.Values.Max(o => o.ID) + 1;
            }

            return 1;
        }
    }
   
}

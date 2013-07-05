using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace FrameMarker
{
    public class NamedEntityMarkerJsonConverter : JsonConverter
    {
        Dictionary<int, NamedEntity> NamedEntities;

        public NamedEntityMarkerJsonConverter(Dictionary<int, NamedEntity> namedEntities)
        {
            NamedEntities = namedEntities;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var marker = value as NamedEntityMarker;

            var jobj = new JObject();

            jobj["id"] = marker.NamedEntity.ID;
            jobj["x"] = marker.Location.X;
            jobj["y"] = marker.Location.Y;

            serializer.Serialize(writer, jobj);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jobj = serializer.Deserialize<JObject>(reader);

            var marker = new NamedEntityMarker {
                Location = new Point((int)jobj["x"], (int)jobj["y"]),
                NamedEntity = NamedEntities[(int)jobj["id"]]
            };

            return marker;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NamedEntityMarker);
        }
    }

    public class NamedEntitiesJsonConverter : JsonConverter
    {
        Dictionary<int, NamedEntity> NamedEntities;

        public NamedEntitiesJsonConverter(Dictionary<int, NamedEntity> namedEntities)
        {
            NamedEntities = namedEntities;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jNamedEntities = new JObject();

            foreach (var item in NamedEntities)
            {
                var namedEntity = item.Value as NamedEntity;

                var jNamedEntity = new JObject();
                jNamedEntity["id"] = namedEntity.ID;
                jNamedEntity["type"] = namedEntity.Type;

                var aliases = new JArray();
                aliases.Add(namedEntity.Name);
                foreach (var alias in namedEntity.AliasSet)
                    aliases.Add(alias);

                jNamedEntity["aliases"] = aliases;

                jNamedEntities[item.Key.ToString()] = jNamedEntity;
            }

            serializer.Serialize(writer, jNamedEntities);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // ielasošo konvertāciju veic izsaucošajā deserializācijā
            return existingValue;

            /*
            //return NamedEntities;

            //var NamedEntities = (Dictionary<int, NamedEntity>)existingValue;

            var jNamedEntities = serializer.Deserialize<JObject>(reader);

            //foreach (var ss in jNamedEntities)
            //    Debug.WriteLine(ss);

            //Debug.WriteLine("NAMED ENTITIES:");
            //foreach (var jNamedEntity in jNamedEntities)
            foreach (var item in jNamedEntities)
            {
                var jNamedEntity = item.Value;
                int id = int.Parse(item.Key);

                //int id = (int)jNamedEntity["id"];

                var entity = new NamedEntity();
                entity.ID = (int)jNamedEntity["id"];
                entity.Type = (string)jNamedEntity["type"];

                JArray jAliases = (JArray)jNamedEntity["aliases"];
                entity.Name = (string)jAliases[0];

                //Debug.WriteLine(entity.Name);

                for (int i = 1; i < jAliases.Count; i++)
                    entity.AliasSet.Add((string)jAliases[i]);

                NamedEntities.Add(entity.ID, entity);
            }

            return NamedEntities;
            //return existingValue;
            */
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<int, NamedEntity>);
        }
    }

    public class FrameJsonConverter : JsonConverter
    {
        public static List<Frame> Frames;
        public static JsonSerializer jsonSerializer;

        Dictionary<int, NamedEntity> NamedEntities;

        public FrameJsonConverter(Dictionary<int, NamedEntity> namedEntities)
        {
            NamedEntities = namedEntities;
            jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });
            jsonSerializer.Converters.Add(new FrameElementsJsonConverter());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var frame = value as FrameInstance;

            JObject jframe = JObject.FromObject(value, jsonSerializer);

            jframe["type"] = frame.Frame.Name;

            serializer.Serialize(writer, jframe);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            /*
            var type = serializer.Deserialize<string>(reader);

            var Frame = Frames.FirstOrDefault(o => o.Name == type);
            Debug.WriteLine("FRAME:");
            Debug.WriteLine(Frame);
            serializer.Context = new StreamingContext(StreamingContextStates.All, Frame);
            return Frame;
            */

            JObject jObject = serializer.Deserialize<JObject>(reader);

            string type = (string)jObject["type"];
            var Frame = Frames.FirstOrDefault(o => o.Name == type);
            jsonSerializer.Context = new StreamingContext(StreamingContextStates.All, Frame);

            //serializer.Context = new StreamingContext(StreamingContextStates.All, (Frame)value);
            //jsonSerializer.Context = new StreamingContext(StreamingContextStates.All, (Frame)value);
            
            //jObject.Remove();

            FrameInstance frame = jObject.ToObject<FrameInstance>(jsonSerializer);

            NamedEntities[frame.TargetID].Frames.Add(frame);

            frame.Frame = Frame;
            return frame;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FrameInstance);
        }
    }

    public class FrameElementsJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var elements = new JArray();
            var ElementReferences = value as Dictionary<FrameElement, Reference>;

            foreach (var item in ElementReferences)
            {
                var element = new JObject();
                var reference = item.Value as Reference;
                var frameElement = item.Key as FrameElement;

                if(reference.WordIndex > 0)
                    element["tokenIndex"] = reference.WordIndex;
                if(reference.ID != -1)
                    element["namedEntityID"] = reference.ID;
                element["name"] = frameElement.Name;
                if(frameElement.NameLV != null && frameElement.NameLV.Length > 0)
                    element["nameLV"] = frameElement.NameLV;

                elements.Add(element);
            }

            serializer.Serialize(writer, elements);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Debug.WriteLine(existingValue);
            var ElementReferences = (Dictionary<FrameElement, Reference>)existingValue;

            var jElements = serializer.Deserialize<JArray>(reader);

            // laikam nevajag, jo pavisam svaigs
            //ElementReferences.Clear();

            var Frame = (Frame)serializer.Context.Context;
            //Debug.WriteLine(typeof(Frame));

            //Debug.WriteLine(String.Format("FRAME {0} ELEMENTS:", Frame.Name));
            foreach (var jElement in jElements)
            {
                int wordIndex = -1;
                if(jElement["tokenIndex"] != null)
                    wordIndex = (int)jElement["tokenIndex"];

                int namedEntityID = -1;
                if(jElement["namedEntityID"] != null)
                    namedEntityID = (int)jElement["namedEntityID"];

                string name = (string)jElement["name"];

                //Debug.WriteLine(String.Format("   {0}", name));

                //Debug.WriteLine(jElement);
                //Debug.WriteLine(String.Format("ID = {0}, wordIndex = {1}", namedEntityID, wordIndex));

                var frameElement = Frame.Elements.FirstOrDefault(o => o.Name == name);
                var elReference = new Reference { ID = namedEntityID, WordIndex = wordIndex };
                ElementReferences.Add(frameElement, elReference);
            }

            //Debug.WriteLine(String.Format("ElementReferences.Count = {0}", ElementReferences.Count));

            return existingValue;
        }

        public override bool CanConvert(Type objectType)
        {
            // teorētiski te varētu būt vēl citi freima elementu uzskaitīšanas veidi
            return objectType == typeof(Dictionary<FrameElement, Reference>);
        }
    }

    /* // sākotnējā variantā vajadzēja reģistrēt konvertorus
    public class ConverterRegistration
    {
        // see: http://www.kinlan.co.uk/2006/10/assigining-typeconverter-to-class-you.html
        public static void Register<T, TC>() //where TC : TypeConverter
        {
            Attribute[] attr = new Attribute[1];
            TypeConverterAttribute vConv = new TypeConverterAttribute(typeof(TC));
            attr[0] = vConv;
            TypeDescriptor.AddAttributes(typeof(T), attr);
        }

        class PointXYConverter : JsonConverter
        {
            public override void WriteJson(
                JsonWriter writer, object value, JsonSerializer serializer)
            {
                var point = (Point)value;

                serializer.Serialize(
                    writer, new JObject { { "X", point.X }, { "Y", point.Y } });
            }

            public override object ReadJson(
                JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                var jObject = serializer.Deserialize<JObject>(reader);

                return new Point((int)jObject["X"], (int)jObject["Y"]);
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Point);
            }
        }

        public static void Register()
        {
            Register<Point, PointXYConverter>();
        }
    }
    */

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DocumentInfo
    {
        [JsonProperty("uid")]
        string uid;

        [JsonProperty("title")]
        string title;

        // TODO: convert to real date object or don't bother ?
        [JsonProperty("date")]
        string date;

        [JsonProperty("language")]
        string language;

        [JsonProperty("type")]
        string type;

    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Document
    {
        // ---------------------- Copied from SemanticDatabase ------------------------

        // sākotnējais variants
        //[JsonProperty("namedEntities"), JsonConverter(typeof(NamedEntitiesJsonConverter))] 
        //public Dictionary<int, NamedEntity> NamedEntities = new Dictionary<int, NamedEntity>();

        [JsonProperty("namedEntities")] 
        public Dictionary<int, NamedEntity> NamedEntities = new Dictionary<int, NamedEntity>();

        [JsonProperty("sentences")] 
        public List<Sentence> Sentences = new List<Sentence>();

        [JsonProperty("document")] 
        public DocumentInfo info;

        public List<FrameInstance> GetAllFrameInstances()
        {
            return NamedEntities.SelectMany(o => o.Value.Frames).ToList();
        }

        // netiek izmantots!
        //public List<FrameInstance> GetSentenceFrameInstances(string sentenceID)
        //{
        //    return NamedEntities.SelectMany(o => o.Value.Frames.Where(f => f.SentenceID == sentenceID)).ToList();
        //}

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

        public int GetNewEntityID()
        {
            if(NamedEntities.Count > 0)
            {
                return NamedEntities.Values.Max(o => o.ID) + 1;    
            }

            return 1;
        }

        // ------------------ END OF Copied from SemanticDatabase ---------------------


        /* // tika izmantots .xml ielādei
        static string GetValue(XAttribute attr)
        {
            if (attr == null)
                return "";
            else
                return attr.Value;
        } */

        public bool Save(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                return false;

            using (StreamWriter file = File.CreateText(filename))
            {
                JsonTextWriter writer = new JsonTextWriter(file);
                writer.Formatting = Formatting.Indented;
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });

                serializer.Converters.Add(new NamedEntityMarkerJsonConverter(NamedEntities));
                serializer.Converters.Add(new FrameJsonConverter(NamedEntities));
                serializer.Converters.Add(new NamedEntitiesJsonConverter(NamedEntities));

                serializer.Serialize(writer, this);
            }

            return true;

            /*
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
            */
        }

        public static Document Open(string filename/*, SemanticDatabase DB*/)
        {
            if (!File.Exists(filename))
                return null;

            /*
            Debug.WriteLine("type descriptor");
            Debug.WriteLine(TypeDescriptor.GetConverter(typeof(Point)));

            Point p = new Point();
            p.X = 4;
            p.Y = 6;

            string tt;
            tt = @"{'X': 99, 'Y':88}";
            //tt = @"'99, 88'";
            Debug.WriteLine(JsonConvert.DeserializeObject<Point>(tt));
            */

            Document doc;
            Dictionary<int, NamedEntity> namedEntities = new Dictionary<int, NamedEntity>();

            using (StreamReader file = File.OpenText(filename))
            {
                JsonSerializer serializer = new JsonSerializer();

                JObject jDocument = (JObject)serializer.Deserialize(file, typeof(JObject));

                JObject jNamedEntities = (JObject)jDocument["namedEntities"];
                jDocument.Remove("namedEntities");

                //foreach (var ss in jNamedEntities)
                //    Debug.WriteLine(ss);

                //Debug.WriteLine("NAMED ENTITIES:");
                foreach (var item in jNamedEntities)
                {
                    var jNamedEntity = item.Value;
                    int id = int.Parse(item.Key);

                    //int id = (int)jNamedEntity["id"];

                    var entity = new NamedEntity();
                    entity.ID = (int)jNamedEntity["id"];
                    entity.Type = (string)jNamedEntity["type"];

                    JArray jAliases = (JArray)jNamedEntity["aliases"];
                    entity.Name = (string)jAliases[0];

                    //Debug.WriteLine(entity.Name);

                    for (int i = 1; i < jAliases.Count; i++)
                        entity.AliasSet.Add((string)jAliases[i]);

                    namedEntities.Add(entity.ID, entity);
                }

                serializer.Converters.Add(new NamedEntityMarkerJsonConverter(namedEntities));
                serializer.Converters.Add(new FrameJsonConverter(namedEntities));
                //serializer.Converters.Add(new NamedEntitiesJsonConverter(namedEntities));

                // citi deserializēšanas veidi
                //doc = JsonConvert.DeserializeObject<Document>(file.ReadToEnd(), new JsonSerializerSettings() { ContractResolver = new OrderedContractResolver() });//, JsonSerializerSettings);
                //doc = (Document)serializer.Deserialize(file, typeof(Document));

                doc = jDocument.ToObject<Document>(serializer);
                doc.NamedEntities = namedEntities;
            }

            /*
            // salinko parentus (rīks to jau dara, tikai vēlāk)
            foreach (Sentence sentence in doc.Sentences)
            {
                // parentu saraksts pēc indeksiem
                Dictionary<int,Word> tokens = new Dictionary<int,Word>();
                foreach (Word token in sentence.Words)
                    tokens.Add(token.Index, token);

                foreach (Word token in sentence.Words)
                {
                    if (token.ParentIndex > 0)
                    {
                        Word parent = tokens[token.ParentIndex];
                        if (parent != null)
                            token.Parent = parent;
                        else
                            token.Parent = null;
                    }
                }
            }
            */

            int sentenceIndex = 0;
            foreach (Sentence sentence in doc.Sentences)
            {
                sentence.index = sentenceIndex;

                foreach (Word token in sentence.Words)
                {
                    if (token.namedEntityID != -1)
                        sentence.WordToNamedEntityMap.Add(sentence.Words.First<Word>(o => o.Index == token.Index), namedEntities[token.namedEntityID]);
                }

                // Sakārto augošā secībā, bet ko darīt, ja kāds no indeksiem pa vidu iztrūkst ?
                sentence.Words = sentence.Words.OrderBy(o => o.Index).ToList();

                foreach (FrameInstance frame in sentence.Frames)
                {
                    frame.sentenceIndex = sentenceIndex;
                    //Debug.WriteLine(frame);
                    //Debug.WriteLine(String.Format("ElementReferences.Count = {0}", frame.ElementReferences.Count));
                }

                sentenceIndex++;
            }

            return doc;

            /*
            if (!File.Exists(filename))
                return null;

            var xdoc = XDocument.Load(filename);
            if (xdoc != null)
            {
                var doc_ = new Document();
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
                            // FIXME: šī ir static funkcija, kas izveido instanci, ar tās NamedEntities objektu tad ir jāstrādā
                            // pagaidām izkomentē
                            //sent.WordToNamedEntityMap.Add(sent.Words[wordIndex], DB.NamedEntities[ID]);
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
                            // FIXME: tieši tā pati situācija, izkomentē, kas ir NamedEntity bez objekta ?
                            //NamedEntity = DB.NamedEntities[tuple.Item1]
                            // ok, tas ir kaut kāds objekta pieraksta veids, NamedEntity šeit laikam būs dictionary atslēga
                        });
                    }
                }
                
                return doc;
            }

            return null;
            */
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]       // iekļauj tikai tos, kam ir JsonProperty atribūts
    //[JsonObject(MemberSerialization = MemberSerialization.OptOut)]    // iekļauj visus izņemot ar JsonIgnore atribūtu
    public class Sentence
    {
        // Identifikācijas nolūkiem (mērķis ir atbrīvoties no SentenceID vismaz kā no obligāta satura):
        // tā kā šis redaktors nav spējīgs pats pieveinot vai dzēst teikumus,
        // tad vajadzētu pietikt ar autoģenerētu indeksu vien, lai unikāli identificētu teikumus.
        public int index;

        // vai sentence id vispār vajag ?
        [JsonProperty("id")] 
        public string ID;

        [JsonProperty("text")] 
        public string Text;

        [JsonProperty("tokens")] 
        public List<Word> Words = new List<Word>();

        // freimu instances pieder teikumam
        [JsonProperty("frames")] 
        public List<FrameInstance> Frames = new List<FrameInstance>();

        // aizpilda pēc automātiskās deserializācijas
        public Dictionary<Word, NamedEntity> WordToNamedEntityMap = new Dictionary<Word, NamedEntity>();

        [JsonProperty("detachedNamedEntityMarkers")] 
        public List<NamedEntityMarker> NamedEntityMarkers = new List<NamedEntityMarker>();

        public List<FrameInstance> GetFrames()
        {            
            // lai varētu atbrīvoties no SentenceID
            //return WordToNamedEntityMap.Values.SelectMany(o => o.Frames.Where(f => f.SentenceID == ID)).ToList();
            return WordToNamedEntityMap.Values.SelectMany(o => o.Frames.Where(f => f.sentenceIndex == index)).ToList();
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
        [JsonProperty("index")] 
        public int Index;

        [JsonProperty("form")] 
        public string Original = "Vārds";
        
        // TODO: kurš ir kurš: Morph1 vai Morph2 ?
        [JsonProperty("pos")] 
        public string Morph1 = "";

        [JsonProperty("tag")] 
        public string Morph2 = "";

        [JsonProperty("lemma")] 
        public string Lemma = "";

        [JsonProperty("parentIndex")] 
        public int ParentIndex;

        [NonSerialized]
        public Word Parent;

        // kas tas ir ?
        [JsonProperty("constituent")]
        public bool IsConstituent;

        //[NonSerialized]
        [JsonProperty("namedEntityID"), DefaultValue(-1)]
        public int namedEntityID = -1;
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
        [JsonProperty("x")] 
        public int X;

        [JsonProperty("y")] 
        public int Y;
    }

        
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]       // iekļauj tikai tos, kam ir JsonProperty atribūts
    //[JsonObject(MemberSerialization = MemberSerialization.OptOut)]    // iekļauj visus izņemot ar JsonIgnore atribūtu
    public class FrameInstance
    {
        // NOTE: sakarā ar to, ka JsonProperty(Order = -2) nedarbojas kā tas būtu sagaidāms
        // (skat.: http://james.newtonking.com/projects/json/help/index.html?topic=html/JsonPropertyOrder.htm),
        // ir nepieciešama rūpīgāka pieeja, kas neaprobežojas tikai ar atbilstoša konvertora izstrādi, tāpēc šo ceļu iet nevar.
        //[JsonProperty(PropertyName = "type"), JsonConverter(typeof(FrameJsonConverter))] 
        public Frame Frame;

        [JsonProperty("typeLV")] 
        public string TypeLV {
            get {
                return Frame.NameLV;
            }
            set {
                // TODO: vai šis vispār ir vajadzīgs ? Varbūt vienīgi, ja Frame.NameLV atšķiras, bet Frame nav "instances" tipa objekts.
            }
        }

        [JsonProperty("elements"), JsonConverter(typeof(FrameElementsJsonConverter))]
        public Dictionary<FrameElement, Reference> ElementReferences = new Dictionary<FrameElement, Reference>();        

        [JsonProperty("namedEntityID")] 
        public int TargetID;
                
        // šis nav vajadzīgs
        //[JsonIgnore]
        public string SentenceID;
        public int sentenceIndex;       // jaunais teikumu identifikācijas veids

        [JsonProperty("tokenIndex")] 
        public int WordIndex;

        [JsonProperty("marker")] 
        public LayoutMarker Marker;

        public override string ToString()
        {
            return "EV[" + TargetID + "]: " + Frame.Name; //+ " - " + Name;
        }        
    }

           
    /* public class SemanticDatabase
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
    */
}

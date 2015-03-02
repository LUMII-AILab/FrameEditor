using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;

namespace FrameMarker
{
    class Editor
    {
        public bool IsNew = true;
        public string Filename = "untitled";
        public Document Doc = new Document();
        //public SemanticDatabase DB = new SemanticDatabase();
        public UndoEngine Undo = new UndoEngine();

        public List<Frame> Frames = new List<Frame>();
        
        public bool IsCtrlPressed;
        public bool IsShiftPressed;
        public bool IsAltPressed;
        public Point MoveStartPos;

        public int SelectedSentenceIndex;

        //////////////////////////////////////////////////////////////////////////////////////                                
        public void CreateDefaultSentence()
        {
            Doc.Sentences.Add(
                new Sentence
                {
                    ID = "S1",
                    Text = "Labvakar!",
                    Words = new List<Word>
                    {
                        new Word{Index = 0, 
                            Original = "Labvakar!", 
                            Lemma= "Labvakar",
                            ParentIndex = -1
                        }
                    }
                }
            );
        }
        //////////////////////////////////////////////////////////////////////////////////////                                
        public void LoadFramesCSV(string text/*filename*/)
        {        
            /*var text = File.ReadAllText(filename);*/
            Frames = new List<Frame>();
            /*
            //var text = File.ReadAllText(filename);
            Frames.AddRange(
                text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Split(',').Select(s => s.Trim()))
                .Select(o => new Frame(o.First(), o.Skip(1).ToArray()))
            );*/

            /*
            // en-only variants
            var frames = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Split(',').Select(s => s.Trim()).ToList())
                .GroupBy(o => o.First(), o => o.Skip(1).ToList());

            foreach (var frameGroup in frames)
            {
                var frameElems = frameGroup
                    .Where(o => o.First() == "mFE")
                    .Select(o => new FrameElement { Name = o[2], Type = o[1] })
                    .ToArray();

                var frame = new Frame(frameGroup.Key, frameElems);

                Frames.Add(frame);
            }
            */

            // en & lv variants
            var frames = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(o => o.Split(',').Select(s => s.Trim()).ToList())
                .GroupBy(o => o[1], o => o);

            foreach (var frameGroup in frames)
            {
                var frameElems = frameGroup
                    .Where(o => o[2] == "mFE")
                    .Select(o => new FrameElement { Name = o[5], NameLV = o[4], Type = o[3] })
                    .ToArray();

                var frame = new Frame(frameGroup.Key, frameGroup.First()[0], frameElems);

                Frames.Add(frame);
            }

            // *** make it global (for converter)
            FrameJsonConverter.Frames = Frames;
        }        

        //////////////////////////////////////////////////////////////////////////////////////
        public void Save()
        {
            Save(Filename);
        }
        public void Save(string filename)
        {
            if (Doc.Save(filename))
            {
                //DB.Save("SemanticDB.xml");
                IsNew = false;
                Filename = Path.GetFullPath(filename);
                Undo.MarkAllOtherStatesDirty();
                Undo.SetDirty(false);   
            }
            
        }                

        public bool Open(string filename)
        {
            var doc = Document.Open(filename/*, DB*/);

            if(doc != null)
            {
                Doc = doc;
                IsNew = false;
                Filename = filename;
                return true;
            }

            return false;
        }
    }
}

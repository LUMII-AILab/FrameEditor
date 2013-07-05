using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrameMarker
{
    public class Frame
    {
        public string Name;
        public string NameLV;

        public List<FrameElement> Elements = new List<FrameElement>();

        public Frame(string name, string nameLV, FrameElement[] frameElements)
        {
            Name = name;
            NameLV = nameLV;
            Elements.AddRange(frameElements);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class FrameElement
    {
        public string Name;
        public string NameLV;

        public string Type;

        public override string ToString()
        {
            return Name;
        }
    }       
}

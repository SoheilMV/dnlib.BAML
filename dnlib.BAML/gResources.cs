using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace dnlib.DotNet.Resources
{
    public class gResources
    {
        private System.Resources.ResourceReader _resourceReader;
        private Dictionary<string, ElementData> _resourceElements { get; set; }

        /// <summary>
        /// Resource name.
        /// </summary>
        public UTF8String Name { get; private set; }

        /// <summary>
        /// ManifestResource flags. See CorHdr.h/CorManifestResourceFlags.
        /// </summary>
        public ManifestResourceAttributes Attributes { get; private set; }

        /// <summary>
        /// Get the elements.
        /// </summary>
        public List<ElementData> Elements
        {
            get
            {
                List<ElementData> list = new List<ElementData>();
                foreach (var element in _resourceElements.Values)
                {
                    list.Add(element);
                }
                return list;
            }
        }

        /// <summary>
        /// Get the count of elements.
        /// </summary>
        public int Count { get { return Elements.Count; } }

        public gResources(UTF8String name, ManifestResourceAttributes attributes, Stream stream)
        {
            _resourceElements = new Dictionary<string, ElementData>();
            _resourceReader = new System.Resources.ResourceReader(stream);
            Name = name;
            Attributes = attributes;
            IDictionaryEnumerator enumerator = _resourceReader.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = (string)enumerator.Key;
                var ed = GetElementData(key);
                _resourceElements.Add(key, ed);
            }
        }

        /// <summary>
        /// Read the elements.
        /// </summary>
        /// <param name="name">Enter the name of the element.</param>
        public ElementManegment Read(string name)
        {
            var ed = _resourceElements[name];
            BamlDocument document = BamlReader.ReadDocument(new MemoryStream(ed.Data, 4, ed.Data.Length - 4));
            document.RemoveWhere((BamlRecord rec) => rec is LineNumberAndPositionRecord || rec is LinePositionRecord);
            document.DocumentName = ed.Name;

            ElementManegment em = new ElementManegment()
            {
                Document = document,
                ElementData = ed
            };

            return em;
        }

        /// <summary>
        /// Write the elements.
        /// </summary>
        /// <param name="name">Enter a new name for the element.</param>
        public void Write(ElementManegment elementManegment, string name)
        {
            MemoryStream stream = new MemoryStream();
            stream.Position = 4;
            BamlWriter.WriteDocument(elementManegment.Document, stream);
            stream.Position = 0L;
            stream.Write(BitConverter.GetBytes((int)stream.Length - 4), 0, 4);

            _resourceElements[elementManegment.ElementData.Name] = new ElementData(name, elementManegment.ElementData.Type, stream.ToArray());
        }

        /// <summary>
        /// Write the elements.
        /// </summary>
        public void Write(ElementManegment elementManegment)
        {
            Write(elementManegment, elementManegment.ElementData.Name);
        }

        /// <summary>
        /// Get the g.resources as Stream.
        /// </summary>
        public MemoryStream GetAsStream()
        {
            MemoryStream stream = new MemoryStream();
            SetElementData(stream);
            return stream;
        }

        /// <summary>
        /// Get the g.resources as EmbeddedResource.
        /// </summary>
        public EmbeddedResource GetAsEmbeddedResource()
        {
            var ms = GetAsStream();
            return new EmbeddedResource(Name, ms.ToArray(), Attributes);
        }

        private ElementData GetElementData(string name)
        {
            string type;
            byte[] data;
            _resourceReader.GetResourceData(name, out type, out data);
            return new ElementData(name, type, data);
        }

        private void SetElementData(Stream stream)
        {
            System.Resources.ResourceWriter rw = new System.Resources.ResourceWriter(stream);
            foreach (var data in _resourceElements)
            {
                rw.AddResourceData(data.Value.Name, data.Value.Type, data.Value.Data);
            }
            rw.Generate();
        }
    }

    public class ElementManegment
    {
        internal ElementData ElementData { get; set; }
        public BamlDocument Document { get; set; }
    }

    public class ElementData
    {
        internal ElementData(string name, string type, byte[] data)
        {
            Name = name;
            Type = type;
            Data = data;
        }

        /// <summary>
        /// Element name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Element type.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Element Buffer.
        /// </summary>
        public byte[] Data { get; private set; }
    }
}

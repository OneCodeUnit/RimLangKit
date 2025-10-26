using System.Xml;
using System.Xml.Linq;

namespace RimLangKit.Models
{
    // Класс, в котором описывается непонятный мне трюк, позволяющий вписывать специальные символы «как есть»
    internal sealed class XRaw : XText
    {
        public XRaw(string text) : base(text) { }
        public XRaw(XText text) : base(text) { }

        public override void WriteTo(XmlWriter writer)
        {
            writer.WriteRaw(Value);
        }
    }
}
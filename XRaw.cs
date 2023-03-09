using System.Xml;
using System.Xml.Linq;

namespace RimLangKit
{
    //Класс, в котором описывается не до конца понятный мне трюк, позволяющий вписывать специальные символы «как есть»
    sealed internal class XRaw : XText
    {
        public XRaw(string text) : base(text) { }
        public XRaw(XText text) : base(text) { }

        public override void WriteTo(XmlWriter writer)
        {
            writer.WriteRaw(Value);
        }
    }
}

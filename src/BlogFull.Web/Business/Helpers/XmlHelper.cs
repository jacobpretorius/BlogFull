using System.Xml.Linq;

namespace BlogFull.Web.Business.Helpers
{
    public static class XmlHelper
    {
        public static string ReadValue(XElement doc, XName name, string defaultValue = "")
        {
            if (doc.Element(name) != null)
            {
                return doc.Element(name).Value;
            }

            return defaultValue;
        }

        public static bool ReadBool(XElement doc, XName name)
        {
            if (doc.Element(name) != null)
            {
                return doc.Element(name).Value == "True";
            }

            return false;
        }
    }
}
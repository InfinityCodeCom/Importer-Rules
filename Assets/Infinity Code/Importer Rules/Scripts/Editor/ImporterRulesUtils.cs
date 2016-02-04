/*     INFINITY CODE 2013-2016      */
/*   http://www.infinity-code.com   */

using System.Reflection;
using System.Xml;
using UnityEngine;

namespace InfinityCode.ImporterRules
{
    public static class ImporterRulesUtils
    {
        public static XmlElement CreateChild(this XmlDocument doc, string nodeName)
        {
            XmlElement el = doc.CreateElement(nodeName);
            doc.AppendChild(el);
            return el;
        }

        public static XmlElement CreateChild(this XmlNode node, string nodeName)
        {
            XmlElement el = node.OwnerDocument.CreateElement(nodeName);
            node.AppendChild(el);
            return el;
        }

        public static XmlElement CreateChild(this XmlNode node, string nodeName, string value)
        {
            XmlElement el = node.CreateChild(nodeName);
            el.AppendChild(el.OwnerDocument.CreateTextNode(value));
            return el;
        }

        public static XmlElement CreateChild(this XmlNode node, string nodeName, bool value)
        {
            return node.CreateChild(nodeName, value ? "True" : "False");
        }

        public static XmlElement CreateChild(this XmlNode node, string nodeName, int value)
        {
            return node.CreateChild(nodeName, value.ToString());
        }

        public static XmlElement CreateChild(this XmlNode node, string nodeName, float value)
        {
            return node.CreateChild(nodeName, value.ToString());
        }

        public static XmlElement CreateChild(this XmlNode node, string nodeName, Vector2 value)
        {
            XmlElement el = node.CreateChild(nodeName);
            el.CreateChild("X", value.x);
            el.CreateChild("Y", value.y);
            return el;
        }

        public static XmlElement CreateChild(this XmlNode node, string nodeName, Vector3 value)
        {
            XmlElement el = node.CreateChild(nodeName, (Vector2) value);
            el.CreateChild("Z", value.z);
            return el;
        }

        public static XmlElement CreateChild(this XmlNode node, string nodeName, Vector4 value)
        {
            XmlElement el = node.CreateChild(nodeName, (Vector3) value);
            el.CreateChild("W", value.w);
            return el;
        }

        public static string FixPath(this string path)
        {
            return path.Replace("\\", "/");
        }

        public static void SetValue(this MemberInfo memberInfo, object target, object value)
        {
            if (memberInfo is PropertyInfo) ((PropertyInfo) memberInfo).SetValue(target, value, null);
            else if (memberInfo is FieldInfo) ((FieldInfo) memberInfo).SetValue(target, value);
        }

        public static bool TryGetValue(this XmlNode node, string nodeName, ref string value)
        {
            XmlNode n = node[nodeName];
            if (n == null) return false;
            value = n.InnerXml;
            return true;
        }

        public static bool TryGetValue(this XmlNode node, string nodeName, ref bool value)
        {
            string str = "";
            if (node.TryGetValue(nodeName, ref str))
            {
                value = str == "True";
                return true;
            }
            return false;
        }

        public static bool TryGetValue(this XmlNode node, string nodeName, ref int value)
        {
            string str = "";
            if (node.TryGetValue(nodeName, ref str))
            {
                value = int.Parse(str);
                return true;
            }
            return false;
        }

        public static bool TryGetValue(this XmlNode node, string nodeName, ref float value)
        {
            string str = "";
            if (node.TryGetValue(nodeName, ref str))
            {
                value = float.Parse(str);
                return true;
            }
            return false;
        }

        public static bool TryGetValue(this XmlNode node, string nodeName, ref uint value)
        {
            string str = "";
            if (node.TryGetValue(nodeName, ref str))
            {
                value = uint.Parse(str);
                return true;
            }
            return false;
        }

        public static bool TryGetValue(this XmlNode node, string nodeName, ref Vector2 value)
        {
            XmlNode n = node[nodeName];
            if (n == null) return false;
            if (!n.TryGetValue("X", ref value.x)) return false;
            if (!n.TryGetValue("Y", ref value.y)) return false;
            return true;
        }

        public static bool TryGetValue(this XmlNode node, string nodeName, ref Vector3 value)
        {
            XmlNode n = node[nodeName];
            if (n == null) return false;
            if (!n.TryGetValue("X", ref value.x)) return false;
            if (!n.TryGetValue("Y", ref value.y)) return false;
            if (!n.TryGetValue("Z", ref value.z)) return false;
            return true;
        }

        public static bool TryGetValue(this XmlNode node, string nodeName, ref Vector4 value)
        {
            XmlNode n = node[nodeName];
            if (n == null) return false;
            if (!n.TryGetValue("X", ref value.x)) return false;
            if (!n.TryGetValue("Y", ref value.y)) return false;
            if (!n.TryGetValue("Z", ref value.z)) return false;
            if (!n.TryGetValue("W", ref value.w)) return false;
            return true;
        }
    }
}
/*     INFINITY CODE 2013-2015      */
/*   http://www.infinity-code.com   */

using System;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace InfinityCode.ImporterRules
{
    [Serializable]
    public abstract class ImporterRuleBaseSettings
    {
        protected void LoadSerialized(XmlNode node, object settings, MemberInfo[] props)
        {
            foreach (MemberInfo info in props)
            {
                object val = null;
                if (info is PropertyInfo) val = ((PropertyInfo) info).GetValue(settings, null);
                else if (info is FieldInfo) val = ((FieldInfo) info).GetValue(settings);
                
                if (val == null) continue;
                Type type = val.GetType();
                if (type == typeof (Boolean))
                {
                    bool value = false;
                    if (node.TryGetValue(info.Name, ref value)) info.SetValue(settings, value);
                }
                else if (type.IsEnum)
                {
                    string value = "";
                    if (node.TryGetValue(info.Name, ref value))
                    {
                        object obj = Enum.Parse(type, value);
                        info.SetValue(settings, obj);
                    }
                }
                else if (type == typeof (string))
                {
                    string value = "";
                    if (node.TryGetValue(info.Name, ref value)) info.SetValue(settings, value);
                }
                else if (type == typeof (int))
                {
                    int value = 0;
                    if (node.TryGetValue(info.Name, ref value)) info.SetValue(settings, value);
                }
                else if (type == typeof (float))
                {
                    float value = 0;
                    if (node.TryGetValue(info.Name, ref value)) info.SetValue(settings, value);
                }
                else if (type == typeof (uint))
                {
                    uint value = 0;
                    if (node.TryGetValue(info.Name, ref value)) info.SetValue(settings, value);
                }
                else if (type == typeof (Vector2))
                {
                    Vector2 value = Vector2.zero;
                    if (node.TryGetValue(info.Name, ref value)) info.SetValue(settings, value);
                }
                else if (type == typeof (Vector3))
                {
                    Vector3 value = Vector3.zero;
                    if (node.TryGetValue(info.Name, ref value)) info.SetValue(settings, value);
                }
                else if (type == typeof (Vector4))
                {
                    Vector4 value = Vector4.zero;
                    if (node.TryGetValue(info.Name, ref value)) info.SetValue(settings, value);
                }
                else Debug.Log(info.Name + "   " + type);
            }
        }

        protected static void FieldsToProps(object from, object to)
        {
            Type fromType = from.GetType();
            FieldInfo[] fields = fromType.GetFields();

            Type toType = to.GetType();

            foreach (FieldInfo field in fields)
            {
                PropertyInfo prop = toType.GetProperty(field.Name);
                if (prop != null) prop.SetValue(to, field.GetValue(from));
            }
        }

        protected static void PropsToFields(object from, object to)
        {
            Type fromType = from.GetType();
            PropertyInfo[] props = fromType.GetProperties();

            Type toType = to.GetType();

            foreach (PropertyInfo prop in props)
            {
                FieldInfo field = toType.GetField(prop.Name);
                if (field != null) field.SetValue(to, prop.GetValue(from, null));
            }
        }

        protected void SaveSerialized(XmlElement element, object settings, MemberInfo[] props)
        {
            foreach (MemberInfo info in props)
            {
                object value = null;
                if (info is PropertyInfo) value = ((PropertyInfo) info).GetValue(settings, null);
                else if (info is FieldInfo) value = ((FieldInfo) info).GetValue(settings);

                if (value == null) continue;
                Type type = value.GetType();
                if (type == typeof (Boolean)) element.CreateChild(info.Name, (Boolean) value);
                else if (type.IsEnum) element.CreateChild(info.Name, value.ToString());
                else if (type == typeof (string) || type == typeof (int) || type == typeof (float) ||
                         type == typeof (uint)) element.CreateChild(info.Name, value.ToString());
                else if (type == typeof (Vector2)) element.CreateChild(info.Name, (Vector2) value);
                else if (type == typeof (Vector3)) element.CreateChild(info.Name, (Vector3) value);
                else if (type == typeof (Vector4)) element.CreateChild(info.Name, (Vector4) value);
                else Debug.Log(info.Name + "   " + type);
            }
        }

        protected static GUIContent TextContent(string value)
        {
            return new GUIContent(value);
        }

        public abstract void DrawEditor();
        public abstract void GetSettingsFromImporter(AssetImporter importer);
        public abstract void Load(XmlNode node);
        public abstract void Save(XmlElement element);
        public abstract void SetSettingsToImporter(AssetImporter importer, string assetPath);
    }
}
// Copyright © 2011 Libor Tinka
namespace XMLSettings
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    #endregion

    /// <summary>
    /// Provides settings stored in XML file.
    /// </summary>
    public sealed class XmlSettings
    {
        #region Public Constants

        /// <summary>
        ///   default extension of the settings file
        /// </summary>
        public const string DefaultExtension = ".xml";

        #endregion

        #region Private Constants

        private const string XmlNodeSettings = "Settings";
        private const string XmlNodeProperty = "Property";
        private const string XmlAttributeName = "name";
        private const string XmlAttributeValue = "value";
        private const string XmlAttributeIsBinary = "isBinary";

        #endregion

        #region Private Fields

        private readonly Dictionary<string, XmlSettingsProperty> properties = new Dictionary<string, XmlSettingsProperty>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Check whether the settings contains property with the specified name.
        /// </summary>
        /// <param name="name">settings property name</param>
        /// <returns>the settings contains property with the specified name</returns>
        public bool Contains(string name)
        {
            return this.properties.ContainsKey(name);
        }

        /// <summary>
        /// Get value of settings property with the specified name.
        /// </summary>
        /// <typeparam name="T">settings property type</typeparam>
        /// <param name="name">settings property name</param>
        /// <returns>settings property value</returns>
        public T GetValue<T>(string name)
        {
            return GetValue(name, default(T));
        }

        /// <summary>
        /// Get value of settings property with the specified name.
        /// </summary>
        /// <typeparam name="T">settings property type</typeparam>
        /// <param name="name">settings property name</param>
        /// <param name="defaultValue">value of the settings property used when settings property not found</param>
        /// <returns>settings property value</returns>
        public T GetValue<T>(string name, T defaultValue)
        {
            XmlSettingsProperty property;

            if (this.properties.TryGetValue(name, out property))
            {
                return (T)property.GetValue(typeof(T));
            }

            return defaultValue;
        }

        /// <summary>
        /// Set value of settings property with the specified name.
        /// </summary>
        /// <param name="name">settings property name</param>
        /// <param name="value">settings property value</param>
        public void SetValue(string name, object value)
        {
            XmlSettingsProperty property;

            if (this.properties.TryGetValue(name, out property))
            {
                property.SetValue(value);
            }
            else
            {
                this.properties.Add(
                    name,
                    new XmlSettingsProperty(value));
            }
        }

        /// <summary>
        /// Load settings from the specified file.
        /// </summary>
        /// <param name="path">path to the XML file with settings</param>
        public void Load(string path)
        {
            try
            { 
            this.properties.Clear();

            XmlDocument document = new XmlDocument();

            document.Load(path);

            XmlNodeList nodesProperties = document.SelectNodes(String.Format("{0}/{1}", XmlNodeSettings, XmlNodeProperty));

            if (nodesProperties != null)
            {
                foreach (XmlNode nodeProperty in nodesProperties)
                {
                    XmlAttribute attributeName = nodeProperty.Attributes[XmlAttributeName];
                    XmlAttribute attributeValue = nodeProperty.Attributes[XmlAttributeValue];
                    XmlAttribute attributeIsBinary = nodeProperty.Attributes[XmlAttributeIsBinary];

                    if (attributeName == null ||
                        attributeValue == null ||
                        attributeIsBinary == null)
                    {
                        // some property attribute not provided
                        continue;
                    }

                    string propertyName = attributeName.Value;
                    string propertyValueSerialized = attributeValue.Value;
                    bool propertyIsBinary;

                    if (String.IsNullOrEmpty(propertyName) ||
                        String.IsNullOrEmpty(propertyValueSerialized) ||
                        Boolean.TryParse(attributeIsBinary.Value, out propertyIsBinary) == false)
                    {
                        // some property attribute has invalid value
                        continue;
                    }

                    this.properties.Add(
                        propertyName,
                        new XmlSettingsProperty(propertyValueSerialized, propertyIsBinary));
                    }
                    }
                }
            catch(Exception e)
            {
                return;
            }
        }

        /// <summary>
        /// Save settings to the specified file.
        /// </summary>
        /// <param name="path">path to the XML file with settings</param>
        public void Save(string path)
        {
            XmlTextWriter writer = new XmlTextWriter(path, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;

            writer.WriteStartDocument();
            writer.WriteStartElement(XmlNodeSettings);

            foreach (KeyValuePair<string, XmlSettingsProperty> pair in this.properties)
            {
                string propertyName = pair.Key;
                XmlSettingsProperty property = pair.Value;

                if (String.IsNullOrEmpty(property.ValueSerialized))
                {
                    continue;
                }

                writer.WriteStartElement(XmlNodeProperty);
                writer.WriteAttributeString(XmlAttributeName, propertyName);
                writer.WriteAttributeString(XmlAttributeValue, property.ValueSerialized);
                writer.WriteAttributeString(XmlAttributeIsBinary, property.IsBinary.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }

        #endregion
    }
}

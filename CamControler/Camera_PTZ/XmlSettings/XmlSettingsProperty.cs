// Copyright © 2011 Libor Tinka
namespace XMLSettings
{
    #region Usings

    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    #endregion

    /// <summary>
    ///   Represents a settings property for XmlSettings.
    /// </summary>
    internal sealed class XmlSettingsProperty
    {
        #region Public Properties

        /// <summary>
        ///   serialized settings property value
        /// </summary>
        public string ValueSerialized
        {
            get
            {
                if (this.valueSerialized == null)
                {
                    SerializeValue();
                }

                return this.valueSerialized;
            }
        }

        /// <summary>
        ///   settings property value is binary serialized
        /// </summary>
        public bool IsBinary
        {
            get
            {
                if (this.valueSerialized == null)
                {
                    SerializeValue();
                }

                return this.isBinary;
            }
        }

        #endregion

        #region Private Fields

        private object value;
        private string valueSerialized;
        private bool isBinary;

        #endregion

        #region Public Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmlSettingsProperty" /> class.
        /// </summary>
        /// <param name = "value">settings property value</param>
        public XmlSettingsProperty(object value)
        {
            this.value = value;
            this.valueSerialized = null;
            this.isBinary = false;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "XmlSettingsProperty" /> class.
        /// </summary>
        /// <param name = "valueSerialized">serialized settings property value</param>
        /// <param name = "isBinary">settings property value is binary serialized</param>
        public XmlSettingsProperty(string valueSerialized, bool isBinary)
        {
            this.value = null;
            this.valueSerialized = valueSerialized;
            this.isBinary = isBinary;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Get value of this settings property.
        /// </summary>
        /// <param name = "propertyType">expected type of the settings property value</param>
        /// <returns>value of this settings property</returns>
        public object GetValue(Type propertyType)
        {
            if (this.value == null)
            {
                DeserializeValue(propertyType);
            }

            return this.value;
        }

        /// <summary>
        ///   Set value of this settings property.
        /// </summary>
        /// <param name = "value">value of this settings property</param>
        public void SetValue(object value)
        {
            this.value = value;
            this.valueSerialized = null;
        }

        #endregion

        #region Private Methods

        private void DeserializeValue(Type propertyType)
        {
            if (String.IsNullOrEmpty(this.valueSerialized))
            {
                this.value = null;

                return;
            }

            if (IsBinary)
            {
                // deserialize from binary
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(this.valueSerialized));

                this.value = (new BinaryFormatter()).Deserialize(stream);

                stream.Close();
                stream.Dispose();
            }
            else
            {
                // deserialize from String
                this.value = TypeDescriptor.GetConverter(propertyType).ConvertFromString(this.valueSerialized);
            }
        }

        private void SerializeValue()
        {
            if (this.value == null)
            {
                this.valueSerialized = String.Empty;
                this.isBinary = false;

                return;
            }

            TypeConverter typeConverter = TypeDescriptor.GetConverter(this.value.GetType());

            if (typeConverter == null ||
                typeConverter.CanConvertFrom(typeof(String)) == false ||
                typeConverter.CanConvertTo(typeof(String)) == false)
            {
                // serialize to binary
                MemoryStream stream = new MemoryStream();

                (new BinaryFormatter()).Serialize(stream, this.value);

                byte[] data = stream.ToArray();

                stream.Close();
                stream.Dispose();

                this.valueSerialized = Convert.ToBase64String(data);
                this.isBinary = true;
            }
            else
            {
                // serialize to String
                this.valueSerialized = typeConverter.ConvertToString(this.value);
                this.isBinary = false;
            }
        }

        #endregion
    }
}

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace ADSD
{
    /// <summary>Wraps an XSD <see langword="hexBinary" /> type.</summary>
    [ComVisible(true)]
    [Serializable]
    public sealed class SoapHexBinary 
    {
        private StringBuilder sb = new StringBuilder(100);
        private byte[] _value;

        /// <summary>Gets the XML Schema definition language (XSD) of the current SOAP type.</summary>
        /// <returns>A <see cref="T:System.String" /> indicating the XSD of the current SOAP type.</returns>
        public static string XsdType
        {
            get
            {
                return "hexBinary";
            }
        }

        /// <summary>Returns the XML Schema definition language (XSD) of the current SOAP type.</summary>
        /// <returns>A <see cref="T:System.String" /> that indicates the XSD of the current SOAP type.</returns>
        public string GetXsdType()
        {
            return SoapHexBinary.XsdType;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary" /> class.</summary>
        public SoapHexBinary()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary" /> class.</summary>
        /// <param name="value">A <see cref="T:System.Byte" /> array that contains a hexadecimal number. </param>
        public SoapHexBinary(byte[] value)
        {
            this._value = value;
        }

        /// <summary>Gets or sets the hexadecimal representation of a number.</summary>
        /// <returns>A <see cref="T:System.Byte" /> array containing the hexadecimal representation of a number.</returns>
        public byte[] Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        /// <summary>Returns <see cref="P:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary.Value" /> as a <see cref="T:System.String" />.</summary>
        /// <returns>A <see cref="T:System.String" /> that is obtained from <see cref="P:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary.Value" />.</returns>
        public override string ToString()
        {
            this.sb.Length = 0;
            for (int index = 0; index < this._value.Length; ++index)
            {
                string str = this._value[index].ToString("X", (IFormatProvider) CultureInfo.InvariantCulture);
                if (str.Length == 1)
                    this.sb.Append('0');
                this.sb.Append(str);
            }
            return this.sb.ToString();
        }

        /*
        /// <summary>Converts the specified <see cref="T:System.String" /> into a <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary" /> object.</summary>
        /// <param name="value">The <see langword="String" /> to convert. </param>
        /// <returns>A <see cref="T:System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary" /> object that is obtained from <paramref name="value" />.</returns>
        public static SoapHexBinary Parse(string value)
        {
            return new SoapHexBinary(SoapHexBinary.ToByteArray(SoapType.FilterBin64(value)));
        }*/

        private static byte[] ToByteArray(string value)
        {
            char[] charArray = value.ToCharArray();
            if (charArray.Length % 2 != 0) throw new Exception("Remoting_SOAPInteropxsdInvalid");

            byte[] numArray = new byte[charArray.Length / 2];
            for (int index = 0; index < charArray.Length / 2; ++index)
                numArray[index] = (byte) ((uint) SoapHexBinary.ToByte(charArray[index * 2], value) * 16U + (uint) SoapHexBinary.ToByte(charArray[index * 2 + 1], value));
            return numArray;
        }

        private static byte ToByte(char c, string value)
        {
            c.ToString();
            try
            {
                return byte.Parse(c.ToString(), NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new Exception("Remoting_SOAPInteropxsdInvalid", ex);
            }
        }
    }
}
using System.Text;

namespace RNG.Utils
{
    public class StringWriterWithEncoding : StringWriter
    {
        private readonly Encoding encoding;

        public StringWriterWithEncoding(StringBuilder sb, Encoding encoding) : base(sb)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding => encoding;
    }
}

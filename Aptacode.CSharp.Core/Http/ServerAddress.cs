using System.Text;

namespace Aptacode.CSharp.Core.Http
{
    public struct ServerAddress
    {
        public string Protocol { get; set; }
        public string Address { get; set; }
        public string Port { get; set; }
        
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(Protocol))
            {
                stringBuilder.Append(Protocol);
                stringBuilder.Append("://");
            }

            if (string.IsNullOrEmpty(Address))
            {
                return string.Empty;
            }

            stringBuilder.Append(Address);
            if (!string.IsNullOrEmpty(Port))
            {
                stringBuilder.Append(":");
                stringBuilder.Append(Port);
            }

            return stringBuilder.ToString();
        }
    }
}
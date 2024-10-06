using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteBank.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NomeColunaAttribute : Attribute
    {
        public string HeaderName { get; }
        public NomeColunaAttribute(string HeaderName)
        {
            this.HeaderName = HeaderName;
        }
    }
}

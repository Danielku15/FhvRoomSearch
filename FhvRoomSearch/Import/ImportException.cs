using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FhvRoomSearch.Import
{
    [Serializable]
    public class ImportException : Exception
    {
        public ImportException()
        {
        }

        public ImportException(string message) : base(message)
        {
        }

        public ImportException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ImportException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}

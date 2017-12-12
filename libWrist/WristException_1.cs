using System;
using System.Collections.Generic;
using System.Text;

namespace libWrist
{

    [global::System.Serializable]
    public class WristException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public WristException() { }
        public WristException(string message) : base(message) { }
        public WristException(string message, Exception inner) : base(message, inner) { }
        protected WristException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

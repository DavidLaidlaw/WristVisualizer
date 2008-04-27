using System;
using System.Collections.Generic;
using System.Text;

namespace WristVizualizer
{
    [global::System.Serializable]
    public class WristVizualizerException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public WristVizualizerException() { }
        public WristVizualizerException(string message) : base(message) { }
        public WristVizualizerException(string message, Exception inner) : base(message, inner) { }
        protected WristVizualizerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}

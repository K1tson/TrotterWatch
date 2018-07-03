using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrotterWatch.Core.Rbl
{
    internal class RblCollection
    {
        private static IEnumerable<RblItem> _rblCollection;

        private RblCollection()
        { }

        public static IEnumerable<RblItem> ReturnRblCollection(IEnumerable<RblItem> rblCollection)
        {
            if (_rblCollection != null)
                return _rblCollection;

            var locker = new object();

            lock (locker)
            {
                if (_rblCollection != null)
                    return _rblCollection;

                InitiateRblCollection(rblCollection);
                return _rblCollection;
            }
        }

        private static void InitiateRblCollection(IEnumerable<RblItem> rblCollection)
        {
           _rblCollection = rblCollection.Any()
                ? _rblCollection
                : throw new ArgumentOutOfRangeException(paramName: nameof(rblCollection), message: "No RblItems Found in Collection!");
        }
    }
}

using System;

namespace Acciaio
{
    [Serializable]
    public sealed class AutoId : Id
    {
        public AutoId() : base(Guid.NewGuid().ToString()) { }
    }
}
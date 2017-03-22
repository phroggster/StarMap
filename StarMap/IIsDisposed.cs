using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarMap
{
    public interface IIsDisposed : IDisposable
    {
        bool IsDisposed { get; }
    }
}

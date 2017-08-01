using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPL
{
    public class LineManager : IDisposable
    {
        public int index;
        public int Length;
        public Line[] Lines;
        public bool HasJumped;
        
        public LineManager(Line[] lines)
        {
            Lines = lines;
            Length = Lines.Length;
        }
        
        /// <summary>
        /// returns if finished
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if(HasJumped)
            {
                HasJumped = false;
                return !AtEnd();
            }
            else
            {
                return (index++ < Length);
            }
        }

        public bool AtEnd()
        {
            return (index >= Length);
        }

        public bool MoveNext(int line)
        {
            if (line < 1)
                line = 1;
            index += line;            
            return !AtEnd();
        }

        public void MoveBack()
        {
            index--;
        }

        public void Jump(int line)
        {
            HasJumped = true;
            index += line;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~LineManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}

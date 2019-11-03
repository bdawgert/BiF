using System;

namespace BiF.DAL.Concrete
{
    public class EFUnitOfWork: IDisposable
    {
        private static EFUnitOfWork _eow;

        private EFUnitOfWork() {
            Context = new BifDbContext();
        }

        private EFUnitOfWork(string connectionString) {
            Context = new BifDbContext();
            if (connectionString != null) {
                Context.Database.Connection.ConnectionString = connectionString;
                Context.Database.Connection.Open();
            }
        }

        public BifDbContext Context { get; private set; }


        public static EFUnitOfWork Create(string connectionString = null) {
            if (_eow == null)
                _eow = new EFUnitOfWork();
            return _eow;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (this.Context != null) {
                    this.Context.Dispose();
                    this.Context = null;
                }
            }
        }

    }

}


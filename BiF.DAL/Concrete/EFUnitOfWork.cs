﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BiF.DAL.Concrete
{
    public class EFUnitOfWork: IDisposable
    {
        private static EFUnitOfWork _eow;


        private EFUnitOfWork(string connectionString) {
            Context = new BifDbContext(connectionString);
            //if (connectionString != null) {
            //    Context.Database.Connection.ConnectionString = connectionString;
            //    Context.Database.Connection.Open();
            //}
        }

        public BifDbContext Context { get; private set; }

        public void Reset() {
            List<DbEntityEntry> changedEntriesCopy =  Context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }

        public static EFUnitOfWork Create(string connectionString) {
            if (_eow == null)
                _eow = new EFUnitOfWork(connectionString);
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


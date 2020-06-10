using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public EliteDataContext _context { get; }
        public UnitOfWork(EliteDataContext context)
        {
            _context = context;
        }

        // Calling saveChanges only in one place, would decouple this app from Entity framework, so that if 
        // the database type changes, Entity framework context would be changed only here and the Repositories and 
        // not in thousands of places in our app, and we need this because we shouldn't put 
        // SaveChanges in our Repositories, because it might cause problems when multiple transactions have to be saved
        // if one had failed, another part might be saved, which makes the code not consistent. This is the UnitOfWork Pattern.
        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

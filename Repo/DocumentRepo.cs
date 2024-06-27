using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
namespace Repo
{
    public class DocumentRepo
    {
        private readonly AppointmentSchedulingContext _context;

        public DocumentRepo(AppointmentSchedulingContext context)
        {
            _context = context;
        }
         public async Task AddDocument(TblDocument document)
        {
            await _context.TblDocuments.AddAsync(document);
            await _context.SaveChangesAsync();
        }

        public List<TblDocument> GetAllDocuments()
        {
            return _context.TblDocuments.ToList();
        }

    }
}

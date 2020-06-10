using EliteForce.AppWideHelpers;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public class FaqRepository : IFaqRepository
    {
        private readonly EliteDataContext _context;

        public FaqRepository(EliteDataContext context)
        {
            _context = context;
        }

        // public async Task<EntityEntry<Faq>> AddAnFaq(FaqPostDto entity)
        public async Task<int> AddAnFaq(FaqPostDto newFaq)
        {
            var EntityFaq = new Faq();
            EntityFaq.FaqQuestion = newFaq.FaqQuestion;
            EntityFaq.FaqAnswer = newFaq.FaqAnswer;
            await _context.FaqItems.AddAsync(EntityFaq);
            var numberInserted = _context.SaveChanges();
            return numberInserted;
        }

        public async Task<IEnumerable<Faq>> GetFaqs() 
        {
           var faqs = await _context.FaqItems.ToListAsync();
            return faqs;
        }

        public async Task<Faq> GetSingleFaq(int faqId) 
        {
            return await _context.FaqItems.FindAsync(faqId);
        }


        public async Task<int> EditAnFaq(Faq changedFaq)
        {
            var faq = await _context.FaqItems.FirstOrDefaultAsync(f => f.FaqId == changedFaq.FaqId);
            if (faq != null)
            {
                faq.FaqAnswer = changedFaq.FaqAnswer;
                faq.FaqQuestion = changedFaq.FaqQuestion;

                _context.FaqItems.Update(faq);
                var numEdited = await _context.SaveChangesAsync();
                return numEdited;
            }

            return 0;

        }


        public async Task<int> DeleteAnFaq(string faqId)
        {
            var faq = await _context.FaqItems.FirstOrDefaultAsync(f => f.FaqId == faqId);
            if (faq != null)
            {
                _context.FaqItems.Remove(faq);
                var numEdited = await _context.SaveChangesAsync();
                return numEdited;
            }
            return 0;
        }

    }
}


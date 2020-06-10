using EliteForce.AppWideHelpers;
using EliteForce.Dtos;
using EliteForce.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface IFaqRepository
    {
        Task<int> AddAnFaq(FaqPostDto entity);
        Task<int> DeleteAnFaq(string faqId);
        Task<IEnumerable<Faq>> GetFaqs();
        Task<Faq> GetSingleFaq(int faqId);

        Task<int> EditAnFaq(Faq updatedFaq);
    }
}

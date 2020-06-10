using EliteForce.Dtos;
using EliteForce.Entities;
using IdentityServer4.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public class NewsRepository : INewsRepository
    {
        private readonly EliteDataContext _context;
        public NewsRepository(EliteDataContext context)
        {
            _context = context;
        }

        public async Task<List<News>> GetnewsArticles(string category)
        {
            var articles = await _context.NewsItems.Where(n => n.NewsCategory == category).ToListAsync();
            
            if (articles.Count == 0)
            {
                throw new Exception("No item was found");
            }
            return articles;
        }


        public async Task<int> AddANewsItem(NewsPostDto newsItem)
        {
            var EntityNewsItem = new News();
            EntityNewsItem.NewsTitle = newsItem.NewsTitle;
            EntityNewsItem.NewsSummary = newsItem.NewsSummary; 
            EntityNewsItem.NewsFullStory = newsItem.NewsFullStory;
            EntityNewsItem.NewsCategory = newsItem.NewsCategory;
            EntityNewsItem.DatePublished = DateTime.UtcNow;
            EntityNewsItem.ImagePath = newsItem.ImagePath;

            await _context.NewsItems.AddAsync(EntityNewsItem);
            var numberInserted = _context.SaveChanges();
            return numberInserted;
        }

        
        public async Task<int> UpdateNewsItem(NewsUpdateDto newsItem, int newsId)
        {
            var article = await _context.NewsItems.FindAsync(newsId);
            if (article == null)
            {
                throw new Exception("News article is not found.");
            }

            if (!string.IsNullOrEmpty(newsItem.NewsTitle)) {
                article.NewsTitle = newsItem.NewsTitle;
            }
            if (!string.IsNullOrEmpty(newsItem.NewsSummary))
            {
                article.NewsSummary = newsItem.NewsSummary;
            }
            if (!string.IsNullOrEmpty(newsItem.NewsFullStory))
            {
                article.NewsFullStory = newsItem.NewsFullStory;
            }
            if (!string.IsNullOrEmpty(newsItem.NewsCategory))
            {
                article.NewsCategory = newsItem.NewsCategory;
            }
            if (!string.IsNullOrEmpty(newsItem.ImagePath))
            {
                article.ImagePath = newsItem.ImagePath;
            }
            // This should actually be date updated
            article.DatePublished = DateTime.UtcNow;

            _context.NewsItems.Update(article);
            var numberInserted = _context.SaveChanges();
            return numberInserted;
        }


        public async Task<int> AddAScrollNewsItem(ScrollNewsPostDto scrollNewsItem)
        {
            var EntityScrollNewsItem = new ScrollingNews();
            var flashObject = await _context.ScrollingNewsItems.FirstAsync();
            if (string.IsNullOrEmpty(flashObject.NewsScrollbar))
            {
                EntityScrollNewsItem.NewsScrollbar = scrollNewsItem.NewsScrollbar;

                await _context.ScrollingNewsItems.AddAsync(EntityScrollNewsItem);
                var numberInserted = _context.SaveChanges();
                return numberInserted;
            }

            // Else there is, so replace it and save changes
            flashObject.NewsScrollbar = scrollNewsItem.NewsScrollbar;
             _context.Update(flashObject);
            var numberupdated= _context.SaveChanges();
            return numberupdated;
        }

        public async Task<string> GetnewsNewsFlash()
        {
            var flashObject = await _context.ScrollingNewsItems.FirstAsync();
            return flashObject.NewsScrollbar;
        }

    }
}

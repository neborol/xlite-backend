using EliteForce.Dtos;
using EliteForce.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EliteForce.Data
{
    public interface INewsRepository
    {
        Task<int> AddANewsItem(NewsPostDto newsItem);
        Task<int> AddAScrollNewsItem(ScrollNewsPostDto scrollNewsItem);
        Task<List<News>> GetnewsArticles(string category);
        Task<string> GetnewsNewsFlash();
        Task<int> UpdateNewsItem(NewsUpdateDto newsInput, int newsId);
    }
}

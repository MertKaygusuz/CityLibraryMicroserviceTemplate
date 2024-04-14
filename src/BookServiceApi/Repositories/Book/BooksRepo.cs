using BookServiceApi.ContextRelated;
using BookServiceApi.Entities;
using BookServiceApi.Repositories.Base;

namespace BookServiceApi.Repositories.Book
{
    public class BooksRepo(AppDbContext dbContext) : BaseRepo<Entities.Book, int>(dbContext), IBooksRepo
    {
    }
}
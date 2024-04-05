using BookServiceApi.Dtos.Book;
using BookServiceApi.Dtos.BookReservation;
using BookServiceApi.Entities;
using CityLibrary.Shared.BaseCheckService;

namespace BookServiceApi.Services.Book.Interfaces
{
    public interface IBookService : IBaseCheckService
    {
        /// <summary>
        /// Save new books
        /// </summary>
        /// <param name="dto">Registration parameters</param>
        /// <returns>Book id</returns>
        Task<int> BookRegisterAsync(RegisterBookDto dto);

        /// <summary>
        /// Update book's information of existing book
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task UpdateBookInfoAsync(UpdateBookDto dto);

        /// <summary>
        /// Soft delete for book records
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task DeleteBookAsync(DeleteBookDto dto);

        Task<int> GetNumberOfDistinctTitleAsync();

        Task<int> GetNumberOfAuthorsFromBookTableAsync();

        Task<IEnumerable<Books>> GetAllBooks();

        /// <summary>
        /// Inserts data to active book reservations
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task AssignBookToUserAsync(AssignBookToUserDto dto);

        /// <summary>
        /// Soft delete from active book reservations and insert data to book reservation history
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task UnAssignBookFromUserAsync(AssignBookToUserDto dto); //uses same dto

        Task<bool> CheckIfBookExistsAsync(int bookId);

        Task<bool> CheckIfAnyAvailableBooksAsync(int bookId);
    }
}

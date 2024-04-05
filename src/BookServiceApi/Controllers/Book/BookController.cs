using BookServiceApi.ActionFilters.Base;
using BookServiceApi.ActionFilters.Interfaces;
using BookServiceApi.Dtos.Book;
using BookServiceApi.Dtos.BookReservation;
using BookServiceApi.Services.Book.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookServiceApi.Controllers.Book
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    // [Authorize(Policy = "TokenKeysPolicy")]
    [Authorize(Roles = "Admin")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var result = await _bookService.GetAllBooks();
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<int> BookRegister(RegisterBookDto dto)
        {
            return await _bookService.BookRegisterAsync(dto);
        }

        [HttpPut]
        [ServiceFilter(typeof(GenericNotFoundFilter<IBookService>))]
        public async Task<IActionResult> BookInfoUpdate(UpdateBookDto dto)
        {
            await _bookService.UpdateBookInfoAsync(dto);
            return Ok();
        }

        [HttpDelete]
        [ServiceFilter(typeof(GenericNotFoundFilter<IBookService>))]
        public async Task<IActionResult> DeleteBook(DeleteBookDto dto)
        {
            await _bookService.DeleteBookAsync(dto);
            return Ok();
        }
        
        [HttpPost]
        [ServiceFilter(typeof(IAssigningBookFilter))]
        public async Task<IActionResult> AssignBookToUser(AssignBookToUserDto dto)
        {
            await _bookService.AssignBookToUserAsync(dto);
            return Ok();
        }

        [HttpPost]
        [ServiceFilter(typeof(IUnAssigningBookFilter))]
        public async Task<IActionResult> UnAssignBookFromUser(AssignBookToUserDto dto)
        {
            await _bookService.UnAssignBookFromUserAsync(dto);
            return Ok();
        }
    }
}

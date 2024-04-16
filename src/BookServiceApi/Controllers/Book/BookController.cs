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
    [Authorize(Roles = "Admin")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Entities.Book>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Entities.Book>>> GetAllBooks()
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> BookInfoUpdate(UpdateBookDto dto)
        {
            await _bookService.UpdateBookInfoAsync(dto);
            return NoContent();
        }

        [HttpDelete]
        [ServiceFilter(typeof(GenericNotFoundFilter<IBookService>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteBook(DeleteBookDto dto)
        {
            await _bookService.DeleteBookAsync(dto);
            return NoContent();
        }
        
        [HttpPost]
        [ServiceFilter(typeof(IAssigningBookFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AssignBookToUser(AssignBookToUserDto dto)
        {
            await _bookService.AssignBookToUserAsync(dto);
            return NoContent();
        }

        [HttpPost]
        [ServiceFilter(typeof(IUnAssigningBookFilter))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UnAssignBookFromUser(AssignBookToUserDto dto)
        {
            await _bookService.UnAssignBookFromUserAsync(dto);
            return NoContent();
        }
    }
}

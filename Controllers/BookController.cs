using ExadelBookstoreAPI.Models;
using ExadelBookstoreAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExadelBookstoreAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController(BookStoreService bookStoreService) : ControllerBase
{
    private readonly BookStoreService _bookStoreService = bookStoreService;

    [HttpGet("/getEveryBook", Name = "GetEveryBook")]
    public ActionResult<List<Book>> GetBooks()
    {
        return Ok(_bookStoreService.GetAllAsync().Result);
    }

    [HttpGet("/getBookPopularity/{title}")]
    public ActionResult<float> GetBookPopularity(string title)
    {
        var book = GetBookFromResult(title);
        if (book is null)
            return NotFound();

        return Ok(book.GetPopularity());
    }
    
    [HttpGet("/getBookByTitle/{title}", Name = "GetBookByTitle")]
    public ActionResult<Book> GetBookByTitle(string title)
    {
        var book = GetBookFromResult(title);
        if (book is null)
        {
            return NotFound();
        }

        return Ok(book);
    }
    
    [HttpPost("/addBook", Name = "AddBookToDatabase")]
    public ActionResult<Book> AddBook(Book newBook)
    {
        // check if the request body is empty
        if (newBook is null)
            return BadRequest();
        
        // check if the new book's title is already in the repo
        if (DoesBookExist(newBook.title))
            return BadRequest();
        
        _ = _bookStoreService.CreateBookAsync(newBook);

        return CreatedAtAction(nameof(GetBookByTitle), new {newBook.title}, newBook);
    }
    
    [HttpPut("/updateBook/{title}", Name = "UpdateBookByTitle")]
    public IActionResult UpdateBook(string title, Book updatedBook)
    {
        var bookToUpdate = GetBookFromResult(title);
        
        //check that book with given title exists
        if (bookToUpdate is null) 
            return NotFound();
        
        //check that updated book isn't null and that it doesn't conflict with any existing books
        if (updatedBook is null || DoesBookExist(title))
            return BadRequest();

        bookToUpdate.title = updatedBook.title;
        bookToUpdate.authorName = updatedBook.authorName;
        bookToUpdate.publicationYear = updatedBook.publicationYear;
        bookToUpdate.viewCount = updatedBook.viewCount;

        return NoContent();
    }

    [HttpDelete("/deleteByTitle/{title}", Name = "DeleteBookByTitle")]
    public IActionResult DeleteBookByTitle(string title)
    {
        if (DoesBookExist(title)) 
            return NotFound();

        _ = _bookStoreService.DeleteByTitleAsync(title);

        return NoContent();
    }
    private bool DoesBookExist(string title) 
        => _bookStoreService.GetByTitleAsync(title).Result is not null;

    private Book? GetBookFromResult(string title)
        => _bookStoreService.GetByTitleAsync(title).Result;
}
using ExadelBookstoreAPI.Models;
using ExadelBookstoreAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExadelBookstoreAPI.Controllers;


/// <summary>
/// CRUD Operations For Books
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BookController(BookStoreService bookStoreService) : ControllerBase
{
    private readonly BookStoreService _bookStoreService = bookStoreService;

    
    /// <summary>
    /// Returns every book in order of most popular to least popular paginated by the given page number and pageSize
    /// </summary>
    /// <param name="page">This is which page is requested after dividing the total number of books by pageSize</param>
    /// <param name="pageSize">Parameter which tells us how to split up total number of books</param>
    /// <returns>HTTP 200 with the list of paginated books</returns>
    [HttpGet("/getEveryBook", Name = "GetEveryBookTitlePaginatedOrderByPopularityDecreasing")]
    public async Task<ActionResult<List<string>>> GetBookTitlesPaginatedOrderByPopularityDecreasing
        ([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var books = await _bookStoreService.GetAllPaginatedAsync(page, pageSize);

        books = books.OrderByDescending(b => b.GetPopularity()).ToList();

        var titles = books.Select(b => b.title).ToList();
        
        return Ok(titles);
    }

    /// <summary>
    /// Returns the popularity of the book with the given title
    /// </summary>
    /// <param name="title">The title of the book we want the popularity of</param>
    /// <returns>HTTP 200 and the popularity of the chosen book, or HTTP 404 if the book doesn't exist or is soft deleted</returns>
    [HttpGet("/getBookPopularity/{title}", Name = "GetPopularityOfABook")]
    public async Task<ActionResult<float>> GetBookPopularity(string title)
    {
        var book = await GetBookFromResult(title);

        if (book is null || book.isDeleted)     
            return NotFound();

        return Ok(book.GetPopularity());
    }
    /// <summary>
    /// Returns the details of the book with the given title and update the view count of the given book by 1
    /// </summary>
    /// <param name="title">The title of the book we want to get</param>
    /// <returns>HTTP 200 and the book with the given title, or HTTP 404 if the book doesn't exist or is soft deleted</returns>
    [HttpGet("/getBookByTitle/{title}", Name = "GetBookByTitle")]
    public async Task<ActionResult<Book>> GetBookByTitle(string title)
    {
        var book = await GetBookFromResult(title);

        if (book is null || book.isDeleted)
            return NotFound();
        
        await _bookStoreService.UpdateViewCountByTitleAsync(book.title);
        
        return Ok(book);
    }
    
    /// <summary>
    /// Adds a new book to the repository if the book with the new one's title doesn't exist.
    /// </summary>
    /// <param name="newBook">The book user wants added to repository</param>
    /// <returns>
    /// HTTP 201 and the book which was created, if the creation was successful, or HTTP 400 if the given book
    /// is null or the book with this title already exists. 
    /// </returns>
    [HttpPost("/addBook", Name = "AddBookToDatabase")]
    public async Task<ActionResult<Book>> AddBook(Book newBook)
    {
        // check if the request body is empty
        if (newBook is null)
            return BadRequest();
        
        // check if the new book's title is already in the repo
        if (await DoesBookExist(newBook.title))
            return BadRequest();
        
        await _bookStoreService.CreateBookAsync(newBook);

        return CreatedAtAction(nameof(GetBookByTitle), new {newBook.title}, newBook);
    }
    
    /// <summary>
    /// Adds the given list of books to repository, omits ones with duplicate names and those who share title with existing books
    /// </summary>
    /// <param name="books">The list of books user wants added</param>
    /// <returns>HTTP 201</returns>
    [HttpPost("/addBooksBulk", Name = "AddBooksWithAList")]
    public async Task<IActionResult> AddBulk(List<Book> books)
    {
        books = books.DistinctBy(b => b.title).ToList();

        foreach (var book in books)
        {
            if (!await DoesBookExist(book.title))
            {
                await _bookStoreService.CreateBookAsync(book);
            } 
        }
        
        return Created();
    }

    /// <summary>
    /// Updates the book with the given title by the given book object.
    /// </summary>
    /// <param name="title">Book user wants updated</param>
    /// <param name="updatedBook">Book user wants to update with </param>
    /// <returns>
    /// HTTP 404 if a book with the given title doesn't exists, HTTP 400 if the given book is null or its title
    /// conflicts with existing books, HTTP 204 if update happens.
    /// </returns>
    [HttpPut("/updateBook/{title}", Name = "UpdateBookByTitle")]
    public async Task<IActionResult> UpdateBook(string title, Book updatedBook)
    {
        var bookToUpdate = await GetBookFromResult(title);
        
        //check that book with given title exists
        if (bookToUpdate is null) 
            return NotFound();
        
        //check that updated book isn't null and that it doesn't conflict with any existing books
        if (updatedBook is null || (await DoesBookExist(title) && title != updatedBook.title))
            return BadRequest();

        await _bookStoreService.UpdateByTitleAsync(title, updatedBook);

        return NoContent();
    }

    /// <summary>
    /// Soft deletes book with given title
    /// </summary>
    /// <param name="title">Title of the book to be deleted</param>
    /// <returns>HTTP 404 if the book with the title doesn't exists, HTTP 204 if deletion happens</returns>
    [HttpDelete("/deleteByTitle/{title}", Name = "DeleteBookByTitle")]
    public async Task<IActionResult> DeleteBookByTitle(string title)
    {
        if (!await DoesBookExist(title)) 
            return NotFound();

        _ = _bookStoreService.DeleteByTitleAsync(title);

        return NoContent();
    }

    /// <summary>
    /// Soft deletes books with given titles. Omits non-existent ones.
    /// </summary>
    /// <param name="titles">List of titles to be deleted</param>
    /// <returns>HTTP 204</returns>
    [HttpDelete("/bulkDelete", Name = "SoftDeleteBooksBulk")]
    public async Task<IActionResult> BulkDeleteBooksByTitle(List<string> titles)
    {
        foreach (var title in titles)
        {
            if (await DoesBookExist(title))
            {
                await _bookStoreService.DeleteByTitleAsync(title);
            }
        }

        return NoContent();
    }
    
    //AUXILIARY FUNCTIONS
    
    /// <summary>
    /// Checks if the book with the given title exits in the repository.
    /// </summary>
    private async Task<bool> DoesBookExist(string title) 
        => await _bookStoreService.GetByTitleAsync(title) is not null;

    /// <summary>
    /// Returns the book with the given title from repository. 
    /// </summary>
    private async Task<Book?> GetBookFromResult(string title)
        => await _bookStoreService.GetByTitleAsync(title);
}
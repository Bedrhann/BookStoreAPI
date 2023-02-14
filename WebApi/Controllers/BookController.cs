using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.BookOperations.Commands.CreateBook;
using WebApi.Application.BookOperations.Commands.DeleteBook;
using WebApi.Application.BookOperations.Commands.UpdateBook;
using WebApi.Application.BookOperations.Queries.GetBooks;
using WebApi.Application.BookOperations.Queries.GetByIdBook;
using WebApi.DBOperations;

namespace WebApi.AddControllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]s")]
    public class BookController : ControllerBase
    {
        private readonly IBookStoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public BookController(IBookStoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            GetBooksQuery booksQuery = new GetBooksQuery(_dbContext, _mapper);
            var result = booksQuery.Handle();
            return Ok(result);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            GetByIdBookQuery.GetByIdBookModel result;
            GetByIdBookQueryValidator validator = new GetByIdBookQueryValidator();
            GetByIdBookQuery query = new GetByIdBookQuery(_dbContext);
            query.BookId = id;
            validator.ValidateAndThrow(query);
            result = query.Handle();



            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] CreateBooksCommand.CreateBookModel newBook)
        {
            CreateBooksCommand booksCommand = new CreateBooksCommand(_dbContext, _mapper);

            booksCommand.Model = newBook;
            CreateBookCommandValidator validator = new CreateBookCommandValidator();
            validator.ValidateAndThrow(booksCommand);
            booksCommand.Handle();
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] UpdateBookViewCommand.UpdateBookModel updatedBook)
        {

            UpdateBookViewCommand updateViewCommand = new UpdateBookViewCommand(_dbContext);
            UpdateBookViewCommandValidator validator = new UpdateBookViewCommandValidator();
            updateViewCommand.Id = id;
            updateViewCommand.Model = updatedBook;

            validator.ValidateAndThrow(updateViewCommand);
            updateViewCommand.Handle();

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            DeleteBookCommand deleteBookCommand = new DeleteBookCommand(_dbContext);
            deleteBookCommand.BookId = id;
            DeleteBookCommandValidator validator = new DeleteBookCommandValidator();
            validator.ValidateAndThrow(deleteBookCommand);
            deleteBookCommand.Handle();

            return Ok();
        }

    }
}
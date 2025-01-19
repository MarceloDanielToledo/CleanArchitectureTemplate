using Application.Exceptions;
using Application.UseCases.Products.Commands;
using Application.UseCases.Products.Queries;
using Application.UseCases.Products.Requests;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProductsController : BaseApiController
    {

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="request">Request body.</param>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            return CreatedAtAction(nameof(Create), await Mediator.Send(new CreateProductCommand(request)));
        }
        /// <summary>
        /// Edit a product
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="request">Request</param>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditProductRequest request)
        {
            if (id != request.Id)
            {
                throw new ValidationException("The ID provided in the URL does not match the product ID.");
            }
            return Ok(await Mediator.Send(new EditProductCommand(request)));
        }
        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <param name="id">Id</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetProductByIdQuery(id)));
        }

    }
}

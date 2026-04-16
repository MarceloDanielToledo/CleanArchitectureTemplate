using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.UseCases.Products.Commands;
using Application.UseCases.Products.Queries;
using Application.UseCases.Products.Requests;
using Application.UseCases.Products.Responses;
using Application.Wrappers;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ProductsController(
        ICommandHandler<CreateProductCommand, Response<ProductResponse>> createHandler,
        ICommandHandler<EditProductCommand, Response<ProductResponse>> editHandler,
        IQueryHandler<GetProductByIdQuery, Response<ProductResponse>> getByIdHandler
    ) : BaseApiController
    {

        /// <summary>
        /// Create a new product.
        /// </summary>
        /// <param name="request">Request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Product created successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType<Response<ProductResponse>>(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        {
            return CreatedAtAction(nameof(Create), await createHandler.HandleAsync(new CreateProductCommand(request), cancellationToken));
        }
        /// <summary>
        /// Edit a product
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="request">Request</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Product updated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">Product not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType<Response<ProductResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit(int id, [FromBody] EditProductRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
            {
                throw new ValidationException("The ID provided in the URL does not match the product ID.");
            }
            return Ok(await editHandler.HandleAsync(new EditProductCommand(request), cancellationToken));
        }
        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the product.</response>
        /// <response code="404">Product not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType<Response<ProductResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            return Ok(await getByIdHandler.HandleAsync(new GetProductByIdQuery(id), cancellationToken));
        }

    }
}

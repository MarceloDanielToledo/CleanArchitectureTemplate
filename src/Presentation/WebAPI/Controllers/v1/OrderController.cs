using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.UseCases.Orders.Commands;
using Application.UseCases.Orders.Queries;
using Application.UseCases.Orders.Requests;
using Application.UseCases.Orders.Responses;
using Application.Wrappers;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1
{
    /// <summary>
    /// Order Controller
    /// </summary>
    [ApiVersion("1.0")]
    public class OrderController(
        ICommandHandler<CreateOrderCommand, Response<OrderResponse>> createHandler,
        ICommandHandler<EditOrderCommand, Response<OrderResponse>> editHandler,
        ICommandHandler<DeleteOrderCommand, Response<string>> deleteHandler,
        IQueryHandler<GetOrderByIdQuery, Response<OrderResponse>> getByIdHandler
    ) : BaseApiController
    {
        /// <summary>
        /// Create a new order.
        /// </summary>
        /// <param name="request">Request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Order created successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType<Response<OrderResponse>>(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
            => CreatedAtAction(nameof(Create), await createHandler.HandleAsync(new CreateOrderCommand(request), cancellationToken));


        /// <summary>
        /// Edit order
        /// </summary>
        /// <param name="request">Request body.</param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Order updated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">Order not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType<Response<OrderResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit(int id, [FromBody] EditOrderRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
            {
                throw new ValidationException("The ID provided in the URL does not match the order ID.");
            }
            return Ok(await editHandler.HandleAsync(new EditOrderCommand(request), cancellationToken));
        }

        /// <summary>
        /// Delete order
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Order deleted successfully.</response>
        /// <response code="404">Order not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType<Response<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
            => Ok(await deleteHandler.HandleAsync(new DeleteOrderCommand(id), cancellationToken));


        /// <summary>
        /// Get order by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the order.</response>
        /// <response code="404">Order not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType<Response<OrderResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
            => Ok(await getByIdHandler.HandleAsync(new GetOrderByIdQuery(id), cancellationToken));
    }
}

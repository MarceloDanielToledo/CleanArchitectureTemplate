using Application.Abstractions.Messaging;
using Application.Exceptions;
using Application.UseCases.OrderItems.Commands;
using Application.UseCases.OrderItems.Queries;
using Application.UseCases.OrderItems.Requests;
using Application.UseCases.OrderItems.Responses;
using Application.Wrappers;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1
{
    /// <summary>
    /// Order Item Controller
    /// </summary>
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/order/{orderId}/item")]
    public class OrderItemController(
        ICommandHandler<CreateOrderItemCommand, Response<OrderItemResponse>> createHandler,
        ICommandHandler<DeleteOrderItemCommand, Response<string>> deleteHandler,
        ICommandHandler<EditOrderItemCommand, Response<OrderItemResponse>> editHandler,
        IQueryHandler<GetOrderItemByIdQuery, Response<OrderItemResponse>> getByIdHandler
    ) : BaseApiController
    {
        /// <summary>
        /// Create a new order item.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="request">Request body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Order item created successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">Order not found.</response>
        /// <response code="500">Internal server error.</response>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [ProducesResponseType<Response<OrderItemResponse>>(StatusCodes.Status201Created)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(int orderId,[FromBody] CreateOrderItemRequest request, CancellationToken cancellationToken)
            => CreatedAtAction(nameof(Create), await createHandler.HandleAsync(new CreateOrderItemCommand(orderId, request), cancellationToken));

        /// <summary>
        /// Delete order item
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Order item deleted successfully.</response>
        /// <response code="404">Order item not found.</response>
        /// <response code="500">Internal server error.</response>
        [Microsoft.AspNetCore.Mvc.HttpDelete("{id:int}")]
        [ProducesResponseType<Response<string>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int orderId, int id, CancellationToken cancellationToken)
            => Ok(await deleteHandler.HandleAsync(new DeleteOrderItemCommand(orderId, id), cancellationToken));

        /// <summary>
        /// Edit order item
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="request">EditOrderItem Request</param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Order item updated successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="404">Order item not found.</response>
        /// <response code="500">Internal server error.</response>
        [Microsoft.AspNetCore.Mvc.HttpPut("{id:int}")]
        [ProducesResponseType<Response<OrderItemResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Edit(int orderId, int id, [FromBody] EditOrderItemRequest request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
            {
                throw new ValidationException("The ID provided in the URL does not match the record ID.");
            }
            return Ok(await editHandler.HandleAsync(new EditOrderItemCommand(orderId, request), cancellationToken));
        }

        /// <summary>
        /// Get order item by Id
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="id">Id</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the order item.</response>
        /// <response code="404">Order item not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType<Response<OrderItemResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType<Response<object>>(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int orderId,int id, CancellationToken cancellationToken)
            => Ok(await getByIdHandler.HandleAsync(new GetOrderItemByIdQuery(orderId,id), cancellationToken));

    }
}

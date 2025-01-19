using Application.UseCases.OrderItems.Commands;
using Application.UseCases.OrderItems.Queries;
using Application.UseCases.OrderItems.Requests;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Controllers.v1
{
    /// <summary>
    /// Order Item Controller
    /// </summary>
    [ApiVersion("1.0")]
    [Microsoft.AspNetCore.Mvc.Route("api/v{version:apiVersion}/order/{orderId}/item")]
    public class OrderItemController : BaseApiController
    {
        /// <summary>
        /// Create a new order item.
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="request">Request body.</param>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> Create(int orderId,[FromBody] CreateOrderItemRequest request)
            => CreatedAtAction(nameof(Create), await Mediator.Send(new CreateOrderItemCommand(orderId, request)));

        /// <summary>
        /// Delete order item
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="id">Id</param>
        [Microsoft.AspNetCore.Mvc.HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int orderId, int id)
            => Ok(await Mediator.Send(new DeleteOrderItemCommand(orderId, id)));

        /// <summary>
        /// Edit order item
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="request">EditOrderItem Request</param>
        /// <param name="id">Id</param>
        [Microsoft.AspNetCore.Mvc.HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int orderId, int id, [FromBody] EditOrderItemRequest request)
        {
            if (id != request.Id)
            {
                throw new ValidationException("The ID provided in the URL does not match the record ID.");
            }
            return Ok(await Mediator.Send(new EditOrderItemCommand(orderId, request)));
        }

        /// <summary>
        /// Get order item by Id
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="id">Id</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int orderId,int id)
            => Ok(await Mediator.Send(new GetOrderItemByIdQuery(orderId,id)));

    }
}

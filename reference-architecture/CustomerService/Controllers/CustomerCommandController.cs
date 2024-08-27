using AutoMapper;
using Common.CQRS.Abstration.command;
using CustomerService.Domain.CustomerAggregate;
using CustomerService.Domain.CustomerAggregate.Commands;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerCommandController : ControllerBase
    {
        private readonly ICommandBroker _CommandBroker;
        private readonly IMapper _mapper;

        public CustomerCommandController(
            ICommandBroker mediator,
            IMapper mapper)
        {
            _CommandBroker = mediator;
            _mapper = mapper;
        }

        // POST api/customer
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTO.Write.Customer customerDto)
        {
            var customerIn = _mapper.Map<Customer>(customerDto);
            var result = await _CommandBroker.SendAsync(new CreateCustomer(customerIn));
            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var customerOut = _mapper.Map<DTO.Write.Customer>(result.Entity);
            return CreatedAtAction(nameof(Create), new { id = customerOut.Id }, customerOut);
        }

        // PUT api/customer
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DTO.Write.Customer customerDto)
        {
            var customerIn = _mapper.Map<Customer>(customerDto);
            var result = await _CommandBroker.SendAsync(new UpdateCustomer(customerIn));
            if (result.Outcome != CommandOutcome.Accepted)
                return result.ToActionResult();
            var customerOut = _mapper.Map<DTO.Write.Customer>(result.Entity);
            return Ok(customerOut);
        }

        // DELETE api/customer/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            var result = await _CommandBroker.SendAsync(new RemoveCustomer(id));
            return result.Outcome != CommandOutcome.Accepted
                ? result.ToActionResult()
                : NoContent();
        }
    }
}
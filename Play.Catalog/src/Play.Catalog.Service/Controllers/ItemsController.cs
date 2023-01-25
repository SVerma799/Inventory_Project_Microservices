using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using System;
using System.Linq;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Interfaces;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemsRepository _repository;

        public ItemsController(IItemsRepository _repository)
        {
            this._repository = _repository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await _repository.GetAllAsync()).Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = (await _repository.GetAsync(id));
            if (item == null)
                return NotFound();
            else
                return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            if (itemDto == null)
                return BadRequest();

            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _repository.CreateAsync(item);
            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("id")]
        public async Task<ActionResult<ItemDto>> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {

            Item existingItem = await _repository.GetAsync(id);
            if (existingItem == null)
                return NotFound();
            else
            {
                existingItem.Name = itemDto.Name;
                existingItem.Description = itemDto.Description;
                existingItem.Price = itemDto.Price;
                await _repository.UpdateAsync(existingItem);
                return NoContent();
            }
        }

        [HttpDelete("id")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            Item existingItem = await _repository.GetAsync(id);
            if (existingItem == null)
                return NotFound();
            else
            {
                await _repository.DeleteAsync(existingItem.Id);
                return NoContent();
            }
        }
    }
}
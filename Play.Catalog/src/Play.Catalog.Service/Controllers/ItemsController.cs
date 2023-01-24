using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using System;
using System.Linq;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze Sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
        };

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync()
        {
            return await Task.FromResult(Ok(items));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = items.FirstOrDefault(item => item.Id == id);
            if (item == null)
                return NotFound();
            else
                return await Task.FromResult(Ok(item));
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync(CreateItemDto itemDto)
        {
            ItemDto item = new ItemDto(Guid.NewGuid(), itemDto.Name, itemDto.Description, itemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);
            return await Task.FromResult(CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item));
        }

        [HttpPut("id")]
        public async Task<ActionResult<ItemDto>> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            ItemDto existingItem = items.FirstOrDefault(item => item.Id == id);
            if (existingItem == null)
                return NotFound();
            else
            {
                ItemDto updatedItem = existingItem with
                {
                    Name = itemDto.Name,
                    Description = itemDto.Description,
                    Price = itemDto.Price
                };
                int index = items.FindIndex(existingItem => existingItem.Id == id);
                items[index] = updatedItem;
                return await Task.FromResult(Ok(updatedItem));
            }
        }

        [HttpDelete("id")]
        public async Task<ActionResult> DeleteItemAsync(Guid id)
        {
            ItemDto existingItem = items.FirstOrDefault(item => item.Id == id);
            if (existingItem == null)
                return NotFound();
            else
            {
                items.Remove(existingItem);
                return await Task.FromResult(NoContent());
            }
        }
    }
}
namespace MVCWEB.Models
{
    public interface IItemRepository
    {
        
        IEnumerable<ItemDto> GetAllItems();
    }
}

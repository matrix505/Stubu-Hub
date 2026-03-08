
namespace MVCWEB.Models
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<IItemRepository> _logger;

        public ItemRepository(ILogger<IItemRepository> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IEnumerable<ItemDto> GetAllItems()
        {
            var items = new List<ItemDto>();
            try
            {
                return _context.Items.ToList();
            }
            catch (Exception ex) {
                _logger.LogError(ex, "try");
                throw;
            }
        }
    }
}

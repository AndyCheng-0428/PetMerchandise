using PetMerchandise.core.db.entity;

namespace PetMerchandise.core.db;

public class ContextManager
{
    private static volatile SaleContext _context;


    public static SaleContext GetInstance()
    {
        if (_context == null)
        {
            _context = new SaleContext();
        }

        return _context;
    }
}
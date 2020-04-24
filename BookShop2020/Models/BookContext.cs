using System.Data.Entity;


namespace BookShop2020.Models
{
    public class BookContext : DbContext
    {
        static BookContext()
        {
            Database.SetInitializer<BookContext>(null);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<CartItem> ShoppingCarts { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
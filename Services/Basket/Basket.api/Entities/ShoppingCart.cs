namespace Basket.api.Entities
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {

        }
        public ShoppingCart(string username)
        {
            UserName = username;
        }

        public string UserName { get; set; }
        public List<ShoppingCardItem> Items { get; set; }

        public decimal TotalPrice 
        { 
            get 
            {
                decimal totalPrice = 0;
                if(Items != null && Items.Any())
                {
                    foreach (var item in Items)
                    {
                        totalPrice += item.Price * item.Quantity;

                    }
                }
                return totalPrice;
            } 
        }
    }
}

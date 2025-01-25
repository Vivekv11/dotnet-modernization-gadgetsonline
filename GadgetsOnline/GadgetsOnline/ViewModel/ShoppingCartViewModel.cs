using System.Collections.Generic;
using GadgetsOnline.Models;

namespace GadgetsOnline.ViewModel
{
    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
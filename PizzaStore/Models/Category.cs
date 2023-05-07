namespace PizzaStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Pizza> Pizzas { get;set; }
    }
}

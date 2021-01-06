using Nest;

namespace Es.Models
{

    public class CategoriesModel
    {
        [PropertyName("category"),Nested]
        public CategoryModel[] Category { get; set; }
    }

    public class CategoryModel
    {
        [Number(Name = "id",Index =true)]
        public int Id { get; set; }
        [Text(Name = "categoryname")]
        public string CategoryName { get; set; }
        [Text(Name = "sename")]
        public string SeName { get; set; }
        [Number(Name = "parentid")]
        public int Parent { get; set; }
    }
}

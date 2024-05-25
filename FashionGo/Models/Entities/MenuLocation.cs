namespace FashionGo.Models.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class MenuLocation
    {
        public MenuLocation()
        {
            Menus = new HashSet<Menu>();
        }

        [Key]
        public int Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; }

        public virtual ICollection<Menu> Menus { get; set; }
    }
}

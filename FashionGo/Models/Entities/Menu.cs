namespace FashionGo.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    public enum Target
    {
        _blank, _self, _parent, _top, framename
    };

    public partial class Menu
    {


        [Key]
        public int Id { get; set; }

        public int? LocationId { get; set; }

        [StringLength(250)]
        public string Text { get; set; }

        [StringLength(250)]
        public string Url { get; set; }

        public Target Target { get; set; }
        public int Catld { get; set; }


        public virtual MenuLocation MenuLocation { get; set; }
    }
}

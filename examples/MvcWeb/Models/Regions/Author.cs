using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace MvcWeb.Models.Regions
{
    /// <summary>
    /// Simple link region.
    /// </summary>
    public class Author
    {
        [Field(Title = "Button Text", Options = FieldOption.HalfWidth)]
        public StringField ButtonText { get; set; }

        [Field(Title = "Button Link", Options = FieldOption.HalfWidth)]
        public PageField ButtonLink { get; set; }
    }
}
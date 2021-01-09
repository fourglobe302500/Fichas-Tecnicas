using System.Collections.Generic;

using CLI.Utilities;

namespace CLI.Entities.Structs
{
    internal struct RecipeStruct
    {
        public int id;
        public string name;
        public Link[] links;

        public RecipeStruct(int id, string name, Link[] links)
        {
            this.id = id;
            this.name = name;
            this.links = links;
        }

        public override bool Equals(object obj) => obj is RecipeStruct other && id == other.id && name == other.name && EqualityComparer<Link[]>.Default.Equals(links, other.links);

        public override int GetHashCode( ) => System.HashCode.Combine(id, name, links);

        public void Deconstruct(out int id, out string name, out Link[] links)
        {
            id = this.id;
            name = this.name;
            links = this.links;
        }

        public override string ToString( ) => $"{id},{name},{links.ToArrayString()}";

        public static implicit operator (int id, string name, Link[] links)(RecipeStruct value) => (value.id, value.name, value.links);

        public static implicit operator RecipeStruct((int id, string name, Link[] links) value) => new RecipeStruct(value.id, value.name, value.links);
    }
}
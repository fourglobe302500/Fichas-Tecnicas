using System.Collections.Generic;

using CLI.Utilities;

namespace CLI.Entities.Structs
{
    internal struct RecipeStruct
    {
        public int id;
        public string name;
        public float rendimento;
        public Link[] links;

        public RecipeStruct(int id, string name, float rendimento, Link[] links)
        {
            this.id = id;
            this.name = name;
            this.rendimento = rendimento;
            this.links = links;
        }

        public override bool Equals(object obj) => obj is RecipeStruct other && id == other.id && name == other.name && rendimento == other.rendimento && EqualityComparer<Link[]>.Default.Equals(links, other.links);

        public override int GetHashCode() => System.HashCode.Combine(id, name, rendimento, links);

        public void Deconstruct(out int id, out string name, out float rendimento, out Link[] links)
        {
            id = this.id;
            name = this.name;
            rendimento = this.rendimento;
            links = this.links;
        }

        public override string ToString() => $"{id},{name},{rendimento},{links.ToArrayString()}";

        public static implicit operator (int id, string name, Link[] links)(RecipeStruct value) => (value.id, value.name, value.links);

        public static implicit operator RecipeStruct((int id, string name, float rendimento, Link[] links) value) => new RecipeStruct(value.id, value.name, value.rendimento, value.links);
    }
}
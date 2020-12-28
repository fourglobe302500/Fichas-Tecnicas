using System;

namespace CLI.Entities
{
    internal struct IngredientStruct
    {
        public int id;
        public string name;
        public float rendimento;
        public float price;

        public IngredientStruct(int id, string name, float rendimento, float price)
        {
            this.id = id;
            this.name = name;
            this.rendimento = rendimento;
            this.price = price;
        }

        public override bool Equals(object obj) => obj is IngredientStruct other && id == other.id && name == other.name && rendimento == other.rendimento && price == other.price;

        public override int GetHashCode( ) => HashCode.Combine(id, name, rendimento, price);

        public void Deconstruct(out int id, out string name, out float rendimento, out float price)
        {
            id = this.id;
            name = this.name;
            rendimento = this.rendimento;
            price = this.price;
        }

        public override string ToString( ) => $"{id},{name},{rendimento.ToString().Replace(',', '.')},{price.ToString().Replace(',', '.')}";

        public static implicit operator (int id, string name, float rendimento, float price)(IngredientStruct value) => (value.id, value.name, value.rendimento, value.price);

        public static implicit operator IngredientStruct((int id, string name, float rendimento, float price) value) => new IngredientStruct(value.id, value.name, value.rendimento, value.price);
    }
}
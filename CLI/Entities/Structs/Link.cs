namespace CLI.Entities.Structs
{
    internal struct Link
    {
        public int ingredientId;
        public float quantity;
        public float price;

        public Link(int ingredientId, float quantity, float price)
        {
            this.ingredientId = ingredientId;
            this.quantity = quantity;
            this.price = price;
        }

        public override bool Equals(object obj) => obj is Link other && ingredientId == other.ingredientId && quantity == other.quantity && price == other.price;

        public override int GetHashCode( ) => System.HashCode.Combine(ingredientId, quantity, price);

        public void Deconstruct(out int ingredientId, out float quantity, out float price)
        {
            ingredientId = this.ingredientId;
            quantity = this.quantity;
            price = this.price;
        }

        public string ToStringPrint( ) => $"{{id: {ingredientId}, Quantidade: {quantity}, Preço/Un: {price}}}";

        public override string ToString( ) => $"{{{ingredientId}:{quantity.ToString().Replace(',', '.')}:{price.ToString().Replace(',', '.')}}}";

        public static implicit operator (int ingredientId, float quantity, float price)(Link value) => (value.ingredientId, value.quantity, value.price);

        public static implicit operator Link((int ingredientId, float quantity, float price) value) => new Link(value.ingredientId, value.quantity, value.price);
    }
}
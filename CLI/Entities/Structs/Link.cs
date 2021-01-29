namespace CLI.Entities.Structs
{
    internal struct Link
    {
        public int ingredientId;
        public float quantity;

        public Link(int ingredientId, float quantity)
        {
            this.ingredientId = ingredientId;
            this.quantity = quantity;
        }

        public override bool Equals(object obj) => obj is Link other && ingredientId == other.ingredientId && quantity == other.quantity;

        public override int GetHashCode() => System.HashCode.Combine(ingredientId, quantity);

        public void Deconstruct(out int ingredientId, out float quantity)
        {
            ingredientId = this.ingredientId;
            quantity = this.quantity;
        }

        public string ToStringPrint() => $"{{id: {ingredientId}, Quantidade: {quantity}}}";

        public override string ToString() => $"{{{ingredientId}:{quantity.ToString().Replace(',', '.')}}}";

        public static implicit operator (int ingredientId, float quantity)(Link value) => (value.ingredientId, value.quantity);

        public static implicit operator Link((int ingredientId, float quantity) value) => new Link(value.ingredientId, value.quantity);
    }
}
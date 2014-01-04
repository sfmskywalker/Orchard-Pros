namespace OrchardPros.Models {
    public class Country {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }

        public override string ToString() {
            return Name;
        }
    }
}
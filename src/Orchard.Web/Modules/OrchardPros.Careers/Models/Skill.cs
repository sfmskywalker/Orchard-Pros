namespace OrchardPros.Careers.Models {
    public class Skill {
        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual string Name { get; set; }
        public virtual int Rating { get; set; }
    }
}
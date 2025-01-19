namespace Domain.Contracts
{
    public class BaseEntity
    {
        public virtual int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}

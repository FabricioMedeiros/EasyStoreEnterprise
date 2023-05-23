using ESE.Payments.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESE.Payments.API.Data.Mapping
{
    public class PaymentMapping : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Ignore(c => c.CreditCard);

            // 1 : N => Pagamento : Transacao
            builder.HasMany(c => c.Transactions)
                .WithOne(c => c.Payment)
                .HasForeignKey(c => c.PaymentId);

            builder.ToTable("Payments");
        }
       
    }
 
}

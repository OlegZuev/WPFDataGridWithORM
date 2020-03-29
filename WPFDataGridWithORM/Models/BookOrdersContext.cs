using System.Data.Entity;

namespace WPFDataGridWithORM.Models {
    public partial class BookOrdersContext : DbContext {
        public BookOrdersContext()
            : base("name=BookOrdersContext") {
        }

        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<LiteratureType> LiteratureTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<LiteratureType>()
                .HasMany(e => e.Genres)
                .WithRequired(e => e.LiteratureType)
                .HasForeignKey(e => e.LiteratureTypeId)
                .WillCascadeOnDelete(false);
        }
    }
}

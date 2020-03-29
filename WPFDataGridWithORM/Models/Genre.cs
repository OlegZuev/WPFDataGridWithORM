using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PropertyChanged;
using ReactiveValidation;
using ReactiveValidation.Extensions;

namespace WPFDataGridWithORM.Models {
    [AddINotifyPropertyChangedInterface]
    [Table("public.genres")]
    public class Genre : ValidatableObject, IEditableObject {
        [Column("id")] public int Id { get; set; }

        [Required]
        [StringLength(8000)]
        [Column("name")]
        public string Name { get; set; }

        [Column("popularity")] public int Popularity { get; set; }

        [Column("literature_type_id")] public int LiteratureTypeId { get; set; }

        public virtual LiteratureType LiteratureType { get; set; }

        public bool IsDirty => _backupCopy == null || Name != _backupCopy.Name ||
                               Popularity != _backupCopy.Popularity ||
                               LiteratureTypeId != _backupCopy.LiteratureTypeId;

        private Genre _backupCopy;

        private bool _inEdit;

        public void BeginEdit() {
            if (_inEdit) return;
            _inEdit = true;
            _backupCopy = MemberwiseClone() as Genre;
        }

        public void CancelEdit() {
            if (!_inEdit) return;
            _inEdit = false;
            Id = _backupCopy.Id;
            Name = _backupCopy.Name;
            Popularity = _backupCopy.Popularity;
            LiteratureTypeId = _backupCopy.LiteratureTypeId;
        }

        public void EndEdit() {
            if (!_inEdit) return;
            _inEdit = false;
            _backupCopy = null;
        }

        public void InitializeValidator(IList<Genre> genres = null) {
            Validator = GetValidator(genres);
        }

        private IObjectValidator GetValidator(IList<Genre> genres = null) {
            var builder = new ValidationBuilder<Genre>();

            builder.RuleFor(genre => genre.Name)
                   .NotEmpty()
                   .WithMessage("Name can't be empty");
            builder.RuleFor(genre => genre.Name)
                   .Must(name => genres == null || !genres.Any(genre => genre.Id != Id && genre.Name == name))
                   .WithMessage("Name should be unique");

            return builder.Build(this);
        }
    }
}
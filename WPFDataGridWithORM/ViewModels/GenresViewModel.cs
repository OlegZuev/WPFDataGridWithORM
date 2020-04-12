using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using WPFDataGridWithORM.Models;

namespace WPFDataGridWithORM.ViewModels {
    public class GenresViewModel {
        public ObservableCollection<Genre> Genres { get; set; }

        public ObservableCollection<LiteratureType> LiteratureTypes { get; set; }

        public GenresViewModel() {
            using (var context = new BookOrdersContext()) {
                Genres = new ObservableCollection<Genre>(context.Genres);
                LiteratureTypes = new ObservableCollection<LiteratureType>(context.LiteratureTypes);
            }

            foreach (Genre genre in Genres) {
                genre.PopularityText = genre.Popularity.ToString();
                genre.InitializeValidator(Genres);
            }

            DataGridAddingNewItemCommand = new DelegateCommand<AddingNewItemEventArgs>(DataGridAddingNewItem);

            PreviewKeyDownCommand = new DelegateCommand<object>(PreviewKeyDown);

            DataGridRowEditEndingCommand = new DelegateCommand<DataGridRowEditEndingEventArgs>(DataGridRowEditEnding);

            PageLoadedCommand = new DelegateCommand<object>(PageLoaded);
        }

        public ICommand DataGridAddingNewItemCommand { get; set; }

        public ICommand PreviewKeyDownCommand { get; set; }

        public ICommand DataGridBeginningEditCommand { get; set; }

        public ICommand DataGridRowEditEndingCommand { get; set; }
        public ICommand PageLoadedCommand { get; set; }

        private void DataGridAddingNewItem(AddingNewItemEventArgs e) {
            e.NewItem = new Genre();
            ((Genre) e.NewItem).InitializeValidator(Genres);
        }

        private static void PreviewKeyDown(object eventArgs) {
            var tempEventArgs = (object[]) eventArgs;
            if (!(tempEventArgs[0] is KeyEventArgs e && tempEventArgs[1] is DataGrid sender)) {
                throw new ArgumentException("Bad arguments in PreviewKeyDown");
            }

            switch (e.Key) {
                case Key.Escape:
                    sender.CancelEdit();
                    sender.CancelEdit();
                    return;
                case Key.Delete:
                    if (!(sender.SelectedItem is Genre genre)) return;
                    using (var context = new BookOrdersContext()) {
                        context.Genres.Attach(genre);
                        context.Genres.Remove(genre);
                        context.SaveChanges();
                    }

                    return;
            }
        }

        private void PageLoaded(object sender) {
            UpdateLiteratureTypes();
        }

        private void UpdateLiteratureTypes() {
            using (var context = new BookOrdersContext()) {
                for (int i = LiteratureTypes.Count - 1; i >= 0; --i) {
                    if (context.LiteratureTypes.Find(LiteratureTypes[i].Id) == null) {
                        LiteratureTypes.Remove(LiteratureTypes[i]);
                    }
                }

                foreach (LiteratureType literatureType in context.LiteratureTypes) {
                    if (LiteratureTypes.FirstOrDefault(element => element.Id == literatureType.Id) is LiteratureType
                        tempLiteratureType)
                        tempLiteratureType.Name = literatureType.Name;
                    else
                        LiteratureTypes.Add(literatureType);
                }
            }
        }

        private static void DataGridRowEditEnding(DataGridRowEditEndingEventArgs e) {
            if (e.EditAction == DataGridEditAction.Cancel || !(e.Row.Item is Genre genre) || !genre.Validator.IsValid ||
                !genre.IsDirty)
                return;

            using (var context = new BookOrdersContext()) {
                if (e.Row.IsNewItem) {
                    context.Genres.Add(genre);
                } else {
                    context.Entry(genre).State = EntityState.Modified;
                }

                context.SaveChanges();
            }
        }
    }
}
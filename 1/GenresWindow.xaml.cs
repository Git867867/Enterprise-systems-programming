using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class GenresWindow : Window
    {
        private LibraryContext _context;

        public GenresWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            LoadGenres();
        }

        private void LoadGenres()
        {
            GenresGrid.ItemsSource = _context.Genres.ToList();
        }

        private void AddGenre_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenreEditWindow(_context);
            if (window.ShowDialog() == true)
            {
                LoadGenres();
            }
        }

        private void EditGenre_Click(object sender, RoutedEventArgs e)
        {
            var selected = GenresGrid.SelectedItem as Genre;
            if (selected == null)
            {
                MessageBox.Show("Выберите жанр");
                return;
            }

            var window = new GenreEditWindow(_context, selected);
            if (window.ShowDialog() == true)
            {
                LoadGenres();
            }
        }

        private void DeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            var selected = GenresGrid.SelectedItem as Genre;
            if (selected == null)
            {
                MessageBox.Show("Выберите жанр");
                return;
            }

            if (selected.Books != null && selected.Books.Any())
            {
                MessageBox.Show("Нельзя удалить жанр, у которого есть книги");
                return;
            }

            var result = MessageBox.Show($"Удалить жанр {selected.Name}?", 
                "Подтверждение", MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes)
            {
                _context.Genres.Remove(selected);
                _context.SaveChanges();
                LoadGenres();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
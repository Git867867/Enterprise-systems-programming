using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class AuthorsWindow : Window
    {
        private LibraryContext _context;

        public AuthorsWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            AuthorsGrid.ItemsSource = _context.Authors.ToList();
        }

        private void AddAuthor_Click(object sender, RoutedEventArgs e)
        {
            var window = new AuthorEditWindow(_context);
            if (window.ShowDialog() == true)
            {
                LoadAuthors();
            }
        }

        private void EditAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selected = AuthorsGrid.SelectedItem as Author;
            if (selected == null)
            {
                MessageBox.Show("Выберите автора");
                return;
            }

            var window = new AuthorEditWindow(_context, selected);
            if (window.ShowDialog() == true)
            {
                LoadAuthors();
            }
        }

        private void DeleteAuthor_Click(object sender, RoutedEventArgs e)
        {
            var selected = AuthorsGrid.SelectedItem as Author;
            if (selected == null)
            {
                MessageBox.Show("Выберите автора");
                return;
            }

            if (selected.Books != null && selected.Books.Any())
            {
                MessageBox.Show("Нельзя удалить автора, у которого есть книги");
                return;
            }

            var result = MessageBox.Show($"Удалить автора {selected.FullName}?", 
                "Подтверждение", MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes)
            {
                _context.Authors.Remove(selected);
                _context.SaveChanges();
                LoadAuthors();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
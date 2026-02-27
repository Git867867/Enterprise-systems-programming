using System;
using System.Windows;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class GenreEditWindow : Window
    {
        private LibraryContext _context;
        private Genre _genre;

        public GenreEditWindow(LibraryContext context, Genre genre = null)
        {
            InitializeComponent();
            _context = context;
            _genre = genre ?? new Genre();

            if (genre != null)
            {
                NameBox.Text = genre.Name;
                DescriptionBox.Text = genre.Description;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Название жанра обязательно");
                return;
            }

            _genre.Name = NameBox.Text;
            _genre.Description = DescriptionBox.Text;

            if (_genre.Id == 0)
                _context.Genres.Add(_genre);
            else
                _context.Genres.Update(_genre);

            _context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
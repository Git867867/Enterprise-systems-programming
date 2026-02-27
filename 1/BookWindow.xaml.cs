using System;
using System.Linq;
using System.Windows;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class BookWindow : Window
    {
        private LibraryContext _context;
        private Book _currentBook;

        public BookWindow(LibraryContext context, Book book = null)
        {
            InitializeComponent();
            _context = context;
            _currentBook = book ?? new Book();

            LoadComboBoxes();
            
            if (book != null)
            {
                Title = "Редактирование книги";
                LoadBookData();
            }
            else
            {
                Title = "Добавление книги";
            }
        }

        private void LoadComboBoxes()
        {
            AuthorCombo.ItemsSource = _context.Authors.ToList();
            GenreCombo.ItemsSource = _context.Genres.ToList();
        }

        private void LoadBookData()
        {
            TitleTextBox.Text = _currentBook.Title;
            AuthorCombo.SelectedValue = _currentBook.AuthorId;
            GenreCombo.SelectedValue = _currentBook.GenreId;
            YearTextBox.Text = _currentBook.PublishYear.ToString();
            ISBNTextBox.Text = _currentBook.ISBN;
            QuantityTextBox.Text = _currentBook.QuantityInStock.ToString();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                ErrorText.Text = "Введите название книги";
                return false;
            }

            if (AuthorCombo.SelectedItem == null)
            {
                ErrorText.Text = "Выберите автора";
                return false;
            }

            if (GenreCombo.SelectedItem == null)
            {
                ErrorText.Text = "Выберите жанр";
                return false;
            }

            if (!int.TryParse(YearTextBox.Text, out int year) || year < 1800 || year > DateTime.Now.Year)
            {
                ErrorText.Text = "Введите корректный год";
                return false;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                ErrorText.Text = "Введите корректное количество";
                return false;
            }

            return true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                _currentBook.Title = TitleTextBox.Text;
                _currentBook.AuthorId = (int)AuthorCombo.SelectedValue;
                _currentBook.GenreId = (int)GenreCombo.SelectedValue;
                _currentBook.PublishYear = int.Parse(YearTextBox.Text);
                _currentBook.ISBN = ISBNTextBox.Text;
                _currentBook.QuantityInStock = int.Parse(QuantityTextBox.Text);

                if (_currentBook.Id == 0)
                {
                    _context.Books.Add(_currentBook);
                }
                else
                {
                    _context.Books.Update(_currentBook);
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
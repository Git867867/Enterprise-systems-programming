using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class MainWindow : Window
    {
        private LibraryContext _context;
        private List<Book> _allBooks;

        public MainWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Создание БД
                _context.Database.EnsureCreated();

                // Загрузка всех книг с авторами и жанрами
                _allBooks = _context.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .ToList();

                // Загрузка авторов и жанры для фильтров
                var authors = _context.Authors.ToList();
                var genres = _context.Genres.ToList();

                // Пустые элементы для сброса фильтра
                authors.Insert(0, new Author { Id = 0, FirstName = "Все", LastName = "авторы" });
                genres.Insert(0, new Genre { Id = 0, Name = "Все жанры" });

                AuthorFilterCombo.ItemsSource = authors;
                GenreFilterCombo.ItemsSource = genres;

                // Отображение всех книги
                BooksDataGrid.ItemsSource = _allBooks;
                UpdateTotalBooksCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void UpdateTotalBooksCount()
        {
            int total = _allBooks.Sum(b => b.QuantityInStock);
            TotalBooksText.Text = $"Всего книг: {total}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var filteredBooks = _allBooks.AsEnumerable();

            // Фильтр по поиску
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string search = SearchTextBox.Text.ToLower();
                filteredBooks = filteredBooks.Where(b => 
                    b.Title.ToLower().Contains(search) || 
                    b.ISBN.ToLower().Contains(search));
            }

            // Фильтр по автору
            var selectedAuthor = AuthorFilterCombo.SelectedItem as Author;
            if (selectedAuthor != null && selectedAuthor.Id != 0)
            {
                filteredBooks = filteredBooks.Where(b => b.AuthorId == selectedAuthor.Id);
            }

            // Фильтр по жанру
            var selectedGenre = GenreFilterCombo.SelectedItem as Genre;
            if (selectedGenre != null && selectedGenre.Id != 0)
            {
                filteredBooks = filteredBooks.Where(b => b.GenreId == selectedGenre.Id);
            }

            BooksDataGrid.ItemsSource = filteredBooks.ToList();
        }

        private void ResetFilter_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            AuthorFilterCombo.SelectedIndex = 0;
            GenreFilterCombo.SelectedIndex = 0;
            BooksDataGrid.ItemsSource = _allBooks;
        }

        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            var window = new BookWindow(_context);
            if (window.ShowDialog() == true)
            {
                RefreshBooks();
            }
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = BooksDataGrid.SelectedItem as Book;
            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу для редактирования");
                return;
            }

            var window = new BookWindow(_context, selectedBook);
            if (window.ShowDialog() == true)
            {
                RefreshBooks();
            }
        }

        private void DeleteBook_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = BooksDataGrid.SelectedItem as Book;
            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу для удаления");
                return;
            }

            var result = MessageBox.Show($"Удалить книгу '{selectedBook.Title}'?", 
                "Подтверждение", MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes)
            {
                _context.Books.Remove(selectedBook);
                _context.SaveChanges();
                RefreshBooks();
            }
        }

        private void ManageAuthors_Click(object sender, RoutedEventArgs e)
        {
            var window = new AuthorsWindow(_context);
            window.ShowDialog();
            RefreshBooks();
        }

        private void ManageGenres_Click(object sender, RoutedEventArgs e)
        {
            var window = new GenresWindow(_context);
            window.ShowDialog();
            RefreshBooks();
        }

        private void RefreshBooks()
        {
            _allBooks = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToList();
            ApplyFilters();
            UpdateTotalBooksCount();
        }
    }
}
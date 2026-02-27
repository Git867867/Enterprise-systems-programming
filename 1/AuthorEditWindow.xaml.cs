using System;
using System.Windows;
using LibraryManagement.Data;
using LibraryManagement.Models;

namespace LibraryManagement
{
    public partial class AuthorEditWindow : Window
    {
        private LibraryContext _context;
        private Author _author;

        public AuthorEditWindow(LibraryContext context, Author author = null)
        {
            InitializeComponent();
            _context = context;
            _author = author ?? new Author();

            if (author != null)
            {
                FirstNameBox.Text = author.FirstName;
                LastNameBox.Text = author.LastName;
                BirthYearBox.Text = author.BirthYear.ToString();
                CountryBox.Text = author.Country;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || 
                string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Имя и фамилия обязательны");
                return;
            }

            if (!int.TryParse(BirthYearBox.Text, out int year) || year < 1800 || year > DateTime.Now.Year)
            {
                MessageBox.Show("Введите корректный год рождения");
                return;
            }

            _author.FirstName = FirstNameBox.Text;
            _author.LastName = LastNameBox.Text;
            _author.BirthYear = year;
            _author.Country = CountryBox.Text;

            if (_author.Id == 0)
                _context.Authors.Add(_author);
            else
                _context.Authors.Update(_author);

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
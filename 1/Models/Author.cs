using System.Collections.Generic;

namespace LibraryManagement.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int BirthYear { get; set; }
        public string Country { get; set; }

        // Навигационное свойство
        public ICollection<Book> Books { get; set; }
        
        // имя автора
        public string FullName => $"{FirstName} {LastName}";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LibMiniWPF
{
    public partial class MainWindow : Window
    {
        public class Book
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public string Genre { get; set; }
            public int Rating { get; set; }
            public DateTime AddedDate { get; set; }
            public bool IsRead { get; set; }

            public string RatingStars => new string('★', Rating) + new string('☆', 5 - Rating);
        }

        private List<Book> allBooks = new List<Book>();
        private List<Book> filteredBooks = new List<Book>();

        public MainWindow()
        {
            InitializeComponent();

            cmbGenre.Items.Add("Фантастика");
            cmbGenre.Items.Add("Детектив");
            cmbGenre.Items.Add("Роман");
            cmbGenre.Items.Add("Научная литература");
            cmbGenre.Items.Add("Фэнтези");

            UpdateStatistics();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            int bookcount = Convert.ToInt32(txtTotalBooks.Text);
            int bookreaded = Convert.ToInt32(txtReadBooks.Text);
            

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название книги");
                return;
            }

            int rating = 5;
            if (star1.IsChecked == true) rating = 1;
            else if (star2.IsChecked == true) rating = 2;
            else if (star3.IsChecked == true) rating = 3;
            else if (star4.IsChecked == true) rating = 4;

            var book = new Book
            {
                Title = txtTitle.Text,
                Author = txtAuthor.Text,
                Genre = cmbGenre.Text,
                Rating = rating,
                AddedDate = DateTime.Now,
                IsRead = false
            };

            allBooks.Add(book);
            ApplyFilters();
            var button = sender as Button;

            if (button == btnSortByDate)
            {
                filteredBooks = filteredBooks.OrderBy(b => b.AddedDate).ToList();
            }
            else if (button == btnSortByAuthor)
            {
                filteredBooks = filteredBooks.OrderBy(b => b.Author).ThenBy(b => b.Title).ToList();
            }
            else if (button == btnSortByRating)
            {
                filteredBooks = filteredBooks.OrderByDescending(b => b.Rating).ThenBy(b => b.Title).ToList();
            }

            booksGrid.ItemsSource = filteredBooks;



            txtTitle.Text = "";
            txtAuthor.Text = "";
            cmbGenre.Text = "";
            star5.IsChecked = true;
            UpdateStatistics();
            bookcount++;
            txtTotalBooks.Text = bookcount.ToString();
        }

        private void ApplyFilters()
        {
            filteredBooks = allBooks.ToList();


            UpdateStatistics();
        }

        private void UpdateStatistics()
        {
            //txtTotalBooks.Text = allBooks.Count.ToString();
            //txtReadBooks.Text = allBooks.Count(b => b.IsRead).ToString();
            //txtAvgRating.Text = allBooks.Any() ? allBooks.Average(b => b.Rating).ToString("0.00") : "0";
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortBooks(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == btnSortByDate)
            {
                filteredBooks = filteredBooks.OrderBy(b => b.AddedDate).ToList();
            }
            else if (button == btnSortByAuthor)
            {
                filteredBooks = filteredBooks.OrderBy(b => b.Author).ThenBy(b => b.Title).ToList();
            }
            else if (button == btnSortByRating)
            {
                filteredBooks = filteredBooks.OrderByDescending(b => b.Rating).ThenBy(b => b.Title).ToList();
            }

            booksGrid.ItemsSource = filteredBooks;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            var book = checkBox.DataContext as Book;
            book.IsRead = checkBox.IsChecked == true;
            int bookreaded = Convert.ToInt32(txtReadBooks.Text);

            if (checkBox.IsChecked == false)
            {
                bookreaded--;
                txtReadBooks.Text = bookreaded.ToString();
            }
            else
            {
                bookreaded++;
                txtReadBooks.Text = bookreaded.ToString();
            }
            UpdateStatistics();

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var book = button.DataContext as Book;
            int bookcount = Convert.ToInt32(txtTotalBooks.Text);

            if (MessageBox.Show($"Удалить книгу '{book.Title}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                allBooks.Remove(book);
                ApplyFilters();
            }

            if (button == btnSortByDate)
            {
                filteredBooks = filteredBooks.OrderBy(b => b.AddedDate).ToList();
            }
            else if (button == btnSortByAuthor)
            {
                filteredBooks = filteredBooks.OrderBy(b => b.Author).ThenBy(b => b.Title).ToList();
            }
            else if (button == btnSortByRating)
            {
                filteredBooks = filteredBooks.OrderByDescending(b => b.Rating).ThenBy(b => b.Title).ToList();
            }

            booksGrid.ItemsSource = filteredBooks;
            bookcount--;
            txtTotalBooks.Text = bookcount.ToString();
        }

        private void booksGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (booksGrid.SelectedItem is Book selectedBook)
            {
                var editWindow = new Window
                {
                    Title = "Редактирование книги",
                    Width = 300,
                    Height = 250,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = this
                };

                var grid = new Grid();
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                var titleLabel = new Label { Content = "Название:" };
                var titleBox = new TextBox { Text = selectedBook.Title, Margin = new Thickness(0, 0, 0, 5) };
                Grid.SetRow(titleLabel, 0);
                Grid.SetRow(titleBox, 1);
                grid.Children.Add(titleLabel);
                grid.Children.Add(titleBox);

                var genreLabel = new Label { Content = "Жанр:" };
                var genreBox = new ComboBox { Text = selectedBook.Genre, IsEditable = true, Margin = new Thickness(0, 0, 0, 5) };
                foreach (var genre in cmbGenre.Items) genreBox.Items.Add(genre);
                Grid.SetRow(genreLabel, 2);
                Grid.SetRow(genreBox, 3);
                grid.Children.Add(genreLabel);
                grid.Children.Add(genreBox);

                var ratingLabel = new Label { Content = "Рейтинг:" };
                var ratingPanel = new StackPanel { Orientation = Orientation.Horizontal };
                for (int i = 1; i <= 5; i++)
                {
                    var radio = new RadioButton { Content = "★", FontSize = 16, Tag = i, IsChecked = i == selectedBook.Rating };
                    ratingPanel.Children.Add(radio);
                }
                Grid.SetRow(ratingLabel, 4);
                Grid.SetRow(ratingPanel, 5);
                grid.Children.Add(ratingLabel);
                grid.Children.Add(ratingPanel);

                var saveButton = new Button { Content = "Сохранить", Margin = new Thickness(0, 10, 0, 0), Width = 80 };
                Grid.SetRow(saveButton, 6);
                grid.Children.Add(saveButton);

                saveButton.Click += (s, args) =>
                {
                    selectedBook.Title = titleBox.Text;
                    selectedBook.Genre = genreBox.Text;
                    selectedBook.Rating = ratingPanel.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked == true)?.Tag as int? ?? 5;
                    ApplyFilters();
                    editWindow.Close();
                };

                editWindow.Content = grid;
                editWindow.ShowDialog();
            }
        }
    }
}
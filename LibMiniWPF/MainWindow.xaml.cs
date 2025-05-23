using System;
using System.Collections.Generic;
using System.Linq;
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

            // Инициализация жанров
            cmbGenre.Items.Add("Фантастика");
            cmbGenre.Items.Add("Детектив");
            cmbGenre.Items.Add("Роман");
            cmbGenre.Items.Add("Научная литература");
            cmbGenre.Items.Add("Фэнтези");

            // Инициализация фильтра по жанрам
            cmbGenreFilter.Items.Add("Все жанры");
            foreach (var genre in cmbGenre.Items)
            {
                cmbGenreFilter.Items.Add(genre);
            }
            cmbGenreFilter.SelectedIndex = 0;

            // Инициализация DataGrid
            booksGrid.ItemsSource = filteredBooks;
            UpdateStatistics();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
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


            if (!cmbGenreFilter.Items.Contains(book.Genre) && !string.IsNullOrEmpty(book.Genre))
            {
                cmbGenreFilter.Items.Add(book.Genre);
            }

            txtTitle.Text = "";
            txtAuthor.Text = "";
            cmbGenre.Text = "";
            star5.IsChecked = true;
        }

        private void ApplyFilters()
{
    try
    {
        if (allBooks == null) return;

        filteredBooks = allBooks.ToList();

        // Применение поиска
        if (!string.IsNullOrWhiteSpace(txtSearch?.Text))
        {
            string searchTerm = txtSearch.Text.ToLower();
            filteredBooks = filteredBooks.Where(b =>
                (b.Title?.ToLower().Contains(searchTerm)) == true ||
                (b.Author?.ToLower().Contains(searchTerm)) == true ||
                (b.Genre?.ToLower().Contains(searchTerm) == true)).ToList();
        }

        // Фильтрация по жанру
        if (cmbGenreFilter?.SelectedItem != null && cmbGenreFilter.SelectedItem.ToString() != "Все жанры")
        {
            string selectedGenre = cmbGenreFilter.SelectedItem.ToString();
            filteredBooks = filteredBooks.Where(b => b.Genre == selectedGenre).ToList();
        }

        // Фильтрация по статусу (с проверкой на null)
        if (cmbStatusFilter?.SelectedItem != null)
        {
            var selectedItem = cmbStatusFilter.SelectedItem as ComboBoxItem;
            if (selectedItem?.Content != null)
            {
                string selectedStatus = selectedItem.Content.ToString();
                switch (selectedStatus)
                {
                    case "Прочитано":
                        filteredBooks = filteredBooks.Where(b => b.IsRead).ToList();
                        break;
                    case "Не прочитано":
                        filteredBooks = filteredBooks.Where(b => !b.IsRead).ToList();
                        break;
                }
            }
        }

        // Фильтрация по рейтингу
        filteredBooks = filteredBooks.Where(b => b.Rating >= (sliderRatingFilter?.Value ?? 1)).ToList();

        booksGrid.ItemsSource = filteredBooks;
        UpdateStatistics();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Произошла ошибка при фильтрации: {ex.Message}");
    }
}

        private void UpdateStatistics()
        {
            txtTotalBooks.Text = filteredBooks.Count.ToString();
            txtReadBooks.Text = filteredBooks.Count(b => b.IsRead).ToString();
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
            UpdateStatistics();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var book = button.DataContext as Book;

            if (MessageBox.Show($"Удалить книгу '{book.Title}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                allBooks.Remove(book);
                ApplyFilters();
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void FilterChanged(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
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

        private void OguzokPic_KeyDown(object sender, KeyEventArgs e)
        {
            OguzokPic.Visibility = Visibility.Collapsed;
        }

        private void PrikolBtn_Click(object sender, RoutedEventArgs e)
        {
            OguzokPic.Visibility = Visibility.Visible;
        }

        private void PrikolBtn_KeyUp(object sender, KeyEventArgs e)
        {
            OguzokPic.Visibility = Visibility.Visible;
        }
    }
}
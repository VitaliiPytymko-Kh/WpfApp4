using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly World1Context db;
        DataTable dt;
        public MainWindow()
        {
            InitializeComponent();
            db = new World1Context();
        }

        private void ShowCountry_Click(object sender, RoutedEventArgs e)
        {
          try
          {
              string countryName = Country.Text.Trim();
              string continentName = Continent.Text.Trim();
              string countryCode = CountryCode.Text.Trim();

              var countries = db.Countries
                  .Where(c =>
                      (string.IsNullOrEmpty(countryName) || c.Name.ToLower().Contains(countryName.ToLower())) &&
                      (string.IsNullOrEmpty(continentName) || c.Continent.ToLower().Contains(continentName.ToLower())) &&
                      (string.IsNullOrEmpty(countryCode) || c.Code.ToLower() == countryCode.ToLower()))
                  .Select(c => new
                  {
                      c.Code,
                      c.Name,
                      c.Continent,
                      c.SurfaceArea,
                      c.Population,
                      CapitalName = c.Capital != null ? db.Cities.FirstOrDefault(city => city.Id == c.Capital).Name : "",
                  })
                  .ToList();

              if (countries.Count == 0)
              {
                  MessageBox.Show("Not found");
              }
              else
              {
                  DGrid1.ItemsSource = countries;
              }
          }
          catch (Exception ex)
          {
              MessageBox.Show(ex.Message);
          }
        }

        private void ShowCapital_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IQueryable<Country> query = db.Countries;
                if (!string.IsNullOrEmpty(Country.Text))
                {
                    string[] searchTerms = Country.Text.Split(',').Select(term => term.Trim()).ToArray();

                    query = query.Where(c => c.Capital.HasValue && searchTerms.Any(term => db.Cities.Any(city => city.Id == c.Capital && city.Name.ToLower().Contains(term.ToLower()))));
                }

                var capitals = query.Select(c => new
                {
                    Country = c.Name,
                    Capital = c.Capital.HasValue
                            ? db.Cities.FirstOrDefault(city => city.Id == c.Capital).Name
                            : ""
                })
                    .ToList();

                if (capitals.Count == 0)
                {
                    MessageBox.Show("Not found");
                }
                else
                {
                    DGrid1.ItemsSource = capitals;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EuropeanCountries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IQueryable<Country> query = db.Countries.Where(c => c.Continent == "Europe");

                if (!string.IsNullOrEmpty(Country.Text))
                {
                    string[] searchTerms = Country.Text.Split(',').Select(term => term.Trim()).ToArray();

                    query = query.Where(c => searchTerms.Any(term => c.Name.ToLower().Contains(term.ToLower())));
                }

                var europeanCountries = query.Select(c => new { CountryName = c.Name, ContinentName = c.Continent }).ToList();

                if (europeanCountries.Count == 0)
                {
                    MessageBox.Show("Not found");
                }
                else
                { DGrid1.ItemsSource = europeanCountries; }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CountrySurfaceArea_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal? fromValue = null;
                decimal? toValue = null;

                if (!string.IsNullOrEmpty(From.Text))
                {
                    if (!decimal.TryParse(From.Text, out decimal tempFromValue))
                    {
                        MessageBox.Show("Please enter a valid numeric value in the 'From' field.");
                        return;
                    }
                    fromValue = tempFromValue;
                }
                if (!string.IsNullOrEmpty(To.Text))
                {
                    if (!decimal.TryParse(To.Text, out decimal tempToValue))
                    {
                        MessageBox.Show("Please enter a valid numeric value in the 'From' field.");
                        return;
                    }
                    toValue = tempToValue;
                }
                var countriesInAreaRange = db.Countries
                        .Where(c => (!fromValue.HasValue || c.SurfaceArea >= fromValue) &&
                        (!toValue.HasValue || c.SurfaceArea <= toValue))
                        .Select(c => new
                        {
                            CountryName = c.Name,
                            SurfaceArea = c.SurfaceArea
                        })
                        .ToList();
                if (countriesInAreaRange.Any())
                {
                    DGrid1.ItemsSource = countriesInAreaRange;
                }
                else { MessageBox.Show("Not found"); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CountryPopulation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal? fromValue = null;
                decimal? toValue = null;

                if (!string.IsNullOrEmpty(From.Text))
                {
                    if (!decimal.TryParse(From.Text, out decimal tempFromValue))
                    {
                        MessageBox.Show("Please enter a valid numeric value in the 'From' field.");
                        return;
                    }
                    fromValue = tempFromValue;
                }
                if (!string.IsNullOrEmpty(To.Text))
                {
                    if (!decimal.TryParse(To.Text, out decimal tempToValue))
                    {
                        MessageBox.Show("Please enter a valid numeric value in the 'From' field.");
                        return;
                    }
                    toValue = tempToValue;
                }
                var countriesPopulation = db.Countries
                        .Where(c => (!fromValue.HasValue || c.Population >= fromValue) &&
                        (!toValue.HasValue || c.Population <= toValue))
                        .Select(c => new
                        {
                            CountryName = c.Name,
                            Population = c.Population
                        })
                        .ToList();
                if (countriesPopulation.Any())
                {
                    DGrid1.ItemsSource = countriesPopulation;
                }
                else
                {
                    MessageBox.Show("Not found");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Top5Population_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var top5Population = db.Countries
                    .OrderByDescending(c => c.Population)
                    .Take(5)
                      .Select(c => new
                      {
                          CountryName = c.Name,
                          Population = c.Population
                      })
                    .ToList();

                if (top5Population.Any())
                {
                    DGrid1.ItemsSource = top5Population;
                }
                else { MessageBox.Show("Not found"); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MaxPopulation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var countryWithLargestPopulation = db.Countries
                    .OrderByDescending(c => c.Population)
                    .Select(c => new { CountryName = c.Name, Population = c.Population })
                    .FirstOrDefault();
                if (countryWithLargestPopulation != null)
                {
                    DGrid1.ItemsSource = new List<dynamic> { countryWithLargestPopulation };
                }
                else { MessageBox.Show("Not found"); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CountryminArea_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string continent = Continent.Text.Trim();
                var countryWithMInArea = new Country();

                if (string.IsNullOrEmpty(continent))
                {
                    countryWithMInArea = db.Countries.OrderBy(c => c.SurfaceArea).FirstOrDefault();
                }
                else
                {
                    countryWithMInArea = db.Countries
                        .Where(c => c.Continent == continent)
                        .OrderBy(c => c.SurfaceArea)
                        .FirstOrDefault();
                }
                if (countryWithMInArea != null)
                {
                    DGrid1.ItemsSource = new List<Country> { countryWithMInArea };
                }
                else
                {
                    MessageBox.Show("No country found.");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CountryCount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int totalCountries = db.Countries.Count();
                var continentMaxCountries = db.Countries
                    .GroupBy(c => c.Continent)
                    .Select(group => new
                    {
                        Continent = group.Key,
                        CountryCount = group.Count()
                    })
                    .OrderByDescending(group => group.CountryCount)
                    .FirstOrDefault();
                var countriesPerContinent = db.Countries
                    .GroupBy(c => c.Continent)
                    .Select(group => new
                    {
                        Continent = group.Key,
                        CountryCount = group.Count()
                    })
                    .ToList();
                var results = new List<object>();
                results.Add(new { Description = "Total Countries", Value = totalCountries });
                results.Add(new { Description = "Continent with Most Countries", Value = continentMaxCountries?.Continent });
                results.AddRange(countriesPerContinent.Select(item => new { Description = $"{item.Continent} Countries", Value = item.CountryCount }));

                DGrid1.ItemsSource = results;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PopulationContinents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var populationContinent = db.Countries
                    .GroupBy(c => c.Continent)
                    .Select(group => new
                    {
                        Continent = group.Key,
                        TotalPopulation = group.Sum(c => (long)c.Population)
                    })
                    .ToList();
                DGrid1.ItemsSource = populationContinent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowCity_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string cityName = City.Text.Trim();
                string countryCode = CountryCode.Text.Trim();

                var cities = db.Cities
                    .Where(c=> string.IsNullOrEmpty(cityName)||c.Name.ToLower().Contains(cityName.ToLower()))
                    .Where(c=>string.IsNullOrEmpty(countryCode)||c.CountryCode.ToLower().Contains(countryCode.ToLower()))
                    .Select(c => new
                { c.Id, c.Name, c.Population, Country = c.CountryCodeNavigation.Name }).ToList();

                if (cities.Count == 0)
                {
                    MessageBox.Show("No cities found");
                }
                else
                {
                    DGrid1.ItemsSource = cities;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void AddCountry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string countryCode = CountryCode.Text.Trim();
                string countryName = Country.Text.Trim();
                string continent = Continent.Text.Trim();
                decimal surfaceArea;
                int population;

                if (!decimal.TryParse(SurfaceArea.Text.Trim(), out surfaceArea) || (!int.TryParse(Population.Text.Trim(), out population)))
                {
                    MessageBox.Show("Invalid input for Surface Area or Population.");
                    return;
                }

                Country newCountry = new Country()
                { 
                    Code = countryCode,
                    Name = countryName,
                    Continent= continent,
                    SurfaceArea = surfaceArea,
                    Population = population
                };
                db.Countries.Add(newCountry);
                db.SaveChanges();
                MessageBox.Show("Country added soccessfully. ");

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void DeleteCountry_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                string searchTerm = Country.Text;
                string searchTerm2 = CountryCode.Text;
                var countryToDelete = db.Countries.FirstOrDefault(c => c.Code == searchTerm2 || c.Name == searchTerm);
                if(countryToDelete != null)
                {
                    db.Countries.Remove(countryToDelete);
                    db.SaveChanges();
                    MessageBox.Show("Country deleted soccessfully.");
                }
                else { MessageBox.Show("Country not found."); }
            
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void AddCity_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                string cityName = City.Text.Trim();
                string countryCode = CountryCode.Text.Trim();
                int population;
                if(!int.TryParse(Population.Text.Trim(),out population)) 
                { MessageBox.Show("Invalid input for Population"); return; }

                City newCity = new City()
                {
                    Name = cityName,
                    CountryCode = countryCode,
                    Population = population
                };
                db.Add(newCity);
                db.SaveChanges();
                MessageBox.Show("City added soccessfully.");
            }
            catch (Exception ex){ MessageBox.Show(ex.Message); }
        }

        private void DeleteCity_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                string cityName = City.Text.Trim();
                if (string.IsNullOrEmpty(cityName))
                {
                    MessageBox.Show("Please enter the name of the city to delete.");
                    return;
                }

                var cityToDelete = db.Cities.FirstOrDefault(c => c.Name.ToLower() == cityName.ToLower());
                if (cityToDelete == null)
                {
                    MessageBox.Show($"City '{cityName}' not found.");
                    return;
                }

                db.Cities.Remove(cityToDelete);
                db.SaveChanges();
               

                MessageBox.Show($"City '{cityName}' has been successfully deleted.");

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Country.Text = "";
            City.Text = "";
            CountryCode.Text = "";
            Population.Text = "";
            SurfaceArea.Text = "";
            Continent.Text = "";
            From.Text = "";
            To.Text = "";
            Inc.Text = "";
        }

    }
}
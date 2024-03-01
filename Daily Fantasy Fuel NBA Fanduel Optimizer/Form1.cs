using System.Net.Http;
using HtmlAgilityPack; // This specifies that you're using HtmlAgilityPack
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks; // Make sure you have this for async Task
using System; // For the Exception class
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;

namespace Daily_Fantasy_Fuel_NBA_Fanduel_Optimizer
{
    public partial class Form1 : Form
    {

        private List<string> excludedPlayerNames = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
           // If you want to load the data when the form loads, call the method here.
             await DisplayDataInGridView();
         
        }



public async Task<DataTable> GetTableFromWebAsync(string url)
    {
        // Initialize the HttpClient
        using (HttpClient http = new HttpClient())
        {
            // Make sure the URL is absolute
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException("The URL must be an absolute URI.", nameof(url));
            }

            // Get the html content from the website
            var response = await http.GetAsync(url);
            var pageContents = await response.Content.ReadAsStringAsync();

            // Load the HTML into the HtmlDocument (HtmlAgilityPack.HtmlDocument)
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(pageContents);

            // Initialize the DataTable
            DataTable dt = new DataTable("NBA");

            // Define column names and their respective data types
            var columnDefinitions = new Dictionary<string, Type>
        {
            {"POS", typeof(string)}, // varchar
            {"NAME", typeof(string)}, // varchar
            {"SALARY", typeof(decimal)}, // money
            {"REST", typeof(int)}, // int
            {"START", typeof(string)}, // varchar
            {"TEAM", typeof(string)}, // varchar
            {"OPP", typeof(string)}, // varchar
            {"DvP", typeof(int)}, // int
            {"FD FP PROJECTED", typeof(decimal)}, // int
            {"VALUE PROJECTED", typeof(decimal)}, // int
            {"FP MIN", typeof(decimal)}, // int
            {"FP AVG", typeof(decimal)}, // int
            {"FP MAX", typeof(decimal)}, // int
            {"O/U", typeof(decimal)}, // int
            {"TM PTS", typeof(decimal)} // int
        };

                // Add columns with specific data types
                foreach (var columnDefinition in columnDefinitions)
                {
                    dt.Columns.Add(columnDefinition.Key, columnDefinition.Value);
                }

                // Locate the table you want by an identifier if possible, otherwise assume first table
                var table = doc.DocumentNode.SelectSingleNode("//table");

            // Check if the table is not null
            if (table != null)
            {
            

                    // Process rows, skip the first row
                    var rows = table.SelectNodes("tbody/tr").Skip(1);
                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes("td").Skip(1).Select(td => td.InnerText.Trim()).ToList();
                        var rowValues = new object[cells.Count];

                        for (int i = 0; i < cells.Count; i++)
                        {
                            var columnType = dt.Columns[i].DataType;
                            var cellValue = cells[i];

                            // Convert cell value to appropriate type
                            if (columnType == typeof(int))
                            {
                                rowValues[i] = ParseToInt(cellValue);
                            }
                            else if (columnType == typeof(decimal))
                            {
                                rowValues[i] = ParseToDecimal(cellValue);
                            }
                            else
                            {
                                rowValues[i] = cellValue; // Use the string value for non-numeric types
                            }
                        }

                        dt.Rows.Add(rowValues); // Add row data to DataTable
                    }
                }

                return dt;
            }
        }

        private int ParseToInt(string value)
        {
            // Remove non-numeric characters, convert shorthand notation as needed
            value = value.Replace("$", "").Replace("k", "000");
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return 0; // Default to 0 if parsing fails
        }

        private decimal ParseToDecimal(string value)
        {
            // Handle decimal values correctly
            value = value.Replace("$", "").Replace("k", "00").Replace(",", "");

            // Attempt to parse the value directly as a decimal
            if (decimal.TryParse(value, out decimal result))
            {
                return result; // Return parsed value directly
            }

            return 0M; // Default to 0 if parsing fails
        }


        private async Task DisplayDataInGridView(string url = "https://www.dailyfantasyfuel.com/nba/projections/fanduel")
        {
            try
            {
                DataTable table = await GetTableFromWebAsync(url);
                this.Invoke(new Action(() =>
                {
                    dataGridView1.DataSource = table;
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        public class Player
        {
            public List<string> Positions { get; set; } // List to hold multiple positions
            public string POS { get; set; }
            public string Name { get; set; }
            public double Salary { get; set; }
            public int Rest { get; set; }
            public string Start { get; set; }
            public string Team { get; set; }
            public string Opp { get; set; }
            public int DvP { get; set; }
            public double FDProjectedPoints { get; set; }
            public double ValueProjected { get; set; }
            public double FPMin { get; set; }
            public double FPAvg { get; set; }
            public double FPMax { get; set; }
            public double OverUnder { get; set; }
            public double TeamPoints { get; set; }
        }



        // Assuming this is somewhere in your form code
        private void DisplayFinalTeam(List<Player> finalTeam)
        {
            listViewFinalTeam.Items.Clear();
            listViewFinalTeam.Columns.Clear();
            listViewFinalTeam.Columns.Add("Position", -2, HorizontalAlignment.Left);
            listViewFinalTeam.Columns.Add("Name", -2, HorizontalAlignment.Left);
            listViewFinalTeam.Columns.Add("Salary", -2, HorizontalAlignment.Right);
            listViewFinalTeam.Columns.Add("Fantasy Points", -2, HorizontalAlignment.Right);

            double totalFantasyPoints = 0;
            double totalSalary = 0;

            foreach (var player in finalTeam)
            {
                ListViewItem item = new ListViewItem(string.Join("/", player.Positions));
                item.SubItems.Add(player.Name);

                // Assuming salary is stored as '9.9' for $9,900, multiply by 1,000 for correct conversion
                double actualSalary = player.Salary * 1000;
                item.SubItems.Add($"{actualSalary:C0}"); // Format as currency

                item.SubItems.Add(player.FDProjectedPoints.ToString("N2"));

                listViewFinalTeam.Items.Add(item);

                totalFantasyPoints += player.FDProjectedPoints;
                totalSalary += actualSalary; // Add the correctly converted salary
            }

            ListViewItem totalsItem = new ListViewItem("Total");
            totalsItem.SubItems.Add("");
            totalsItem.SubItems.Add($"{totalSalary:C0}");
            totalsItem.SubItems.Add(totalFantasyPoints.ToString("N2"));

            listViewFinalTeam.Items.Add(totalsItem);

            ResizeListViewColumns();
            listViewFinalTeam.Refresh();
        }

        private void ResizeListViewColumns()
        {
            listViewFinalTeam.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listViewFinalTeam.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            // The Name column should fill the remaining space
            int totalWidthOfOtherColumns = 0;
            for (int i = 0; i < listViewFinalTeam.Columns.Count; i++)
            {
                // Exclude the Name column index, if it's not 1, adjust the index here
                if (i != 1)
                {
                    totalWidthOfOtherColumns += listViewFinalTeam.Columns[i].Width;
                }
            }
            // Set the Name column to fill the rest of the ListView width
            int nameColumnIndex = 1; // Adjust if the Name column index is different
            listViewFinalTeam.Columns[nameColumnIndex].Width = listViewFinalTeam.Width - totalWidthOfOtherColumns - SystemInformation.VerticalScrollBarWidth;
        }

        private void AddPlayerToExclusionList(string playerName)
        {
            if (!excludedPlayerNames.Contains(playerName))
            {
                excludedPlayerNames.Add(playerName);
            }
        }


        // Parse salary string and convert to double
        private double ParseSalary(string salaryStr)
        {
            // Remove dollar sign and trim whitespace
            salaryStr = salaryStr.Replace("$", "").Trim();

            // Check if the salary ends with 'k'
            if (salaryStr.EndsWith("k", StringComparison.OrdinalIgnoreCase))
            {
                // Remove 'k' and parse the number
                if (double.TryParse(salaryStr.Substring(0, salaryStr.Length - 1), out double number))
                {
                    return number * 1000; // Multiply by 1000 if 'k' is present
                }
            }
            else
            {
                // If there's no 'k', just parse the number
                if (double.TryParse(salaryStr, out double number))
                {
                    return number;
                }
            }

            return 0; // Return 0 if parsing fails
        }

        private async Task<List<Player>> GetAllPlayerDataAsync()
        {
            DataTable dataTable = await GetTableFromWebAsync(textBox1.Text);
            List<Player> players = new List<Player>();

            foreach (DataRow row in dataTable.Rows)
            {
                string playerName = row["NAME"].ToString();

                // Check if the player is in the excluded list
                if (!excludedPlayerNames.Contains(playerName))
                {
                    Player player = new Player
                    {
                        Positions = row["POS"].ToString().Split('/').ToList(),
                        Name = playerName,
                        Salary = ParseSalary(row["SALARY"].ToString()),
                        Rest = int.TryParse(row["REST"].ToString(), out int rest) ? rest : 0,
                        Start = row["START"].ToString(),
                        Team = row["TEAM"].ToString(),
                        Opp = row["OPP"].ToString(),
                        DvP = int.TryParse(row["DvP"].ToString(), out int dvp) ? dvp : 0,
                        FDProjectedPoints = double.TryParse(row["FD FP PROJECTED"].ToString(), out double fdPts) ? fdPts : 0,
                        ValueProjected = double.TryParse(row["VALUE PROJECTED"].ToString(), out double valueProjected) ? valueProjected : 0,
                        FPMin = double.TryParse(row["FP MIN"].ToString(), out double fpMin) ? fpMin : 0,
                        FPAvg = double.TryParse(row["FP AVG"].ToString(), out double fpAvg) ? fpAvg : 0,
                        FPMax = double.TryParse(row["FP MAX"].ToString(), out double fpMax) ? fpMax : 0,
                        OverUnder = double.TryParse(row["O/U"].ToString(), out double overUnder) ? overUnder : 0,
                        TeamPoints = double.TryParse(row["TM PTS"].ToString(), out double teamPoints) ? teamPoints : 0
                    };

                    players.Add(player);
                  //  Debug.WriteLine($"Parsed Salary for {player.Name}: {player.Salary}");
                }
            }

            return players;
        }


        private async void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Read the player name from the TextBox and add to the exclusion list
                string playerNameToExclude = textBox2.Text.Trim();
                if (!string.IsNullOrEmpty(playerNameToExclude))
                {
                    excludedPlayerNames.Add(playerNameToExclude);
                }

                // Define your position requirements
                Dictionary<string, int> positionRequirements = new Dictionary<string, int>
        {
            {"PG", 2},
            {"SG", 2},
            {"PF", 2},
            {"SF", 2},
            {"C", 1}
        };

                // Retrieve all player data
                List<Player> allPlayers = await GetAllPlayerDataAsync();

                // Exclude players if needed
                allPlayers = allPlayers.Where(player => !excludedPlayerNames.Contains(player.Name)).ToList();

                // Optimize the team
                List<Player> optimalTeam = OptimizeTeam(allPlayers, 60000, positionRequirements);

                if (optimalTeam == null || !optimalTeam.Any())
                {
                    MessageBox.Show("No optimal team could be formed.");
                    return;
                }

                // Display the final team
                DisplayFinalTeam(optimalTeam);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public const int TotalPlayersToSelect = 9;

        public List<Player> OptimizeTeam(List<Player> allPlayers, double salaryCap, Dictionary<string, int> positionRequirements)
        {

            // Sort players by their point/salary ratio
            allPlayers = allPlayers.OrderByDescending(p => p.FDProjectedPoints / p.Salary).ToList();

            // Initialize the team structure based on position requirements
            var team = new Dictionary<string, List<Player>>();
            foreach (var pos in positionRequirements.Keys)
            {
                team[pos] = new List<Player>();
            }

            // Attempt to fill the team with initial picks
            foreach (var player in allPlayers)
            {
                foreach (var pos in player.Positions)
                {
                    if (team.ContainsKey(pos) && team[pos].Count < positionRequirements[pos])
                    {
                        team[pos].Add(player);
                        break; // Stop looking for positions to fill for this player
                    }
                }
            }

            // Function to calculate the total salary of the team
            double CalculateTotalSalary(Dictionary<string, List<Player>> team)
            {
                return team.SelectMany(kvp => kvp.Value).Sum(p => p.Salary);
            }

            // Function to calculate the total points of the team
            double CalculateTotalPoints(Dictionary<string, List<Player>> team)
            {
                return team.SelectMany(kvp => kvp.Value).Sum(p => p.FDProjectedPoints);
            }

            // Now let's try to improve the team by looking for potential player swaps
            bool improved;
            do
            {
                improved = false;
                foreach (var pos in positionRequirements.Keys)
                {
                    foreach (var playerToReplace in team[pos])
                    {
                        var potentialReplacements = allPlayers.Except(team.SelectMany(kvp => kvp.Value))
                            .Where(p => p.Positions.Contains(pos) && p.Salary <= playerToReplace.Salary)
                            .OrderByDescending(p => p.FDProjectedPoints);

                        foreach (var replacement in potentialReplacements)
                        {
                            double currentSalary = CalculateTotalSalary(team);
                            double currentPoints = CalculateTotalPoints(team);

                            // Calculate new potential salary and points
                            double newSalary = currentSalary - playerToReplace.Salary + replacement.Salary;
                            double newPoints = currentPoints - playerToReplace.FDProjectedPoints + replacement.FDProjectedPoints;

                            if (newSalary <= salaryCap && newPoints > currentPoints)
                            {
                                // Perform the swap
                                team[pos].Remove(playerToReplace);
                                team[pos].Add(replacement);
                                improved = true;
                                break; // Only do one improvement at a time
                            }
                        }
                        if (improved) break; // Exit early if an improvement was made
                    }
                    if (improved) break; // Exit early if an improvement was made
                }
            } while (improved); // Keep trying to improve until no more improvements can be made

            // Flatten the team dictionary into a list
            List<Player> finalTeam = team.SelectMany(kvp => kvp.Value).Distinct().ToList();

            // Return the final team
            return finalTeam;

        }


        private List<Player> KnapsackForPosition(List<Player> players, double salaryCap, int positionCount)
        {
            // Sort the players by the value they provide divided by their salary
            var sortedPlayers = players.OrderByDescending(p => AdjustedProjectedPoints(p) / p.Salary).ToList();

            List<Player> selectedPlayers = new List<Player>();
            double currentSalary = 0;

            foreach (var player in sortedPlayers)
            {
                // If adding this player doesn't exceed the salary cap and we still need players for the position
                if (currentSalary + player.Salary <= salaryCap && selectedPlayers.Count < positionCount)
                {
                    selectedPlayers.Add(player);
                    currentSalary += player.Salary;
                }
            }

            // If we couldn't find enough players for the position within the salary cap, this needs to be handled
            if (selectedPlayers.Count < positionCount)
            {
                // Handle the situation, maybe by selecting cheaper players or changing the algorithm
            }

            return selectedPlayers;
        }

        // This is a simplified version. You need to define how exactly you want to weigh these metrics.
        public double AdjustedProjectedPoints(Player player)
        {
            // Start with base projected points
            double adjustedPoints = player.FDProjectedPoints;

            // Check each checkbox and apply the condition if checked
            if (dVPcheckBox1.Checked && player.DvP >= 15) // If the player is up against one of the worst defenses for their position
            {
                adjustedPoints *= 1.1; // Increase points by 10% for easy matchups
            }
            else if (dVPcheckBox1.Checked && player.DvP <= 14) // If the player is up against one of the best defenses for their position
            {
                adjustedPoints *= 0.9; // Decrease points by 10% for tough matchups
            }

            if (OverUndercheckBox2.Checked && player.OverUnder > 220) // If it's expected to be a high-scoring game
            {
                adjustedPoints *= 1.05; // Increase points by 5%
            }

            if (totalTeamPointscheckBox3.Checked && player.TeamPoints > 110) // If the player's team is expected to score a lot
            {
                adjustedPoints *= 1.05; // Increase points by 5%
            }

            if (RestcheckBox4.Checked && player.Rest < 1) // If REST is less than 1
            {
                adjustedPoints *= 1.02; // Increase points by 2%
            }

            if (StartcheckBox.Checked && player.Start == "EXP") // If START is "EXP"
            {
                adjustedPoints *= 1.05; // Increase points by 5%
            }

            return adjustedPoints;
        }
        private void AdjustTeamForSalaryCap(ref List<Player> finalTeam, List<Player> allPlayers, double salaryCap)
        {
            double totalSalary = finalTeam.Sum(p => p.Salary);
          //  Debug.WriteLine($"Adjusting Team. Initial Salary: {totalSalary}");
            while (totalSalary > salaryCap)
            {
                var playerToRemove = finalTeam.OrderBy(p => p.FDProjectedPoints).First();
              //  Debug.WriteLine($"Removing Player: {playerToRemove.Name}, Salary: {playerToRemove.Salary}");
                finalTeam.Remove(playerToRemove);
                totalSalary -= playerToRemove.Salary;

                var replacement = FindReplacementPlayer(allPlayers, finalTeam, playerToRemove, totalSalary, salaryCap);
                if (replacement != null)
                {
                //    Debug.WriteLine($"Adding Replacement: {replacement.Name}, Salary: {replacement.Salary}");
                    finalTeam.Add(replacement);
                    totalSalary += replacement.Salary;
                }
            }

            while (salaryCap - totalSalary > 700)
            {
                var playerToReplace = finalTeam.OrderBy(p => p.Salary).First();
                var upgrade = FindUpgradePlayer(allPlayers, finalTeam, playerToReplace, totalSalary, salaryCap);
                if (upgrade != null)
                {
                    Debug.WriteLine($"Upgrading Player: {playerToReplace.Name} with {upgrade.Name}, Salary Change: {upgrade.Salary - playerToReplace.Salary}");
                    finalTeam.Remove(playerToReplace);
                    finalTeam.Add(upgrade);
                    totalSalary = finalTeam.Sum(p => p.Salary);
                }
                else
                {
                    break;
                }
            }
          //  Debug.WriteLine($"Final Adjusted Salary: {totalSalary}");
        }



        private Player FindReplacementPlayer(List<Player> allPlayers, List<Player> currentTeam, Player removedPlayer, double currentSalary, double salaryCap)
        {
            return allPlayers.Except(currentTeam)
                             .Where(p => p.Positions.Contains(removedPlayer.POS) && p.Salary + currentSalary <= salaryCap)
                             .OrderByDescending(p => p.FDProjectedPoints)
                             .FirstOrDefault();
        }


        private Player FindUpgradePlayer(List<Player> allPlayers, List<Player> currentTeam, Player playerToReplace, double currentSalary, double salaryCap)
        {
            return allPlayers.Except(currentTeam)
                             .Where(p => p.Positions.Contains(playerToReplace.POS) && p.Salary > playerToReplace.Salary && p.Salary + currentSalary - playerToReplace.Salary <= salaryCap)
                             .OrderByDescending(p => p.FDProjectedPoints)
                             .FirstOrDefault();
        }



        private async void button2_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(url))
            {
                await DisplayDataInGridView(url);
            }
            else
            {
                MessageBox.Show("Please enter a valid URL.");
            }
        }
    }
}


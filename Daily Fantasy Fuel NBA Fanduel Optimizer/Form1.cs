using System.Net.Http;
using HtmlAgilityPack;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using System;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic.ApplicationServices;
using System.IO; // For File operations
using System.Collections.Generic; // For List<>

namespace Daily_Fantasy_Fuel_NBA_Fanduel_Optimizer
{
    public partial class Form1 : Form
    {
        private List<string> excludedPlayerNames = new List<string>();


        public Form1()
        {
            InitializeComponent();
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        }

        private async void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Update the player data list and write to JSON only if a specific cell was changed.
            // For example, if the cell for excluding players or any other editable cell is changed.
            if (e.ColumnIndex == dataGridView1.Columns["chk"].Index)
            {
                List<Player> players = await GetAllPlayerDataFromGridViewAsync();
                await UpdateJsonFile(players);
            }
        }

        private async Task UpdateJsonFile(List<Player> players)
        {
            string jsonFilePath = @"C:\Users\Administrator\source\repos\Daily Fantasy Fuel NBA Fanduel Optimizer\Daily Fantasy Fuel NBA Fanduel Optimizer\players.json";
            var includedPlayers = players.Where(p => !p.IsExcluded).ToList(); // Filter out excluded players

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            try
            {
                var json = JsonSerializer.Serialize(includedPlayers, options);
                await File.WriteAllTextAsync(jsonFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to write JSON file: {ex.Message}");
            }
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            chk.HeaderText = "Exclude";
            chk.Name = "chk";
            dataGridView1.Columns.Add(chk);
            await DisplayDataInGridView();
        }

        public async Task<string> OptimizeWithDraftfast(List<Player> players)
        {
            await UpdateJsonFile(players);
            // Make sure this path is correct
            string jsonFilePath = @"C:\Users\Administrator\source\repos\Daily Fantasy Fuel NBA Fanduel Optimizer\Daily Fantasy Fuel NBA Fanduel Optimizer\player_pool.json";

            // Ensure the includedPlayers list only contains players that are not excluded
            var includedPlayers = players.Where(p => !p.IsExcluded).ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            try
            {
                var json = JsonSerializer.Serialize(includedPlayers, options);
                await File.WriteAllTextAsync(jsonFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to write JSON file: {ex.Message}");
                return null;
            }


            var startInfo = new ProcessStartInfo()
            {
                FileName = "python",
                Arguments = @"C:\Users\Administrator\source\repos\Daily Fantasy Fuel NBA Fanduel Optimizer\Daily Fantasy Fuel NBA Fanduel Optimizer\draftfast_optimizer.py",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            using (var process = new Process() { StartInfo = startInfo })
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();
                return output;
            }
        }

        public async Task<DataTable> GetTableFromWebAsync(string url)
        {
            using (HttpClient http = new HttpClient())
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    throw new ArgumentException("The URL must be an absolute URI.", nameof(url));
                }
                var response = await http.GetAsync(url);
                var pageContents = await response.Content.ReadAsStringAsync();

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(pageContents);

                DataTable dt = new DataTable("NBA");
                var columnDefinitions = new Dictionary<string, Type>
                {
                    {"POS", typeof(string)},
                    {"NAME", typeof(string)},
                    {"SALARY", typeof(decimal)},
                    {"REST", typeof(int)},
                    {"START", typeof(string)},
                    {"TEAM", typeof(string)},
                    {"OPP", typeof(string)},
                    {"DvP", typeof(int)},
                    {"FD FP PROJECTED", typeof(decimal)},
                    {"VALUE PROJECTED", typeof(decimal)},
                    {"FP MIN", typeof(decimal)},
                    {"FP AVG", typeof(decimal)},
                    {"FP MAX", typeof(decimal)},
                    {"O/U", typeof(decimal)},
                    {"TM PTS", typeof(decimal)}
                };

                foreach (var columnDefinition in columnDefinitions)
                {
                    dt.Columns.Add(columnDefinition.Key, columnDefinition.Value);
                }

                var table = doc.DocumentNode.SelectSingleNode("//table");

                if (table != null)
                {
                    var rows = table.SelectNodes("tbody/tr").Skip(1);
                    foreach (var row in rows)
                    {
                        var cells = row.SelectNodes("td").Skip(1).Select(td => td.InnerText.Trim()).ToList();
                        var rowValues = new object[cells.Count];

                        for (int i = 0; i < cells.Count; i++)
                        {
                            var columnType = dt.Columns[i].DataType;
                            var cellValue = cells[i];
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
                                rowValues[i] = cellValue;
                            }
                        }
                        dt.Rows.Add(rowValues);
                    }
                }
                return dt;
            }
        }

        private int ParseToInt(string value)
        {
            value = value.Replace("$", "").Replace("k", "000");
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            return 0;
        }

        private decimal ParseToDecimal(string value)
        {
            value = value.Replace("$", "").Replace("k", "00").Replace(",", "");
            if (decimal.TryParse(value, out decimal result))
            {
                return result;
            }
            return 0M;
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

        private async Task<List<Player>> GetAllPlayerDataFromGridViewAsync()
        {
            List<Player> players = new List<Player>();
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke(new MethodInvoker(delegate
                {
                    players = FetchPlayers();
                }));
            }
            else
            {
                players = FetchPlayers();
            }
            return players;
        }

        private List<Player> FetchPlayers()
        {
            // Ensure you're filtering out the excluded players here before they are written to the JSON file
            return dataGridView1.Rows
       .Cast<DataGridViewRow>()
       .Where(row => !row.IsNewRow && Convert.ToBoolean(row.Cells["chk"].Value) == false) // Ensures only players without a check are included
       .Select(row => new Player
       {
                    Positions = new List<string> { row.Cells["POS"].Value.ToString() },
                    Name = row.Cells["NAME"].Value.ToString(),
                    Salary = ParseToDouble(row.Cells["SALARY"].Value.ToString()) * 1000,
                    Rest = ParseToInt(row.Cells["REST"].Value.ToString()),
                    Start = row.Cells["START"].Value.ToString(),
                    Team = row.Cells["TEAM"].Value.ToString(),
                    Opp = row.Cells["OPP"].Value.ToString(),
                    DvP = ParseToInt(row.Cells["DvP"].Value.ToString()),
                    OverUnder = ParseToInt(row.Cells["O/U"].Value.ToString()),
                    TeamPoints = ParseToInt(row.Cells["TM PTS"].Value.ToString()),
                    FDProjectedPoints = ParseToDouble(row.Cells["FD FP PROJECTED"].Value.ToString()),
                    IsExcluded = Convert.ToBoolean(row.Cells["chk"].Value)
                }).ToList();
        }

        private double ParseToDouble(string value)
        {
            value = value.Replace("$", "").Replace("k", "000").Replace(",", "");
            if (double.TryParse(value, out double result))
            {
                return result;
            }
            return 0.0;
        }


        private void HighlightPositionalBeneficiaries()
        {
            Dictionary<string, List<string>> outPositionsByTeam = new Dictionary<string, List<string>>();

            // Step 1: Identify the positions and teams of "out" players
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string playerName = row.Cells["NAME"].Value.ToString();
                    if (playerName.EndsWith(" O"))
                    {
                        string playerTeam = row.Cells["TEAM"].Value.ToString();
                        string playerPosition = row.Cells["POS"].Value.ToString();

                        if (!outPositionsByTeam.ContainsKey(playerTeam))
                        {
                            outPositionsByTeam[playerTeam] = new List<string>();
                        }

                        outPositionsByTeam[playerTeam].Add(playerPosition);
                    }
                }
            }

            // Step 2: Highlight teammates who share a position with an "out" player from the same team
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    string playerTeam = row.Cells["TEAM"].Value.ToString();
                    string playerPosition = row.Cells["POS"].Value.ToString();

                    if (outPositionsByTeam.TryGetValue(playerTeam, out List<string> outPositions))
                    {
                        foreach (string outPosition in outPositions)
                        {
                            // Check if the current player's position matches any of the out positions for their team
                            if (playerPosition.Equals(outPosition) ||
                                (playerPosition.Contains("/") && outPosition.Contains("/") && playerPosition.Split('/').Intersect(outPosition.Split('/')).Any()))
                            {
                                // Highlight the row or specific cell for matching position and team
                                row.DefaultCellStyle.BackColor = Color.LightGreen;
                                break;
                            }
                        }
                    }
                }
            }
        }



        public class Player
        {
            [JsonPropertyName("positions")]
            public List<string> Positions { get; set; }

            [JsonPropertyName("NAME")]
            public string Name { get; set; }

            [JsonPropertyName("salary")]
            public double Salary { get; set; }

            [JsonPropertyName("rest")]
            public int Rest { get; set; }

            [JsonPropertyName("start")]
            public string Start { get; set; }

            [JsonPropertyName("team")]
            public string Team { get; set; }

            [JsonPropertyName("opp")]
            public string Opp { get; set; }

            [JsonPropertyName("dvp")]
            public int DvP { get; set; }

            [JsonPropertyName("OverUnder")]
            public int OverUnder { get; set; }

            [JsonPropertyName("TeamPoints")]
            public int TeamPoints { get; set; }

            [JsonPropertyName("proj")]
            public double FDProjectedPoints { get; set; }

            [JsonIgnore]
            public bool IsExcluded { get; set; }
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

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                List<Player> allPlayers = await GetAllPlayerDataFromGridViewAsync();
                if (allPlayers == null || !allPlayers.Any())
                {
                    MessageBox.Show("No players were fetched from the grid.");
                    return;
                }
                string optimizationResult = await OptimizeWithDraftfast(allPlayers);

                string pythonExecutable = "python";
                string scriptPath = @"C:\Users\Administrator\source\repos\Daily Fantasy Fuel NBA Fanduel Optimizer\Daily Fantasy Fuel NBA Fanduel Optimizer\draftfast_optimizer.py";
                string scriptArguments = "";

                var psi = new ProcessStartInfo();
                psi.FileName = pythonExecutable;
                psi.Arguments = $"\"{scriptPath}\" {scriptArguments}";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;

                using (var process = Process.Start(psi))
                {
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();
                    process.WaitForExit();
                    richTextBox1.Clear();
                    richTextBox1.Text = output;
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show("Error: " + error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while optimizing lineup: {ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HighlightPositionalBeneficiaries();

            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                var row = dataGridView1.Rows[i];
                if (!row.IsNewRow)
                {
                    string playerName = row.Cells["NAME"].Value.ToString();
                    if (playerName.EndsWith(" O"))
                    {
                        dataGridView1.Rows.RemoveAt(i);
                    }
                }
            }

        }
    }
}

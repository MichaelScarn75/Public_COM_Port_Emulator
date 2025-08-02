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
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Diagnostics;
using Syncfusion.SfSkinManager;
using System.IO;
using Syncfusion.Linq;
using Syncfusion.Data.Extensions;

namespace WeighingScaleEmulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> DataBit_List = new List<string>()
        {
            "4",
            "5",
            "6",
            "7",
            "8"
        };
        public List<string> Parity_List = new List<string>()
        {
            "None",
            "Odd",
            "Even",
            "Mark",
            "Space"
        };
        public List<string> Baudrate_List = new List<string>()
        {
            "75",
            "110",
            "134",
            "150",
            "300",
            "600",
            "1200",
            "1800",
            "2400",
            "4800",
            "7200",
            "9600",
            "14400",
            "19200",
            "38400",
            "57600",
            "115200",
            "128000"
        };

        public List<string> Stopbits_List = new List<string>()
        {
            "1",
            "1.5",
            "2",
        };
        public List<string> Flow_List = new List<string>()
        {
            "Xon / Xoff",
            "Hardware",
            "None",
        };
        public MainWindow()
        {
            InitializeComponent();
            SfSkinManager.SetTheme(this, new Theme("MaterialDark"));
            Task task = Task.Run(() => Load_Config());
        }
        public async void Load_Config()
        {
            try
            {
                bool COM_NUM_FOUND = false;
                bool WEIGHT_DATA_FOUND = false;
                bool DATABITS_FOUND = false;
                bool PARITY_FOUND = false;
                bool BAUDRATE_FOUND = false;
                bool STOPBITS_FOUND = false;
                bool FLOW_CONTROL_FOUND = false;

                string filePath = "./config.txt";

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string data1 = line.Split("=", StringSplitOptions.TrimEntries)[0];
                        string data2 = line.Split("=", StringSplitOptions.TrimEntries)[1];

                        if (data1 == "COM_NUM")
                        {
                            double result_data2;
                            bool success = double.TryParse(data2, out result_data2);
                            if (success)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    TextBox1.Value = Convert.ToInt32(result_data2);
                                }));
                                COM_NUM_FOUND = true;
                            }
                        }
                        if (data1 == "WEIGHT_DATA")
                        {
                            double result_data2;
                            bool success = double.TryParse(data2, out result_data2);
                            if (success)
                            {
                                Dispatcher.Invoke(new Action(() =>
                                {
                                    TextBox2.Value = Convert.ToDouble(result_data2);
                                }));
                                WEIGHT_DATA_FOUND = true;
                            }
                        }
                        if (data1 == "DATABITS" && DataBit_List.IndexOf(data2) != -1)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                DATABIT_COMBOBOX.SelectedIndex = DataBit_List.IndexOf(data2);
                            }));
                            DATABITS_FOUND = true;
                        }
                        if (data1 == "PARITY" && Parity_List.IndexOf(data2) != -1)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                PARITY_COMBOBOX.SelectedIndex = Parity_List.IndexOf(data2);
                            }));
                            PARITY_FOUND = true;
                        }
                        if (data1 == "BAUDRATE" && Baudrate_List.IndexOf(data2) != -1)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                BAUDRATE_COMBOBOX.SelectedIndex = Baudrate_List.IndexOf(data2);
                            }));
                            BAUDRATE_FOUND = true;
                        }
                        if (data1 == "STOPBITS" && Stopbits_List.IndexOf(data2) != -1)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                STOPBITS_COMBOBOX.SelectedIndex = Stopbits_List.IndexOf(data2);
                            }));
                            STOPBITS_FOUND = true;
                        }
                        if (data1 == "FLOW_CONTROL" && Flow_List.IndexOf(data2) != -1)
                        {
                            Dispatcher.Invoke(new Action(() =>
                            {
                                FLOW_COMBOBOX.SelectedIndex = Flow_List.IndexOf(data2);
                            }));
                            FLOW_CONTROL_FOUND = true;
                        }
                    }
                }
                if (COM_NUM_FOUND == false)
                {
                    TextBox1.Value = 0;
                }
                if (WEIGHT_DATA_FOUND == false)
                {
                    TextBox2.Value = 0.00;
                }
                if (DATABITS_FOUND == false)
                {
                    DATABIT_COMBOBOX.SelectedIndex = 0;
                }
                if (PARITY_FOUND == false)
                {
                    PARITY_COMBOBOX.SelectedIndex = 0;
                }
                if (BAUDRATE_FOUND == false)
                {
                    BAUDRATE_COMBOBOX.SelectedIndex = 0;
                }
                if (STOPBITS_FOUND == false)
                {
                    STOPBITS_COMBOBOX.SelectedIndex = 0;
                }
                if (FLOW_CONTROL_FOUND == false)
                {
                    FLOW_COMBOBOX.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nSomething is wrong with your config file or another process is using it.", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Dispatcher.Invoke(new Action(() =>
                {
                    TextBox1.Value = 0;
                    TextBox2.Value = 0.00;
                    DATABIT_COMBOBOX.SelectedIndex = 0;
                    PARITY_COMBOBOX.SelectedIndex = 0;
                    BAUDRATE_COMBOBOX.SelectedIndex = 0;
                    STOPBITS_COMBOBOX.SelectedIndex = 0;
                    FLOW_COMBOBOX.SelectedIndex = 0;
                }));
            }
        }
        public void start()
        {
            try
            {
                SerialPort serialPort = new SerialPort("COM" + TextBox1.Value.ToString(), 9600); // Replace "COM1" with the actual COM port name and set the baud rate

                if(PARITY_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] == "None")
                {
                serialPort.Parity = Parity.None;
                }
                else if(PARITY_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] == "Odd")
                {
                serialPort.Parity = Parity.Odd;
                }
                else if(PARITY_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] == "Even")
                {
                serialPort.Parity = Parity.Even;
                }
                else if(PARITY_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] == "Mark")
                {
                serialPort.Parity = Parity.Mark;
                }
                else if(PARITY_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] == "Space")
                {
                serialPort.Parity = Parity.Space;
                }

                if(STOPBITS_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] == "1")
                {
                serialPort.StopBits = StopBits.One;
                }
                else if(STOPBITS_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] == "1.5")
                {
                serialPort.StopBits = StopBits.OnePointFive;
                }
                else if(STOPBITS_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] == "2")
                {
                serialPort.StopBits = StopBits.Two;
                }

                string write_value = String.Format("{0:0.000}",TextBox2.Value);
                serialPort.BaudRate = Convert.ToInt32(BAUDRATE_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1]);
                serialPort.WriteTimeout = 2000;
                serialPort.Open(); // Open the serial port
                serialPort.WriteLine($"ST,GS,A,     {write_value} kg\r");
                serialPort.Close();
                serialPort.Dispose();
                OutputTextBox.AppendText(DateTime.UtcNow.ToString() + "\t data sent!\n");
                OutputTextBox.ScrollToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nCOM Port Error!", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            /*
            int nombar = 0;
            while (true)
            {
                Console.WriteLine(nombar++.ToString());
                try
                {
                    List<string> DelimitedSerialData = new List<string>();
                    List<string> List1 = new List<string>() { "kg", "KG", "k", "g" };
                    double FinalData;

                    serialPort.Parity = Parity.None;
                    serialPort2.Parity = Parity.None;
                    serialPort.Open(); // Open the serial port
                    serialPort2.Open(); // Open the serial port
                    Console.WriteLine("Serial port is open. Press Enter to exit.");
                    string data = serialPort.ReadLine(); // Read the received data
                    data = data.Replace(" ", "");
                    DelimitedSerialData = data.Split(",").ToList();
                    foreach (string temp1 in List1)
                    {
                        DelimitedSerialData[DelimitedSerialData.Count - 1] = DelimitedSerialData.LastOrDefault().Replace(temp1, "");
                    }
                    FinalData = float.Parse(DelimitedSerialData.LastOrDefault());
                    FinalData = Math.Round(FinalData, 4);
                    Console.WriteLine("Received data: " + data);
                    Console.WriteLine("Shit I want: " + FinalData.ToString("0.000"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    serialPort.Close(); // Close the serial port when done
                }
            }
            */
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                string filePath = "./config.txt";

                string data = "COM_NUM=" + TextBox1.Value.ToString() +
                    "\nWEIGHT_DATA=" + TextBox2.Value.ToString() +
                    "\nDATABITS=" + DATABIT_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] +
                    "\nPARITY=" + PARITY_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] +
                    "\nBAUDRATE=" + BAUDRATE_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] +
                    "\nSTOPBITS=" + STOPBITS_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1] +
                    "\nFLOW_CONTROL=" + FLOW_COMBOBOX.SelectedValue.ToString().Split(":", StringSplitOptions.TrimEntries)[1];
                File.WriteAllText(filePath, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\nSomething is wrong with your config file or another process is using it.\nAll config values will be reset to 0 on next startup.", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                string filePath = "./config.txt";

                string data = "COM_NUM=0" +
                    "\nWEIGHT_DATA=0.00" +
                    "\nDATABITS=8" +
                    "\nPARITY=None" +
                    "\nBAUDRATE=9600" +
                    "\nSTOPBITS=1" +
                    "\nFLOW_CONTROL=None";

                File.WriteAllText(filePath, data);
            }
        }

        private void ButtonAdv_Click(object sender, RoutedEventArgs e)
        {
            start();
        }
    }
}
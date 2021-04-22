using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Collections;

namespace Digital_Measures
{
    public partial class Form1 : Form
    {
        private string sfdPath = "";
        List<string> wrongHeader = new List<string>();
        List<string> correctHeader = new List<string>();
        List<string> badModes = new List<string>();
        List<string> goodModes = new List<string>();


        public Form1()
        {
            InitializeComponent();
        }


        OpenFileDialog ofd = new OpenFileDialog();

        //open file button
        private void button2_Click(object sender, EventArgs e)
        {
            //This filter makes it so you can only open csv files
            ofd.Filter = "CSV|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //show file name and extension
                textBox2.Text = ofd.SafeFileName;
            }
        }


        //select destination folder
        private void button3_Click(object sender, EventArgs e)
        {
            //this filter makes it so it automatically saves as a csv
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV|*.csv";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //show file path
                textBox3.Text = sfd.FileName;
                sfdPath = sfd.FileName;
            }
        }


        //convert file button
        private void button1_Click(object sender, EventArgs e)
        {
            //used background worker so the updating of the progress bar is done more accurately
            backgroundWorker1.RunWorkerAsync();
        }
        

        private void progressBar1_Click(object sender, EventArgs e)
        {
            
        }


        //done button
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            int progress = 0;
            try
            {
                TextFieldParser parser = new TextFieldParser(ofd.FileName);
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");
                string[] line = { };
                List<string> lineList = new List<string>();
                //reads in the csv file and discards commas
                while (!parser.EndOfData)
                {
                    //reads the current lines and stores that in the line array
                    line = parser.ReadFields();
                    //adds each element of the array into an arraylist
                    foreach (string field in line)
                    {
                        if (field.Contains(","))
                        {
                            lineList.Add("\"" + field + "\"");
                        }
                        else
                        {
                            lineList.Add(field);
                        }
                    }

                }
                parser.Close();
                //removing empty elements from arraylist
                for (int i = 0; i < lineList.Count; i++)
                {
                    if (lineList[i] == "")
                    {
                        lineList.RemoveAt(i);
                    }
                }
                //converting arraylist into string array
                string[] s1 = lineList.ToArray();
                progress += 25;
                backgroundWorker1.ReportProgress(progress);
                HeaderConversion(s1);

                //convert to proper headers
                void HeaderConversion(string[] fileLine)
                {
                    foreach (string s in wrongHeader)
                    {
                        for (int i = 0; i < wrongHeader.Count; i++)
                        {
                            if (fileLine[i] == s)
                            {
                                fileLine[i] = correctHeader[i];
                            }
                        }
                    }
                    DeliveryModeConversion(fileLine);
                    progress += 25;
                    backgroundWorker1.ReportProgress(progress);
                }

                //convert to proper delivery modes
                void DeliveryModeConversion(string[] fileLine)
                {
                    int numOfColumns = wrongHeader.Count - 1;
                    for (int i = numOfColumns; i < fileLine.Length; i++)
                    {
                        int x = 0;
                        if (i % numOfColumns == 0)
                        {
                            fileLine[i] = fileLine[i] + "\n";
                        }
                        foreach (string s in badModes)
                        {
                            if (fileLine[i] == s + "\n")
                            {
                                fileLine[i] = goodModes[x] + "\n";
                            }
                            x++;
                        }
                    }
                    progress += 25;
                    backgroundWorker1.ReportProgress(progress);
                    CombineBack(fileLine);
                }

                //combines back to proper csv format
                void CombineBack(string[] fileLine)
                {
                    string endString = "";
                    //joining all array elements to string seperated by comma
                    for (int i = 0; i < fileLine.Length; i++)
                    {
                        endString = String.Join(",", fileLine);
                    }
                    //writing to the csv file
                    using (StreamWriter sw = new StreamWriter(sfdPath))
                    {
                        sw.WriteLine(endString);
                    }
                    progress += 25;
                    backgroundWorker1.ReportProgress(progress);
                }
            }
            catch
            {
                MessageBox.Show("Missing file");
            }

            
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //updating progressbar
            int progress = 0;
            int i = 0;
            progress += e.ProgressPercentage;
            while (progressBar1.Value != 100)
            {
                progressBar1.Value = i;
                i++;
                if (i >= progress)
                {
                    System.Threading.Thread.Sleep(50);
                }
                System.Threading.Thread.Sleep(40);
            }
        }


        //completed message box
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(progressBar1.Value == 100)
            {
                MessageBox.Show("Successfully Converted File");
            }
        }


        //clear button
        private void button5_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            goodModes.Clear();
            badModes.Clear();
            correctHeader.Clear();
            wrongHeader.Clear();
        }
        

        OpenFileDialog ofd2 = new OpenFileDialog();
        //correct headers button
        private void button6_Click(object sender, EventArgs e)
        {
            ofd2.Filter = "CSV|*.csv";
            if (ofd2.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd2.SafeFileName;
                TextFieldParser parser = new TextFieldParser(ofd2.FileName);
                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");
                string[] line = { };
                List<string> lineList = new List<string>();
                //reads in the csv file and discards commas
                while (!parser.EndOfData)
                {
                    //reads the current lines and stores that in the line array
                    line = parser.ReadFields();

                    if (line.Length == 2)
                    {
                        foreach (string field in line)
                        {
                            if (field.Contains(","))
                            {
                                lineList.Add("\"" + field + "\"");
                            }
                            else if (field.Equals(""))
                            {
                                continue;
                            }
                            else
                            {
                                lineList.Add(field);
                            }
                        }
                    }
                }
                parser.Close();
                int i = 1;
                try
                {
                    //populating wrong/correct headers
                    while (lineList[i] != "stop")
                    {
                        if (i % 2 == 0)
                        {
                            correctHeader.Add(lineList[i]);
                        }
                        else
                        {
                            wrongHeader.Add(lineList[i]);
                        }
                        i++;
                    }
                    //finding the second "start" in file
                    int startIndex = lineList.IndexOf("start", 2);
                    int z = startIndex + 1;
                    //populating bad/good modes
                    while (lineList[z] != "stop")
                    {
                        if (z % 2 == 0)
                        {
                            goodModes.Add(lineList[z]);
                        }
                        else
                        {
                            badModes.Add(lineList[z]);
                        }
                        z++;
                    }
                }
                catch
                {
                    MessageBox.Show("Error in file formatting");
                }
            }
        }
    }
}

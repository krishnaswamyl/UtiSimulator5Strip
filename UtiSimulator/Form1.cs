using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;


namespace UtiSimulator
{
    public partial class Form1 : Form
    {
        static private SerialPort serp = new SerialPort();
        static public bool read_timeout_flag = false;
        private static Mutex mut = new Mutex();     //For Read Write command.
        delegate void SetStatusBarTextCallback(string text);
        private static EventWaitHandle adcSyc = new EventWaitHandle(true, EventResetMode.AutoReset);
        public string comportname = "COM1";

        private List<TextBox> P1_450nm;
        private List<TextBox> P1_630nm;
        private List<TextBox> P2_450nm;
        private List<TextBox> P2_630nm;
        private List<TextBox> P3_450nm;
        private List<TextBox> P3_630nm;

        private List<TextBox> P4_450nm;
        private List<TextBox> P4_630nm;

        private List<TextBox> P5_450nm;
        private List<TextBox> P5_630nm;

        private List<TextBox> P6_450nm;
        private List<TextBox> P6_630nm;

        private List<TextBox> P7_450nm;
        private List<TextBox> P7_630nm;
        private int no_of_strips = 3;
        public Form1()
        {
            InitializeComponent();
        }

        private void SetText(string text)
        {
            if (this.statusStrip1.InvokeRequired)
            {
                SetStatusBarTextCallback d = new SetStatusBarTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.toolStripStatusLabel1.Text = text;
            }
        }

        private void OnLoadForm(object sender, EventArgs e)
        {
            populateTextBox();
            comboBox_COMPORT.BeginUpdate();
            comboBox_COMPORT.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox_COMPORT.Items.Add(s);
            }
            string len = comboBox_COMPORT.Items.Count.ToString();
            if (len.Equals("0"))
            {
                MessageBox.Show("No Comports detected on systems");
                return;
            }
            comboBox_COMPORT.SelectedIndex = 0;
            comboBox_COMPORT.EndUpdate();
            


        }



        private string WriteAndReadCom(string com)
        {
            mut.WaitOne();
            string temp = "NACK";
            if (serp.IsOpen == false)
            {
                MessageBox.Show("Please connect to comm");
                mut.ReleaseMutex();
                read_timeout_flag = true;
                return ("NACK");
            }
            serp.DiscardInBuffer();

            serp.WriteLine(com);
            read_timeout_flag = false;
            try
            {
                temp = serp.ReadLine();
            }
            catch (System.TimeoutException)
            {
                mut.ReleaseMutex();
                read_timeout_flag = true;
                return ("Time Out");
            }
            SetText(temp);
            mut.ReleaseMutex();
            return (temp);
        }

        private void button_OpenPort_Click(object sender, EventArgs e)
        {
            if (button_OpenPort.Text.Equals("Open Port"))
            {
                string Comportname;
                Comportname = comboBox_COMPORT.SelectedItem.ToString();
                serp.PortName = Comportname;
                serp.BaudRate = 115200;
                serp.ReadBufferSize = 1024;
                serp.NewLine = "\r\n";
                try
                {
                    serp.Open();
                }
                catch (Exception se)
                {
                    MessageBox.Show(se.Message.ToString());
                    return;
                }
                serp.ReadTimeout = 1000;
                Thread.Sleep(100);
                
                WriteAndReadCom("10 ");
                if (read_timeout_flag == false)
                {
                    button_OpenPort.Text = "Close Port";
                    button_OpenPort.BackColor = Color.HotPink;

                }
                else
                {
                    serp.Dispose();
                    serp.Close();
                    SetText("No Communication");
                }

            }
            else
            {

                serp.Dispose();
                serp.Close();
                button_OpenPort.Text = "Open Port";
                button_OpenPort.BackColor = Color.YellowGreen;
                
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            string temp;
            int lp = 0, olp = 0;
            List< TextBox > mylist= new List<TextBox>();

            temp = "01 " + textBoxBaseLine.Text.ToString();
            temp = WriteAndReadCom(temp);
            if(temp.Equals("NACK"))
            {
                return;
            }
            temp = "02 " + textBoxNoStrips.Text.ToString();
            temp = WriteAndReadCom(temp);
            if (temp.Equals("NACK"))
            {
                return;
            }
            getNo_Of_Strips();
            if (no_of_strips > 7 || no_of_strips < 3)
            {
                return;
            }
            for (olp = 0; olp < (no_of_strips*2); olp++)
            {
                temp = "03 " + (olp + 1);
                temp = WriteAndReadCom(temp);
                switch (olp)
                {
                    case 0:
                        mylist = P1_450nm;
                        break;
                    case 1:
                        mylist = P1_630nm;
                        break;
                    case 2:
                        mylist = P2_450nm;
                        break;
                    case 3:
                        mylist = P2_630nm;
                        break;
                    case 4:
                        mylist = P3_450nm;
                        break;
                    case 5:
                        mylist = P3_630nm;
                        break;
                    case 6:
                        mylist = P4_450nm;
                        break;
                    case 7:
                        mylist = P4_630nm;
                        break;
                    case 8:
                        mylist = P5_450nm;
                        break;
                    case 9:
                        mylist = P5_630nm;
                        break;
                    case 10:
                        mylist = P6_450nm;
                        break;
                    case 11:
                        mylist = P6_630nm;
                        break;
                    case 12:
                        mylist = P7_450nm;
                        break;
                    case 13:
                        mylist = P7_630nm;
                        break;

                };
                for (lp = 0; lp < 8; lp++)
                {
                    
                    temp = mylist[lp].Text.ToString();
                    temp = WriteAndReadCom(temp);
                    if (temp.Equals("NACK"))
                    {
                        return;
                    }
                }
                SetText(" Strip " + (olp+1) + " Transmitted");                

            }

            return;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string temp;
            temp = WriteAndReadCom("04 ");
            if(temp.Equals("Ack"))
                {
                SetText("Calcuations Done");

            }
            if (temp.Equals("NACK"))
            {
                SetText("Error System did not respond");
                return;
            }
        }

        private void buttonGetResults_Click(object sender, EventArgs e)
        {
            string temp;
            richTextBox1.Clear();
            int no_of_results = 16;
            temp = WriteAndReadCom("11");
            if (temp.Equals("NACK"))
            {
                SetText("Error System did not respond");
                return;
            }
            try
            {
                no_of_results = int.Parse(temp);
            }catch(Exception)
            {
                MessageBox.Show("No of Results returned from UTI card in not a number");
                return;
            }
            for ( int k=0; k < no_of_results; k++)
            {
                temp = WriteAndReadCom("05 " + k);
                if (temp.Equals("NACK"))
                {
                    SetText("Error System did not respond");
                    return;
                }
                richTextBox1.AppendText(temp);
                richTextBox1.AppendText("\r\n");

            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            List<TextBox> mylistvf = new List<TextBox>();
            getNo_Of_Strips();
            if (no_of_strips > 7 || no_of_strips < 3)
            {
                return;
            }
            for (int olp = 0; olp < (no_of_strips*2); olp++)
            {

                switch (olp)
                {
                    case 0:
                        mylistvf = P1_450nm;
                        break;
                    case 1:
                        mylistvf = P1_630nm;
                        break;
                    case 2:
                        mylistvf = P2_450nm;
                        break;
                    case 3:
                        mylistvf = P2_630nm;
                        break;
                    case 4:
                        mylistvf = P3_450nm;
                        break;
                    case 5:
                        mylistvf = P3_630nm;
                        break;
                    case 6:
                        mylistvf = P4_450nm;
                        break;
                    case 7:
                        mylistvf = P4_630nm;
                        break;
                    case 8:
                        mylistvf = P5_450nm;
                        break;
                    case 9:
                        mylistvf = P5_630nm;
                        break;
                    case 10:
                        mylistvf = P6_450nm;
                        break;
                    case 11:
                        mylistvf = P6_630nm;
                        break;
                    case 12:
                        mylistvf = P7_450nm;
                        break;
                    case 13:
                        mylistvf = P7_630nm;
                        break;

                };
                for (int lp = 0; lp < 8; lp++)
                {
                    mylistvf[lp].BackColor = Color.White;
                }


            }//loop ends here

        }

       
        private void buttonVerify_Click(object sender, EventArgs e)
        {
            string temp, command;
            double xt = 0.1, dt = 5.0;
            int sx = 3, ds = 2;
            List<TextBox> mylistvf = new List<TextBox>();

            temp = WriteAndReadCom("06");
            if (temp.Equals("NACK"))
            {
                return;
            }
            try
            {
                xt = double.Parse(temp);
                dt = double.Parse(textBoxBaseLine.Text.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (xt == dt)
            {
                textBoxBaseLine.BackColor = Color.LightGreen;
            }
            else
            {
                textBoxBaseLine.BackColor = Color.Orange;
            }

            temp = WriteAndReadCom("07");
            if (temp.Equals("NACK"))
            {
                return;
            }
            try
            {
                sx = int.Parse(temp);
                ds = int.Parse(textBoxNoStrips.Text.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            if (sx == ds)
            {
                textBoxNoStrips.BackColor = Color.LightGreen;
            }
            else
            {
                textBoxNoStrips.BackColor = Color.Orange;
            }
            getNo_Of_Strips();
            if (no_of_strips > 7 || no_of_strips < 3)
            {
                return;
            }
            for (int olp = 0; olp < (no_of_strips*2); olp++)
            {
                
                switch (olp)
                {
                    case 0:
                        mylistvf = P1_450nm;
                        break;
                    case 1:
                        mylistvf = P1_630nm;
                        break;
                    case 2:
                        mylistvf = P2_450nm;
                        break;
                    case 3:
                        mylistvf = P2_630nm;
                        break;
                    case 4:
                        mylistvf = P3_450nm;
                        break;
                    case 5:
                        mylistvf = P3_630nm;
                        break;
                    case 6:
                        mylistvf = P4_450nm;
                        break;
                    case 7:
                        mylistvf = P4_630nm;
                        break;
                    case 8:
                        mylistvf = P5_450nm;
                        break;
                    case 9:
                        mylistvf = P5_630nm;
                        break;
                    case 10:
                        mylistvf = P6_450nm;
                        break;
                    case 11:
                        mylistvf = P6_630nm;
                        break;
                    case 12:
                        mylistvf = P7_450nm;
                        break;
                    case 13:
                        mylistvf = P7_630nm;
                        break;

                };
                for (int lp = 0; lp < 8; lp++)
                {
                    command = String.Format("{0:2} {1:D2} {2:D2}","08",(olp+1),lp);
                    temp = WriteAndReadCom(command);
                    if (temp.Equals("NACK"))
                    {
                        return;
                    }
                    try
                    {
                        xt = double.Parse(temp);
                        dt = double.Parse(mylistvf[lp].Text.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                    if(xt == dt)
                    {
                        mylistvf[lp].BackColor = Color.LightGreen;
                    }
                    else
                    {
                        mylistvf[lp].BackColor = Color.LightPink;
                    }
                  
                }
               

            }//loop ends here
        }
        private void getNo_Of_Strips()
        {
            
            try
            {
                no_of_strips = int.Parse(textBoxNoStrips.Text.ToString());
            }
            catch (FormatException)
            {
                MessageBox.Show("No of Strips not a number Please edit.."); return;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("No of Strips not a number Please edit.."); return;
            }
            if (no_of_strips > 7 || no_of_strips < 3)
            {
                MessageBox.Show("Please enter a value for \nNo of Strips between 3 - 7");
                return;
            }
        }
        private void buttonPaste_Click(object sender, EventArgs e)
        {
            getNo_Of_Strips();
            if (no_of_strips > 7 || no_of_strips < 3)
            {
                return;
            }
            string s = Clipboard.GetText();
            string[] lines  = s.Split('\n');
            string[] sCells = lines[0].Split('\t');            
            int count = sCells.Length;
            int i = 0, k = 0;
            if(count < (no_of_strips*16))
            {
                MessageBox.Show(" Clip Board Data Length not Correct");
                return;
            }

            for (i = 0; i < count; i += (no_of_strips * 2))
            {
                
                P1_450nm[k].Text = sCells[i + 0];
                P1_630nm[k].Text = sCells[i + 1];

                P2_450nm[k].Text = sCells[i + 2];
                P2_630nm[k].Text = sCells[i + 3];

                P3_450nm[k].Text = sCells[i + 4];
                P3_630nm[k].Text = sCells[i + 5];

                if (no_of_strips == 3) { k++; continue; }

                P4_450nm[k].Text = sCells[i + 6];
                P4_630nm[k].Text = sCells[i + 7];

                if (no_of_strips == 4) { k++; continue; }

                P5_450nm[k].Text = sCells[i + 8];
                P5_630nm[k].Text = sCells[i + 9];

                if (no_of_strips == 5) { k++; continue; }

                P6_450nm[k].Text = sCells[i + 10];
                P6_630nm[k].Text = sCells[i + 11];

                if (no_of_strips == 6) { k++; continue; }

                P7_450nm[k].Text = sCells[i + 12];
                P7_630nm[k].Text = sCells[i + 13];
                k++;

            }


        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }


}

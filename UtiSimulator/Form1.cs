using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Media;

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

        private List<CheckBox> checkBoxes;

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
            no_of_strips = getNo_Of_Strips_Selected();
            if (no_of_strips > 7 || no_of_strips < 1)
            {
                MessageBox.Show("Please select valid Strips to be sent:");
                return;
            }
            List< TextBox > mylist= new List<TextBox>();

            temp = "01 " + textBoxBaseLine.Text.ToString();
            temp = WriteAndReadCom(temp);
            if(temp.Equals("NACK"))
            {
                return;
            }

            
            Boolean cflag = false;
            for (olp = 0; olp < 14; olp++)
            {
                cflag = false;
                switch (olp)
                {
                    case 0:
                        if (checkBoxes[0].Checked) { cflag = true; }
                            mylist = P1_450nm;                        
                        break;
                    case 1:
                        if (checkBoxes[0].Checked) { cflag = true; }
                        mylist = P1_630nm;                        
                        break;
                    case 2:
                        if (checkBoxes[1].Checked) { cflag = true; }
                        mylist = P2_450nm;
                        break;
                    case 3:
                        if (checkBoxes[1].Checked) { cflag = true; }
                        mylist = P2_630nm;
                        break;
                    case 4:
                        if (checkBoxes[2].Checked) { cflag = true; }
                        mylist = P3_450nm;
                        break;
                    case 5:
                        if (checkBoxes[2].Checked) { cflag = true; }
                        mylist = P3_630nm;
                        break;
                    case 6:
                        if (checkBoxes[3].Checked) { cflag = true; }
                        mylist = P4_450nm;
                        break;
                    case 7:
                        if (checkBoxes[3].Checked) { cflag = true; }
                        mylist = P4_630nm;
                        break;
                    case 8:
                        if (checkBoxes[4].Checked) { cflag = true; }
                        mylist = P5_450nm;
                        break;
                    case 9:
                        if (checkBoxes[4].Checked) { cflag = true; }
                        mylist = P5_630nm;
                        break;
                    case 10:
                        if (checkBoxes[5].Checked) { cflag = true; }
                        mylist = P6_450nm;
                        break;
                    case 11:
                        if (checkBoxes[5].Checked) { cflag = true; }
                        mylist = P6_630nm;
                        break;
                    case 12:
                        if (checkBoxes[6].Checked) { cflag = true; }
                        mylist = P7_450nm;
                        break;
                    case 13:
                        if (checkBoxes[6].Checked) { cflag = true; }
                        mylist = P7_630nm;
                        break;

                };
                if (cflag == false) continue;
                temp = "03 " + (olp + 1);
                temp = WriteAndReadCom(temp);
                for (lp = 0; lp < 8; lp++)
                {
                    
                    temp = mylist[lp].Text.ToString();
                    temp = WriteAndReadCom(temp);
                    if (temp.Equals("NACK"))
                    {
                        return;
                    }
                }
                SetText("No of Strips Transmitted to UTI card is: " + (olp + 1)+" Nos");

            }

            return;
        }

        private void buttonVerify_Click(object sender, EventArgs e)
        {
            no_of_strips = getNo_Of_Strips_Selected();
            if (no_of_strips > 7 || no_of_strips < 1)
            {
                MessageBox.Show("Please select valid Strips to be sent:");
                return;
            }
            string temp, command;
            double xt = 0.1, dt = 5.0;
            Boolean cflag = false;
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

            for (int olp = 0; olp < 14; olp++)
            {
                cflag = false;
                switch (olp)
                {
                    case 0:
                        if (checkBoxes[0].Checked) { cflag = true; }
                        mylistvf = P1_450nm;
                        break;
                    case 1:
                        if (checkBoxes[0].Checked) { cflag = true; }
                        mylistvf = P1_630nm;
                        break;
                    case 2:
                        if (checkBoxes[1].Checked) { cflag = true; }
                        mylistvf = P2_450nm;
                        break;
                    case 3:
                        if (checkBoxes[1].Checked) { cflag = true; }
                        mylistvf = P2_630nm;
                        break;
                    case 4:
                        if (checkBoxes[2].Checked) { cflag = true; }
                        mylistvf = P3_450nm;
                        break;
                    case 5:
                        if (checkBoxes[2].Checked) { cflag = true; }
                        mylistvf = P3_630nm;
                        break;
                    case 6:
                        if (checkBoxes[3].Checked) { cflag = true; }
                        mylistvf = P4_450nm;
                        break;
                    case 7:
                        if (checkBoxes[3].Checked) { cflag = true; }
                        mylistvf = P4_630nm;
                        break;
                    case 8:
                        if (checkBoxes[4].Checked) { cflag = true; }
                        mylistvf = P5_450nm;
                        break;
                    case 9:
                        if (checkBoxes[4].Checked) { cflag = true; }
                        mylistvf = P5_630nm;
                        break;
                    case 10:
                        if (checkBoxes[5].Checked) { cflag = true; }
                        mylistvf = P6_450nm;
                        break;
                    case 11:
                        if (checkBoxes[5].Checked) { cflag = true; }
                        mylistvf = P6_630nm;
                        break;
                    case 12:
                        if (checkBoxes[6].Checked) { cflag = true; }
                        mylistvf = P7_450nm;
                        break;
                    case 13:
                        if (checkBoxes[6].Checked) { cflag = true; }
                        mylistvf = P7_630nm;
                        break;

                };
                if (cflag == false) continue;
                for (int lp = 0; lp < 8; lp++)
                {
                    command = String.Format("{0:2} {1:D2} {2:D2}", "08", (olp + 1), lp);
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

                    if (xt == dt)
                    {
                        mylistvf[lp].BackColor = Color.LightGreen;
                    }
                    else
                    {
                        mylistvf[lp].BackColor = Color.LightPink;
                    }

                }


            }//loop ends here
            SetText("Data Verification Done....");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Send Calculate command.
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
            List<TextBox> mylistvf = new List<TextBox>();
            for (int olp = 0; olp < 14; olp++)
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
                    mylistvf[lp].Text = "0.000";
                }


            }//loop ends here
            foreach(CheckBox chb in checkBoxes)
            {
                chb.Checked = false;
            }
            SetText("Strips Cleared...");

        }

       
        
        private int getNo_Of_Strips_Selected()
        {
            int count = 0;
            foreach(CheckBox ch in checkBoxes)
            {
                if(ch.Checked)
                {
                    count++;
                }
            }

            return count;  
        }
        private void buttonPaste_Click(object sender, EventArgs e)
        {
            String[] lines;
            String[] sCells;
            int count = 0;
            int i = 0, k = 0;
            String st = String.Empty;
            List<TextBox> list450 = new List<TextBox>();
            List<TextBox> list630 = new List<TextBox>();
            foreach(CheckBox chb in checkBoxes)
            {
                chb.Checked = false;
            }
            
            st = Clipboard.GetText();
            lines  = st.Split('\n');
            count = lines.Length-1;
            if(count < 10)
            {
                SetText("Invalid rows in the Clip Board");
                return;
            }
            int strips = count / 10;
            int remind = count % 10;
            if(remind > 0)
            {
                SetText("Data in ClipBoard Not multiples of 8. Please check xls file..");
                return;
            }
            for(i=0; i < strips; i++)
            {
                sCells = lines[(i * 10)].Split('\t');
                switch(sCells[0])
                {
                    case "P-1 absorbance":
                        list450 = P1_450nm;
                        list630 = P1_630nm;
                        checkBoxes[0].Checked = true;
                        break;
                    case "P-2 absorbance":
                        list450 = P2_450nm;
                        list630 = P2_630nm;
                        checkBoxes[1].Checked = true;
                        break;
                    case "P-3 absorbance":
                        list450 = P3_450nm;
                        list630 = P3_630nm;
                        checkBoxes[2].Checked = true;
                        break;
                    case "P-4 absorbance":
                        list450 = P4_450nm;
                        list630 = P4_630nm;
                        checkBoxes[3].Checked = true;
                        break;
                    case "P-5 absorbance":
                        list450 = P5_450nm;
                        list630 = P5_630nm;
                        checkBoxes[4].Checked = true;
                        break;
                    case "P-6 absorbance":
                        list450 = P6_450nm;
                        list630 = P6_630nm;
                        checkBoxes[5].Checked = true;
                        break;
                    case "P-7 absorbance":
                        list450 = P7_450nm;
                        list630 = P7_630nm;
                        checkBoxes[6].Checked = true;
                        break;
                }
                for (k=0; k < 8; k++)
                {
                    sCells = lines[(i * 10)+k+2].Split('\t');
                    if(sCells.Length < 2)
                    {
                        SetText("Clipboard contains only One Cloumn, Please copy two coloumns");
                        return;
                    }
                    list450[k].Text = sCells[0];
                    list630[k].Text = sCells[1];
                }
                
            }
            SetText("Data Imported .....  " + stripsSelected());
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonClearResults_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            SystemSounds.Beep.Play();
        }
        private String stripsSelected()
        {
            String com = "12 ";          
            foreach (CheckBox ch in checkBoxes)
            {
                if(ch.Checked)
                {
                    com += "1,";
                }
                else
                {
                    com += "0,";
                }

            }

            
            return com;
        }
    }


}

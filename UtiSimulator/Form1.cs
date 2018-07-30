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
        public string stripSelected = "11 0,0,0,0,0,0,0,0";
        public Byte strip = 0;

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

        private List<TextBox> Pi_450nm;
        private List<TextBox> Pi_630nm;

        private List<TextBox> PiN_450nm;
        private List<TextBox> PiN_630nm;



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
            comboBoxMode.SelectedIndex = 0;


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
            
            mut.ReleaseMutex();
            return (temp);
        }

        private void button_OpenPort_Click(object sender, EventArgs e)
        {
            if (button_OpenPort.Text.Equals("Open Port"))
            {
                string Comportname, temp;
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
                
                temp = WriteAndReadCom("20 ");  // Send Default Command
                if (read_timeout_flag == false)
                {
                    //Communication Successful with UTI card.
                    button_OpenPort.Text = "Close Port";
                    button_OpenPort.BackColor = Color.HotPink;
                    if(temp.Equals("Default"))
                    {
                        SetText("Communication With UTI Card Successful");
                    }
                }
                else
                {
                    serp.Dispose();
                    serp.Close();
                    SetText("Communication To UTI Card Failed:");
                }

            }
            else
            {

                serp.Dispose();
                serp.Close();
                button_OpenPort.Text = "Open Port";
                button_OpenPort.BackColor = Color.YellowGreen;
                SetText("Com Port Closed:.......");
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if(serp.IsOpen == false)
            {
                MessageBox.Show("Please Open Com Port And Try Again ....");
                return;
            }
            string temp;
            int lp = 0, olp = 0;
            no_of_strips = getNo_Of_Strips_Selected();
            if (no_of_strips > 6 || no_of_strips < 1)
            {
                MessageBox.Show("Please select valid Strips to be sent:");
                return;
            }
            // Send the Mode Ex: 0 for mono and 1 for bichrmo
            int index = comboBoxMode.SelectedIndex;
            switch (index)
            {
                case 0:
                    temp = WriteAndReadCom("03 00");
                    break;
                case 1:
                    temp = WriteAndReadCom("03 01");
                    break;
                case -1:
                    MessageBox.Show("Please select a valid Mode");
                    return;

            }
            //Send strip selected
            Byte striptemp = 0;
            temp = WriteAndReadCom(stripSelected);
            try
            {
                striptemp = Convert.ToByte(temp, 16);
            }
            catch(ArgumentException)
            {
                MessageBox.Show("Error while receiving back No of Strips Selected");
                return;
            }
            if(striptemp != strip)
            {
                MessageBox.Show("Error while receiving back No of Strips Selected");
                return;
            }

            List< TextBox > mylist= new List<TextBox>();            
            Boolean cflag = false;
            for (olp = 0; olp < 12; olp++)
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

                };
                if (cflag == false) continue;
                temp = "09 " + (olp + 1);
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
            }

                // Send Pi450nm and Pi630nm
                for (olp = 0; olp < 2; olp++)
                {
                    cflag = false;
                    switch (olp)
                    {
                        case 0:
                            mylist = Pi_450nm;
                            break;
                        case 1:
                            if (index == 0) { cflag = true; }
                            mylist = Pi_630nm;
                            break;
                    }
                    if (cflag == true) continue;
                    temp = "05 " + (olp + 1);           // case 5: to send Pi data. 1 to 4
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
                }
                using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Print complete.wav"))
                {
                    soundPlayer.Play(); // can also use soundPlayer.PlaySync()
                }
                SetText("Data of Pi and Selected Panels sent to UTI card: "+stripSelected);

            return;
        }

        private void buttonVerify_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void buttonGetResults_Click(object sender, EventArgs e)
        {
            string temp;
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
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            List<TextBox> mylistvf = new List<TextBox>();
            int olp;
            for (olp = 0; olp < 12; olp++)
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
                    
                };
                for (int lp = 0; lp < 8; lp++)
                {
                    mylistvf[lp].BackColor = Color.White;
                    mylistvf[lp].Text = "0.000";
                }


            }//loop ends here

            for (olp = 0; olp < 4; olp++)
            {
                
                switch (olp)
                {
                    case 0:
                        mylistvf = Pi_450nm;
                        break;
                    case 1:
                        mylistvf = Pi_630nm;
                        break;
                    case 2:
                        mylistvf = PiN_450nm;
                        break;
                    case 3:
                        mylistvf = PiN_630nm;
                        break;
                }
                for (int lp = 0; lp < 8; lp++)
                {
                    mylistvf[lp].BackColor = Color.White;
                    mylistvf[lp].Text = "0.000";
                }
            }


            foreach (CheckBox chb in checkBoxes)
            {
                chb.Checked = false;
            }
            SetText("Strips Cleared...");

        }

       
        
        private int getNo_Of_Strips_Selected()
        {
           
            int count = 0;
            stripSelected = "11 ";
            foreach (CheckBox ch in checkBoxes)
            {
                if(ch.Checked)
                {
                    stripSelected += "1,";
                    strip <<= 1;
                    strip |= 1;
                    count++;
                }else
                {
                    strip <<= 1;
                    stripSelected += "0,";
                }
            }
            stripSelected += "0,";
            stripSelected += "0,";
            strip <<= 1;
            strip <<= 1;

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
                    case "P-i absorbance":
                        list450 = Pi_450nm;
                        list630 = Pi_630nm;
                        checkBoxes[0].Checked = true;
                        break;
                    case "P-iN absorbance":
                        list450 = PiN_450nm;
                        list630 = PiN_630nm;
                        checkBoxes[0].Checked = true;
                        break;
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
            SetText("Data Imported From Excel Sheet .....  " + stripsSelected());
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonClearResults_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            SystemSounds.Beep.Play();
        }
        private String stripsSelected()
        {
            String com = "11 ";          
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

        private void buttonSendPi_Click(object sender, EventArgs e)
        {
            if (serp.IsOpen == false)
            {
                MessageBox.Show("Please connect to comm");
                return;
            }
            string temp;
            int mins = 120;
            int olp=0, lp = 0;
            // send the no of mins elapsed since the first Pi was read.
            try
            {
                mins = int.Parse(No_of_Mins.Text.ToString());
            }
            catch (FormatException)
            {
                MessageBox.Show("No of Mins Not a Valid Integer..");
                return;
            }
            catch (ArgumentException)
            {
                MessageBox.Show("No of Mins Not a Valid Integer..");
                return;
            }
            temp = "01 " + mins.ToString("000");
            temp = WriteAndReadCom(temp);
            // Send the Mode Ex: 0 for mono and 1 for bichrmo
            int index = comboBoxMode.SelectedIndex;
            switch(index)
            {
                case 0:
                    temp = WriteAndReadCom("03 00");
                    break;
                case 1:
                    temp = WriteAndReadCom("03 01");
                    break;
                case -1:
                    MessageBox.Show("Please select a valid Mode");
                    return;            

            }
            // Send Pi450nm Send Pi630nm Send PiN450nm Send PiN630nm
            List<TextBox> mylist = new List<TextBox>();

            Boolean cflag = false;
            for (olp = 0; olp < 4; olp++)
            {
                cflag = false;
                switch (olp)
                {
                    case 0:
                        mylist = Pi_450nm;
                        break;
                    case 1:
                        if (index == 0) { cflag = true; }
                        mylist = Pi_630nm;
                        break;
                    case 2:
                        mylist = PiN_450nm;
                        break;
                    case 3:
                        if (index == 0) { cflag = true; }
                        mylist = PiN_630nm;
                        break;
                }
                if (cflag == true) continue;
                temp = "05 " + (olp + 1);           // case 5: to send Pi data. 1 to 4
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
            }
            SetText("Panel Identification Pi Data Sent to UTI Card..");
            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Print complete.wav"))
            {
                soundPlayer.Play(); // can also use soundPlayer.PlaySync()
            }
            return;
        }

        private void buttonValidateASTdata_Click(object sender, EventArgs e)
        {
            no_of_strips = getNo_Of_Strips_Selected();
            if (no_of_strips > 6 || no_of_strips < 1)
            {
                MessageBox.Show("Please select valid Strips to be Received:");
                return;
            }
            string temp, command;
            double xt = 0.1, dt = 5.0;
            // Read Mode
            temp = WriteAndReadCom("04 ");
            int mode = int.Parse(temp);
            int Mode = comboBoxMode.SelectedIndex;
            if (mode != Mode)
            {
                comboBoxMode.BackColor = Color.LightPink;
            }
            else
            {
                comboBoxMode.BackColor = Color.LightGreen;
            }

            Boolean cflag = false;
            List<TextBox> mylistvf = new List<TextBox>();
            
            for (int olp = 0; olp < 12; olp++)
            {
                cflag = false;
                switch (olp)
                {
                    case 0:
                        if (checkBoxes[0].Checked) { cflag = true; }
                        mylistvf = P1_450nm;
                        break;
                    case 1:                       
                        if (checkBoxes[0].Checked && Mode == 1) { cflag = true; }
                        mylistvf = P1_630nm;
                        break;
                    case 2:
                        if (checkBoxes[1].Checked) { cflag = true; }
                        mylistvf = P2_450nm;
                        break;
                    case 3:
                        if (checkBoxes[1].Checked && Mode == 1) { cflag = true; }
                        mylistvf = P2_630nm;
                        break;
                    case 4:
                        if (checkBoxes[2].Checked) { cflag = true; }
                        mylistvf = P3_450nm;
                        break;
                    case 5:
                        if (checkBoxes[2].Checked && Mode == 1) { cflag = true; }
                        mylistvf = P3_630nm;
                        break;
                    case 6:
                        if (checkBoxes[3].Checked) { cflag = true; }
                        mylistvf = P4_450nm;
                        break;
                    case 7:
                        if (checkBoxes[3].Checked && Mode == 1) { cflag = true; }
                        mylistvf = P4_630nm;
                        break;
                    case 8:
                        if (checkBoxes[4].Checked) { cflag = true; }
                        mylistvf = P5_450nm;
                        break;
                    case 9:
                        if (checkBoxes[4].Checked && Mode == 1) { cflag = true; }
                        mylistvf = P5_630nm;
                        break;
                    case 10:
                        if (checkBoxes[5].Checked) { cflag = true; }
                        mylistvf = P6_450nm;
                        break;
                    case 11:
                        if (checkBoxes[5].Checked && Mode == 1) { cflag = true; }
                        mylistvf = P6_630nm;
                        break;


                };
                if (cflag == false) continue;
                for (int lp = 0; lp < 8; lp++)
                {
                    command = String.Format("{0:2} {1:D2} {2:D2}", "10", (olp + 1), lp);
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
            //Read back Pi data and verify
            for (int olp = 0; olp < 2; olp++)
            {
                cflag = false;
                switch (olp)
                {
                    case 0:
                        mylistvf = Pi_450nm;
                        break;
                    case 1:
                        if (Mode == 0) { cflag = true; }
                        mylistvf = Pi_630nm;
                        break;

                }
                if (cflag == true) continue;
                for (int lp = 0; lp < 8; lp++)
                {
                    command = String.Format("{0:2} {1:D2} {2:D2}", "06", (olp + 1), lp);
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
            }
            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Print complete.wav"))
            {
                soundPlayer.Play(); // can also use soundPlayer.PlaySync()
            }
            SetText("Data Verification Done....");
        }




        private void buttonGetASTresult_Click(object sender, EventArgs e)
        {
            //Send Calculate command.
            string temp, command;
            temp = WriteAndReadCom("12 ");
            if (temp.Equals("Ack"))
            {
                SetText("Calcuations Done");

            }
            if (temp.Equals("NACK"))
            {
                SetText("Error System did not respond");
                return;
            }
            dataGridView1.Rows.Clear();
            char[] splitchar = { ',' };
            string[] results = { "01", " ", " " };
            temp = WriteAndReadCom("13 00");            // Organism Name
            results[1] = "Organism Name"; results[2] = temp;
            dataGridView1.Rows.Add(results);
            temp = WriteAndReadCom("13 01");            // Cell Volume
            results[0] = "02"; results[1] = "Volume"; results[2] = temp;
            dataGridView1.Rows.Add(results);
            int sr = 1;
            Boolean cflag = false;
            

            for (int k=1; k < 7; k++)
            {
                cflag = false;
                switch(k)
                {
                    case 1:
                        if(checkBox1.Checked) { cflag = true; }
                        break;
                    case 2:
                        if (checkBox2.Checked) { cflag = true; }
                        break;
                    case 3:
                        if (checkBox3.Checked) { cflag = true; }
                        break;
                    case 4:
                        if (checkBox4.Checked) { cflag = true; }
                        break;                    
                    case 5:
                        if (checkBox5.Checked) { cflag = true; }
                        break;
                    case 6:
                        if (checkBox6.Checked) { cflag = true; }
                        break;
                }
                if (cflag == false) continue;
                dataGridView1.Rows.Add(" ", " ", " ");
                dataGridView1.Rows.Add(" ", "Panel-Results: "+ k.ToString()," ");
                for (int p=0; p < 7; p++)
                {
                    command = String.Format("{0:2} {1:D2} {2:D2}", "14",k, p);
                    temp = WriteAndReadCom(command);
                    string[] results2 = temp.Split(splitchar);
                    results[0] = sr++.ToString("00");
                    results[1] = results2[0].Trim();
                    results[2] = results2[1];
                    dataGridView1.Rows.Add(results);
                }
               
            }

            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Print complete.wav"))
            {
                soundPlayer.Play(); // can also use soundPlayer.PlaySync()
            }

        }
        /// <summary>
        /// Validate Pi Data read from UTI card.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReceivePiData_Click(object sender, EventArgs e)
        {
            if (serp.IsOpen == false)
            {
                MessageBox.Show("Please connect to comm");
                return;
            }
            string temp, command;
            double xt = 0.1, dt = 5.0;
            Boolean cflag = false;
            int mins = 0, NoOfMins = 0;
            List<TextBox> mylistvf = new List<TextBox>();
            int index = comboBoxMode.SelectedIndex;
            temp = WriteAndReadCom("02 ");
            try
            {
                mins = int.Parse(temp);
                NoOfMins = int.Parse(No_of_Mins.Text.ToString());
            }
            catch(FormatException)
            {
                MessageBox.Show("No Of Mins read back from UTI card not valid\n Or No of Mins enterned is wrong");
                return;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("No Of Mins read back from UTI card not valid\n Or No of Mins enterned is wrong");
                return;
            }
            if(mins != NoOfMins)
            {
                No_of_Mins.BackColor = Color.LightPink;
            }else
            {
                No_of_Mins.BackColor = Color.LightGreen;
            }
            // Read Mode
            temp = WriteAndReadCom("04 ");
            int mode=int.Parse(temp);
            int Mode = comboBoxMode.SelectedIndex;
            if(mode != Mode)
            {
                comboBoxMode.BackColor = Color.LightPink;
            }
            else
            {
                comboBoxMode.BackColor = Color.LightGreen;
            }
            //Read back Pi data and verify
            for (int olp = 0; olp < 4; olp++)
            {
                cflag = false;
                switch (olp)
                {
                    case 0:
                        mylistvf = Pi_450nm;
                        break;
                    case 1:
                        if (index == 0) { cflag = true; }
                        mylistvf = Pi_630nm;
                        break;
                    case 2:
                        mylistvf = PiN_450nm;
                        break;
                    case 3:
                        if (index == 0) { cflag = true; }
                        mylistvf = PiN_630nm;
                        break;


                };
                if (cflag == true) continue;
                for (int lp = 0; lp < 8; lp++)
                {
                    command = String.Format("{0:2} {1:D2} {2:D2}", "06", (olp + 1), lp);
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
            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Print complete.wav"))
            {
                soundPlayer.Play(); // can also use soundPlayer.PlaySync()
            }
        }

        private void buttonValidatePi_Click(object sender, EventArgs e)
        {
            if (serp.IsOpen == false)
            {
                MessageBox.Show("Please connect to comm");
                return;
            }
            if (serp.IsOpen == false)
            {
                MessageBox.Show("Please connect to comm");                
                return;
            }
            String temp = WriteAndReadCom("07 ");
            if (temp != "Ack")
            {
                MessageBox.Show("UTI card did not Respond......");
            }
            // Read the Pi result from UTI card.

            textBoxPiResult.Text = WriteAndReadCom("08 ");
            SetText("Panel Identification \"Pi\" Result Fetched ....");
            using (var soundPlayer = new SoundPlayer(@"c:\Windows\Media\Windows Print complete.wav"))
            {
                soundPlayer.Play(); // can also use soundPlayer.PlaySync()
            }
            return;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            double y = 0.0;
            string res = WriteAndReadCom("16 ");
            try
            {
                y = float.Parse(res);
            }catch(FormatException)
            {
                MessageBox.Show(" NAN");
                return;
            }

            MessageBox.Show(y.ToString(format: "0.0000"));
        }
    }


}

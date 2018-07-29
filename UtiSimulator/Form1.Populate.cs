
using System.Collections.Generic;
using System.Windows.Forms;


namespace UtiSimulator
{
   partial class Form1
    {
        private void populateTextBox()
        {
            P1_450nm = new List<TextBox>();
            P1_450nm.Add(textBox1);
            P1_450nm.Add(textBox2);
            P1_450nm.Add(textBox3);
            P1_450nm.Add(textBox4);
            P1_450nm.Add(textBox5);
            P1_450nm.Add(textBox6);
            P1_450nm.Add(textBox7);
            P1_450nm.Add(textBox8);

            P1_630nm = new List<TextBox>();
            P1_630nm.Add(textBox9);
            P1_630nm.Add(textBox10);
            P1_630nm.Add(textBox11);
            P1_630nm.Add(textBox12);
            P1_630nm.Add(textBox13);
            P1_630nm.Add(textBox14);
            P1_630nm.Add(textBox15);
            P1_630nm.Add(textBox16);

            P2_450nm = new List<TextBox>();
            P2_450nm.Add(textBox17);
            P2_450nm.Add(textBox18);
            P2_450nm.Add(textBox19);
            P2_450nm.Add(textBox20);
            P2_450nm.Add(textBox21);
            P2_450nm.Add(textBox22);
            P2_450nm.Add(textBox23);
            P2_450nm.Add(textBox24);

            P2_630nm = new List<TextBox>();
            P2_630nm.Add(textBox25);
            P2_630nm.Add(textBox26);
            P2_630nm.Add(textBox27);
            P2_630nm.Add(textBox28);
            P2_630nm.Add(textBox29);
            P2_630nm.Add(textBox30);
            P2_630nm.Add(textBox31);
            P2_630nm.Add(textBox32);

            P3_450nm = new List<TextBox>();
            P3_450nm.Add(textBox33);
            P3_450nm.Add(textBox34);
            P3_450nm.Add(textBox35);
            P3_450nm.Add(textBox36);
            P3_450nm.Add(textBox37);
            P3_450nm.Add(textBox38);
            P3_450nm.Add(textBox39);
            P3_450nm.Add(textBox40);

            P3_630nm = new List<TextBox>();
            P3_630nm.Add(textBox41);
            P3_630nm.Add(textBox42);
            P3_630nm.Add(textBox43);
            P3_630nm.Add(textBox44);
            P3_630nm.Add(textBox45);
            P3_630nm.Add(textBox46);
            P3_630nm.Add(textBox47);
            P3_630nm.Add(textBox48);

            P4_450nm = new List<TextBox>();
            P4_450nm.Add(textBox49);
            P4_450nm.Add(textBox50);
            P4_450nm.Add(textBox51);
            P4_450nm.Add(textBox52);
            P4_450nm.Add(textBox53);
            P4_450nm.Add(textBox54);
            P4_450nm.Add(textBox55);
            P4_450nm.Add(textBox56);

            P4_630nm = new List<TextBox>();
            P4_630nm.Add(textBox57);
            P4_630nm.Add(textBox58);
            P4_630nm.Add(textBox59);
            P4_630nm.Add(textBox60);
            P4_630nm.Add(textBox61);
            P4_630nm.Add(textBox62);
            P4_630nm.Add(textBox63);
            P4_630nm.Add(textBox64);

            P5_450nm = new List<TextBox>();
            P5_450nm.Add(textBox65);
            P5_450nm.Add(textBox66);
            P5_450nm.Add(textBox67);
            P5_450nm.Add(textBox68);
            P5_450nm.Add(textBox69);
            P5_450nm.Add(textBox70);
            P5_450nm.Add(textBox71);
            P5_450nm.Add(textBox72);

            P5_630nm = new List<TextBox>();
            P5_630nm.Add(textBox73);
            P5_630nm.Add(textBox74);
            P5_630nm.Add(textBox75);
            P5_630nm.Add(textBox76);
            P5_630nm.Add(textBox77);
            P5_630nm.Add(textBox78);
            P5_630nm.Add(textBox79);
            P5_630nm.Add(textBox80);

            P6_450nm = new List<TextBox>();
            P6_450nm.Add(textBox81);
            P6_450nm.Add(textBox82);
            P6_450nm.Add(textBox83);
            P6_450nm.Add(textBox84);
            P6_450nm.Add(textBox85);
            P6_450nm.Add(textBox86);
            P6_450nm.Add(textBox87);
            P6_450nm.Add(textBox88);

            P6_630nm = new List<TextBox>();
            P6_630nm.Add(textBox89);
            P6_630nm.Add(textBox90);
            P6_630nm.Add(textBox91);
            P6_630nm.Add(textBox92);
            P6_630nm.Add(textBox93);
            P6_630nm.Add(textBox94);
            P6_630nm.Add(textBox95);
            P6_630nm.Add(textBox96);

            Pi_450nm = new List<TextBox>();
            Pi_450nm.Add(textBoxpi1);
            Pi_450nm.Add(textBoxpi2);
            Pi_450nm.Add(textBoxpi3);
            Pi_450nm.Add(textBoxpi4);
            Pi_450nm.Add(textBoxpi5);
            Pi_450nm.Add(textBoxpi6);
            Pi_450nm.Add(textBoxpi7);
            Pi_450nm.Add(textBoxpi8);

            Pi_630nm = new List<TextBox>();
            Pi_630nm.Add(textBoxpi9);
            Pi_630nm.Add(textBoxpi10);
            Pi_630nm.Add(textBoxpi11);
            Pi_630nm.Add(textBoxpi12);
            Pi_630nm.Add(textBoxpi13);
            Pi_630nm.Add(textBoxpi14);
            Pi_630nm.Add(textBoxpi15);
            Pi_630nm.Add(textBoxpi16);

            PiN_450nm = new List<TextBox>();
            PiN_450nm.Add(textBoxpin1);
            PiN_450nm.Add(textBoxpin2);
            PiN_450nm.Add(textBoxpin3);
            PiN_450nm.Add(textBoxpin4);
            PiN_450nm.Add(textBoxpin5);
            PiN_450nm.Add(textBoxpin6);
            PiN_450nm.Add(textBoxpin7);
            PiN_450nm.Add(textBoxpin8);

            PiN_630nm = new List<TextBox>();
            PiN_630nm.Add(textBoxpin9);
            PiN_630nm.Add(textBoxpin10);
            PiN_630nm.Add(textBoxpin11);
            PiN_630nm.Add(textBoxpin12);
            PiN_630nm.Add(textBoxpin13);
            PiN_630nm.Add(textBoxpin14);
            PiN_630nm.Add(textBoxpin15);
            PiN_630nm.Add(textBoxpin16);


            checkBoxes = new List<CheckBox>();
            checkBoxes.Add(checkBox1);
            checkBoxes.Add(checkBox2);
            checkBoxes.Add(checkBox3);
            checkBoxes.Add(checkBox4);
            checkBoxes.Add(checkBox5);
            checkBoxes.Add(checkBox6);

        }
    }
}
